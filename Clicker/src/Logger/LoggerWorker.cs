using Clicker.src.Params;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clicker.src.Logger
{
    class LoggerWorker
    {
        private SeleniumParams param;

        TextLogger textLogger = null;
        ImageLogger imageLogger = null;
        VideoLogger videoLogger = null;

        private void Initialize()
        {
            string logName = string.Format("{0}-{1}-{2}-{3}_{4}_{5}_{6}.log", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, param.ParamName);

            if (param.UseTextLog)
            {
                textLogger = new TextLogger(logName);
            }

            if (param.UseImageLog)
            {
                imageLogger = new ImageLogger(logName);
            }

            if (param.UseVideoLog)
            {
                videoLogger = new VideoLogger();
            }

        }

        public LoggerWorker(SeleniumParams param)
        {
            this.param = param;
            Initialize();
        }

        public void Add(string message, IWebDriver webDriver)
        {
            if (textLogger != null)
                textLogger.Add(message);
            if (imageLogger != null)
                imageLogger.Add(webDriver, param.ParamName);
            if (videoLogger != null)
            { 
                //TODO Доделать запись 
            }
        }

        public void AddText(string message)
        {
            if (textLogger != null)
                textLogger.Add(message);
        }
    }
}
