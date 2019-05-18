using Newtonsoft.Json;
using SegmentDownloader.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using UADAPI.PlatformSpecific;
using ErrorEventArgs = Newtonsoft.Json.Serialization.ErrorEventArgs;
using ResourceLocation = SegmentDownloader.Core.ResourceLocation;

namespace UADAPI
{
    /// <summary>
    /// A class the handle downloading videos and create required files for UAD to recognize
    /// </summary>
    public class DownloadInstance : BaseViewModel
    {
        /// <summary>
        /// The information about this anime series
        /// </summary>
        [JsonConverter(typeof(Converters.IAnimeSeriesManagerDeserializer))]
        public IAnimeSeriesManager AttachedManager { get; set; }
        //For serializer
        public AnimeSeriesInfo Info { get; set; }
        public List<int> EpisodeId { get; set; }
        public VideoQuality PreferedQuality { get; set; }
        public List<EpisodeInfo> EpisodeToDownload { get; private set; } = new List<EpisodeInfo>();
        private bool IsCompletedDownloading { get; set; } = false;
        [JsonIgnore]
        public bool IsInstanceFromSerialzation { get; set; }

        #region BindableProperty
        private int _CompletedEpisodeCount;
        public int CompletedEpisodeCount
        {
            get
            {
                return _CompletedEpisodeCount;
            }
            set
            {
                if (_CompletedEpisodeCount != value)
                {
                    _CompletedEpisodeCount = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _EpisodeToDownloadCount;
        public int EpisodeToDownloadCount
        {
            get
            {
                return _EpisodeToDownloadCount;
            }
            set
            {
                if (_EpisodeToDownloadCount != value)
                {
                    _EpisodeToDownloadCount = value;
                    OnPropertyChanged();
                }
            }
        }



        private UADDownloaderState _State = UADDownloaderState.NotStarted;
        public UADDownloaderState State
        {
            get
            {
                return _State;
            }
            set
            {
                if (_State != value)
                {
                    _State = value;
                    OnPropertyChanged();
                }
            }
        }


        #endregion

        /// <summary>
        /// Start the downloading process
        /// </summary>
        public async void Start()
        {
            if (AttachedManager == null || EpisodeId == null)
            {
                throw new InvalidDataException("Missing properties value. \r\nIf you get this error, try using DownloadManager.CreateNewDownloadInstance() instead!");
            }

            State = UADDownloaderState.Working;

            if (State == UADDownloaderState.Canceled)
            {
                throw new InvalidOperationException("Download is already cancel, please create a new download instance");
            }



            GetEpisodeToDownload();

            if (string.IsNullOrEmpty(AttachedManager.AttachedAnimeSeriesInfo.AnimeSeriesSavedDirectory))
            {
                AssignDirectory();
            }

            if (!Directory.Exists(AttachedManager.AttachedAnimeSeriesInfo.AnimeSeriesSavedDirectory))
            {
                Directory.CreateDirectory(AttachedManager.AttachedAnimeSeriesInfo.AnimeSeriesSavedDirectory);
            }

            await Task.Run(async () => await DownloadEpisodes());
            await DownloadThumbnail();
            await CheckOfflineEpiodeAvaiblity();
            CreateManagerFile();
            if (State != UADDownloaderState.Canceled)
            {
                State = UADDownloaderState.Finished;
            }

            OnFinishedDownloading();
        }

        private async Task DownloadThumbnail()
        {
            //Download the series thumbnail
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(AttachedManager.AttachedAnimeSeriesInfo.Thumbnail.Url);
                req = AddHeader(req, AttachedManager.AttachedAnimeSeriesInfo.Thumbnail.Headers);

                using (HttpWebResponse resp = (HttpWebResponse)await req.GetResponseAsync())
                {
                    if (resp.StatusCode == HttpStatusCode.OK)
                    {
                        using (Stream stream = resp.GetResponseStream())
                        {
                            if (File.Exists(AttachedManager.AttachedAnimeSeriesInfo.Thumbnail.LocalFile))
                            {
                                File.Delete(AttachedManager.AttachedAnimeSeriesInfo.Thumbnail.LocalFile);
                            }

                            using (FileStream fs = File.Create(AttachedManager.AttachedAnimeSeriesInfo.Thumbnail.LocalFile))
                            {
                                await stream.CopyToAsync(fs);
                            }
                        }
                    }
                }
            }
            catch { }

            //Do async download
            List<Task> streamTasks = new List<Task>();
            List<Stream> streams = new List<Stream>();
            List<FileStream> fileStreams = new List<FileStream>();
            List<HttpWebResponse> webResponses = new List<HttpWebResponse>();

            foreach (var item in AttachedManager.AttachedAnimeSeriesInfo.Episodes)
            {
                if (item.Thumbnail != null)
                {
                    if (!item.Thumbnail.IsFinishedRequesting && !item.Thumbnail.IsFinishedRequesting)
                    {
                        bool isSuccess = await DownloadUsingHttpWebRequest(item.Thumbnail.Url, item.Thumbnail.Headers, item.Thumbnail.LocalFile);
                        item.Thumbnail.IsFinishedRequesting = isSuccess;
                    }
                }
            }
        }

        private async Task<bool> DownloadUsingHttpWebRequest(string url, WebHeaderCollection collectionHeaders, string localFile)
        {
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req = AddHeader(req, collectionHeaders);
                using (HttpWebResponse resp = (HttpWebResponse)await req.GetResponseAsync())
                {
                    if (resp.StatusCode == HttpStatusCode.OK)
                    {
                        using (Stream stream = resp.GetResponseStream())
                        {
                            if (File.Exists(localFile))
                            {
                                File.Delete(localFile);
                            }

                            using (FileStream fs = File.Create(localFile))
                            {
                                await stream.CopyToAsync(fs);
                                return true;
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch
            {
                return false;
            }

        }

        private HttpWebRequest AddHeader(HttpWebRequest request, WebHeaderCollection headers)
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

            return request;
        }

        private async Task DownloadEpisodes(int startAt = 0)
        {
            for (int i = startAt; i < EpisodeToDownload.Count; i++)
            {
                EpisodeInfo item = EpisodeToDownload[i];
                MediaSourceInfo source = await GetSource(item.FilmSources, PreferedQuality);
                CompletedEpisodeCount++;

                if (string.IsNullOrEmpty(source.LocalFile))
                {
                    source.LocalFile = $"{AttachedManager.AttachedAnimeSeriesInfo.AnimeSeriesSavedDirectory}{item.EpisodeID}-{item.Index}-{item.Name.RemoveInvalidChar()}.mp4";
                }

                if (File.Exists(source.LocalFile))
                {
                    File.Delete(source.LocalFile);
                }

                source.IsFinishedRequesting = false;

                try
                {
                    if (!string.IsNullOrEmpty(source.Url))
                    {
                        //Test for expired link
                        bool renewUrl = false;
                        while (!renewUrl)
                        {
                            try
                            {
                                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(source.Url);
                                AddHeader(request, source.Headers);
                                using (HttpWebResponse resp = (HttpWebResponse)await request.GetResponseAsync())
                                {
                                    renewUrl = true;
                                }
                            }
                            catch (Exception e)
                            {
                                var newUrl = (await AttachedManager.GetEpisodes(new List<int>() { item.EpisodeID })).FirstOrDefault();
                                if (newUrl != null)
                                {
                                    EpisodeToDownload[i] = newUrl;
                                    item = EpisodeToDownload[i];
                                    source = await GetSource(item.FilmSources, PreferedQuality);
                                    if (string.IsNullOrEmpty(source.LocalFile))
                                    {
                                        source.LocalFile = $"{AttachedManager.AttachedAnimeSeriesInfo.AnimeSeriesSavedDirectory}{item.EpisodeID}-{item.Index}-{item.Name.RemoveInvalidChar()}.mp4";
                                    }

                                    if (File.Exists(source.LocalFile))
                                    {
                                        File.Delete(source.LocalFile);
                                    }

                                    source.IsFinishedRequesting = false;
                                }
                            }
                        }

                        var downloader = SegmentDownloader.Core.DownloadManager.Instance.Add(
                        ResourceLocation.FromURL(source.Url.ToString()), null, source.LocalFile, DownloadManager.SegmentCount, false, source.Headers);

                        downloader.Ending += (s, e) =>
                        {
                            IsCompletedDownloading = true;
                            Downloader ss = s as Downloader;
                            DownloadCompletedEventArgs ee = new DownloadCompletedEventArgs()
                            {
                                CompletedEpisode = item,
                                InnerDownloader = ss,
                            };

                            if (ss.State == DownloaderState.EndedWithError)
                            {
                                ee.IsSuccess = false;
                                source.IsFinishedRequesting = true;
                                item.AvailableOffline = false;
                                item.EpisodeDownloadState.EpisodeDownloadState = EpisodeDownloadState.FailedDownloading;
                            }
                            else
                            {
                                ee.IsSuccess = true;
                                source.IsFinishedRequesting = true;
                                item.AvailableOffline = true;
                                item.EpisodeDownloadState.EpisodeDownloadState = EpisodeDownloadState.FinishedDownloading;

                            }

                            OnEpisodeDownloadCompleted(ee);
                        };

                        downloader.Start();
                        item.EpisodeDownloadState.EpisodeDownloadState = EpisodeDownloadState.Downloading;
                        IsCompletedDownloading = false;
                        while (!IsCompletedDownloading)
                        {
                            if (downloader.State == DownloaderState.EndedWithError)
                            {
                                throw new Exception("Failed downloading");
                            }

                            if (State == UADDownloaderState.Canceled)
                            {
                                SegmentDownloader.Core.DownloadManager.Instance.RemoveDownload(downloader);
                                return;
                            }


                            if (State == UADDownloaderState.Paused)
                            {
                                if (downloader.State != DownloaderState.Paused || downloader.State != DownloaderState.Pausing)
                                {
                                    await Task.Run(() => downloader.Pause());
                                }
                            }
                            else
                            {
                                ReportProgress(downloader, item);
                                if (downloader.State != DownloaderState.Paused || downloader.State != DownloaderState.Pausing)
                                {
                                    downloader.Start();
                                }
                            }
                            await Task.Delay((int)DownloadManager.ReportInterval.TotalMilliseconds);
                        }
                    }
                    else
                    {
                        DownloadCompletedEventArgs ee = new DownloadCompletedEventArgs();
                        ee.IsSuccess = false;
                        source.IsFinishedRequesting = true;
                        item.AvailableOffline = false;
                        item.EpisodeDownloadState.EpisodeDownloadState = EpisodeDownloadState.FailedDownloading;
                        OnEpisodeDownloadCompleted(ee);
                    }
                }
                catch
                {
                    DownloadCompletedEventArgs ee = new DownloadCompletedEventArgs();
                    ee.IsSuccess = false;
                    source.IsFinishedRequesting = true;
                    item.AvailableOffline = false;
                    item.EpisodeDownloadState.EpisodeDownloadState = EpisodeDownloadState.FailedDownloading;
                    OnEpisodeDownloadCompleted(ee);
                }
            }
        }

        private async Task<MediaSourceInfo> GetSource(Dictionary<VideoQuality, MediaSourceInfo> filmSources, VideoQuality preferedQuality)
        {
            int index = VideoQualityHelper.GetValue(preferedQuality);
            if (index < 0)
            {
                return null;
            }

            for (int i = index; i >= 0; i--)
            {
                var result = await AttachedManager.GetCommonQuality(filmSources, VideoQualityHelper.GetQualityStringFromValue(index));
                if (result != null)
                {
                    return result;
                }
            }

            foreach (var item in filmSources.Keys)
            {
                return filmSources[item];
            }

            return null;
        }

        private void CreateManagerFile()
        {
            var jsonSetting = new JsonSerializerSettings()
            {
                Error = new EventHandler<ErrorEventArgs>((s, e) => e.ErrorContext.Handled = true),
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full,
            };

            if (File.Exists(AttachedManager.AttachedAnimeSeriesInfo.ManagerFileLocation))
            {
                string fileContent = File.ReadAllText(AttachedManager.AttachedAnimeSeriesInfo.ManagerFileLocation);
                var oldManager = JsonConvert.DeserializeObject<AnimeSeriesInfo>(fileContent, jsonSetting);

                //Update the episode list
                int missingEpisodeCount = AttachedManager.AttachedAnimeSeriesInfo.Episodes.Count - oldManager.Episodes.Count;
                for (int i = 0; i < missingEpisodeCount; i++)
                {
                    oldManager.Episodes.Add(AttachedManager.AttachedAnimeSeriesInfo.Episodes[oldManager.Episodes.Count + i]);
                }

                //Change the episode downloaded before
                foreach (var item in EpisodeToDownload)
                {
                    int index = oldManager.Episodes.FindIndex(p => p.EpisodeID == item.EpisodeID);
                    if (index != -1)
                    {
                        oldManager.Episodes[index] = item;
                    }
                }

                string modifiedManagerFileContent = JsonConvert.SerializeObject(oldManager, jsonSetting);
                File.WriteAllText(AttachedManager.AttachedAnimeSeriesInfo.ManagerFileLocation, modifiedManagerFileContent);
                return;
            }



            string managerFileContent = JsonConvert.SerializeObject(AttachedManager.AttachedAnimeSeriesInfo, jsonSetting);
            File.WriteAllText(AttachedManager.AttachedAnimeSeriesInfo.ManagerFileLocation, managerFileContent);
        }

        /// <summary>
        /// Do all sort of validating after the download film source is complete
        /// </summary>
        /// <returns></returns>
        private async Task CheckOfflineEpiodeAvaiblity()
        {
            await Task.Run(() =>
            {
                //Check if all episode is present -> mark IsSelectiveDownload to false;
                AttachedManager.AttachedAnimeSeriesInfo.IsSelectiveDownload = AttachedManager.AttachedAnimeSeriesInfo.Episodes.All(f => f.AvailableOffline) ? false : true;
            });
        }

        private void ReportProgress(Downloader downloader, EpisodeInfo info)
        {
            var e = new DownloadProgressChangedEventArgs()
            {
                FileSize = downloader.FileSize,
                EstimatedTimeLeft = downloader.Left,
                Progress = downloader.Progress,
                DownloadSpeed = downloader.Rate,
                Transfered = downloader.Transfered,
                State = downloader.State,
                Segments = downloader.Segments
            };
            info.EpisodeDownloadState.FileSize = downloader.FileSize;
            info.EpisodeDownloadState.EstimatedTimeLeft = downloader.Left;
            info.EpisodeDownloadState.Progress = downloader.Progress;
            info.EpisodeDownloadState.DownloadSpeed = downloader.Rate;
            info.EpisodeDownloadState.Transfered = downloader.Transfered;
            info.EpisodeDownloadState.State = downloader.State;
            if (downloader.Segments.Count != 0)
            {
                info.EpisodeDownloadState.Segments.Clear();
                foreach (var item in downloader.Segments)
                {
                    info.EpisodeDownloadState.Segments.Add(item);
                }
            }


            OnDownloadProgressChanged(e);
        }

        private void AssignDirectory()
        {
            AttachedManager.AttachedAnimeSeriesInfo.AnimeSeriesSavedDirectory = DownloadManager.DownloadDirectory + AttachedManager.AttachedAnimeSeriesInfo.AnimeID.ToString() + "-" + AttachedManager.AttachedAnimeSeriesInfo.Name.RemoveInvalidChar() + "\\";
            AttachedManager.AttachedAnimeSeriesInfo.ManagerFileLocation = AttachedManager.AttachedAnimeSeriesInfo.AnimeSeriesSavedDirectory + "Manager.json";



            foreach (EpisodeInfo item in EpisodeToDownload)
            {
                if (string.IsNullOrEmpty(item.Thumbnail.LocalFile))
                {
                    item.Thumbnail.LocalFile = $"{AttachedManager.AttachedAnimeSeriesInfo.AnimeSeriesSavedDirectory}{item.EpisodeID}-{item.Index}-Thumbnail.png";
                }
            }

            //Check for the series thumbnail
            if (string.IsNullOrEmpty(AttachedManager.AttachedAnimeSeriesInfo.Thumbnail.LocalFile))
            {
                AttachedManager.AttachedAnimeSeriesInfo.Thumbnail.LocalFile = $"{AttachedManager.AttachedAnimeSeriesInfo.AnimeSeriesSavedDirectory}SeriesThumbnail.png";
                AttachedManager.AttachedAnimeSeriesInfo.Thumbnail.IsFinishedRequesting = false;
            }
        }


        /// <summary>
        /// Get episodes to download based on episode indexes
        /// </summary>
        private void GetEpisodeToDownload()
        {
            //null check
            foreach (int item in EpisodeId)
            {
                EpisodeInfo[] queryRes = AttachedManager.AttachedAnimeSeriesInfo.Episodes.Where(query => query.Index == item).ToArray();

                if (queryRes.Length != 0)
                {
                    EpisodeInfo info = queryRes[0];
                    info.EpisodeDownloadState.EpisodeDownloadState = EpisodeDownloadState.InDownloadQueue;
                    EpisodeToDownload.Add(info);
                }
            }

            EpisodeToDownloadCount = EpisodeToDownload.Count;
        }

        /// <summary>
        /// Pause the downloading process
        /// </summary>
        public void Pause()
        {
            if (State == UADDownloaderState.Canceled)
            {
                throw new InvalidOperationException("Download has already cancel, cannot pause");
            }

            State = UADDownloaderState.Paused;
        }

        /// <summary>
        /// Resume the downloading process
        /// </summary>
        public void Resume()
        {
            if (State == UADDownloaderState.Canceled)
            {
                throw new InvalidOperationException("Download has already cancel, cannot resume");
            }

            State = UADDownloaderState.Working;
            if (IsInstanceFromSerialzation)
            {
                RestartFromSerialzation();
            }
        }

        private async void RestartFromSerialzation()
        {
            var index = EpisodeToDownload.FindIndex(p => p.EpisodeDownloadState.EpisodeDownloadState == EpisodeDownloadState.Downloading);
            if (index == -1)
            {
                index = 0;
            }

            GetEpisodeToDownload();

            CompletedEpisodeCount--;
            await DownloadEpisodes(index);
            await DownloadThumbnail();
            await CheckOfflineEpiodeAvaiblity();
            CreateManagerFile();
            if (State != UADDownloaderState.Canceled)
            {
                State = UADDownloaderState.Finished;
            }

            OnFinishedDownloading();
        }

        /// <summary>
        /// Resume the downloading process
        /// </summary>
        public void Cancel()
        {
            State = UADDownloaderState.Canceled;
        }

        public event EventHandler<DownloadCompletedEventArgs> EpisodeDownloadCompleted;
        public void OnEpisodeDownloadCompleted(DownloadCompletedEventArgs e) => EpisodeDownloadCompleted?.Invoke(this, e);

        public event EventHandler FinishedDownloading;
        public void OnFinishedDownloading() => FinishedDownloading?.Invoke(this, EventArgs.Empty);

        public EventHandler<DownloadProgressChangedEventArgs> DownloadProgressChanged;
        public void OnDownloadProgressChanged(DownloadProgressChangedEventArgs e) => DownloadProgressChanged?.Invoke(this, e);

    }
}
