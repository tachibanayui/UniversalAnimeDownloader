using System;
using System.Linq;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using SegmentDownloader.Core;

namespace SegmentDownloader.Protocol
{
    public class HttpProtocolProvider : BaseProtocolProvider, IProtocolProvider
    {
        static HttpProtocolProvider()
        {
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(certificateCallBack);
        }

        static bool certificateCallBack(
            object sender,
            X509Certificate certificate,
            X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        private void FillCredentials(HttpWebRequest request, ResourceLocation rl)
        {
            if (rl.Authenticate)
            {
                string login = rl.Login;
                string domain = string.Empty;

                int slashIndex = login.IndexOf('\\');

                if (slashIndex >= 0)
                {
                    domain = login.Substring(0, slashIndex );
                    login = login.Substring(slashIndex + 1);
                }

                NetworkCredential myCred = new NetworkCredential(login, rl.Password);
                myCred.Domain = domain;

                request.Credentials = myCred;
            }
        }

        #region IProtocolProvider Members

        public virtual void Initialize(Downloader downloader)
        {
        }

        public virtual RemoteFileInfo GetFileInfo(ResourceLocation rl, WebHeaderCollection headers, out Stream stream)
        {
            HttpWebRequest request = (HttpWebRequest)GetRequest(rl);

            AddHeader(headers, request);
            request.ReadWriteTimeout = Timeout;

            FillCredentials(request, rl);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            RemoteFileInfo result = new RemoteFileInfo();
            result.MimeType = response.ContentType;
            result.LastModified = response.LastModified;
            result.FileSize = response.ContentLength;
            result.AcceptRanges = String.Compare(response.Headers["Accept-Ranges"], "bytes", true) == 0;

            stream = response.GetResponseStream();

            return result;
        }

        private static void AddHeader(WebHeaderCollection headers, HttpWebRequest request)
        {
            //Add headers
            if (headers != null)
            {
                WebHeaderCollection hds = new WebHeaderCollection();
                foreach (string item in headers.AllKeys)
                {
                    switch (item)
                    {
                        case "Accept":
                            request.Accept = headers.GetValues(item)[0];
                            break;
                        case "Connection":
                            request.Connection = headers.GetValues(item)[0];
                            break;
                        case "Content-Length":
                            request.ContentLength = long.Parse(headers.GetValues(item)[0]);
                            break;
                        case "Content-Type":
                            request.ContentType = headers.GetValues(item)[0];
                            break;
                        case "Date":
                            request.Date = DateTime.Parse(headers.GetValues(item)[0]);
                            break;
                        case "Expect":
                            request.Expect = headers.GetValues(item)[0];
                            break;
                        case "Host":
                            request.Host = headers.GetValues(item)[0];
                            break;
                        case "If-Modified-Since":
                            request.IfModifiedSince = DateTime.Parse(headers.GetValues(item)[0]);
                            break;
                        case "Range":
                            request.AddRange(int.Parse(headers.GetValues(item)[0]));
                            break;
                        case "Referer":
                            request.Referer = headers.GetValues(item)[0];
                            break;
                        case "Transfer-Encoding":
                            request.TransferEncoding = headers.GetValues(item)[0];
                            break;
                        case "User-Agent":
                            request.UserAgent = headers.GetValues(item)[0];
                            break;
                        case "Method":
                            request.Method = headers.GetValues(item)[0];
                            break;
                        case "Proxy-Connection":
                            break;
                        default:
                            hds.Add(item, headers.GetValues(item)[0]);
                            break;
                    }
                }

                request.Headers = hds;
            }
        }

        public Stream CreateStream(ResourceLocation rl, long initialPosition, long endPosition, WebHeaderCollection headers)
        {
            HttpWebRequest request = (HttpWebRequest)GetRequest(rl);
            request.ReadWriteTimeout = Timeout;
            AddHeader(headers, request);
            FillCredentials(request, rl);

            if (initialPosition != 0)
            {
                if (endPosition == 0)
                {
                    request.AddRange(initialPosition);
                }
                else
                {
                    request.AddRange(initialPosition, endPosition);
                }
            }

            WebResponse response = request.GetResponse();

            return response.GetResponseStream();
        }

        #endregion
    }
}
