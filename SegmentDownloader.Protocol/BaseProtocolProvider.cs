using System;
using System.Net;
using SegmentDownloader.Core;

namespace SegmentDownloader.Protocol
{
    public class BaseProtocolProvider
    {
        public const int Timeout = 30000;

        static BaseProtocolProvider()
        {
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;
        }

        protected WebRequest GetRequest(ResourceLocation location)
        {
            WebRequest request = WebRequest.Create((string) location.URL);
            request.Timeout = Timeout;
            SetProxy(request);
            return request;
        }

        protected virtual void SetProxy(WebRequest request)
        {
            if (Settings.Default.UseProxy)
            {
                WebProxy proxy = new WebProxy(Settings.Default.ProxyAddress, Settings.Default.ProxyPort);
                proxy.BypassProxyOnLocal = Settings.Default.ProxyByPassOnLocal;
                request.Proxy = proxy;

                if (!String.IsNullOrEmpty(Settings.Default.ProxyUserName))
                {
                    request.Proxy.Credentials = new NetworkCredential(
                        Settings.Default.ProxyUserName,
                        Settings.Default.ProxyPassword,
                        Settings.Default.ProxyDomain);
                }
            }
        }
    }
}
