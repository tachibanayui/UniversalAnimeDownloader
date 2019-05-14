using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace UADAPI
{
    /// <summary>
    /// A HttpWebRequest that cache the result
    /// </summary>
    public static class AnimeInformationRequester
    {
        public static string UserAgent { get; set; } = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36";
        public static List<RequestCacheItem> HistoricalRequests { get; set; } = new List<RequestCacheItem>();

        public static async Task<Stream> GetStreamAsync(string url, WebHeaderCollection headers = null, DateTime? reuseExpirationDate = null, int? retryLimit = null)
        {
            DateTime dt = DateTime.MinValue;
            int retryLim = 3;
            if (reuseExpirationDate != null)
            {
                dt = (DateTime)reuseExpirationDate;
            }

            if (retryLimit != null)
            {
                retryLim = (int)retryLimit;
            }

            var queryHistoricalRequest = HistoricalRequests.Where(query => query.Url == url);
            foreach (var item in queryHistoricalRequest)
            {
                if (CompareHeaders(item.Headers, headers) && dt < item.RequestedDateTime)
                {
                    MemoryStream returnStream = new MemoryStream();
                    item.Result.Position = 0;
                    await item.Result.CopyToAsync(returnStream);
                    returnStream.Position = 0;
                    return returnStream;
                }
            }

            int retryCount = 0;
            Exception lastError = null;
            while (retryCount < retryLim)
            {
                try
                {
                    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                    if (headers != null)
                    {
                        req = AddHeader(req, headers);
                    }

                    using (HttpWebResponse resp = (HttpWebResponse)await req.GetResponseAsync())
                    {
                        if (resp.StatusCode == HttpStatusCode.OK)
                        {
                            Stream stream = resp.GetResponseStream();
                            //This memStream only used for caching, we will return a copy of it
                            MemoryStream memStream = new MemoryStream();
                            await stream.CopyToAsync(memStream);
                            stream.Close();

                            HistoricalRequests.Add(new RequestCacheItem()
                            {
                                Headers = headers,
                                RequestedDateTime = DateTime.Now,
                                Result = memStream,
                                Url = url,
                            });

                            MemoryStream returnStream = new MemoryStream();
                            memStream.Position = 0;
                            await memStream.CopyToAsync(returnStream);
                            returnStream.Position = 0;
                            return returnStream;
                        }
                        else
                        {
                            retryCount++;
                        }
                    }
                }
                catch (Exception e)
                {
                    await Task.Delay(3000);
                    lastError = e;
                    retryCount++;
                }
            }

            throw new InvalidOperationException("Failed to get the requested data!" + url, lastError);
        }

        /// <summary>
        /// Similar to HttpWebrequest but it reuse the request result if it re-request again, You shouldn't use this method to download large data larger than 25Mb
        /// </summary>
        /// <param name="url">The url to request</param>
        /// <param name="headers">The header included when requesting</param>
        /// <param name="reuseExpirationDate">The date time that the request don't use the cache one
        /// <para>For example if the past request done on 1 Jan 2018 but you set the exp.Date to 3 Jan 2018. The request cache will be ignored</para>
        /// </param>
        /// <param name="retryLimit">The limit of retrying intil throw an exception</param>
        /// <returns></returns>
        public static async Task<string> Request(string url, WebHeaderCollection headers = null, DateTime? reuseExpirationDate = null, int? retryLimit = null)
        {
            var stream = await GetStreamAsync(url, headers, reuseExpirationDate, retryLimit);
            StreamReader reader = new StreamReader(stream);
            string res = await reader.ReadToEndAsync();
            return res;
        }

        /// <summary>
        /// Ulility method for adding header
        /// </summary>
        /// <param name="request"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static HttpWebRequest AddHeader(HttpWebRequest request, WebHeaderCollection headers)
        {
            //Add headers
            if (headers != null)
            {
                WebHeaderCollection hds = new WebHeaderCollection();
                foreach (string item in headers.AllKeys)
                {
                    switch (item.ToLower())
                    {
                        case "accept":
                            request.Accept = headers.GetValues(item)[0];
                            break;
                        case "connection":
                            request.Connection = headers.GetValues(item)[0];
                            break;
                        case "content-length":
                            request.ContentLength = long.Parse(headers.GetValues(item)[0]);
                            break;
                        case "content-type":
                            request.ContentType = headers.GetValues(item)[0];
                            break;
                        case "date":
                            request.Date = DateTime.Parse(headers.GetValues(item)[0]);
                            break;
                        case "expect":
                            request.Expect = headers.GetValues(item)[0];
                            break;
                        case "host":
                            request.Host = headers.GetValues(item)[0];
                            break;
                        case "if-modified-since":
                            request.IfModifiedSince = DateTime.Parse(headers.GetValues(item)[0]);
                            break;
                        case "range":
                            request.AddRange(int.Parse(headers.GetValues(item)[0]));
                            break;
                        case "referer":
                            request.Referer = headers.GetValues(item)[0];
                            break;
                        case "transfer-encoding":
                            request.TransferEncoding = headers.GetValues(item)[0];
                            break;
                        case "user-agent":
                            request.UserAgent = headers.GetValues(item)[0];
                            break;
                        case "method":
                            request.Method = headers.GetValues(item)[0];
                            break;
                        case "proxy-connection":
                            break;
                        default:
                            hds.Add(item, headers.GetValues(item)[0]);
                            break;
                    }
                }

                foreach (var item in hds.AllKeys)
                {
                    request.Headers.Add(item, headers.GetValues(item)[0]);
                }
            }

            return request;
        }

        private static bool CompareHeaders(WebHeaderCollection a, WebHeaderCollection b)
        {
            if (a != null && b != null)
            {
                for (int i = 0; i < a.Count; i++)
                {
                    if (a.GetKey(i) != b.GetKey(i) || a.GetValues(i)[0] != b.GetValues(i)[0])
                    {
                        return false;
                    }
                }
            }
            else if (a != null || b != null)
            {
                return false;
            }

            return true;
        }

        private static void Serialize() => JsonConvert.SerializeObject(HistoricalRequests);

        private static void Deserialize(string value) => HistoricalRequests = JsonConvert.DeserializeObject<List<RequestCacheItem>>(value);
    }
}
