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
            this.imagePath = Path.ChangeExtension(imagePath, "");
            if (Directory.Exists(imagePath))
                throw new Exception(string.Format("Директория {0} уже существует", this.imagePath));
            else
                Directory.CreateDirectory(this.imagePath);
        }

        public void Add(IWebDriver webDriver, string paramName)
        {
            string fileName = string.Format("{0}-{1}-{2}-{3}_{4}_{5}_{6}-{7}.jpg", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond, paramName);
            Screenshot ss = ((ITakesScreenshot)webDriver).GetScreenshot();
            ss.SaveAsFile(Path.Combine(imagePath, fileName), ScreenshotImageFormat.Jpeg);
            
        }
    }
}
