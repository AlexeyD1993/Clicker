using Clicker.src.Logger;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clicker.src.Searches
{
    class DuckDuckGo : ISearches
    {
        private IWebDriver webDriver;
        private LoggerWorker log;

        public DuckDuckGo(IWebDriver webDriver, LoggerWorker log)
        {
            this.webDriver = webDriver;
            this.log = log;
        }

        public void ClickAcceptEigthTerms()
        {  
        }

        public void ClickAcceptTerms()
        {
        }

        public void CookieReset()
        {
            //Ничего делать не надо
        }

        public ReadOnlyCollection<IWebElement> FindElements()
        {
            return webDriver.FindElements(By.XPath(".//h2/a"));
        }

        public IWebElement FindNextPageButton()
        {
            try
            {
                return webDriver.FindElement(By.ClassName("result--more"));
            }
            catch
            {
                try
                {
                    return webDriver.FindElement(By.XPath("//*[@id=\"rld - 3\"]"));
                }
                catch
                {
                    try
                    {
                        return webDriver.FindElement(By.PartialLinkText("Next"));
                    }
                    catch
                    {
                        try
                        {
                            return webDriver.FindElement(By.XPath("//*[@id=\"links\"]/div[*]/form"));
                        }
                        catch
                        {
                            log.Add("Закончен поиск по страницам поисковика. Искомых сайтов не найдено", webDriver);
                            return null;
                        }
                    }
                }
            }
            log.Add("Кнопка перехода на следующую страницу найдена", webDriver);
        }

        public IWebElement FindSearchTextBox()
        {
            return webDriver.FindElement(By.XPath("//*[@id=\"search_form_input_homepage\"]"));
        }

        public void FocusOnCoockie()
        {
        }

        public string GetPageLinkNameBy(IWebElement elem)
        {
            return GetWebElemBy(elem).GetAttribute("href");
        }

        public IWebElement GetWebElemBy(IWebElement elem)
        {
            return elem;// elem.FindElement(By.TagName("a"));
        }

        public bool IsEightTermsExist()
        {
            return false;
        }

        public bool IsFoundCookiePolicies()
        {
            return false;
        }

        public bool IsFoundPrivacyTools()
        {
            return false;
        }
    }
}
