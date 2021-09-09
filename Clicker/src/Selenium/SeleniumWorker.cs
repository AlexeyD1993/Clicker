using Clicker.src.Logger;
using Clicker.src.Params;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
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
                {
                    var firefoxOptions = new FirefoxOptions();
                    FirefoxProfile p = new FirefoxProfile();
                    if (!((seleniumParams.ProxyIP == IPAddress.Loopback) || seleniumParams.ProxyIP == null))
                    {
                        var proxy = new Proxy();
                        proxy.HttpProxy = HttpUtility.UrlEncode(seleniumParams.ProxyLogin) + ':' +
                                          HttpUtility.UrlEncode(seleniumParams.ProxyPassword) + '@' +
                                          seleniumParams.ProxyPort.ToString();
                        
                        firefoxOptions.Proxy = proxy;
                    }
                    if (!seleniumParams.UseJS)
                    {
                        p.SetPreference("javascript.enabled", false);
                    }
                    if (!seleniumParams.UseCookie)
                    {
                        p.SetPreference("network.cookie.cookieBehavior", 2);
                    }
                    firefoxOptions.Profile = p;

                    webDriver = new FirefoxDriver(Properties.Resources.FirefoxDriver, firefoxOptions);
                }
                if (seleniumParams.Browser == BrowserEnums.Browsers.chrome)
                {
                    var chromeOptions = new ChromeOptions();
                    if (seleniumParams.ProxyIP != IPAddress.Loopback)
                    {
                        var proxy = new Proxy();
                        proxy.HttpProxy = HttpUtility.UrlEncode(seleniumParams.ProxyLogin) + ':' +
                                          HttpUtility.UrlEncode(seleniumParams.ProxyPassword) + '@' +
                                          seleniumParams.ProxyPort.ToString();
                        chromeOptions.Proxy = proxy;
                    }
                    if (!seleniumParams.UseJS)
                    {
                        chromeOptions.AddArgument("--disable - javascript");
                    }
                    if (!seleniumParams.UseCookie)
                    {
                        chromeOptions.AddUserProfilePreference("network.cookie.cookieBehavior", 2);
                    }

                    webDriver = new ChromeDriver(Properties.Resources.ChromeDriver, chromeOptions);
                }
                if (seleniumParams.Browser == BrowserEnums.Browsers.yandex)
                {
                    var chromeOptions = new ChromeOptions();

                    //chromeOptions.BinaryLocation = ;
                    webDriver = new ChromeDriver(Properties.Resources.YandexDriver, chromeOptions);

                    //TODO YandexDriver!
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
            IWebElement findedStringTextBox = null;
            if (seleniumParams.FinderUrl.Contains("google"))
                findedStringTextBox = webDriver.FindElement(By.XPath("/html/body/div[1]/div[3]/form/div[1]/div[1]/div[1]/div/div[2]/input"));
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
                try
                {
                    rememberedHrefSite.Click();
                    log.Add("Зашли на сайт", webDriver);
                    Thread.Sleep(seleniumParams.TimeWork * 1000);
                }
                catch (Exception e)
                {
                    log.Add(string.Format("Не могу перейти по ссылке на страницу. {0}", e.Message), webDriver);
                }
            }
            if (seleniumParams.GotoPageAndRun)
            {
                try
                {
                    rememberedHrefSite.Click();
                    log.Add("Зашли на сайт", webDriver);

                    Thread.Sleep(3000);

                    DateTime startDate = DateTime.Now;

                    //бродилка по внутренностям страницы
                    while ((DateTime.Now - startDate).TotalSeconds <= seleniumParams.TimeWork)
                    {
                        var hrefLinks = webDriver.FindElements(By.TagName("a"));
                        Random rand = new Random();
                        int randomId = rand.Next(0, hrefLinks.Count);
                        hrefLinks[randomId].Click();
                        log.Add(string.Format("Зашли на страницу {0}", hrefLinks[randomId].Text), webDriver);
                        Thread.Sleep(1000);
                        webDriver.Navigate().Back();
                    }
                }
                catch (Exception e)
                {
                    log.Add(string.Format("Что-то пошло не так. {0}", e.Message), webDriver);
                }
                
            }
            if (seleniumParams.GotoPageAndRunNext)
            {
                try
                {
                    rememberedHrefSite.Click();
                    log.Add("Зашли на сайт", webDriver);
                    Thread.Sleep(5000);
                    //TODO сделать возврат в функцию поиска
                }
                catch (Exception e)
                {
                    log.Add(string.Format("Что-то пошло не так. {0}", e.Message), webDriver);
                }
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
