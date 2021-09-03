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
                    seleniumParams[i] = currParam;
            }

            //load selected item state
            string selectedParamName = dataGridViewWorks.SelectedCells[0].Value.ToString();
            for (int i = 0; i < seleniumParams.Count; i++)
            {
                if (seleniumParams[i].ParamName == selectedParamName)
                    currParam = seleniumParams[i];
            }

            //view state in page
            textBoxRequest.Text = currParam.Request;
            textBoxDomain.Text = currParam.FindUrl;

            if (currParam.Browser == BrowserEnums.Browsers.firefox)
                radioButton4.Checked = true;
            if (currParam.Browser == BrowserEnums.Browsers.chrome)
                radioButton5.Checked = true;
            if (currParam.Browser == BrowserEnums.Browsers.yandex)
                radioButton6.Checked = true;

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

            if (currParam.GotoPageAndRun)
                radioButton7.Checked = true;
            if (currParam.GotoPageAndWait)
                radioButton8.Checked = true;
            

        }

        private void запуститьЗаданиеПоочередноToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (SeleniumParams param in seleniumParams)
            {
                SeleniumWorker seleniumWorker = new SeleniumWorker(param);
                seleniumWorker.RequestFindResult();
                while (!seleniumWorker.FindRefOnWebPage())
                    seleniumWorker.ClickNextPage();

                seleniumWorker.RunTask();

                seleniumWorker.Exit();
            }

            MessageBox.Show(this, "Все задания выполнены!", "Выполнено", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void запуститьЗаданияПараллельноToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (SeleniumParams param in seleniumParams)
            {
                
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
    }
}
