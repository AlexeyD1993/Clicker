using Clicker.src.Params;
using Clicker.src.Selenium;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Clicker
{
    public partial class Form1 : Form
    {
        private Random rand = new Random();
        [XmlArray("SeleniumParams"), XmlArrayItem(typeof(SeleniumParams), ElementName = "SeleniumParam")]
        private List<SeleniumParams> seleniumParams = new List<SeleniumParams>();

        private SeleniumParams currParam;
        private string[] proxyParams = null;
        private string[] userAgents = null;

        private string[] mobileResolutions = null;
        private string[] desktopResolutions = null;

        private bool userChanged = false;

        private int counter;

        public Form1()
        {
            userChanged = false;
            InitializeComponent();
            counter = 0;

            buttonAddWork_Click(null, null);
            userChanged = true;
        }

        private void WriteInRegistry()
        {
            const string userRoot = "HKEY_LOCAL_MACHINE\\SOFTWARE";
            const string subkey = "RegistrySetValueSelenium";
            const string keyName = userRoot + "\\" + subkey;

            int prevValue = 0;
            try
            {
                prevValue = (int)Registry.GetValue(keyName, "Count", 0);
            }
            catch
            { }

            if (prevValue == 80)
            {
                MessageBox.Show(this, "Закончился срок пробной версии.\nПожалуйста, оплатите программу по реквизитам: 5469 3700 1167 2938", "НЕОБХОДИМО ОПЛАТИТЬ ПРОГРАММУ!");
                Registry.SetValue(keyName, "Count", ++prevValue, RegistryValueKind.DWord);
                this.Close();
            }

            Registry.SetValue(keyName, "Count", ++prevValue, RegistryValueKind.DWord);
        }

        private void UpdateDataGridParam()
        {
            userChanged = false;
            dataGridViewWorks.Rows.Clear();

            foreach (SeleniumParams selenParam in seleniumParams)
            {
                dataGridViewWorks.Rows.Add(selenParam.ParamName);
            }
            userChanged = true;
        }

        private SeleniumParams ChangeUserAgent(SeleniumParams param)
        {
            int numStr = rand.Next(0, userAgents.Length - 1);
            string userAgentStr = userAgents[numStr];

            if (userAgentStr.ToLower().Contains("mobile"))
            {
                param.Browser = BrowserEnums.Browsers.mobile;
            }
            else if (userAgentStr.ToLower().Contains("edge"))
            {
                param.Browser = BrowserEnums.Browsers.edge;
            }
            else if (userAgentStr.ToLower().Contains("firefox"))
            {
                param.Browser = BrowserEnums.Browsers.firefox;
            }
            else if (userAgentStr.ToLower().Contains("yabrowser"))
            {
                param.Browser = BrowserEnums.Browsers.yandex;
            }
            else if (userAgentStr.ToLower().Contains("chome"))
            {
                param.Browser = BrowserEnums.Browsers.chrome;
            }
            else if (userAgentStr.ToLower().Contains("safari"))
            {
                param.Browser = BrowserEnums.Browsers.safari;
            }
            else
            {
                param.Browser = BrowserEnums.Browsers.other;
            }

            param.UserAgent = userAgentStr;
            return param;
        }

        private SeleniumParams ChangeProxy(SeleniumParams param)
        {
            int numStr = rand.Next(0, proxyParams.Length - 1);
            //TODO ParceProxy;
            string[] proxyIPPortString = proxyParams[numStr].Split(',');

            if (proxyIPPortString[0].Contains('@'))
            {
                string substring1 = proxyIPPortString[0].Substring(0, proxyIPPortString[0].IndexOf('@'));
                string substring2 = proxyIPPortString[0].Substring(proxyIPPortString[0].IndexOf('@') + 1);
                if ((!substring1.Contains(':')) || (!substring2.Contains(':')))
                {
                    MessageBox.Show(this, "Ошибка параметров прокси в строке " + numStr.ToString(), "Ошибка параматров прокси", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    string userName = substring1.Substring(0, substring1.IndexOf(':'));
                    string userPasswd = substring1.Substring(substring1.IndexOf(':') + 1);
                    string ipAddr = substring2.Substring(0, substring2.IndexOf(':'));
                    string port = substring2.Substring(substring2.IndexOf(':') + 1);

                    param.ProxyLogin = userName;
                    param.ProxyPassword = userPasswd;
                    param.ProxyIP.IPAddress = System.Net.IPAddress.Parse(ipAddr.Replace(" ", ""));
                    param.ProxyPort.IPEndPoint = new System.Net.IPEndPoint(currParam.ProxyIP.IPAddress, Int32.Parse(port));
                }
            }
            else if (proxyIPPortString[0].Contains(':'))
            {
                string ipAddr = proxyIPPortString[0].Substring(0, proxyIPPortString[0].IndexOf(':'));
                string port = proxyIPPortString[0].Substring(proxyIPPortString[0].IndexOf(':') + 1);
                param.ProxyIP.IPAddress = System.Net.IPAddress.Parse(ipAddr.Replace(" ", ""));
                param.ProxyPort.IPEndPoint = new System.Net.IPEndPoint(currParam.ProxyIP.IPAddress, Int32.Parse(port));
            }
            else
            {
                MessageBox.Show(this, "Ошибка параметров прокси в строке " + numStr.ToString(), "Ошибка параматров прокси", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (proxyIPPortString[4].ToUpper().Contains("ssl".ToUpper()))
                param.ProxyType = "http";
            else if (proxyIPPortString[4].Contains("v4"))
                param.ProxyType = "socks v4";
            else if (proxyIPPortString[4].Contains("v5"))
                param.ProxyType = "socks v5";
            else
                param.ProxyType = "Без proxy";
            return param;
        }

        private string GetNewParamName()
        {
            counter++;
            return "Задание № " + counter;
        }

        private SeleniumParams ChangeDesktopResolution(SeleniumParams param)
        {
            int numStr = rand.Next(0, desktopResolutions.Length - 1);
            string[] parcedRes = desktopResolutions[numStr].Split('x');
            if (parcedRes.Length != 2)
                throw new Exception(string.Format("Ошибка разрешения в строке {0}\nУказано {1}", numStr, desktopResolutions[numStr]));
            param.ResX = Int32.Parse(parcedRes[0]);
            param.ResY = Int32.Parse(parcedRes[1]);
            return param;
        }
        private SeleniumParams ChangeMobileResolution(SeleniumParams param)
        {
            int numStr = rand.Next(0, mobileResolutions.Length - 1);
            string[] parcedRes = mobileResolutions[numStr].Split('x');
            if (parcedRes.Length != 2)
                throw new Exception(string.Format("Ошибка разрешения в строке {0}\nУказано {1}", numStr, mobileResolutions[numStr]));
            param.ResX = Int32.Parse(parcedRes[0]);
            param.ResY = Int32.Parse(parcedRes[1]);
            return param;
        }

        private void buttonAddWork_Click(object sender, EventArgs e, string request = null)
        {
            SeleniumParams param = new SeleniumParams();
            param.ParamName = GetNewParamName();
            userChanged = false;

            if (request != "")
                param.Request = request;

            if (proxyParams != null)
            {
                param = ChangeProxy(param);
            }

            if (userAgents != null)
            {
                param = ChangeUserAgent(param);
            }

            if (param.Browser != BrowserEnums.Browsers.mobile)
            {
                if (desktopResolutions != null)
                {
                    param = ChangeDesktopResolution(param);
                }
            }
            else
            {
                if (mobileResolutions != null)
                {
                    param = ChangeMobileResolution(param);
                }
            }

            if (currParam == null)
                currParam = param;

            seleniumParams.Add(param);

            UpdateDataGridParam();

            if (dataGridViewWorks.RowCount != 0)
            {
                dataGridViewWorks.ClearSelection();
                dataGridViewWorks.Rows[dataGridViewWorks.RowCount - 1].Selected = true;
                dataGridViewWorks.Rows[dataGridViewWorks.RowCount - 1].Cells["WorkName"].Selected = true;
                dataGridViewWorks_CellClick(sender, new DataGridViewCellEventArgs(0, dataGridViewWorks.RowCount));
            }

            GenerateTimeToWait();
            GenerateTimeToWork();

            userChanged = true;
        }

        private void SelectFirstZadanRow()
        {
            dataGridViewWorks.Rows[0].Selected = true;
        }

        private void buttonDeleteWork_Click(object sender, EventArgs e)
        {
            String selectedCellValue = dataGridViewWorks.SelectedCells[0].Value.ToString();

            foreach (SeleniumParams selenParam in seleniumParams)
            {
                if (selenParam.ParamName == selectedCellValue)
                {
                    seleniumParams.Remove(selenParam);
                    break;
                }
            }

            UpdateDataGridParam();
            SelectFirstZadanRow();
            currParam = seleniumParams[0];
            UpdateViewSeleniumParamState();
        }

        private void UpdateFindUrlsDataGrid()
        {
            dataGridViewSearchedSites.Rows.Clear();

            foreach (string findUrl in currParam.FindUrl)
            {
                dataGridViewSearchedSites.Rows.Add(findUrl);
            }
        }

        private void UpdateViewSeleniumParamState()
        {
            //view state in page
            textBoxRequest.Text = currParam.Request;
            UpdateFindUrlsDataGrid();


            radioButtonFirefox.Checked = false;
            radioButtonChrome.Checked = false;
            radioButtonYandex.Checked = false;
            radioButtonMobile.Checked = false;

            if (currParam.Browser == BrowserEnums.Browsers.firefox)
                radioButtonFirefox.Checked = true;
            if (currParam.Browser == BrowserEnums.Browsers.chrome)
                radioButtonChrome.Checked = true;
            if (currParam.Browser == BrowserEnums.Browsers.yandex)
                radioButtonYandex.Checked = true;
            if (currParam.Browser == BrowserEnums.Browsers.mobile)
                radioButtonMobile.Checked = true;
            if (currParam.Browser == BrowserEnums.Browsers.edge)
                radioButtonEdge.Checked = true;
            if (currParam.Browser == BrowserEnums.Browsers.other)
                radioButtonOther.Checked = true;
            if (currParam.Browser == BrowserEnums.Browsers.safari)
                radioButtonSafari.Checked = true;

            radioButtonGoogle.Checked = false;
            radioButtonYanex.Checked = false;
            radioButtonDuckDuckGo.Checked = false;

            if (currParam.FinderUrl.Contains("google"))
                radioButtonGoogle.Checked = true;
            if (currParam.FinderUrl.Contains("ya"))
                radioButtonYanex.Checked = true;
            if (currParam.FinderUrl.Contains("duckduckgo"))
                radioButtonDuckDuckGo.Checked = true;

            dataGridViewExcplicitDomain.Rows.Clear();
            foreach (string explicitDomain in currParam.ExplicitDomain)
            {
                dataGridViewExcplicitDomain.Rows.Add(explicitDomain);
            }

            checkBoxExplicitDomain.Checked = false;
            checkBoxGotoPageAndWait.Checked = false;
            checkBoxClickAndRun.Checked = false;

            if (currParam.ExplicitDomain.Count != 0)
                checkBoxExplicitDomain.Checked = true;
            if (currParam.GotoPageAndRun)
                checkBoxClickAndRun.Checked = true;
            if (currParam.GotoPageAndWait)
                checkBoxGotoPageAndWait.Checked = true;
            if (currParam.GotoPageAndRunNext)
                checkBoxExplicitDomain.Checked = true;

            checkBoxJS.Checked = false;
            checkBoxCookie.Checked = false;

            if (currParam.UseJS)
                checkBoxJS.Checked = true;
            if (currParam.UseCookie)
                checkBoxCookie.Checked = true;


            checkBoxTextLog.Checked = false;
            checkBoxImageLog.Checked = false;

            if (currParam.UseTextLog)
                checkBoxTextLog.Checked = true;
            if (currParam.UseImageLog)
                checkBoxImageLog.Checked = true;

            maskedTextBox1.Text = currParam.TimeWork.ToString();
            maskedTextBox4.Text = currParam.TimeInSite.ToString();

            maskedTextBoxProxyIp.Text = currParam.ProxyIP.IPAddress.ToString();
            numericUpDownProxyPort.Value = currParam.ProxyPort.IPEndPoint.Port;
            textBoxProxyUsername.Text = currParam.ProxyLogin;
            textBoxProxyPort.Text = currParam.ProxyPassword;
            comboBoxProxyType.Text = currParam.ProxyType;

            textBoxUserAgent.Text = currParam.UserAgent;

            dateTimePicker1.Value = currParam.TimeStart;

            numericUpDownTimeWaitElem.Value = currParam.TimeToWaitSiteAndElement;

            checkBoxTimeToWait.Checked = currParam.TimeWorkAuto;
            checkBoxSecondOnSite.Checked = currParam.TimeInSiteAuto;

            numericUpDownWaitNextPageMin.Value = currParam.TimeToWaitNextPageMin;
            numericUpDownWaitNextPageMax.Value = currParam.TimeToWaitNextPageMax;

            numericUpDownTimeWaitRecaptcha.Value = currParam.TimeToWaitRecaptcha;

            textBoxGoogleEnd.Text = currParam.GoogleEnd;
            textBoxYandexEnd.Text = currParam.YandexEnd;
            textBoxDuckduckGoEnd.Text = currParam.DuckduckGoEnd;

            numericUpDownResX.Value = currParam.ResX;
            numericUpDownResY.Value = currParam.ResY;
        }

        private void dataGridViewWorks_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            userChanged = false;
            //save curr state
            for (int i = 0; i < seleniumParams.Count; i++)
            {
                if (seleniumParams[i].ParamName == currParam.ParamName)
                {
                    seleniumParams.RemoveAt(i);
                    seleniumParams.Insert(i, currParam);
                    break;
                }
            }

            //load selected item state
            if (dataGridViewWorks.SelectedCells.Count != 0)
            {
                string selectedParamName = dataGridViewWorks.SelectedCells[0].Value.ToString();
                for (int i = 0; i < seleniumParams.Count; i++)
                {
                    if (seleniumParams[i].ParamName == selectedParamName)
                    {
                        currParam = seleniumParams[i];
                        break;
                    }
                }
            }

            UpdateViewSeleniumParamState();

            userChanged = true;
        }

        private void WaitForTime(SeleniumParams param)
        {
            var spin = new SpinWait();
            DateTime b = new DateTime(param.TimeStart.Year, param.TimeStart.Month, param.TimeStart.Day, param.TimeStart.Hour, param.TimeStart.Minute, 0, DateTimeKind.Utc);
            while (true)
            {
                if (DateTime.Now >= b)
                    break;
                spin.SpinOnce();
            }
            //await Task.Delay((int)(b - DateTime.Now).TotalMilliseconds);
        }

        private void RunTask(SeleniumParams param)
        {
            WaitForTime(param);

            SeleniumWorker seleniumWorker = new SeleniumWorker(param);
            seleniumWorker.RequestFindResult();
            try
            {
                seleniumWorker.RunTask();
            }
            finally
            {
                seleniumWorker.Exit();
            }
        }

        private bool CheckParamOnCorrect()
        {
            if (string.IsNullOrWhiteSpace(textBoxRequest.Text))
            {
                MessageBox.Show(this, "Не введено значение парамертра поиска!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            //if (string.IsNullOrWhiteSpace(textBoxDomain.Text) || (textBoxDomain.Text == "*."))
            if (dataGridViewSearchedSites.Rows.Count == 1)
            {
                MessageBox.Show(this, "Не введен ни один искомый домен!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        private void запуститьЗаданиеПоочередноToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CheckParamOnCorrect())
            {

                foreach (SeleniumParams param in seleniumParams)
                {
                    RunTask(param);
                }

                MessageBox.Show(this, "Все задания выполнены!", "Выполнено", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void запуститьЗаданияПараллельноToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CheckParamOnCorrect())
            {
                List<Task> taskList = new List<Task>();
                foreach (SeleniumParams param in seleniumParams)
                {
                    Action<object> action = (object obj) =>
                    {
                        RunTask(param);
                    };

                    taskList.Add(new Task(action, param.ParamName));
                    taskList.Last().Start();
                }


                bool allEnd = false;

                while (!allEnd)
                {
                    for (int i = 0; i < taskList.Count; i++)
                    {
                        if (taskList[i].Status == TaskStatus.Running)
                            break;
                        allEnd = true;
                    }
                }

                MessageBox.Show(this, "Все задания выполнены!", "Выполнено", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void textBoxRequest_TextChanged(object sender, EventArgs e)
        {
            if (userChanged)
                currParam.Request = textBoxRequest.Text;
        }

        private void textBoxDomain_TextChanged(object sender, EventArgs e)
        {
            //if (userChanged)
            //    currParam.FindUrl = textBoxDomain.Text;
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (userChanged)
                if (radioButtonFirefox.Checked)
                {
                    currParam.Browser = BrowserEnums.Browsers.firefox;
                    textBoxUserAgent.Text = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:93.0) Gecko/20100101 Firefox/93.0";

                    if (desktopResolutions != null)
                    {
                        currParam = ChangeDesktopResolution(currParam);
                    }
                    UpdateResolutionView();
                }
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            if (userChanged)
                if (radioButtonChrome.Checked)
                {
                    currParam.Browser = BrowserEnums.Browsers.chrome;
                    textBoxUserAgent.Text = "";

                    if (desktopResolutions != null)
                    {
                        currParam = ChangeDesktopResolution(currParam);
                    }
                    UpdateResolutionView();
                }
        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            if (userChanged)
                if (radioButtonYandex.Checked)
                { 
                    currParam.Browser = BrowserEnums.Browsers.yandex;
                    textBoxUserAgent.Text = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/93.0.4577.82 YaBrowser/21.9.0.1052 Yowser/2.5 Safari/537.36";

                    if (desktopResolutions != null)
                    {
                        currParam = ChangeDesktopResolution(currParam);
                    }
                    UpdateResolutionView();
                }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (userChanged)
                if (radioButtonGoogle.Checked)
                {
                    currParam.FinderUrl = "http:\\\\google";
                    textBoxGoogleEnd.Enabled = true;
                    textBoxYandexEnd.Enabled = false;
                    textBoxDuckduckGoEnd.Enabled = false;
                }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (userChanged)
                if (radioButtonYanex.Checked)
                {
                    currParam.FinderUrl = "http:\\\\yandex";
                    textBoxGoogleEnd.Enabled = false;
                    textBoxYandexEnd.Enabled = true;
                    textBoxDuckduckGoEnd.Enabled = false;
                }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (userChanged)
                if (radioButtonDuckDuckGo.Checked)
                {
                    currParam.FinderUrl = "http:\\\\duckduckgo";
                    textBoxGoogleEnd.Enabled = false;
                    textBoxYandexEnd.Enabled = false;
                    textBoxDuckduckGoEnd.Enabled = true;
                }
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            if (userChanged)
                currParam.ProxyLogin = textBoxProxyUsername.Text;
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            if (userChanged)
                currParam.ProxyPassword = textBoxProxyPort.Text;
        }

        private void checkBoxCookie_CheckedChanged(object sender, EventArgs e)
        {
            if (userChanged)
                currParam.UseCookie = checkBoxCookie.Checked;
        }

        private void checkBoxJS_CheckedChanged(object sender, EventArgs e)
        {
            if (userChanged)
                currParam.UseJS = checkBoxJS.Checked;
        }

        private void checkBoxTextLog_CheckedChanged(object sender, EventArgs e)
        {
            if (userChanged)
                currParam.UseTextLog = checkBoxTextLog.Checked;
        }

        private void checkBoxImageLog_CheckedChanged(object sender, EventArgs e)
        {
            if (userChanged)
                currParam.UseImageLog = checkBoxImageLog.Checked;
        }

        private void checkBoxClickAndRun_CheckedChanged(object sender, EventArgs e)
        {
            
            if (userChanged)
            {
                if (checkBoxClickAndRun.Checked)
                {
                    checkBoxGotoPageAndWait.Checked = false;
                    currParam.GotoPageAndWait = false;
                }
                currParam.GotoPageAndRun = checkBoxClickAndRun.Checked;
            }
        }

        private void checkBoxGotoPageAndWait_CheckedChanged(object sender, EventArgs e)
        {
            
            if (userChanged)
            {
                if (checkBoxGotoPageAndWait.Checked)
                {
                    checkBoxClickAndRun.Checked = false;
                    currParam.GotoPageAndRun = false;
                }

                currParam.GotoPageAndWait = checkBoxGotoPageAndWait.Checked;
            }
        }

        private void checkBoxExplicitDomain_CheckedChanged(object sender, EventArgs e)
        {
            if (userChanged) 
                currParam.GotoPageAndRunNext = checkBoxExplicitDomain.Checked;
        }

        private void UpdateExplicitDomain()
        {
            if (userChanged)
            {
                List<string> explicitDomain = new List<string>();
                for (int i = 0; i < dataGridViewExcplicitDomain.Rows.Count - 1; i++)
                {
                    if (dataGridViewExcplicitDomain.Rows[i].Cells["domain"].Value != null)
                        if (!string.IsNullOrWhiteSpace(dataGridViewExcplicitDomain.Rows[i].Cells["domain"].Value.ToString()))
                            explicitDomain.Add(dataGridViewExcplicitDomain.Rows[i].Cells["domain"].Value.ToString());
                }
                currParam.ExplicitDomain = explicitDomain;
            }
        }

        private void dataGridViewExcplicitDomain_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            UpdateExplicitDomain();
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void comboBoxProxyType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (userChanged)
                currParam.ProxyType = comboBoxProxyType.Text;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (userChanged)
                currParam.ProxyPort.IPEndPoint = new System.Net.IPEndPoint(currParam.ProxyIP.IPAddress, (int)numericUpDownProxyPort.Value);
        }

        private bool TryReadUserAgentTxt(string filename)
        {
            try
            {
                userAgents = File.ReadAllLines(filename);
                buttonUpdateUserAgent.Enabled = true;
                buttonUpdateUserAgent.Visible = true;
                buttonUpdateUserAgents.Visible = true;
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(this, string.Format("Не удалось считать файл {0}\nОшибка:{1}", filename, e.Message), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void TryReadResolutions()
        {
            try
            {
                ReadDesktopResolutions("Desktop_resolution.txt");
            }
            catch (Exception e)
            {
                MessageBox.Show(this, string.Format("Ошибка считывания файла Desktop_resolution.txt\n{0}", e.Message), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            try
            {
                ReadMobileResolutions("Mobile_resolution.txt");
            }
            catch (Exception e)
            {
                MessageBox.Show(this, string.Format("Ошибка считывания файла Mobile_resolution.txt\n{0}", e.Message), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            userChanged = false;

            TryReadUserAgentTxt("Useragent.txt");
            TryReadResolutions();

            if (MessageBox.Show(this, "Считать предыдущие задания из файла?\n", "Считать?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    if (openFileDialogZadan.ShowDialog() == DialogResult.OK)
                    {
                        seleniumParams = src.Files.FileOperate.ReadAllParams(openFileDialogZadan.FileName);
                        counter = seleniumParams.Count;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, string.Format("Ошибка чтения файла заданий:\nВозможно не подходящий формат файла\n{0}", ex.Message), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            if (seleniumParams != null)
            {
                //TODO update dataGrid
                UpdateDataGridParam();
                currParam = seleniumParams[0];

                UpdateViewSeleniumParamState();

                if (userChanged)
                    comboBoxProxyType.Text = "Без proxy";
            }

            WriteInRegistry();
            userChanged = true;
        }

        private void UpdateResolutionView()
        {
            userChanged = false;
            numericUpDownResX.Value = currParam.ResX;
            numericUpDownResY.Value = currParam.ResY;
            userChanged = true;
        }

        private void radioButtonOtherBrowser_CheckedChanged(object sender, EventArgs e)
        {
            if (userChanged)
            {
                if (radioButtonMobile.Checked)
                {
                    currParam.Browser = BrowserEnums.Browsers.mobile;
                    textBoxUserAgent.Text = "Mozilla/5.0 (Linux; 10) AppleWebKit / 86(KHTML, like Gecko) Chrome / 86.0 Mobile Safari/ 576";
                    if (mobileResolutions != null)
                        currParam = ChangeMobileResolution(currParam);
                }
                UpdateResolutionView();
            }
        }

        private void maskedTextBoxProxyIp_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (userChanged)
                    currParam.ProxyIP.IPAddress = System.Net.IPAddress.Parse(maskedTextBoxProxyIp.Text.Replace(" ", ""));
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, string.Format("Не верный формат IP-адреса!\n{0}", ex.Message), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBoxUserAgent_TextChanged(object sender, EventArgs e)
        {
            if (userChanged)
                currParam.UserAgent = textBoxUserAgent.Text;
        }

        private void выбратьФайлДляПроксиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void выбратьФайлДляUserAgenаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void maskedTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (userChanged)
                currParam.TimeWork = Int32.Parse(maskedTextBox1.Text.Replace(" ", "").Trim());
        }

        private void maskedTextBox4_TextChanged(object sender, EventArgs e)
        {
            if (userChanged)
                currParam.TimeInSite = Int32.Parse(maskedTextBox4.Text.Trim().Replace(" ", ""));
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            if (userChanged)
                currParam.TimeStart = dateTimePicker1.Value;
        }

        private void numericUpDown1_ValueChanged_1(object sender, EventArgs e)
        {
            if (userChanged) 
                currParam.TimeToWaitSiteAndElement = (int)numericUpDownTimeWaitElem.Value;
        }

        private void GenerateTimeToWait()
        {
            if (checkBoxTimeToWait.Checked)
            {
                maskedTextBox1.Text = rand.Next(300, 1200).ToString();
            }
        }

        private void GenerateTimeToWork()
        {
            if (checkBoxTimeToWait.Checked)
            {
                maskedTextBox4.Text = rand.Next(1200, 18000).ToString();
            }
        }

        private void checkBoxTimeToWait_CheckedChanged(object sender, EventArgs e)
        {
            if (userChanged)
            {
                if (checkBoxTimeToWait.Checked)
                    GenerateTimeToWait();
                currParam.TimeInSiteAuto = checkBoxTimeToWait.Checked;
                maskedTextBox1.Enabled = !checkBoxTimeToWait.Checked;
            }
        }

        private void checkBoxSecondOnSite_CheckedChanged(object sender, EventArgs e)
        {
            if (userChanged)
            {
                if (checkBoxSecondOnSite.Checked)
                    GenerateTimeToWork();
                currParam.TimeWorkAuto = checkBoxSecondOnSite.Checked;
                maskedTextBox4.Enabled = !checkBoxSecondOnSite.Checked;
            }
        }

        private void radioButtonOther_CheckedChanged(object sender, EventArgs e)
        {
            if (userChanged)
            {
                if (radioButtonOther.Checked)
                {
                    if (userAgents != null)
                    {
                        currParam = ChangeUserAgent(currParam);
                        textBoxUserAgent.Text = currParam.UserAgent;
                    }
                    else
                        MessageBox.Show(this, "Не выбран файл списка userAgent-ов\nЗначение userAgent-а не изменено!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void radioButtonSafari_CheckedChanged(object sender, EventArgs e)
        {
            if (userChanged)
            {
                if (radioButtonSafari.Checked)
                {
                    currParam.UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 12_0_1) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/15.0 Safari/605.1.15";
                    textBoxUserAgent.Text = currParam.UserAgent;

                    if (desktopResolutions != null)
                    {
                        currParam = ChangeDesktopResolution(currParam);
                    }
                    UpdateResolutionView();
                }
            }
        }

        private void radioButtonEdge_CheckedChanged(object sender, EventArgs e)
        {
            if (userChanged)
            {
                if (radioButtonEdge.Checked)
                {
                    currParam.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; Xbox; Xbox One) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/95.0.4638.69 Safari/537.36 Edge/44.18363.8131";
                    textBoxUserAgent.Text = currParam.UserAgent;

                    if (desktopResolutions != null)
                    {
                        currParam = ChangeDesktopResolution(currParam);
                    }
                    UpdateResolutionView();
                }
            }
        }

        private void numericUpDownWaitNextPageMin_ValueChanged(object sender, EventArgs e)
        {
            if (userChanged)
                currParam.TimeToWaitNextPageMin = (int)numericUpDownWaitNextPageMin.Value;
        }

        private void numericUpDownWaitNextPageMax_ValueChanged(object sender, EventArgs e)
        {
            if (userChanged)
                currParam.TimeToWaitNextPageMax = (int)numericUpDownWaitNextPageMax.Value;
        }

        private void buttonCloneTask_Click(object sender, EventArgs e)
        {
            String selectedCellValue = dataGridViewWorks.SelectedCells[0].Value.ToString();
            foreach (SeleniumParams selenParam in seleniumParams)
            {
                if (selenParam.ParamName == selectedCellValue)
                {
                    SeleniumParams changedSelenParam = selenParam.CloneParams(GetNewParamName());
                    if (userAgents != null)
                        changedSelenParam = ChangeUserAgent(changedSelenParam);
                    if (proxyParams != null)
                        changedSelenParam = ChangeProxy(changedSelenParam);
                    if (desktopResolutions != null)
                        if (changedSelenParam.Browser != BrowserEnums.Browsers.mobile)
                            changedSelenParam = ChangeDesktopResolution(changedSelenParam);
                    if (mobileResolutions != null)
                        if (changedSelenParam.Browser == BrowserEnums.Browsers.mobile)
                            changedSelenParam = ChangeMobileResolution(changedSelenParam);

                    seleniumParams.Add(changedSelenParam);
                    break;
                }
            }
            UpdateDataGridParam();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show(this, "Сохранить текущие задания в файл?", "Сохранить?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (saveFileDialogZadan.ShowDialog() == DialogResult.OK)
                    src.Files.FileOperate.WriteAllParams(seleniumParams, saveFileDialogZadan.FileName);
            }
        }

        private void numericUpDown1_ValueChanged_2(object sender, EventArgs e)
        {
            if (userChanged)
                currParam.TimeToWaitRecaptcha = (int) numericUpDownTimeWaitRecaptcha.Value;
        }

        private void textBoxGoogleEnd_TextChanged(object sender, EventArgs e)
        {
            if (userChanged)
                currParam.GoogleEnd = textBoxGoogleEnd.Text;
        }

        private void textBoxYandexEnd_TextChanged(object sender, EventArgs e)
        {
            if (userChanged)
                currParam.YandexEnd = textBoxYandexEnd.Text;
        }

        private void textBoxDuckDuckGoEnd_TextChanged(object sender, EventArgs e)
        {
            if (userChanged)
                currParam.DuckduckGoEnd = textBoxDuckduckGoEnd.Text;
        }

        private void saveZadanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialogZadan.ShowDialog() == DialogResult.OK)
                src.Files.FileOperate.WriteAllParams(seleniumParams, saveFileDialogZadan.FileName);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            currParam = ChangeUserAgent(currParam);
            userChanged = false;
            textBoxUserAgent.Text = currParam.UserAgent;
            userChanged = true;
        }

        private void updateAllUserAgentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void dataGridViewWorks_SelectionChanged(object sender, EventArgs e)
        {
            if (userChanged)
                dataGridViewWorks_CellClick(sender, e as DataGridViewCellEventArgs);
        }

        private void ReadDesktopResolutions(string fileName)
        {
            desktopResolutions = File.ReadAllLines(fileName);
        }

        private void ReadMobileResolutions(string fileName)
        {
            mobileResolutions = File.ReadAllLines(fileName);
        }

        private void дляДесктопаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                ReadDesktopResolutions(openFileDialog1.FileName);
                if (currParam.Browser != BrowserEnums.Browsers.mobile)
                {
                    OfferChangeResOnCurrZadan(true);
                }

                дляДесктопаToolStripMenuItem.Text += " (считано)";
            }
        }

        private void OfferChangeResOnCurrZadan(bool isDesktop)
        {
            if (MessageBox.Show(this, "Изменить разрешение для текущего задания?", "Разрешение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (isDesktop)
                    currParam = ChangeDesktopResolution(currParam);
                else
                    currParam = ChangeMobileResolution(currParam);

                userChanged = false;
                numericUpDownResX.Value = currParam.ResX;
                numericUpDownResY.Value = currParam.ResY;
                userChanged = true;
            }
        }

        private void дляМобильныхToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                ReadMobileResolutions(openFileDialog1.FileName);
                if (currParam.Browser == BrowserEnums.Browsers.mobile)
                {
                    OfferChangeResOnCurrZadan(false);
                }
                дляМобильныхToolStripMenuItem.Text += " (считано)";
            }
        }

        private void numericUpDownResX_ValueChanged(object sender, EventArgs e)
        {
            if (userChanged)
                currParam.ResX = (int)numericUpDownResX.Value;
        }

        private void numericUpDownResY_ValueChanged(object sender, EventArgs e)
        {
            if (userChanged)
                currParam.ResY = (int)numericUpDownResY.Value;
        }

        private void обновитьРасширенияУВсехЗаданийToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ((desktopResolutions != null) && (mobileResolutions != null))
            {
                for (int i = 0; i < seleniumParams.Count; i++)
                {
                    if (seleniumParams[i].ParamName != currParam.ParamName)
                    {
                        if (seleniumParams[i].Browser == BrowserEnums.Browsers.mobile)
                            seleniumParams[i] = ChangeMobileResolution(seleniumParams[i]);
                        else
                            seleniumParams[i] = ChangeDesktopResolution(seleniumParams[i]);
                    }
                    else
                    {
                        userChanged = false;
                        if (currParam.Browser == BrowserEnums.Browsers.mobile)
                            currParam = ChangeMobileResolution(seleniumParams[i]);
                        else
                            currParam = ChangeDesktopResolution(seleniumParams[i]);
                        numericUpDownResX.Value = currParam.ResX;
                        numericUpDownResY.Value = currParam.ResY;
                        userChanged = true;
                    }
                }
            }
            else
            {
                MessageBox.Show(this, "Не выбран файл десктопных или мобильных разрешений!\nНевозможно обновить", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void файлДляUserAgentаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (TryReadUserAgentTxt(openFileDialog1.FileName))
                {
                    labelUserAgentFileName.Text = openFileDialog1.FileName;
                    if (MessageBox.Show(this, "Подставить значение юзерагента в текущее задание?", "Подтвердите подстановку", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        currParam = ChangeUserAgent(currParam);

                        userChanged = false;
                        //Update info in page
                        textBoxUserAgent.Text = currParam.UserAgent;
                        if (currParam.Browser == BrowserEnums.Browsers.firefox)
                            radioButtonFirefox.Checked = true;
                        if (currParam.Browser == BrowserEnums.Browsers.chrome)
                            radioButtonChrome.Checked = true;
                        if (currParam.Browser == BrowserEnums.Browsers.yandex)
                            radioButtonYandex.Checked = true;
                        if (currParam.Browser == BrowserEnums.Browsers.mobile)
                            radioButtonMobile.Checked = true;
                        userChanged = true;
                    }
                    
                }
            }
        }

        private void UpdateViewProxyInfo(SeleniumParams param)
        {
            //Update view info
            maskedTextBoxProxyIp.Text = param.ProxyIP.ToString();
            numericUpDownProxyPort.Value = param.ProxyPort.IPEndPoint.Port;

            textBoxProxyUsername.Text = param.ProxyLogin;
            textBoxProxyPort.Text = param.ProxyPassword;

            comboBoxProxyType.SelectedItem = param.ProxyType;
        }

        private void файлДляПроксиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                proxyParams = File.ReadAllLines(openFileDialog1.FileName);

                labelProxyFileName.Text = openFileDialog1.FileName;

                if (MessageBox.Show(this, "Подставить значение proxy в текущее задание?", "Выбрать для текузего задания?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    userChanged = false;
                    currParam = ChangeProxy(currParam);
                    currParam.ProxyType = currParam.ProxyType;

                    UpdateViewProxyInfo(currParam);

                    userChanged = true;
                }
            }
        }

        private void SaveDefaultValuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.BrowserName = (int)currParam.Browser;

            Properties.Settings.Default.BrowserSizeX = currParam.ResX;
            Properties.Settings.Default.BrowserSizeY = currParam.ResY;
            Properties.Settings.Default.timeToWaitNextPageMax = currParam.TimeToWaitNextPageMax;
            Properties.Settings.Default.timeToWaitNextPageMin = currParam.TimeToWaitNextPageMin;
            Properties.Settings.Default.timeToWaitRecaptcha = currParam.TimeToWaitRecaptcha;
            Properties.Settings.Default.timeToWaitSiteAndElement = currParam.TimeToWaitSiteAndElement;

            Properties.Settings.Default.gotoPageAndGoNext = currParam.GotoPageAndRunNext;
            Properties.Settings.Default.gotoPageAndStart = currParam.GotoPageAndRun;
            Properties.Settings.Default.gotoPageAndWait = currParam.GotoPageAndWait;

            Properties.Settings.Default.googleEnd = currParam.GoogleEnd;
            Properties.Settings.Default.yandexEnd = currParam.YandexEnd;
            Properties.Settings.Default.duckduckgoEnd = currParam.DuckduckGoEnd;

            Properties.Settings.Default.searcher = currParam.FinderUrl;

            Properties.Settings.Default.useCookie = currParam.UseCookie;
            Properties.Settings.Default.useJS = currParam.UseJS;

            Properties.Settings.Default.saveFileLog = currParam.UseTextLog;
            Properties.Settings.Default.saveScreenLog = currParam.UseImageLog;


            Properties.Settings.Default.Save();
        }

        private void buttonUpdateUserAgents_Click(object sender, EventArgs e)
        {
            userChanged = false;
            // (SeleniumParams seleniumParam in seleniumParams)
            for (int i = 0; i < seleniumParams.Count; i++)
            {
                if (seleniumParams[i].ParamName != currParam.ParamName)
                {
                    seleniumParams[i] = ChangeUserAgent(seleniumParams[i]);
                }
                else
                {
                    currParam = ChangeUserAgent(currParam);
                    textBoxUserAgent.Text = currParam.UserAgent;
                }
            }
            userChanged = true;
        }

        private void buttonUpdateProxy_Click(object sender, EventArgs e)
        {
            userChanged = false;
            // (SeleniumParams seleniumParam in seleniumParams)
            for (int i = 0; i < seleniumParams.Count; i++)
            {
                if (seleniumParams[i].ParamName != currParam.ParamName)
                {
                    seleniumParams[i] = ChangeProxy(seleniumParams[i]);
                }
                else
                {
                    currParam = ChangeProxy(currParam);

                    UpdateViewProxyInfo(currParam);
                }
            }
            userChanged = true;
        }

        private void UpdateSearcedSites()
        {
            List<string> findUrl = new List<string>();
            foreach (DataGridViewRow dgRow in dataGridViewSearchedSites.Rows)
            {
                if (dgRow.Cells[0].Value != null)
                    if (!string.IsNullOrWhiteSpace(dgRow.Cells[0].Value.ToString()))
                        findUrl.Add(dgRow.Cells[0].Value.ToString());
            }
            currParam.FindUrl = findUrl;
        }

        private void dataGridViewSearchedSites_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (userChanged)
                UpdateSearcedSites();
        }

        private void dataGridViewSearchedSites_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if (userChanged)
                UpdateSearcedSites();
        }

        private void dataGridViewSearchedSites_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            if (userChanged)
                UpdateSearcedSites();
        }

        private void buttonLoadSearchedSite_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                currParam.FindUrl = File.ReadAllLines(openFileDialog1.FileName).ToList();
                UpdateFindUrlsDataGrid();
            }
        }

        private void созданиеЗаданийToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string[] zadans = File.ReadAllLines(openFileDialog1.FileName);
                foreach (string zadan in zadans)
                {
                    buttonAddWork_Click(sender, e, zadan);
                }
            }
        }
    }
}
