using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clicker.src.Searches
{
    public interface ISearches
    {
        IWebElement FindSearchTextBox();
        IWebElement FindNextPageButton();
        void CookieReset();
        System.Collections.ObjectModel.ReadOnlyCollection<IWebElement> FindElements();
        void FocusOnCoockie();
        bool IsFoundPrivacyTools();
        bool IsFoundCookiePolicies();
        void ClickAcceptTerms();
        bool IsEightTermsExist();
        void ClickAcceptEigthTerms();
        string GetPageLinkNameBy(IWebElement elem);

        IWebElement GetWebElemBy(IWebElement elem);

    }
}
