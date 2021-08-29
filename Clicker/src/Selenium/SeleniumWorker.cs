using Clicker.src.Params;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clicker.src.Selenium
{
    class SeleniumWorker
    {
        private IWebDriver webDriver;
        Params.SeleniumParams seleniumParams;

        private void Init()
        {
            if (seleniumParams.Browser == BrowserEnums.Browsers.firefox)
                webDriver = new FirefoxDriver();
            if (seleniumParams.Browser == BrowserEnums.Browsers.chrome)
                webDriver = new ChromeDriver();
            if (seleniumParams.Browser == BrowserEnums.Browsers.edge)
                webDriver = new EdgeDriver();
            if (seleniumParams.Browser == BrowserEnums.Browsers.ie)
                webDriver = new InternetExplorerDriver();

            webDriver.Navigate().GoToUrl(seleniumParams.FinderUrl);

        }

        public void FindRequest()
        {
            IWebElement findedStringTextBox = webDriver.FindElement(By.XPath("/html/body/div[1]/div[3]/form/div[1]/div[1]/div[1]/div/div[2]/input"));
            if (seleniumParams.FinderUrl.Contains("ya"))
                findedStringTextBox = webDriver.FindElement(By.XPath("//*[@id=\"text\"]"));
            if (seleniumParams.FinderUrl.Contains("duckduckgo"))
                findedStringTextBox = webDriver.FindElement(By.XPath("//*[@id=\"search_form_input_homepage\"]"));

            findedStringTextBox.SendKeys(seleniumParams.Request);
        }



        public SeleniumWorker(Params.SeleniumParams seleniumParams)
        {
            this.seleniumParams = seleniumParams;
            Init();
        }
    }
}
