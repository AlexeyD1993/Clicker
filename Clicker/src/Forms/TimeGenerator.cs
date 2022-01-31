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

namespace Clicker.src.Forms
{
    public partial class TimeGenerator : Form
    {
        public List<SeleniumParams> seleniumParams;

        public TimeGenerator(List<SeleniumParams> params1)
        {
            InitializeComponent();
            seleniumParams = params1;
        }

        private void buttonGenerate_Click_1(object sender, EventArgs e)
        {
            for (int i = 0; i < seleniumParams.Count; i++)
            {
                seleniumParams[i].TimeStart = dateTimePickerFirstStart.Value.AddSeconds(i * (new TimeSpan(dateTimePickerInterval.Value.Hour, dateTimePickerInterval.Value.Minute, dateTimePickerInterval.Value.Second).TotalSeconds));
            }
            this.Close();
        }
    }
}
