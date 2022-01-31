using Clicker.src.Logger;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clicker.src.Searches
{
    class Google : ISearches
    {
        private IWebDriver webDriver;
        private LoggerWorker log;

        public Google(IWebDriver webDriver, LoggerWorker log)
        {
            this.webDriver = webDriver;
            this.log = log;
        }

        public void ClickAcceptEigthTerms()
        {
            webDriver.FindElement(By.ClassName("fSXkBc")).Click();
        }

        public void ClickAcceptTerms()
        {
            try
            {
                if (IsFoundPrivacyTools())
                {
                    webDriver.FindElement(By.XPath("/html/body/div[*]/div[*]/form/input[11]")).Click();
                    log.AddText("Нажал кнопку принять условия гугла по первому XPATH");
                }
                else throw new Exception("");
                //MessageBox.Show("Зашли через 1-ый раз", "Показать", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            catch
            {
                try
                {
                    List<IWebElement> webElems = webDriver.FindElements(By.ClassName("basebutton")).ToList();
                    if (webElems.Count == 2)
                    {
                        webElems.Last().Click();
                        log.AddText("Нажал кнопку принять политику гугла по имени класса basebutton");
                    }
                    else
                        throw new Exception("");
                }
                catch
                {
                    try
                    {
                        webDriver.FindElement(By.Id("L2AGLb")).Click();
                        log.AddText("Нажал кнопку принять условия гугла по ID L2AGLb");
                        //MessageBox.Show("Зашли через ID", "Показать", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    }
                    catch
                    {
                        try
                        {
                            webDriver.FindElement(By.XPath("/html/body/div[2]/div[2]/div[3]/span/div/div/div/div[3]/button[2]/div")).Click();
                            log.AddText("Нажал кнопку принять условия гугла по второму XPATH");
                            //MessageBox.Show("Зашли через XPATH", "Показать", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        }
                        catch
                        {
                            try
                            {
                                webDriver.FindElement(By.XPath("/html/body/div[2]/div[2]/div[3]/span/div/div/div/div[3]/button[2]")).Click();
                                log.AddText("Нажал кнопку принять условия гугла по третьему XPATH");
                                //MessageBox.Show("Зашли через XPATH 2", "Показать", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                            }
                            catch
                            {
                                try
                                {
                                    webDriver.FindElement(By.XPath("/html/body/div[2]/div[3]/form/input[11]")).Click();
                                    log.AddText("Нажал кнопку принять условия гугла по четвертому XPATH");
                                }
                                catch
                                {
                                    log.Add("Проблема с поиском элементов на странице принятия условий политики гугла", webDriver);
                                    //MessageBox.Show("Не смог нажать на кнопку ", "Показать", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void CookieReset()
        {
            //throw new NotImplementedException();
        }

        public ReadOnlyCollection<IWebElement> FindElements()
        {
            try
            {
                try
                {
                    return webDriver.FindElements(By.CssSelector("div.g"));
                    //return webDriver.FindElements(By.TagName("a"));
                    //return webDriver.FindElements(By.XPath("//*[@id=\"main\"]/div[*]/div/div[*]/a"));
                }
                catch
                {
                    return webDriver.FindElements(By.PartialLinkText("http"));
                }
            }
            catch
            {
                return webDriver.FindElements(By.XPath("div[*]/div/div[*]/a"));
            }
        }

        public IWebElement FindNextPageButton()
        {
            try
            {
                if (webDriver.PageSource.Contains("Следующая") && !(webDriver.PageSource.Contains("Следующая &gt;") || webDriver.PageSource.Contains("Следующая&nbsp;&gt;")))
                    return webDriver.FindElement(By.XPath("//*[@id=\"pnnext\"]/span[2]"));
                else
                    throw new NotFoundException("Не найдена кнопка \"Следующая страница\"");
            }
            catch
            {
                try
                {
                    if (webDriver.PageSource.Contains("Показать скрытые результаты"))
                        return webDriver.FindElement(By.PartialLinkText("Показать скрытые результаты"));
                    else
                        throw new NotFoundException("Не найдена кнопка \"Показать скрытые результаты\"");
                }
                catch
                {
                    try
                    {
                        if (webDriver.PageSource.Contains("Следующая &gt;") || webDriver.PageSource.Contains("Следующая&nbsp;&gt;"))
                        {
                            return webDriver.FindElement(By.PartialLinkText("Следующая >"));
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
                                return nextList.Last();
                            }
                            else
                                throw new NotFoundException("Не найдена кнопка \">\"");
                        }
                        catch
                        {
                            try
                            {
                                return webDriver.FindElement(By.XPath("//*[@id=\"ofr\"]/i/a"));
                            }
                            catch
                            {
                                log.Add("Законен поиск по страницам поисковика. Искомых сайтов не найдено", webDriver);
                                return null;
                            }
                        }
                    }
                }
            }
            log.Add("Кнопка перехода на следующую страницу найдена", webDriver);
        }

        public IWebElement FindSearchTextBox()
        {
            return webDriver.FindElement(By.Name("q"));
        }

        public void FocusOnCoockie()
        {
            new Actions(webDriver).MoveToElement(webDriver.FindElement(By.Id("S3BnEe"))).Perform();
        }

        public bool IsEightTermsExist()
        {
            return webDriver.PageSource.Trim().ToLower().Contains("<div class=\"x62GJd\">".Trim().ToLower());
        }

        public bool IsFoundCookiePolicies()
        {
            if (webDriver.PageSource.Contains("productLogoContainer"))
                return true;
            else
                return false;
        }

        public bool IsFoundPrivacyTools()
        {
            if (webDriver.PageSource.Contains("g.co/privacytools"))
                return true;
            else
                return false;
        }
    }
}
