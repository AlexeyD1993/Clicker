using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Clicker.src.Params
{
    [XmlRoot("SeleniumParam")]
    [Serializable]
    public class SeleniumParams
    {
        public SeleniumParams CloneParams(string paramName)
        {
            SeleniumParams clonedParams = new SeleniumParams();

            clonedParams.paramName = paramName;
            clonedParams.finderUrl = this.finderUrl;
            clonedParams.findUrl = new List<string>(this.findUrl);
            clonedParams.request = this.request;
            clonedParams.browser = this.browser;
            clonedParams.explicitDomainList = new List<string>(this.explicitDomainList);
            clonedParams.gotoPageAndRun = this.gotoPageAndRun;
            clonedParams.gotoPageAndRunNext = this.gotoPageAndRunNext;
            clonedParams.gotoPageAndWait = this.gotoPageAndWait;
            clonedParams.timeWork = this.timeWork;
            clonedParams.timeWorkAuto = this.timeWorkAuto;
            clonedParams.timeInSite = this.timeInSite;
            clonedParams.timeInSiteAuto = this.timeInSiteAuto;
            clonedParams.proxyIP.IPAddress = IPAddress.Parse(this.proxyIP.IPAddress.ToString());
            clonedParams.proxyPort.IPEndPoint = new IPEndPoint(proxyIP.IPAddress, this.proxyPort.IPEndPoint.Port);
            clonedParams.proxyLogin = this.proxyLogin;
            clonedParams.proxyPassword = this.proxyPassword;
            clonedParams.proxyType = this.proxyType;
            clonedParams.userAgent = this.userAgent;
            clonedParams.useJS = this.useJS;
            clonedParams.useCookie = this.useCookie;
            clonedParams.useTextLog = this.useTextLog;
            clonedParams.useImageLog = this.useImageLog;
            clonedParams.useVideoLog = this.useVideoLog;
            clonedParams.timeStart = DateTime.Parse(this.timeStart.ToString());
            clonedParams.timeToWaitSiteAndElement = this.timeToWaitSiteAndElement;
            clonedParams.timeToWaitNextPageMin = this.timeToWaitNextPageMin;
            clonedParams.timeToWaitNextPageMax = this.timeToWaitNextPageMax;
            clonedParams.googleEnd = this.googleEnd;
            clonedParams.yandexEnd = this.yandexEnd;
            clonedParams.duckduckGoEnd = this.duckduckGoEnd;
            clonedParams.timeToWaitRecaptcha = this.timeToWaitRecaptcha;
            clonedParams.resX = this.resX;
            clonedParams.resY = this.resY;
            clonedParams.isAll = this.isAll;
            clonedParams.minBypass = this.minBypass;
            clonedParams.maxByPass = this.maxByPass;
            clonedParams.isEnd = SeleniumStatusWork.Status.NotRunning; //Склонированное задание должно отображаться новым заданием!
            return clonedParams;
        }

        private string paramName = "";
        
        private List<string> findUrl = new List<string>();
        private string request = "";

        private BrowserEnums.Browsers browser = (BrowserEnums.Browsers)Properties.Settings.Default.BrowserName;//BrowserEnums.Browsers.other;

        private string finderUrl = "http:\\\\google";

        private List<string> explicitDomainList = new List<string>();

        private bool gotoPageAndRunNext = Properties.Settings.Default.gotoPageAndGoNext;
        private bool gotoPageAndWait = Properties.Settings.Default.gotoPageAndWait;
        private bool gotoPageAndRun = Properties.Settings.Default.gotoPageAndStart;
        private int timeWork = 0;
        private bool timeWorkAuto = true;
        private int timeInSite = 0;
        private bool timeInSiteAuto = true;

        private ToSerializeIP proxyIP = new ToSerializeIP();

        private ToSerializePort proxyPort = new ToSerializePort();//new IPEndPoint(IPAddress.Loopback, 80);
        private string proxyLogin = "";
        private string proxyPassword = "";
        private string proxyType = "Без proxy";

        private string userAgent;

        private bool useJS = Properties.Settings.Default.useJS;
        private bool useCookie = Properties.Settings.Default.useCookie;

        private bool useTextLog = Properties.Settings.Default.saveFileLog;
        private bool useImageLog = Properties.Settings.Default.saveScreenLog;
        private bool useVideoLog = false;

        private DateTime timeStart = DateTime.Now;

        private int timeToWaitSiteAndElement = Properties.Settings.Default.timeToWaitSiteAndElement;

        private int timeToWaitNextPageMin = Properties.Settings.Default.timeToWaitNextPageMin;
        private int timeToWaitNextPageMax = Properties.Settings.Default.timeToWaitNextPageMax;

        private int timeToWaitRecaptcha = Properties.Settings.Default.timeToWaitRecaptcha;

        private string googleEnd = Properties.Settings.Default.googleEnd;
        private string yandexEnd = Properties.Settings.Default.yandexEnd;
        private string duckduckGoEnd = Properties.Settings.Default.duckduckgoEnd;

        private int resX = Properties.Settings.Default.BrowserSizeX;
        private int resY = Properties.Settings.Default.BrowserSizeY;

        private bool isAll = Properties.Settings.Default.useAllSite;
        private int minBypass = 1;
        private int maxByPass = 10;

        private SeleniumStatusWork.Status isEnd = SeleniumStatusWork.Status.NotRunning;

        public BrowserEnums.Browsers Browser { get => browser; set => browser = value; }
        public string FinderUrl { get => finderUrl; set => finderUrl = value; }
        public ToSerializeIP ProxyIP { get => proxyIP; set => proxyIP = value; }
        public ToSerializePort ProxyPort { get => proxyPort; set => proxyPort = value; }
        public List<string> FindUrl { get => findUrl; set => findUrl = value; }
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
        public int TimeInSite { get => timeInSite; set => timeInSite = value; }
        public DateTime TimeStart { get => timeStart; set => timeStart = value; }
        public int TimeToWaitSiteAndElement { get => timeToWaitSiteAndElement; set => timeToWaitSiteAndElement = value; }
        public bool TimeWorkAuto { get => timeWorkAuto; set => timeWorkAuto = value; }
        public bool TimeInSiteAuto { get => timeInSiteAuto; set => timeInSiteAuto = value; }
        public int TimeToWaitNextPageMin { get => timeToWaitNextPageMin; set => timeToWaitNextPageMin = value; }
        public int TimeToWaitNextPageMax { get => timeToWaitNextPageMax; set => timeToWaitNextPageMax = value; }
        public int TimeToWaitRecaptcha { get => timeToWaitRecaptcha; set => timeToWaitRecaptcha = value; }
        public string GoogleEnd { get => googleEnd; set => googleEnd = value; }
        public string YandexEnd { get => yandexEnd; set => yandexEnd = value; }
        public string DuckduckGoEnd { get => duckduckGoEnd; set => duckduckGoEnd = value; }
        public int ResX { get => resX; set => resX = value; }
        public int ResY { get => resY; set => resY = value; }
        public bool IsAll { get => isAll; set => isAll = value; }
        public int MinBypass { get => minBypass; set => minBypass = value; }
        public int MaxByPass { get => maxByPass; set => maxByPass = value; }
        public SeleniumStatusWork.Status IsEnd { get => isEnd; set => isEnd = value; }

        public SeleniumParams()
        {
            proxyIP.IPAddress = IPAddress.Loopback;
            proxyPort.IPEndPoint = new IPEndPoint(IPAddress.Loopback, 80);
        }
    }

    public class ToSerializeIP
    {
        [XmlElement(ElementName = "IPAddress")]
        public string IPAddressAsString
        {
            get { return IPAddress != null ? IPAddress.ToString() : null; }
            set
            {
                IPAddress a;
                if (value != null && IPAddress.TryParse(value, out a))
                    IPAddress = a;
                else
                    IPAddress = null;
            }
        }
        [XmlIgnore]
        public IPAddress IPAddress { get; set; }
    }

    public class ToSerializePort
    {
        [XmlElement(ElementName = "IPEndPoint")]
        public string IPEndPointAsString
        {
            get { return IPEndPoint != null ? IPEndPoint.ToString() : null; }
            set
            {
                if (value != null)
                {
                    string[] ep = value.Split(':');
                    IPAddress ab = null;
                    IPAddress.TryParse(ep[0], out ab);
                    IPEndPoint = new IPEndPoint(ab.Address, Int32.Parse(ep[1]));
                    //IPEndPoint.Address = ab;
                    //IPEndPoint.Port = Int32.Parse(ep[1]);
                }
                else
                    IPEndPoint = null;
            }
        }
        [XmlIgnore]
        public IPEndPoint IPEndPoint { get; set; }
    }
}
