using Clicker.src.Params;
using Clicker.src.Selenium;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Clicker
{
    public partial class Form1 : Form
    {
        private List<SeleniumParams> seleniumParams = new List<SeleniumParams>();
        private SeleniumParams currParam;

        private int counter;

        public Form1()
        {
            InitializeComponent();
            counter = 0;

            buttonAddWork_Click(null, null);
        }

        private void UpdateDataGridParam()
        {
            dataGridViewWorks.Rows.Clear();

            foreach (SeleniumParams selenParam in seleniumParams)
            {
                dataGridViewWorks.Rows.Add(selenParam.ParamName);
            }
        }

        private void buttonAddWork_Click(object sender, EventArgs e)
        {
            counter++;

            SeleniumParams param = new SeleniumParams();
            param.ParamName = "Задание № " + counter;
            currParam = param;

            seleniumParams.Add(param);

            UpdateDataGridParam();
        }

        private void buttonDeleteWork_Click(object sender, EventArgs e)
        {
            String selectedCellValue = dataGridViewWorks.SelectedCells[0].Value.ToString();

            foreach (SeleniumParams selenParam in seleniumParams)
            {
                if (selenParam.ParamName.Contains(selectedCellValue))
                {
                    seleniumParams.Remove(selenParam);
                    break;
                }
            }

            UpdateDataGridParam();
        }

        private void dataGridViewWorks_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //save curr state
            for (int i = 0; i < seleniumParams.Count; i++)
            {
                if (seleniumParams[i].ParamName == currParam.ParamName)
                {
                    seleniumParams.RemoveAt(i);
                    seleniumParams.Insert(i, currParam);
                }
            }

            //load selected item state
            string selectedParamName = dataGridViewWorks.SelectedCells[0].Value.ToString();
            for (int i = 0; i < seleniumParams.Count; i++)
            {
                if (seleniumParams[i].ParamName == selectedParamName)
                {
                    currParam = seleniumParams[i];
                    break;
                }
            }

            //view state in page
            textBoxRequest.Text = currParam.Request;
            textBoxDomain.Text = currParam.FindUrl;
            

            radioButton4.Checked = false;
            radioButton5.Checked = false;
            radioButton6.Checked = false;

            if (currParam.Browser == BrowserEnums.Browsers.firefox)
                radioButton4.Checked = true;
            if (currParam.Browser == BrowserEnums.Browsers.chrome)
                radioButton5.Checked = true;
            if (currParam.Browser == BrowserEnums.Browsers.yandex)
                radioButton6.Checked = true;

            radioButton1.Checked = false;
            radioButton2.Checked = false;
            radioButton3.Checked = false;

            if (currParam.FinderUrl.Contains("google"))
                radioButton1.Checked = true;
            if (currParam.FinderUrl.Contains("ya"))
                radioButton2.Checked = true;
            if (currParam.FinderUrl.Contains("duckduckgo"))
                radioButton3.Checked = true;

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

            maskedTextBoxProxyIp.Text = currParam.ProxyIP.ToString();
            numericUpDownProxyPort.Value = currParam.ProxyPort.Port;
            textBoxProxyUsername.Text = currParam.ProxyLogin;
            textBoxProxyPort.Text = currParam.ProxyPassword;

            
        }

        private void RunTask(SeleniumParams param)
        {
            SeleniumWorker seleniumWorker = new SeleniumWorker(param);
            seleniumWorker.RequestFindResult();
            //while (!seleniumWorker.FindRefOnWebPage())
            //    seleniumWorker.ClickNextPage();

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
            
            if (!radioButton4.Checked &&
                !radioButton5.Checked &&
                !radioButton6.Checked &&
                !radioButtonOtherBrowser.Checked)
            {
                MessageBox.Show(this, "Не выбран браузер для использования!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!radioButton1.Checked &&
                !radioButton2.Checked &&
                !radioButton3.Checked)
            {
                MessageBox.Show(this, "Не выбран поисковик для использования!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                        if (!(taskList[i].IsCompleted || taskList[i].IsFaulted))
                            break;
                        allEnd = true;
                    }
                }

                MessageBox.Show(this, "Все задания выполнены!", "Выполнено", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void textBoxRequest_TextChanged(object sender, EventArgs e)
        {
            currParam.Request = textBoxRequest.Text;
        }

        private void textBoxDomain_TextChanged(object sender, EventArgs e)
        {
            currParam.FindUrl = textBoxDomain.Text;
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton4.Checked)
                currParam.Browser = BrowserEnums.Browsers.firefox;
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton5.Checked)
                currParam.Browser = BrowserEnums.Browsers.chrome;
        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton6.Checked)
                currParam.Browser = BrowserEnums.Browsers.yandex;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
                currParam.FinderUrl = "http:\\\\google.ru";
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
                currParam.FinderUrl = "http:\\\\yandex.ru";
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
                currParam.FinderUrl = "http:\\\\duckduckgo.ru";
        }

        private void maskedTextBox2_Validated(object sender, EventArgs e)
        {
            currParam.ProxyIP = System.Net.IPAddress.Parse(maskedTextBoxProxyIp.Text.Replace(" ", ""));
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            currParam.ProxyLogin = textBoxProxyUsername.Text;
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            currParam.ProxyPassword = textBoxProxyPort.Text;
        }

        private void checkBoxCookie_CheckedChanged(object sender, EventArgs e)
        {
            currParam.UseCookie = checkBoxCookie.Checked;
        }

        private void checkBoxJS_CheckedChanged(object sender, EventArgs e)
        {
            currParam.UseJS = checkBoxJS.Checked;
        }

        private void checkBoxTextLog_CheckedChanged(object sender, EventArgs e)
        {
            currParam.UseTextLog = checkBoxTextLog.Checked;
        }

        private void checkBoxImageLog_CheckedChanged(object sender, EventArgs e)
        {
            currParam.UseImageLog = checkBoxImageLog.Checked;
        }

        private void maskedTextBox4_Validated(object sender, EventArgs e)
        {
            currParam.TimeWork = Int32.Parse(maskedTextBox4.Text);
        }

        private void maskedTextBox1_Validated(object sender, EventArgs e)
        {
            currParam.TimeWork = Int32.Parse(maskedTextBox1.Text);
        }

        private void checkBoxClickAndRun_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxClickAndRun.Checked)
            {
                checkBoxGotoPageAndWait.Checked = false;
                currParam.GotoPageAndWait = false;
            }
            currParam.GotoPageAndRun = checkBoxClickAndRun.Checked;
        }

        private void checkBoxGotoPageAndWait_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxGotoPageAndWait.Checked)
            {
                checkBoxClickAndRun.Checked = false;
                currParam.GotoPageAndRun = false;
            }

            currParam.GotoPageAndWait = checkBoxGotoPageAndWait.Checked;
        }

        private void checkBoxExplicitDomain_CheckedChanged(object sender, EventArgs e)
        {
            currParam.GotoPageAndRunNext = checkBoxExplicitDomain.Checked;
        }

        private void UpdateExplicitDomain()
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
            currParam.ProxyType = comboBoxProxyType.Text;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            currParam.ProxyPort = new System.Net.IPEndPoint(currParam.ProxyIP, (int)numericUpDownProxyPort.Value);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBoxProxyType.Text = "Без proxy";
        }

        private void radioButtonOtherBrowser_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonOtherBrowser.Checked)
                currParam.Browser = BrowserEnums.Browsers.mobile;
        }
    }
}
