using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Clicker.src.Params
{
    class SeleniumParams
    {
        private string paramName;
        
        private string findUrl;
        private string request;

        private BrowserEnums.Browsers browser;

        private string finderUrl;

        private List<string> explicitDomainList;

        private bool gotoPageAndRunNext;
        private bool gotoPageAndWait;
        private bool gotoPageAndRun;
        private int timeWork;

        private IPAddress proxyIP;
        private IPEndPoint proxyPort;
        private string proxyLogin;
        private string proxyPassword;

        private string userAgent;

        private bool useJS;
        private bool useCookie;

        private bool useTextLog;
        private bool useImageLog;
        private bool useVideoLog;

        public BrowserEnums.Browsers Browser { get => browser; set => browser = value; }
        public string FinderUrl { get => finderUrl; set => finderUrl = value; }
        public IPAddress ProxyIP { get => proxyIP; set => proxyIP = value; }
        public IPEndPoint ProxyPort { get => proxyPort; set => proxyPort = value; }
        public string FindUrl { get => findUrl; set => findUrl = value; }
        public string Request { get => request; set => request = value; }
        public string ParamName { get => paramName; set => paramName = value; }
        public string ProxyLogin { get => proxyLogin; set => proxyLogin = value; }
        public string ProxyPassword { get => proxyPassword; set => proxyPassword = value; }
        public List<string> ExplicitDomain { get => explicitDomainList; set => explicitDomainList = value; }
        public bool GotoPageAndWait { get => gotoPageAndWait; set => gotoPageAndWait = value; }
        public bool GotoPageAndRun { get => gotoPageAndRun; set => gotoPageAndRun = value; }
        public int TimeWork { get => timeWork; set => timeWork = value; }
        public bool UseJS { get => useJS; set => useJS = value; }
        public bool UseCookie { get => useCookie; set => useCookie = value; }
        public string UserAgent { get => userAgent; set => userAgent = value; }
        public bool UseTextLog { get => useTextLog; set => useTextLog = value; }
        public bool UseImageLog { get => useImageLog; set => useImageLog = value; }
        public bool UseVideoLog { get => useVideoLog; set => useVideoLog = value; }
        public bool GotoPageAndRunNext { get => gotoPageAndRunNext; set => gotoPageAndRunNext = value; }
        
    }
}
