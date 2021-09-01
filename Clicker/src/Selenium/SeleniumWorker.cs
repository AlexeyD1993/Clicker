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
                webDriver = new FirefoxDriver(Properties.Resources.FirefoxDriver);
            if (seleniumParams.Browser == BrowserEnums.Browsers.chrome)
                webDriver = new ChromeDriver(Properties.Resources.ChromeDriver);
            if (seleniumParams.Browser == BrowserEnums.Browsers.yandex)
            {
                webDriver = new ChromeDriver(Properties.Resources.YandexDriver);
            }
            //if (seleniumParams.Browser == BrowserEnums.Browsers.edge)
            //    webDriver = new EdgeDriver();
            //if (seleniumParams.Browser == BrowserEnums.Browsers.ie)
            //    webDriver = new InternetExplorerDriver();

            webDriver.Navigate().GoToUrl(seleniumParams.FinderUrl);

        }

        private IWebElement FindSearchTextBox()
        {
            IWebElement findedStringTextBox = webDriver.FindElement(By.XPath("/html/body/div[1]/div[3]/form/div[1]/div[1]/div[1]/div/div[2]/input"));
            if (seleniumParams.FinderUrl.Contains("ya"))
                findedStringTextBox = webDriver.FindElement(By.XPath("//*[@id=\"text\"]"));
            if (seleniumParams.FinderUrl.Contains("duckduckgo"))
                findedStringTextBox = webDriver.FindElement(By.XPath("//*[@id=\"search_form_input_homepage\"]"));

            return findedStringTextBox;
        }

        //private void ClickSearchButton()
        //{
        //    IWebElement findedSearchButton = webDriver.FindElement(By.XPath(""));
        //    if (seleniumParams.FinderUrl.Contains("ya"))
        //        findedSearchButton = webDriver.FindElement(By.XPath("/html/body/div[1]/div[2]/div[3]/div/fdpprt/fgn/fwap/dqs/fwap/edvrt/ftgr/edvrt/div/edvrt/dhtaq/div/div/div[1]/div[2]/form/div[2]/button"));
        //    if (seleniumParams.FinderUrl.Contains("duckduckgo"))
        //        findedSearchButton = webDriver.FindElement(By.XPath("//*[@id=\"search_button_homepage\"]"));
            
        //    findedSearchButton.Click();
        //}

        public void RequestFindResult()
        {
            IWebElement findedStringTextBox = FindSearchTextBox();
            findedStringTextBox.SendKeys(seleniumParams.Request);

            findedStringTextBox.Submit();
            //ClickSearchButton();
        }

        public void FindRefOnWebPage()
        {
            try
            {
                webDriver.FindElement(By.PartialLinkText(seleniumParams.FindUrl));
            }
            catch
            { }
        }

        public SeleniumWorker(Params.SeleniumParams seleniumParams)
        {
            this.seleniumParams = seleniumParams;
            Init();
        }
    }
}
