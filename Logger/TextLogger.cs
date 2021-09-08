using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clicker.src.Logger
{
    class TextLogger
    {
        private string logFileName = "";
        public TextLogger(string logFileName)
        {
            this.logFileName = logFileName;
            if (File.Exists(logFileName))
                throw new Exception(string.Format("Ошибка создания текстового протокола. Протокол с именем {0} уже существует", logFileName));
            else
                File.CreateText(logFileName);
        }

        public void Add(string message)
        {
            string dateTime = string.Format("{0}-{1}-{2}-{3}:{4}:{5}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            File.AppendAllText(logFileName, string.Format("{0}: {1}", dateTime, message));
        }
    }
}
