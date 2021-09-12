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
        private string paramName = "";
        
        private string findUrl = "";
        private string request = "";

        private BrowserEnums.Browsers browser;

        private string finderUrl = "";

        private List<string> explicitDomainList = new List<string>();

        private bool gotoPageAndRunNext = true;
        private bool gotoPageAndWait = false;
        private bool gotoPageAndRun = false;
        private int timeWork = 0;

        private IPAddress proxyIP = IPAddress.Loopback;
        private IPEndPoint proxyPort = new IPEndPoint(IPAddress.Loopback, 80);
        private string proxyLogin = "";
        private string proxyPassword = "";
        private string proxyType = "";

        private string userAgent;

        private bool useJS = true;
        private bool useCookie = true;

        private bool useTextLog = false;
        private bool useImageLog = false;
        private bool useVideoLog = false;

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
        public string ProxyType { get => proxyType; set => proxyType = value; }
    }
}
