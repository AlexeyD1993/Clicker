using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clicker.src.Logger
{
    class ImageLogger
    {
        private string imagePath= "";

        public ImageLogger(string imagePath)
        {
            this.imagePath = imagePath;
            if (Directory.Exists(imagePath))
                throw new Exception(string.Format("Директория {0} уже существует", imagePath));
            else
                Directory.CreateDirectory(imagePath);
        }

        public void Add(IWebDriver webDriver, string paramName)
        {
            string fileName = string.Format("{0}-{1}-{2}-{3}_{4}_{5}-{6}.png", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, paramName);
            Screenshot ss = ((ITakesScreenshot)webDriver).GetScreenshot();
            ss.SaveAsFile(Path.Combine(imagePath, fileName), ScreenshotImageFormat.Png);
        }
    }
}
