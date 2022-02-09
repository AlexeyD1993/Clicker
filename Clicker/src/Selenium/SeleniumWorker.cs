using Clicker.src.Logger;
using Clicker.src.Params;
using Clicker.src.Searches;
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
using System.Net.Sockets;
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
        SeleniumParams seleniumParams;
        private IWebElement rememberedHrefSite = null;
        private ISearches searcer;
        private LoggerWorker log = null;
        private Random rnd = new Random();
        private List<string> loadedCrxFiles = new List<string>();
        private string finder = null;

        private bool IsNetworkWork()
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                //ShowMessageBoxError("Отсутствует или ограниченно физическое подключение к сети\nПроверьте настройки вашего сетевого подключения");
                return false;
            }

            bool isConnected = false;
            try
            {
                using (var tcpClient = new TcpClient())
                {
                    tcpClient.Connect(finder.Replace("https:\\\\", "").Replace("http:\\\\", ""), 443); // google
                    isConnected = tcpClient.Connected;
                    tcpClient.Close();
                }
            }
            catch { }
            if (!isConnected)
            {
                //ShowMessageBoxError("Нет подключения к интернету\nПроверьте ваш фаервол или настройки сетевого подключения");
                return false;
            }
            return true;
        }

        private void WaitForInternetConnection()
        {
            bool isShowedMessage = false;
            while (!IsNetworkWork())
            {
                if (!isShowedMessage)
                    ShowMessageBoxError("Нет подключения к интернету\nПроверьте ваш фаервол или настройки сетевого подключения");
                isShowedMessage = true;
                WaitForTime(60000);
            }
        }

        private void ShowMessageBoxError(string message)
        {
            var thread = new Thread(
            () =>
            {
                MessageBox.Show(message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            });
            thread.Start();
            this.seleniumParams.IsEnd = SeleniumStatusWork.Status.Error;
        }
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
                chromeOptions.AddArgument("no-sandbox");
                chromeOptions.AddAdditionalCapability("useAutomationExtension", false);  // ("excludeSwitches", "enable-automation");

                //Load chrome binary
                if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.chromeBinaryPath))
                {
                    chromeOptions.BinaryLocation = Properties.Settings.Default.chromeBinaryPath;
                }

                //LoadExtensions
                if (loadedCrxFiles.Count != 0)
                {
                    foreach (string crxPath in loadedCrxFiles)
                    {
                        chromeOptions.AddExtensions(crxPath);
                    }
                }

                //Выставляем разрешение согласно заданию
                //chromeOptions.AddArguments(string.Format("--window-size={0},{1}", seleniumParams.ResX, seleniumParams.ResY));

                //Запрещаем все загрузки
                chromeOptions.AddUserProfilePreference("download.default_directory", "NUL");
                //chromeOptions.AddUserProfilePreference("intl.accept_languages", "nl");
                chromeOptions.AddUserProfilePreference("disable-popup-blocking", "true");

                webDriver = new ChromeDriver(Properties.Resources.ChromeDriver, chromeOptions, new TimeSpan(0, 0, seleniumParams.TimeToWaitSiteAndElement));
                webDriver.Manage().Timeouts().ImplicitWait = new TimeSpan(0, 0, seleniumParams.TimeToWaitSiteAndElement);
                webDriver.Manage().Timeouts().PageLoad = new TimeSpan(0, 0, seleniumParams.TimeWork);
                log.Add("Вебдрайвер запущен", webDriver);
                //Выставляем разрешение согласно заданию
                webDriver.Manage().Window.Size = new System.Drawing.Size(seleniumParams.ResX, seleniumParams.ResY);
            }
            catch (Exception e)
            {
                log.Add(string.Format("Что-то пошло не так. Ошибка открытия браузера.\n{0}\n{1}", e.Message, seleniumParams.ParamName), webDriver);
                ShowMessageBoxError(string.Format("Что-то пошло не так! Ошибка открытия браузера!\n{0}\n{1}", e.Message, seleniumParams.ParamName));
                Exit();
                throw e;
            }

            try
            {
                DeleteCookie();
            }
            catch (Exception e)
            {
                log.Add(e.Message, webDriver);
            }

            try
            {
                finder = seleniumParams.FinderUrl;

                if (seleniumParams.FinderUrl.Contains("google"))
                {
                    finder += seleniumParams.GoogleEnd;
                    searcer = new Google(webDriver, log);
                }


                if (seleniumParams.FinderUrl.Contains("ya"))
                {
                    finder += seleniumParams.YandexEnd;
                    searcer = new Yandex(webDriver, log);
                }


                if (seleniumParams.FinderUrl.Contains("duckduck"))
                {
                    finder += seleniumParams.DuckduckGoEnd;
                    searcer = new DuckDuckGo(webDriver, log);
                }

                log.AddText("searcer initialized");
                WaitForInternetConnection();
                webDriver.Navigate().GoToUrl(finder);
                log.AddText("Перешли на страницу " + finder);

                WaitRecapcha();

                WaitForTime(3000);
                CookieReset();
                log.Add("Браузер запущен", webDriver);
            }
            catch (WebDriverException ex)
            {
                ShowMessageBoxError(string.Format("{0}\nОшибка подключения к сайту! Проверьте настройки сети", seleniumParams.ParamName));
                log.Add("Ошибка подключения к сайту! Проверьте настройки сети", webDriver);
                webDriver.Close();
                throw ex;
            }
            
        }

        private IWebElement FindSearchTextBox()
        {
            WaitForInternetConnection();
            return searcer.FindSearchTextBox();
        }

        public void RequestFindResult()
        {
            WaitForInternetConnection();
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
                    return string.Format("://{0}", findUrl);
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
        }

        public bool FindRefOnWebPage()
        {
            WaitForInternetConnection();
            try
            {
                for (int j = 0; j < seleniumParams.FindUrl.Count; j++)
                {
                    string findedUrl = seleniumParams.FindUrl[j];
                    if (webDriver.PageSource.Contains(ParceUrl(findedUrl)))
                    {
                        rememberedHrefSite = webDriver.FindElement(By.PartialLinkText(ParceUrl(findedUrl)));
                        log.Add("На текущей странице найден искомый сайт", webDriver);
                        return true;
                    }
                }
                log.Add("На текущей странице искомые сайты не найдены", webDriver);
                return false;
            }
            catch (Exception ex)
            {
                log.Add(ex.Message, webDriver);
                return false;
            }
        }

        private IWebElement FindNextPageButton()
        {
            WaitForInternetConnection();            
            return searcer.FindNextPageButton();
        }

        private void WaitRecapcha()
        {
            WaitForInternetConnection();
            int tmpTime = seleniumParams.TimeToWaitRecaptcha;
            while (IsRecaptchaExist())
            {
                if (tmpTime == 0)
                    throw new NotFoundException(string.Format("Капча не решена в течении {0} секунд!", seleniumParams.TimeToWaitRecaptcha));
                WaitForTime(1000);
                tmpTime--;
            }
        }

        public bool ClickNextPage()
        {
            try
            {
                WaitForInternetConnection();
                log.AddText("Зашли в функцию \"Нажать на следующую страницу\"");
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
            WaitForInternetConnection();
            return searcer.FindElements();
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

        private void ClickByTypeButton()
        {
            WaitForInternetConnection();
            var hrefLinks = webDriver.FindElements(By.TagName("button"));
            Random rand = new Random();
            int randomId = rand.Next(0, hrefLinks.Count);
            hrefLinks[randomId].Click();
            log.Add(string.Format("Зашли на страницу {0}", hrefLinks[randomId].Text), webDriver);
            WaitForTime(2000);
            webDriver.Navigate().Back();
        }

        private void ClickByTypeA()
        {
            WaitForInternetConnection();
            var hrefLinks = webDriver.FindElements(By.TagName("a"));
            Random rand = new Random();
            int randomId = rand.Next(0, hrefLinks.Count);
            hrefLinks[randomId].Click();
            log.Add(string.Format("Зашли на страницу {0}", hrefLinks[randomId].Text), webDriver);
            WaitForTime(2000);
            webDriver.Navigate().Back();
        }

        private bool GotoPageAndRun()
        {
            //do
            {
                WaitForInternetConnection();
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
                            try
                            {
                                ClickByTypeButton();
                            }
                            catch
                            {
                                try
                                {
                                    ClickByTypeA();
                                }
                                catch (Exception ex)
                                {
                                    log.Add(string.Format("Не могу пройти по ссылке на сайте =(\n{0}", ex.Message), webDriver);
                                }
                            }
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
        }

        private bool GotoPageAndWait()
        {
            //do
            {
                WaitForInternetConnection();
                log.Add("Зашли на сайт", webDriver);
                if (FindRefOnWebPage())
                {
                    try
                    {
                        {
                            Actions newTab = new Actions(webDriver);
                            newTab.KeyDown(Keys.Control).Click(rememberedHrefSite).KeyUp(Keys.Control).Build().Perform();
                        }
                        GotoLastTab();
                        log.Add("Зашли на сайт", webDriver);
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
            return searcer.IsFoundPrivacyTools();
        }

        private bool IsFoundCookiePolicies()
        {
            return searcer.IsFoundCookiePolicies();
        }

        private bool CookieExist()
        {
            if (IsFoundPrivacyTools())
                return true;
            else if (IsFoundCookiePolicies())
                return true;
            else
                return false;
        }

        private void ClickAcceptTerms()
        {
            WaitForInternetConnection();
            searcer.ClickAcceptTerms();
        }

        private void CookieReset()
        {
            if (CookieExist())
            {
                try
                {
                    searcer.FocusOnCoockie();
                }
                catch
                { }
                ScrollDown();
                ClickAcceptTerms();
            }
        }

        private void AcceptEightTerms()
        {
            if (searcer.IsEightTermsExist())
            {
                searcer.ClickAcceptEigthTerms();
            }
        }

        private bool IsExcplicitSite(IWebElement newSiteList)
        {
            bool result = false;
            foreach (string excplicitSite in seleniumParams.ExplicitDomain)
            {
                if (newSiteList.Text.Contains(ParceUrl(excplicitSite)))
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        private bool ContainsInFindedSiteList(string siteName)
        {
            bool result = false;
            foreach (string findedSiteName in seleniumParams.FindUrl)
            {
                if (siteName.Contains(ParceUrl(findedSiteName)))
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        private void GotoPageAndRunNext()
        {
            log.AddText("Зашли в задание \"Зайти на страницу и выйти в течении 5 секунд\"");
            var siteList = FindElements();
            log.AddText("Составил список результатов запроса");
            int tmp;
            if (!seleniumParams.IsAll)
                tmp = rnd.Next(seleniumParams.MinBypass, seleniumParams.MaxByPass);
            else
                tmp = siteList.Count;
            log.AddText(string.Format("Определились с количеством сайтов на которые будем заходить. siteCount={0}", tmp));
            //Если нашли искомый сайт - то в любом случае заходим поочередно на каждую страницу и выходим через 5 секунд
            if (FindRefOnWebPage())
            {
                log.AddText("Нашли искомый сайт на странице");
                foreach (var elem in siteList)
                {
                    IWebElement link = elem.FindElement(By.TagName("a"));
                    string site = link.GetAttribute("href");
                    if (!IsHrefFile(site))
                    {
                        log.AddText("Выбрали " + site);
                        if (!IsExcplicitSite(link))
                        {
                            log.AddText("Этот сайт не входит в сайт-исключение");
                            if (!ContainsInFindedSiteList(link.Text.Trim().Split('?')[0]))
                            {
                                log.AddText("Это не искомый сайт");
                                OpenPageInNewTab(link);
                            }
                            else
                            {
                                log.AddText("Это искомый сайт");
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                log.AddText("На текущей странице не найден искомый сайт");
                List<int> tmpList = new List<int>();
                for (int i = 0; i < tmp; i++)
                {
                    int tmpListSize = tmpList.Count;
                    while (tmpList.Count == tmpListSize)
                    {
                        tmpList.Add(rnd.Next(0, siteList.Count));
                        tmpList = tmpList.Distinct().ToList();
                    }
                }
                tmpList.Sort();
                log.AddText("Сгенерировали список сайтов, на которые будем заходить на текущей странице");
                for (int i = 0; i < tmpList.Count; i++)
                {
                    if (!IsExcplicitSite(siteList[tmpList[i]]))
                    {
                        log.AddText("Выбрали и зашли в него" + siteList[tmpList[i]].Text);
                        //IWebElement link = siteList[tmpList[i]].FindElement(By.TagName("a"));
                        string site = searcer.GetPageLinkNameBy(siteList[tmpList[i]]); //link.GetAttribute("href");
                        if (!IsHrefFile(site))
                            OpenPageInNewTab(searcer.GetWebElemBy(siteList[tmpList[i]]));
                    }
                    siteList = FindElements();
                }
            }

        }

        private void OpenPageInNewTab(IWebElement elem)
        {
            Actions newTab = new Actions(webDriver);
            string ext = elem.Text.Split('?')[0];
            try
            {
                ext = Path.GetExtension(elem.Text.Split('?')[0]);
            }
            catch
            { }

            if (ext != "")
            {
                newTab.KeyDown(Keys.Control).Click(elem).KeyUp(Keys.Control).Build().Perform();

                List<string> tabs = new List<string>(webDriver.WindowHandles);
                //newSiteList[i].Click();
                webDriver.SwitchTo().Window(tabs.Last());
                WaitForTime(rnd.Next(2000, 6000));

                if (tabs.Count > 1)
                {
                    webDriver.SwitchTo().Window(tabs.Last()).Close();
                    webDriver.SwitchTo().Window(tabs.First());
                }
                else
                    webDriver.Navigate().Back();
            }
        }

        public void RunTask()
        {
            try
            {
                try
                {
                    WaitForTime(3000);
                    CookieReset();
                }
                catch
                {
                    log.Add("Ошибка при сбросе куки гугла", webDriver);
                }
                do
                {
                    WaitForInternetConnection();
                    WaitRecapcha();

                    AcceptEightTerms();

                    if (seleniumParams.GotoPageAndRunNext)
                    {
                        log.AddText("Выбрано задание зайти на страницу и выйти в течении 5 секунд");
                        GotoPageAndRunNext();
                    }
                    if (seleniumParams.GotoPageAndRun)
                    {
                        log.AddText("Выбрано задание зайти на страницу и гулять по ней");
                        bool isFound = GotoPageAndRun();
                        if (isFound)
                            break;
                    }
                    if (seleniumParams.GotoPageAndWait)
                    {
                        log.AddText("Выбрано задание зайти на страницу и ждать");
                        bool isFound = GotoPageAndWait();
                        if (isFound)
                            break;
                    }

                } while (ClickNextPage());
                seleniumParams.IsEnd = SeleniumStatusWork.Status.Done;
            }
            catch (Exception mainEx)
            {
                Exit();
                throw mainEx;
            }
        }

        private bool IsUriFile(string site)
        {
            try
            {
                Uri uriFile = new Uri(site);
                return uriFile.IsFile;
            }
            catch
            {
                return false;
            }
        }
        private bool IsLinkContainsExtension(string site)
        {
            List<string> listString = site.Split('/').ToList();
            if (Path.GetExtension(listString.Last()) != "")
            {
                if (!Path.GetExtension(listString.Last()).Contains("htm"))
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        private bool IsHrefFile(string site)
        {
            if (IsUriFile(site))
                return true;

            if (IsLinkContainsExtension(site))
                return true;

            return false;
        }

        public void Exit()
        {
            webDriver.Quit();
        }

        public SeleniumWorker(ref SeleniumParams seleniumParams, List<string> loadedCrxFiles)
        {
            this.seleniumParams = seleniumParams;
            this.loadedCrxFiles = loadedCrxFiles;
            Init();
        }
    }
}
