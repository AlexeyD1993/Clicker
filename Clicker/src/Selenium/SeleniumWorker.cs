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
using System.Threading;
using System.Threading.Tasks;

namespace Clicker.src.Selenium
{
    class SeleniumWorker
    {
        private IWebDriver webDriver;
        Params.SeleniumParams seleniumParams;
        private IWebElement rememberedHrefSite = null;

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

        public void RequestFindResult()
        {
            IWebElement findedStringTextBox = FindSearchTextBox();
            findedStringTextBox.SendKeys(seleniumParams.Request);
            findedStringTextBox.Submit();
        }

        public bool FindRefOnWebPage()
        {
            try
            {
                rememberedHrefSite = webDriver.FindElement(By.PartialLinkText(seleniumParams.FindUrl));
                return true;
            }
            catch
            {
                return false;
            }
        }

        private IWebElement FindNextPageButton()
        {
            IWebElement nextPageButton = null;
            try
            {
                nextPageButton = webDriver.FindElement(By.XPath("/html/body/div[7]/div/div[8]/div[1]/div/div[6]/span[1]/table/tbody/tr/td[12]/a/span[2]"));
            }
            catch
            { }
            if (seleniumParams.FinderUrl.Contains("ya"))
            {
                try
                {
                    nextPageButton = webDriver.FindElement(By.XPath("/html/body/div[3]/div[1]/div[2]/div[1]/div[1]/div[3]/div/a[5]"));
                }
                catch { }
            }
            if (seleniumParams.FinderUrl.Contains("duckduckgo"))
            {
                try
                {
                    nextPageButton = webDriver.FindElement(By.XPath("//*[@id=\"rld - 3\"]"));
                }
                catch { }
            }
            return nextPageButton;
        }

        public void ClickNextPage()
        {
            Thread.Sleep(2000);
            FindNextPageButton().Click();
        }

        public void RunTask()
        {
            if (seleniumParams.GotoPageAndWait)
            {
                rememberedHrefSite.Click();
                Thread.Sleep(seleniumParams.TimeWork * 1000);
            }
            if (seleniumParams.GotoPageAndRun)
            {
                rememberedHrefSite.Click();
                //TODO бродилка по внутренностям страницы
            }
            if (seleniumParams.GotoPageAndRunNext)
            {
                rememberedHrefSite.Click();
                Thread.Sleep(5000);
                //TODO сделать возврат в функцию поиска
            }
        }

        public void Exit()
        {
            webDriver.Quit();
        }

        public SeleniumWorker(Params.SeleniumParams seleniumParams)
        {
            this.seleniumParams = seleniumParams;
            Init();
        }
    }
}
