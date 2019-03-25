using Newtonsoft.Json;
using SegmentDownloader.Core;
using SegmentDownloader.Protocol;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using UADAPI.PlatformSpecific;
using ResourceLocation = SegmentDownloader.Core.ResourceLocation;

namespace UADAPI
{
    /// <summary>
    /// Handle anime sources such as download, update, delete,...
    /// </summary>
    public class AnimeSourceControl
    {

        /// <summary>
        /// Init anime control with a anime manager
        /// </summary>
        /// <param name="manager">The mananger use to get this anime series information</param>
        public AnimeSourceControl(IAnimeSeriesManager manager) => CurrentAnimeSeries = manager;

        public string PreferedQuality { get; set; } = "480p";


        /// <summary>
        /// Init a anime control with a manager file
        /// </summary>
        /// <param name="managerFileLocation">The file which store anime series data</param>
        public AnimeSourceControl(string managerFileLocation)
        {
            AnimeSeriesInfo info = JsonConvert.DeserializeObject<AnimeSeriesInfo>(File.ReadAllText(managerFileLocation));

            var manager = ApiHelpper.CreateAnimeSeriesManagerObjectByType(info.ModInfo.ModType);
            manager.AttachedAnimeSeriesInfo = info;

            CurrentAnimeSeries = manager;
        }

        /// <summary>
        /// Current manager to do operation
        /// </summary>
        public IAnimeSeriesManager CurrentAnimeSeries { get; set; }

        /// <summary>
        /// Download anime with selected episodes
        /// </summary>
        /// <param name="episodeIndexes">Anime indexes</param>
        /// <param name="isSelective">If user select to download selective this series, return false when download all, download missing episdes, update</param>
        public void DownloadAnimeByIndexes(List<int> episodeIndexes, bool isSelective = true)
        {
            CurrentAnimeSeries.AttachedAnimeSeriesInfo.IsSelectiveDownload = isSelective;
            var instance = DownloadManager.CreateNewDownloadInstance(CurrentAnimeSeries, episodeIndexes, PreferedQuality, true);
            NotificationManager.Add(new NotificationItem() { Title = "Download started!", Detail = $"{instance.AttachedManager.AttachedAnimeSeriesInfo.Name} has been started!. Prefer quality: {instance.Quality}" });
            instance.FinishedDownloading += FinishedDownloading;
        }

        

        /// <summary>
        /// Sort hand to download all anime indexes
        /// </summary>
        public void DownloadAll()
        {
            List<int> result = new List<int>();

            foreach (EpisodeInfo item in CurrentAnimeSeries.AttachedAnimeSeriesInfo.Episodes)
            {
                result.Add(item.Index);
            }

            DownloadAnimeByIndexes(result, false);
        }

        /// <summary>
        /// Check for new episodes of this CurrentAnimeSeries
        /// </summary>
        public void Update()
        {
            if (CurrentAnimeSeries.AttachedAnimeSeriesInfo.IsSelectiveDownload)
            {
                return;
            }

            CurrentAnimeSeries.GetEpisodes();

            List<int> episodesIndex = new List<int>();
            foreach (EpisodeInfo item in CurrentAnimeSeries.AttachedAnimeSeriesInfo.Episodes)
            {
                if (!item.AvailableOffline)
                {
                    episodesIndex.Add(item.Index);
                }
            }

            DownloadAnimeByIndexes(episodesIndex, false);
        }

        /// <summary>
        /// Delete episode(s) in the anime series. This will change IsSelectiveDownload Property of the AnimeSeries to true
        /// </summary>
        /// <param name="EpisodeIndexes">Episodes to delete</param>
        public void DeleteEpisodes(List<int> EpisodeIndexes)
        {
            CurrentAnimeSeries.AttachedAnimeSeriesInfo.IsSelectiveDownload = true;

            foreach (int item in EpisodeIndexes)
            {
                EpisodeInfo info = CurrentAnimeSeries.AttachedAnimeSeriesInfo.Episodes.Where(query => query.Index == item).ToArray()[0];
                info.AvailableOffline = false;

                foreach (var item2 in info.FilmSources.Keys)
                {
                    File.Delete(info.FilmSources[item2].LocalFile.ToString());
                }
            }
        }

        /// <summary>
        /// Redownload all missing episodes. This will change IsSelectiveDownload Property of the AnimeSeries to false
        /// </summary>
        public async Task DownloadMissingEpisodes()
        {
            List<int> episodeIndex = new List<int>();
            foreach (EpisodeInfo item in await CurrentAnimeSeries.GetEpisodes())
            {
                if (!item.AvailableOffline)
                {
                    episodeIndex.Add(item.Index);
                }
            }

            DownloadAnimeByIndexes(episodeIndex, false);
        }

        public void SerializeAndSaveToFile()
        {
            string maangerFileContent = JsonConvert.SerializeObject(CurrentAnimeSeries.AttachedAnimeSeriesInfo);
            File.WriteAllText(CurrentAnimeSeries.AttachedAnimeSeriesInfo.ManagerFileLocation, maangerFileContent);
        }


        #region EventHanlers
        private void FinishedDownloading(object sender, EventArgs e)
        {
            var instance = sender as DownloadInstance;
            string episodeName = instance.AttachedManager.AttachedAnimeSeriesInfo.Name;
            string quality = instance.Quality;
            int epCount = instance.EpisodeToDownload.Count;
            int epAllCount = instance.AttachedManager.AttachedAnimeSeriesInfo.Episodes.Count;
            var nof = new NotificationItem()
            {
                Title = $"{episodeName} has finished downloading!",
                Detail = $"{episodeName} has been downloaded. {epCount}/{epAllCount} were requested to download. Preferer quality: {quality}",
                ShowActionButton = false
            };
            NotificationManager.Add(nof);
        } 
        #endregion
    }


    /// <summary>
    /// Contain information abuot the modification or extractor
    /// </summary>
    public class ModificatorInformation
    {
        /// <summary>
        /// Create this type with modification name and extractor type
        /// </summary>
        /// <param name="name">The name of this type</param>
        /// <param name="currentType">The type of this extractor, pass in this.GetType() for simpilcity</param>
        public ModificatorInformation(string name, Type currentType)
        {
            ModName = name;
            ModType = currentType;
        }

        /// <summary>
        /// Your modidication name, this will be displayed when user select extractor mod
        /// </summary>
        public string ModName { get; set; }

        /// <summary>
        /// Your modifiaction version.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Your modification description, will be displayed when user select modification
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The type of your extractor, use ModType = typeof(this) if you don't know much about refection
        /// </summary>
        public Type ModType { get; set; }
    }

    /// <summary>
    /// Manager all download instance. Data file: DownloadManager.json
    /// </summary>
    public static class DownloadManager
    {
        /// <summary>
        /// All Donwload Instances currently downloading
        /// </summary>
        public static ObservableCollection<DownloadInstance> Instances { get; private set; } = new ObservableCollection<DownloadInstance>();

        /// <summary>
        /// Number of segment to download
        /// </summary>
        public static int SegmentCount { get; set; } = 8;

        public static TimeSpan ReportInterval { get; set; } = TimeSpan.FromSeconds(0.5);

        public static string DownloadDirectory { get; set; } = AppDomain.CurrentDomain.BaseDirectory + "Anime Library\\";

        private static bool IsRegisterProtocol = false;

        public static DownloadInstance CreateNewDownloadInstance(IAnimeSeriesManager manager, List<int> episodeId, string quality, bool startNow = true)
        {
            if (!IsRegisterProtocol)
            {
                RegisterProtocol();
            }

            if (manager.AttachedAnimeSeriesInfo == null)
            {
                throw new ArgumentNullException("AttachedAnimeSeriesInfo is null!");
            }

            if (manager.AttachedAnimeSeriesInfo.Episodes == null)
            {
                throw new ArgumentNullException("Episodes list is null!");
            }

            DownloadInstance ins = new DownloadInstance() { AttachedManager = manager, EpisodeId = episodeId, Quality = quality };
            Instances.Add(ins);

            if (startNow == true)
            {
                ins.Start();
            }

            return ins;
        }

        public static void DeleteDownloadProcess(DownloadInstance instance)
        {
            instance.Cancel();
            Instances.Remove(instance);
        }

        public static void DeleteAllDownloadProcess()
        {
            foreach (DownloadInstance item in Instances)
            {
                item.Cancel();
                Instances.Remove(item);
            }
        }

        private static void RegisterProtocol()
        {
            ProtocolProviderFactory.RegisterProtocolHandler("http", typeof(HttpProtocolProvider));
            ProtocolProviderFactory.RegisterProtocolHandler("https", typeof(HttpProtocolProvider));
            ProtocolProviderFactory.RegisterProtocolHandler("ftp", typeof(FtpProtocolProvider));

            IsRegisterProtocol = true;
        }
    }


    /// <summary>
    /// A class the handle downloading videos and create required files for UAD to recognize
    /// </summary>
    public class DownloadInstance : BaseViewModel
    {
        /// <summary>
        /// The information about this anime series
        /// </summary>
        public IAnimeSeriesManager AttachedManager { get; set; }
        public List<int> EpisodeId { get; set; }
        public string Quality { get; set; }
        public List<EpisodeInfo> EpisodeToDownload { get; private set; } = new List<EpisodeInfo>();
        private bool IsCompletedDownloading { get; set; } = false;


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

            await DownloadEpisodes();
            await DownloadThumbnail();
            CreateManagerFile();
            if(State != UADDownloaderState.Canceled)
                State = UADDownloaderState.Finished;
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
                        case "Proxy-Connection":
                            break;
                        default:
                            hds.Add(item, headers.GetValues("item")[0]);
                            break;
                    }
                }

                request.Headers = hds;
            }

            return request;
        }

        private async Task DownloadEpisodes()
        {
            foreach (EpisodeInfo item in EpisodeToDownload)
            {
                var source = item.FilmSources[Quality];
                CompletedEpisodeCount++;

                try
                {
                    if (!string.IsNullOrEmpty(source.Url))
                    {
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

        private void CreateManagerFile()
        {
            File.WriteAllText(AttachedManager.AttachedAnimeSeriesInfo.ManagerFileLocation, JsonConvert.SerializeObject(AttachedManager.AttachedAnimeSeriesInfo, new JsonSerializerSettings() { Error = new EventHandler<Newtonsoft.Json.Serialization.ErrorEventArgs>((s, e) => { e.ErrorContext.Handled = true; }) }));
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
            AttachedManager.AttachedAnimeSeriesInfo.AnimeSeriesSavedDirectory = DownloadManager.DownloadDirectory + "-" + AttachedManager.AttachedAnimeSeriesInfo.AnimeID.ToString() + AttachedManager.AttachedAnimeSeriesInfo.Name + "\\";
            AttachedManager.AttachedAnimeSeriesInfo.ManagerFileLocation = AttachedManager.AttachedAnimeSeriesInfo.AnimeSeriesSavedDirectory + "Manager.json";

            foreach (EpisodeInfo item in EpisodeToDownload)
            {
                if (string.IsNullOrEmpty(item.FilmSources[Quality].LocalFile))
                {
                    item.FilmSources[Quality].LocalFile = $"{AttachedManager.AttachedAnimeSeriesInfo.AnimeSeriesSavedDirectory}{item.EpisodeID}-{item.Index} {item.Name}-{Quality}.mp4";
                }

                item.FilmSources[Quality].IsFinishedRequesting = false;
            }

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

    /// <summary>
    /// Use to access UAD Notification system
    /// </summary>
    public class NotificationManager
    {
        public static ObservableCollection<NotificationItem> Notifications { get; private set; } = new ObservableCollection<NotificationItem>();
        public static void Add(NotificationItem item)
        {
            Notifications.Add(item);
            OnItemAdded(item);
        }
        public static void Remove(NotificationItem item)
        {
            Notifications.Remove(item);
            OnItemRemoved(item);
        }
        public static void RemoveAt(int index)
        {
            if (index > Notifications.Count - 1)
            {
                throw new IndexOutOfRangeException();
            }

            var item = Notifications[index];
            Notifications.Remove(item);
            OnItemRemoved(item);
        }

        public static event EventHandler<NotificationEventArgs> ItemAdded;
        public static event EventHandler<NotificationEventArgs> ItemRemoved;
        public static void OnItemAdded(NotificationItem item) => ItemAdded?.Invoke(Notifications, new NotificationEventArgs() { AffectededItem = item });
        public static void OnItemRemoved(NotificationItem item) => ItemAdded?.Invoke(Notifications, new NotificationEventArgs() { AffectededItem = item });
    }

    /// <summary>
    /// Indiviual Notification
    /// </summary>
    public class NotificationItem
    {
        public NotificationItem()
        {
            CreatedTime = DateTime.Now;
        }

        public DateTime CreatedTime { get; set; }
        public string Title { get; set; }
        public bool ShowActionButton { get; set; }
        public string Detail { get; set; }
        /// <summary>
        /// The string will be placed in the button. Can be null or empty
        /// </summary>
        public string ActionButtonContent { get; set; }
        /// <summary>
        /// THe callback if the user click the button, Can be null
        /// </summary>
        public Action ButtonAction { get; set; } = () => { };
    }


    /// <summary>
    /// A HttpWebRequest that cache the result
    /// </summary>
    public static class AnimeInformationRequester
    {
        public static string UserAgent { get; set; } = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36";
        public static List<RequestCacheItem> HistoricalRequests { get; set; } = new List<RequestCacheItem>();

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
                    return item.Result;
                }
            }

            int retryCount = 0;

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
                            using (Stream stream = resp.GetResponseStream())
                            {
                                StreamReader reader = new StreamReader(stream);
                                string res = await reader.ReadToEndAsync();
                                HistoricalRequests.Add(new RequestCacheItem()
                                {
                                    Headers = headers,
                                    RequestedDateTime = DateTime.Now,
                                    Result = res,
                                    Url = url,
                                });

                                return res;
                            }
                        }
                        else
                        {
                            retryCount++;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    retryCount++;
                }
            }

            throw new InvalidOperationException("Failed to get the requested data!");
        }

        private static HttpWebRequest AddHeader(HttpWebRequest request, WebHeaderCollection headers)
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

        private static void SaveCacheToFile(string saveLocation)
        {
            string content = JsonConvert.SerializeObject(HistoricalRequests);
            if (File.Exists(saveLocation))
            {
                File.Delete(saveLocation);
            }

            File.WriteAllText(saveLocation, content);
        }

        private static void LoadCacheFromFile(string saveLocation)
        {
            string content = File.ReadAllText(saveLocation);
            HistoricalRequests = JsonConvert.DeserializeObject<List<RequestCacheItem>>(content);
        }
    }


    /// <summary>
    /// This class is provide option for UAD. 
    /// </summary>
    public static class ApiHelpper
    {
        /// <summary>
        /// Indicate if the modification binary file (*.dll) is loaded
        /// </summary>
        public static bool IsLoadedAssembly { get; set; }

        /// <summary>
        /// Information about this class/modificator
        /// </summary>
        public static ModificatorInformation ModInfo
        {
            get
            {
                return new ModificatorInformation("BaseAPI", typeof(ApiHelpper))
                {
                    Description = "This is a base modification to comunicate between UAD itself and all the custom modification",
                    Version = "v1.0"
                };
            }
        }

        /// <summary>
        /// The release date of this mods
        /// </summary>
        public static DateTime VerionReleaseDate { get; set; }

        /// <summary>
        /// Contain all types which implement IQueryAnimeSeries
        /// </summary>
        public static List<Type> QueryTypes { get; set; } = new List<Type>();

        /// <summary>
        /// Contain all types which implement IAnimeSeriesManager
        /// </summary>
        public static List<Type> ManagerTypes { get; set; } = new List<Type>();

        /// <summary>
        /// Create a instance by class name that implement this interface IAnimeSeriesManager
        /// </summary>
        /// <param name="className">The name of the class</param>
        public static IAnimeSeriesManager CreateAnimeSeriesManagerObjectByClassName(string className)
        {
            if (!IsLoadedAssembly)
            {
                LoadAssembly();
            }

            var queryRes = ManagerTypes.Where(query => query.Name == className).ToList();
            if (queryRes.Count != 0)
            {
                return Activator.CreateInstance(queryRes[0]) as IAnimeSeriesManager;
            }
            else
            {
                return null;
            }
        }


        public static IAnimeSeriesManager CreateAnimeSeriesManagerObjectByType(Type type)
        {
            if (!IsLoadedAssembly)
            {
                LoadAssembly();
            }

            var queryRes = ManagerTypes.Where(query => query == type).ToList();
            if (queryRes.Count() != 0)
            {
                return Activator.CreateInstance(queryRes[0]) as IAnimeSeriesManager;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Create a instance by class name that implement this interface IQueryAnimeSeries
        /// </summary>
        /// <param name="className">The name of the class</param>
        public static IQueryAnimeSeries CreateQueryAnimeObjectByClassName(string className)
        {
            if (!IsLoadedAssembly)
            {
                LoadAssembly();
            }

            var queryRes = QueryTypes.Where(query => query.Name == className).ToList();
            if (queryRes.Count != 0)
            {
                return Activator.CreateInstance(queryRes[0]) as IQueryAnimeSeries;
            }
            else
            {
                return null;
            }
        }

        public static IQueryAnimeSeries CreateQueryAnimeObjectByType(Type type)
        {
            if (!IsLoadedAssembly)
            {
                LoadAssembly();
            }

            var queryRes = QueryTypes.Where(query => query == type).ToList();
            if (queryRes.Count() != 0)
            {
                return Activator.CreateInstance(queryRes[0]) as IQueryAnimeSeries;
            }
            else
            {
                return null;
            }
        }

        public static void LoadAssembly()
        {
            if (CheckForUpdates())
            {
                throw new InvalidOperationException("This modification is out of date, update in our GitHub release!");
            }

            string modDirectory = AppDomain.CurrentDomain.BaseDirectory + "Mods" + "\\";
            List<Assembly> modAssemblies = new List<Assembly>();

            if (!Directory.Exists(modDirectory))
            {
                Directory.CreateDirectory(modDirectory);
            }

            foreach (string item in Directory.EnumerateFiles(modDirectory))
            {
                try
                {
                    if (Path.GetExtension(item).Contains("dll"))
                    {
                        modAssemblies.Add(Assembly.LoadFrom(item));
                    }
                }
                catch { }
            }

            IsLoadedAssembly = true;

            for (int i = 0; i < modAssemblies.Count; i++)
            {
                try
                {
                    var assemblyTypes = modAssemblies[i].ExportedTypes;
                    ManagerTypes.AddRange(assemblyTypes.Where(SearchForManagerTypes));
                    QueryTypes.AddRange(assemblyTypes.Where(SearchForQueryTypes));
                }
                catch (Exception e) { Console.WriteLine(e); }
            }

            ExamineMods();
        }

        private static bool SearchForManagerTypes(Type arg)
        {
            foreach (var item in arg.GetInterfaces())
            {
                if (item.FullName == typeof(IAnimeSeriesManager).FullName)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool SearchForQueryTypes(Type arg)
        {
            foreach (var item in arg.GetInterfaces())
            {
                if (item.FullName == typeof(IQueryAnimeSeries).FullName)
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Not implemented yet!
        /// </summary>
        private static void ExamineMods()
        {
        }

        /// <summary>
        /// Check for updates on GitHubs
        /// </summary>
        private static bool CheckForUpdates()
        {
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Is internet connection avaible</returns>
        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://clients3.google.com/generate_204"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }

        }
    }
}
