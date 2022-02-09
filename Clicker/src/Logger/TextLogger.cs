using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clicker.src.Logger
{
    public class TextLogger
    {
        private string logFileName = "";
        public TextLogger(string logFileName)
        {
            this.logFileName = logFileName;
            if (logFileName != "error.log")
                if (File.Exists(logFileName))
                    throw new Exception(string.Format("Ошибка создания текстового протокола. Протокол с именем {0} уже существует", logFileName));
                else
                {
                    File.CreateText(logFileName).Close();
                }
        }

        public void Add(string message)
        {
            string dateTime = string.Format("{0}-{1}-{2}-{3}:{4}:{5}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            try
            {
                File.AppendAllText(logFileName, string.Format("{0}: {1}{2}", dateTime, message, Environment.NewLine));
            }
            catch
            {
                File.AppendAllText("error.log", string.Format("{0}: {1}: \"{2}{3}\"", "Ошибка добавления текста", dateTime, message, Environment.NewLine));
            }
        }
    }
}
