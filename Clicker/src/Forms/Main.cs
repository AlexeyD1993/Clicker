using Clicker.src.Params;
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
        }

        private void UpdateDataGridParam()
        {
            dataGridView1.Rows.Clear();

            foreach (SeleniumParams selenParam in seleniumParams)
            {
                dataGridView1.Rows.Add(selenParam.ParamName);
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
            String selectedCellValue = dataGridView1.SelectedCells[0].Value.ToString();

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
    }
}
