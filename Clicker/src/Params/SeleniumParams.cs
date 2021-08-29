using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clicker.src.Params
{
    class SeleniumParams
    {
        private BrowserEnums.Browsers browser;
        private string finderUrl;
        private string proxyIp;
        private string proxyPort;
        private string findUrl;
        private string request;

        public BrowserEnums.Browsers Browser { get => browser; set => browser = value; }
        public string FinderUrl { get => finderUrl; set => finderUrl = value; }
        public string ProxyIp { get => proxyIp; set => proxyIp = value; }
        public string ProxyPort { get => proxyPort; set => proxyPort = value; }
        public string FindUrl { get => findUrl; set => findUrl = value; }
        public string Request { get => request; set => request = value; }
    }
}
