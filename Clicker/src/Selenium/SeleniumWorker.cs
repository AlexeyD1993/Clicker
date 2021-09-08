using Clicker.src.Logger;
using Clicker.src.Params;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Clicker.src.Selenium
{
    class SeleniumWorker
    {
        private IWebDriver webDriver;
        Params.SeleniumParams seleniumParams;
        private IWebElement rememberedHrefSite = null;
        private LoggerWorker log = null;

        private void Init()
        {
            log = new LoggerWorker(seleniumParams);
            
            try
            {
                if (seleniumParams.Browser == BrowserEnums.Browsers.firefox)
                    webDriver = new FirefoxDriver(Properties.Resources.FirefoxDriver);
                if (seleniumParams.Browser == BrowserEnums.Browsers.chrome)
                    webDriver = new ChromeDriver(Properties.Resources.ChromeDriver);
                if (seleniumParams.Browser == BrowserEnums.Browsers.yandex)
                {
                    webDriver = new ChromeDriver(Properties.Resources.YandexDriver);
                }
            }
            catch (Exception e)
            {
                log.Add(string.Format("Что-то пошло не так. Ошибка открытия браузера. {0}", e.Message), webDriver);
                MessageBox.Show(e.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            webDriver.Navigate().GoToUrl(seleniumParams.FinderUrl);
            log.Add("Браузер запущен", webDriver);
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
            log.Add("Найдена строка поиска элемента", webDriver);
            findedStringTextBox.SendKeys(seleniumParams.Request);
            log.Add(string.Format("Вписали в строку {0}", seleniumParams.Request), webDriver);
            findedStringTextBox.Submit();
            log.Add("Нажали кнопку поиска результатов", webDriver);
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
            if (seleniumParams.FinderUrl.Contains("google"))
            {
                try
                {
                    nextPageButton = webDriver.FindElement(By.XPath("/html/body/div[7]/div/div[8]/div[1]/div/div[6]/span[1]/table/tbody/tr/td[12]/a/span[2]"));
                    log.Add("Кнопка перехода на следующую страницу найдена", webDriver);
                }
                catch (Exception e)
                {
                    log.Add(string.Format("Не смог найти кнопку перехода на следующую страницу {0}", e.Message), webDriver);
                }
            }
            if (seleniumParams.FinderUrl.Contains("ya"))
            {
                try
                {
                    nextPageButton = webDriver.FindElement(By.XPath("/html/body/div[3]/div[1]/div[2]/div[1]/div[1]/div[3]/div/a[5]"));
                    log.Add("Кнопка перехода на следующую страницу найдена", webDriver);
                }
                catch (Exception e)
                {
                    log.Add(string.Format("Не смог найти кнопку перехода на следующую страницу {0}", e.Message), webDriver);
                }
            }
            if (seleniumParams.FinderUrl.Contains("duckduckgo"))
            {
                try
                {
                    nextPageButton = webDriver.FindElement(By.XPath("//*[@id=\"rld - 3\"]"));
                    log.Add("Кнопка перехода на следующую страницу найдена", webDriver);
                }
                catch (Exception e)
                {
                    log.Add(string.Format("Не смог найти кнопку перехода на следующую страницу {0}", e.Message), webDriver);
                }
            }
            
            return nextPageButton;
        }

        public void ClickNextPage()
        {
            Thread.Sleep(2000);
            try
            {
                FindNextPageButton().Click();
                log.Add("Перешли на следующую страницу", webDriver);
            }
            catch (Exception e)
            {
                log.Add(string.Format("Невозможно перейти на следующую страницу {0}", e.Message), webDriver);
            }
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
