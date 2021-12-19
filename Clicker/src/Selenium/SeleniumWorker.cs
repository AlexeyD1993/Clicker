using Clicker.src.Logger;
using Clicker.src.Params;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Service;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using Keys = OpenQA.Selenium.Keys;

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
                var chromeOptions = new ChromeOptions();
                if (seleniumParams.ProxyIP.IPAddress != IPAddress.Loopback)
                {
                    if (seleniumParams.ProxyType.Contains("http"))
                    {
                        if (string.IsNullOrWhiteSpace(seleniumParams.ProxyLogin))
                            chromeOptions.AddArgument(string.Format("--proxy-server={0}:{1}",
                                                                seleniumParams.ProxyIP.IPAddress.ToString(),
                                                                seleniumParams.ProxyPort.IPEndPoint.Port.ToString()));
                        else
                            chromeOptions.AddArgument(string.Format("--proxy-server={0}:{1}@{2}:{3}",
                                                                                                HttpUtility.UrlEncode(seleniumParams.ProxyLogin),
                                                                                                HttpUtility.UrlEncode(seleniumParams.ProxyPassword),
                                                                                                seleniumParams.ProxyIP.IPAddress.ToString(),
                                                                                                seleniumParams.ProxyPort.IPEndPoint.Port.ToString()));
                        
                    }
                    else if (seleniumParams.ProxyType.Contains("v5"))
                    {
                        if (string.IsNullOrEmpty(seleniumParams.ProxyLogin))
                            chromeOptions.AddArgument(string.Format("--proxy-server=socks5://{0}:{1}",
                                                                seleniumParams.ProxyIP.IPAddress.ToString(),
                                                                seleniumParams.ProxyPort.IPEndPoint.Port.ToString()));
                        else
                            chromeOptions.AddArgument(string.Format("--proxy-server=socks5://{0}:{1}@{2}:{3}",
                                                                    HttpUtility.UrlEncode(seleniumParams.ProxyLogin),
                                                                    HttpUtility.UrlEncode(seleniumParams.ProxyPassword),
                                                                    seleniumParams.ProxyIP.IPAddress.ToString(),
                                                                    seleniumParams.ProxyPort.IPEndPoint.Port.ToString()));
                    }
                    else if (seleniumParams.ProxyType.Contains("v4"))
                    {
                        if (string.IsNullOrEmpty(seleniumParams.ProxyLogin))
                            chromeOptions.AddArgument(string.Format("--proxy-server=socks4://{0}:{1}",
                                                                seleniumParams.ProxyIP.IPAddress.ToString(),
                                                                seleniumParams.ProxyPort.IPEndPoint.Port.ToString()));
                        else
                            chromeOptions.AddArgument(string.Format("--proxy-server=socks4://{0}:{1}@{2}:{3}",
                                                                    HttpUtility.UrlEncode(seleniumParams.ProxyLogin),
                                                                    HttpUtility.UrlEncode(seleniumParams.ProxyPassword),
                                                                    seleniumParams.ProxyIP.IPAddress.ToString(),
                                                                    seleniumParams.ProxyPort.IPEndPoint.Port.ToString()));
                    }
                }
                if (!seleniumParams.UseJS)
                {
                    chromeOptions.AddUserProfilePreference("profile.managed_default_content_settings.javascript", 2);
                    chromeOptions.AddLocalStatePreference("profile.managed_default_content_settings.javascript", 2);
                }
                if (!seleniumParams.UseCookie)
                {
                    chromeOptions.AddUserProfilePreference("network.cookie.cookieBehavior", 2);
                }
                if (!string.IsNullOrEmpty(seleniumParams.UserAgent))
                {
                    chromeOptions.AddArgument(string.Format("user-agent={0}", seleniumParams.UserAgent));
                }

                if (seleniumParams.Browser == BrowserEnums.Browsers.mobile)
                {
                    ChromeMobileEmulationDeviceSettings mobile = new ChromeMobileEmulationDeviceSettings(seleniumParams.UserAgent);
                    try
                    {
                        mobile.Width = seleniumParams.ResX; //Properties.Settings.Default.BrowserSizeY;
                        mobile.Height = seleniumParams.ResY; //Properties.Settings.Default.BrowserSizeX;
                    }
                    catch (Exception e)
                    {
                        throw new Exception(string.Format("Не могу установить разрешение экрана для мобильного браузера!\nПожалуйста, проверьте настройки в файле settings.settings.\n{0}\n{1}", seleniumParams.ParamName, e.Message));
                    }
                    mobile.PixelRatio = 3;
                    mobile.UserAgent = seleniumParams.UserAgent;
                    chromeOptions.EnableMobileEmulation(mobile);
                }

                chromeOptions.AddExcludedArgument("enable-automation");
                chromeOptions.AddAdditionalCapability("useAutomationExtension", false);  // ("excludeSwitches", "enable-automation");

                chromeOptions.BinaryLocation = Properties.Settings.Default.chromeBinaryPath;
                DirectoryInfo di = new DirectoryInfo(Properties.Settings.Default.extensionPath);

                foreach (FileInfo fi in di.EnumerateFiles("*.crx", SearchOption.AllDirectories))
                {
                    chromeOptions.AddExtensions(fi.FullName);
                }
                webDriver = new ChromeDriver(Properties.Resources.ChromeDriver, chromeOptions, new TimeSpan(0, 0, seleniumParams.TimeToWaitSiteAndElement));
                webDriver.Manage().Timeouts().ImplicitWait = new TimeSpan(0, 0, seleniumParams.TimeToWaitSiteAndElement);
                webDriver.Manage().Timeouts().PageLoad = new TimeSpan(0, 0, seleniumParams.TimeWork);
                
                //Выставляем разрешение согласно заданию
                webDriver.Manage().Window.Size = new System.Drawing.Size(seleniumParams.ResX, seleniumParams.ResY);
            }
            catch (Exception e)
            {
                log.Add(string.Format("Что-то пошло не так. Ошибка открытия браузера. {0}", e.Message), webDriver);
                MessageBox.Show(string.Format("Что-то пошло не так! Ошибка открытия браузера\n{0}",e.Message), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw e;
            }

            DeleteCookie();

            try
            {
                string finder = seleniumParams.FinderUrl;
                
                if (seleniumParams.FinderUrl.Contains("google"))
                    finder += seleniumParams.GoogleEnd;

                if (seleniumParams.FinderUrl.Contains("ya"))
                    finder += seleniumParams.YandexEnd;

                if (seleniumParams.FinderUrl.Contains("duckduck"))
                    finder += seleniumParams.DuckduckGoEnd;

                webDriver.Navigate().GoToUrl(finder);
                
                WaitRecapcha();
                
                if (seleniumParams.FinderUrl.Contains("google"))
                    GoogleCookieReset();
                    //TryClickAcceptTerms();
                log.Add("Браузер запущен", webDriver);
            }
            catch (WebDriverException ex)
            {
                MessageBox.Show("Ошибка подключения к сайту! Проверьте настройки сети", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                log.Add("Ошибка подключения к сайту! Проверьте настройки сети", webDriver);
                webDriver.Close();
                throw ex;
            }
            
        }

        private IWebElement FindSearchTextBox()
        {
            IWebElement findedStringTextBox = null;
            if (seleniumParams.FinderUrl.Contains("google"))
                findedStringTextBox = webDriver.FindElement(By.Name("q"));
            if (seleniumParams.FinderUrl.Contains("ya"))
                findedStringTextBox = webDriver.FindElement(By.Name("text"));
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
            WaitForTime(3000);

        }

        private bool LengthCorrect(string urlString)
        {
            if (urlString.Length > 2)
                return true;
            else
                return false;
        }

        private string ParceUrl(string findUrl)
        {
            if (findUrl[0] == '*')
            {
                if (LengthCorrect(findUrl))
                {
                    if (findUrl[1] == '.')
                        return findUrl.Substring(2);
                    else
                        return findUrl.Substring(1);
                }
                else
                    return null;
            }
            else
            {
                if (!findUrl.Contains("://"))
                    return string.Format("://{0}", seleniumParams.FindUrl);
            }
            return null;
        }

        private void WaitForTime(int waitMillisecond)
        {
            var spin = new SpinWait();
            DateTime b = DateTime.Now;
            while (true)
            {
                if ((DateTime.Now - b).TotalMilliseconds >= waitMillisecond)
                    break;
                spin.SpinOnce();
            }
            //await Task.Delay(waitMillisecond);
        }

        public bool FindRefOnWebPage()
        {
            try
            {
                foreach (string findedUrl in seleniumParams.FindUrl)
                {
                    if (webDriver.PageSource.Contains(ParceUrl(findedUrl)))
                    {
                        rememberedHrefSite = webDriver.FindElement(By.PartialLinkText(ParceUrl(findedUrl)));
                        return true;
                    }
                }
                return false;
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
                    if (webDriver.PageSource.Contains("Следующая") && !(webDriver.PageSource.Contains("Следующая &gt;") || webDriver.PageSource.Contains("Следующая&nbsp;&gt;")))
                        nextPageButton = webDriver.FindElement(By.XPath("//*[@id=\"pnnext\"]/span[2]"));
                    else
                        throw new NotFoundException("Не найдена кнопка \"Следующая страница\"");
                }
                catch
                {
                    try
                    {
                        if (webDriver.PageSource.Contains("Показать скрытые результаты"))
                            nextPageButton = webDriver.FindElement(By.PartialLinkText("Показать скрытые результаты"));
                        else
                            throw new NotFoundException("Не найдена кнопка \"Показать скрытые результаты\"");
                    }
                    catch
                    {
                        try 
                        {
                            if (webDriver.PageSource.Contains("Следующая &gt;") || webDriver.PageSource.Contains("Следующая&nbsp;&gt;"))
                            {
                                nextPageButton = webDriver.FindElement(By.PartialLinkText("Следующая >"));
                            }
                            throw new NotFoundException("Не найдена кнопка \"Следующая >\"");
                        }
                        catch
                        {
                            try
                            {
                                if (webDriver.PageSource.Contains(">"))
                                {
                                    List<IWebElement> nextList = webDriver.FindElements(By.PartialLinkText(">")).ToList();
                                    nextPageButton = nextList.Last();
                                }
                                else
                                    throw new NotFoundException("Не найдена кнопка \">\"");
                            }
                            catch
                            {
                                try
                                {
                                    nextPageButton = webDriver.FindElement(By.XPath("//*[@id=\"ofr\"]/i/a"));
                                }
                                catch
                                {
                                    return null;
                                }
                            }
                        }
                    }
                }
                log.Add("Кнопка перехода на следующую страницу найдена", webDriver);
            }
            if (seleniumParams.FinderUrl.Contains("ya"))
            {
                try
                {
                    nextPageButton = webDriver.FindElement(By.XPath("/html/body/div[3]/div[1]/div[2]/div[1]/div[1]/div[3]/div/a[5]"));
                }
                catch
                {
                    try
                    {
                        nextPageButton = webDriver.FindElement(By.PartialLinkText("дальше"));
                        
                    }
                    catch 
                    {
                        return null;
                    }
                }
                log.Add("Кнопка перехода на следующую страницу найдена", webDriver);
            }
            if (seleniumParams.FinderUrl.Contains("duckduckgo"))
            {
                try
                {
                    nextPageButton = webDriver.FindElement(By.ClassName("result--more"));
                }
                catch
                {
                    try
                    {
                        nextPageButton = webDriver.FindElement(By.XPath("//*[@id=\"rld - 3\"]"));
                    }
                    catch
                    {
                        try
                        {
                            nextPageButton = webDriver.FindElement(By.PartialLinkText("Next"));
                        }
                        catch
                        {
                            try
                            {
                                nextPageButton = webDriver.FindElement(By.XPath("//*[@id=\"links\"]/div[*]/form"));
                            }
                            catch
                            {
                                return null;
                            }
                        }
                    }
                }
                log.Add("Кнопка перехода на следующую страницу найдена", webDriver);
            }
            
            return nextPageButton;
        }

        private void WaitRecapcha()
        {
            int tmpTime = seleniumParams.TimeToWaitRecaptcha;
            while (IsRecaptchaExist())
            {
                if (tmpTime == 0)
                    throw new NotFoundException(string.Format("Капча не решена в течении {0} секунд!", seleniumParams.TimeToWaitRecaptcha));
                WaitForTime(1000);
                tmpTime--;
            }
            //if (IsRecaptchaExist())
            //{
            //    WaitForTime(seleniumParams.TimeToWaitRecaptcha * 1000);
            //}
        }

        public bool ClickNextPage()
        {
            try
            {

                ScrollDown();
                log.Add("Спустились в конец страницы", webDriver);
                IWebElement nextPageButton = FindNextPageButton();
                if (nextPageButton != null)
                    nextPageButton.Click();
                else
                {
                    log.Add("Искомая ссылка в поисковике не найдена!", webDriver);
                    throw new Exception("Искомая ссылка в поисковике не найдена!");
                }
                Random rand1 = new Random();
                WaitForTime(rand1.Next(seleniumParams.TimeToWaitNextPageMin * 1000, seleniumParams.TimeToWaitNextPageMax * 1000));
                log.Add("Перешли на следующую страницу", webDriver);
                return true;
            }
            catch (Exception e)
            {
                log.Add(string.Format("Невозможно перейти на следующую страницу {0}", e.Message), webDriver);
            }
            return false;
        }

        private bool IsRecaptchaExist()
        {
            bool result = false;
            if (webDriver.PageSource.Contains("/recaptcha/"))
                result = true;
            return result;
        }

        private System.Collections.ObjectModel.ReadOnlyCollection<IWebElement> FindElements()
        {
            if (seleniumParams.FinderUrl.Contains("google"))
                return webDriver.FindElements(By.PartialLinkText("http"));
            else if (seleniumParams.FinderUrl.Contains("ya"))
                return webDriver.FindElements(By.XPath(".//h2/a"));
            else if (seleniumParams.FinderUrl.Contains("duckduckgo"))
                return webDriver.FindElements(By.XPath(".//h2/a"));
            return null;
        }

        private void GotoLastTab()
        {
            List<string> tabs = new List<string>(webDriver.WindowHandles);
            webDriver.SwitchTo().Window(tabs.Last());
        }

        private void GotoFirstTab()
        {
            List<string> tabs = new List<string>(webDriver.WindowHandles);
            if (tabs.Count > 1)
            {
                webDriver.SwitchTo().Window(tabs.Last()).Close();
            }
            webDriver.SwitchTo().Window(tabs.First());
        }

        private bool GotoPageAndRun()
        {
            //do
            {
                log.Add("Зашли на сайт", webDriver);
                if (FindRefOnWebPage())
                {
                    try
                    {
                        //if (seleniumParams.FinderUrl.Contains("duckduck"))
                        {
                            Actions newTab = new Actions(webDriver);
                            newTab.KeyDown(Keys.Control).Click(rememberedHrefSite).KeyUp(Keys.Control).Build().Perform();
                        }
                        //else
                        //    rememberedHrefSite.Click();
                        GotoLastTab();
                        log.Add("Зашли на сайт", webDriver);

                        WaitForTime(3000);

                        DateTime startDate = DateTime.Now;

                        //бродилка по внутренностям страницы
                        int timeToWorkLocal = 0;
                        if (seleniumParams.TimeInSiteAuto || (seleniumParams.TimeInSite == 0))
                        {
                            Random rnd = new Random();
                            timeToWorkLocal = rnd.Next(1200, 10800);
                        }
                        else
                            timeToWorkLocal = seleniumParams.TimeInSite;
                        while ((DateTime.Now - startDate).TotalSeconds <= timeToWorkLocal)
                        {
                            var hrefLinks = webDriver.FindElements(By.TagName("button"));
                            Random rand = new Random();
                            int randomId = rand.Next(0, hrefLinks.Count);
                            hrefLinks[randomId].Click();
                            log.Add(string.Format("Зашли на страницу {0}", hrefLinks[randomId].Text), webDriver);
                            WaitForTime(2000);
                            webDriver.Navigate().Back();
                        }
                        GotoFirstTab();
                        return true;
                    }
                    catch (Exception e)
                    {
                        log.Add(string.Format("Что-то пошло не так. {0}", e.Message), webDriver);
                        throw e;
                    }
                }
                return false;
            }
            //while (!ClickNextPage());
        }

        private bool GotoPageAndWait()
        {
            //do
            {
                log.Add("Зашли на сайт", webDriver);
                if (FindRefOnWebPage())
                {
                    try
                    {
                        //if (seleniumParams.FinderUrl.Contains("duckduck"))
                        {
                            Actions newTab = new Actions(webDriver);
                            newTab.KeyDown(Keys.Control).Click(rememberedHrefSite).KeyUp(Keys.Control).Build().Perform();
                        }
                        //else
                        //    rememberedHrefSite.Click();
                        GotoLastTab();
                        log.Add("Зашли на сайт", webDriver);
                        //if (!seleniumParams.TimeWorkAuto)
                        //{
                        //}
                        //else
                        WaitForTime(seleniumParams.TimeWork * 1000);
                        GotoFirstTab();
                        return true;
                    }
                    catch (Exception e)
                    {
                        log.Add(string.Format("Что-то пошло не так. {0}", e.Message), webDriver);
                        throw e;
                    }
                }
                else
                    return false;
            }
            //while (!ClickNextPage());
        }

        private void ScrollDown()
        {
            IJavaScriptExecutor jse = (IJavaScriptExecutor)webDriver;
            for (int i = 0; i < 5; i++)
            {
                jse.ExecuteScript("window.scrollBy(0,800)");
                Random rnd = new Random();
                WaitForTime(rnd.Next(1000, 3000));
            }
        }

        private void DeleteCookie()
        {
            webDriver.Manage().Cookies.DeleteCookieNamed("CONSENT");
            webDriver.Manage().Cookies.DeleteCookieNamed("NID");
            webDriver.Manage().Cookies.DeleteCookieNamed("1P_JAR");
            webDriver.Manage().Cookies.DeleteCookieNamed("OGPC");
            webDriver.Manage().Cookies.DeleteCookieNamed("OTZ");
            webDriver.Manage().Cookies.DeleteCookieNamed("UULE");
            webDriver.Manage().Cookies.DeleteCookieNamed("ANID");
        }

        private void AddCookie()
        {
            webDriver.Manage().Cookies.AddCookie(new OpenQA.Selenium.Cookie("CONSENT", "YES+shp.gws-20211018-0-RC1.ru+FX+942"));
            webDriver.Manage().Cookies.AddCookie(new OpenQA.Selenium.Cookie("NID", "511=Mgwn3klaFKUQIekKW_93YQiIUn8F4ibtOzB03TILZohVjfTM_sy-q9qcVJCLP993tS6d9_Y8XMnFjLwQk9laMheufW1tdXE0SLn_GQhNP52IcPObcgrc0e6A1cn1En39mhuOEJRXuH2T4JQaXRmt4oIi4HkfLojE--oCUyIYu9s"));
            webDriver.Manage().Cookies.AddCookie(new OpenQA.Selenium.Cookie("1P_JAR", string.Format("{0}-{1}-{2}-{3}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour)));
            webDriver.Manage().Cookies.AddCookie(new OpenQA.Selenium.Cookie("OGPC", "230377472-1:"));
            webDriver.Manage().Cookies.AddCookie(new OpenQA.Selenium.Cookie("OTZ", "6213120_44_44_123780_40_436260"));
            webDriver.Manage().Cookies.AddCookie(new OpenQA.Selenium.Cookie("UULE", "a+cm9sZTogMQpwcm9kdWNlcjogMTIKdGltZXN0YW1wOiAxNjM1MDkxMjc3OTM4MDAwCmxhdGxuZyB7CiAgbGF0aXR1ZGVfZTc6IDUyMjI5Njc1NgogIGxvbmdpdHVkZV9lNzogMjEwMTIyMjg3Cn0KcmFkaXVzOiA3MjQ2NjIzLjkwMDQzODM3NApwcm92ZW5hbmNlOiA2Cg=="));
        }

        private void TryClickAcceptTerms()
        {
            try
            {
                DeleteCookie();
                AddCookie();
                webDriver.Navigate().Refresh();
            }
            catch
            { 
            }
        }

        private bool IsFoundPrivacyTools()
        {
            if (webDriver.PageSource.Contains("g.co/privacytools"))
                return true;
            else
                return false;
        }

        private bool IsFoundCookiePolicies()
        {
            if (webDriver.PageSource.Contains("policies.google"))
                return true;
            else
                return false;
        }

        private bool GoogleCookieExist()
        {
            if (IsFoundPrivacyTools())
                return true;
            else if (IsFoundCookiePolicies())
                return true;
            else
                return false;
        }

        private void ClickAcceptGogleTerms()
        {
            try
            {
                if (IsFoundPrivacyTools())
                {
                    webDriver.FindElement(By.XPath("/html/body/div[*]/div[*]/form/input[11]")).Click();
                }
                else throw new Exception("");
                //MessageBox.Show("Зашли через 1-ый раз", "Показать", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            catch
            {
                try
                {
                    webDriver.FindElement(By.Id("L2AGLb")).Click();
                    //MessageBox.Show("Зашли через ID", "Показать", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
                catch
                {
                    try
                    {
                        webDriver.FindElement(By.XPath("/html/body/div[2]/div[2]/div[3]/span/div/div/div/div[3]/button[2]/div")).Click();
                        //MessageBox.Show("Зашли через XPATH", "Показать", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    }
                    catch
                    {
                        try
                        {
                            webDriver.FindElement(By.XPath("/html/body/div[2]/div[2]/div[3]/span/div/div/div/div[3]/button[2]")).Click();
                            //MessageBox.Show("Зашли через XPATH 2", "Показать", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        }
                        catch
                        {
                            //MessageBox.Show("Не смог нажать на кнопку ", "Показать", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        }
                    }
                }
            }
        }

        private void FocusOnGoogleCoockie()
        {
            new Actions(webDriver).MoveToElement(webDriver.FindElement(By.Id("S3BnEe"))).Perform();
        }

        private void GoogleCookieReset()
        {
            if (GoogleCookieExist())
            {
                FocusOnGoogleCoockie();
                ScrollDown();
                //MessageBox.Show("Нашли куки", "Показать", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                ClickAcceptGogleTerms();
            }
            //else
                //MessageBox.Show("Не нашел куки", "Показать", MessageBoxButtons.OK, MessageBoxIcon.Hand);
        }

        public void RunTask()
        {
            try
            {
                try
                {
                    GoogleCookieReset();
                }
                catch
                { }
                do
                {
                    WaitRecapcha();

                    if (seleniumParams.GotoPageAndRunNext)
                    {
                        var siteList = FindElements();

                        FindRefOnWebPage();

                        for (int i = 0; i < siteList.Count; i++)
                        {
                            var newSiteList = FindElements();
                            bool isExcplititSite = false;
                            WaitForTime(1000);
                            if (isExcplititSite)
                            {
                                foreach (string excplicitSite in seleniumParams.ExplicitDomain)
                                {
                                    if (newSiteList[i].Text.Contains(excplicitSite))
                                    {
                                        isExcplititSite = true;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                if (rememberedHrefSite != null)
                                {
                                    if (rememberedHrefSite.Text == newSiteList[i].Text)
                                    {
                                        if (seleniumParams.GotoPageAndRun)
                                        {
                                            if (GotoPageAndRun())
                                                break;
                                        }
                                        if (seleniumParams.GotoPageAndWait)
                                        {
                                            if (GotoPageAndWait())
                                                break;
                                        }
                                        return;
                                    }
                                }

                                Actions newTab = new Actions(webDriver);
                                newTab.KeyDown(Keys.Control).Click(newSiteList[i]).KeyUp(Keys.Control).Build().Perform();
                                //newSiteList[i].Click();

                                Random rand = new Random();
                                WaitForTime(rand.Next(1000, 5000));

                                List<string> tabs = new List<string>(webDriver.WindowHandles);
                                if (tabs.Count > 1)
                                {
                                    webDriver.SwitchTo().Window(tabs.Last()).Close();
                                    webDriver.SwitchTo().Window(tabs.First());
                                }
                                else
                                    webDriver.Navigate().Back();
                            }
                        }
                    }
                    if (seleniumParams.GotoPageAndRun)
                    {
                        bool isFound = GotoPageAndRun();
                        if (isFound)
                            break;
                        //return;
                    }
                    if (seleniumParams.GotoPageAndWait)
                    {
                        bool isFound = GotoPageAndWait();
                        if (isFound)
                            break;
                    }

                } while (ClickNextPage());
            }
            catch (Exception mainEx)
            {
                Exit();
                throw mainEx;
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
