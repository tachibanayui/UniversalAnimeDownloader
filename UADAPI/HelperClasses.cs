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
using ErrorEventArgs = Newtonsoft.Json.Serialization.ErrorEventArgs;
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

            var manager = ApiHelpper.CreateAnimeSeriesManagerObjectByClassName(info.ModInfo.ModTypeString);
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
        public DownloadInstance DownloadAnimeByIndexes(List<int> episodeIndexes, bool isSelective = true)
        {
            CurrentAnimeSeries.AttachedAnimeSeriesInfo.IsSelectiveDownload = isSelective;
            var instance = DownloadManager.CreateNewDownloadInstance(CurrentAnimeSeries, episodeIndexes, PreferedQuality, true);
            NotificationManager.Add(new NotificationItem() { Title = "Download started!", Detail = $"{instance.AttachedManager.AttachedAnimeSeriesInfo.Name} has been started!. Prefer quality: {instance.PreferedQuality}" });
            instance.FinishedDownloading += FinishedDownloading;
            return instance;
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
        public async Task<bool> Update()
        {
            if (CurrentAnimeSeries.AttachedAnimeSeriesInfo.IsSelectiveDownload)
            {
                return false;
            }

            await CurrentAnimeSeries.GetPrototypeEpisodes();

            List<int> episodesIndex = new List<int>();
            foreach (EpisodeInfo item in CurrentAnimeSeries.AttachedAnimeSeriesInfo.Episodes)
            {
                if (!item.AvailableOffline)
                {
                    episodesIndex.Add(item.Index);
                    await CurrentAnimeSeries.GetEpisodes(new List<int>() { item.EpisodeID });
                }
            }
            if (episodesIndex.Count != 0)
            {
                DownloadAnimeByIndexes(episodesIndex, false);
                return true;
            }
            else
            {
                return false;
            }
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
                    if (File.Exists(info.FilmSources[item2].LocalFile.ToString()))
                    {
                        File.Delete(info.FilmSources[item2].LocalFile.ToString());
                    }
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
            string quality = instance.PreferedQuality;
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
            ModTypeString = currentType.FullName;
        }

        public ModificatorInformation()
        {

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
        /// The type of your extractor, use ModType = GetType() if you don't know much about refection
        /// </summary>
        //public Type ModType { get => string.IsNullOrEmpty(ModTypeString) ? Type.GetType(ModTypeString) : null; set => ModTypeString = value != null ? value.FullName : string.Empty; }

        /// <summary>
        /// Use to deserialize
        /// </summary>
        public string ModTypeString { get; set; }
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

        private static List<string> _PresetQuality { get; set; } = new List<string>
        {
           "Worse possible", "144p", "288p", "360p", "480p", "720p", "1080p", "1440p", "2160p", "Best possible",
        };

        public static ReadOnlyCollection<string> PresetQuality { get => _PresetQuality.AsReadOnly(); }

        /// <summary>
        /// Number of segment to download
        /// </summary>
        public static int SegmentCount { get; set; } = 5;

        public static TimeSpan ReportInterval { get; set; } = TimeSpan.FromSeconds(0.5);

        public static string DownloadDirectory { get; set; } = AppDomain.CurrentDomain.BaseDirectory + "Anime Library\\";

        private static bool IsRegisterProtocol = false;

        public static DownloadInstance CreateNewDownloadInstance(IAnimeSeriesManager manager, List<int> episodeId, string quality, bool startNow = true)
        {
            RegisterProtocol();

            if (manager.AttachedAnimeSeriesInfo == null)
            {
                throw new ArgumentNullException("AttachedAnimeSeriesInfo is null!");
            }

            if (manager.AttachedAnimeSeriesInfo.Episodes == null)
            {
                throw new ArgumentNullException("Episodes list is null!");
            }

            DownloadInstance ins = new DownloadInstance() { AttachedManager = manager, EpisodeId = episodeId, PreferedQuality = quality };

            for (int i = Instances.Count - 1; i >= 0; i--)
            {
                var tmp = Instances[i];
                if (i + 1 == Instances.Count)
                {
                    Instances.Add(tmp);
                }
                else
                {
                    Instances[i + 1] = tmp;
                }
            }

            if (Instances.Count == 0)
            {
                Instances.Add(ins);
            }
            else
            {
                Instances[0] = ins;
            }

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
                if (item.State == UADDownloaderState.Working)
                {
                    item.Cancel();
                }
            }

            Instances.Clear();
        }

        private static void RegisterProtocol()
        {
            if (!IsRegisterProtocol)
            {
                ProtocolProviderFactory.RegisterProtocolHandler("http", typeof(HttpProtocolProvider));
                ProtocolProviderFactory.RegisterProtocolHandler("https", typeof(HttpProtocolProvider));
                ProtocolProviderFactory.RegisterProtocolHandler("ftp", typeof(FtpProtocolProvider));
                IsRegisterProtocol = true;
            }
        }

        public static string Serialize() => JsonConvert.SerializeObject(Instances);

        public static void Deserialize(string value)
        {
            RegisterProtocol();
            if (value != null)
            {
                var jsonSetting = new JsonSerializerSettings()
                {
                    Error = new EventHandler<ErrorEventArgs>((s, e) => e.ErrorContext.Handled = true),
                    TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full,
                };
                var tmpIns = JsonConvert.DeserializeObject<ObservableCollection<DownloadInstance>>(value, jsonSetting);
                foreach (var item in tmpIns)
                {
                    item.IsInstanceFromSerialzation = true;
                    if (item.State == UADDownloaderState.Working)
                    {
                        item.State = UADDownloaderState.Paused;
                    }
                }

                Instances = new ObservableCollection<DownloadInstance>(tmpIns);
            }
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
        [JsonConverter(typeof(Converters.IAnimeSeriesManagerDeserializer))]
        public IAnimeSeriesManager AttachedManager { get; set; }
        //For serializer
        public AnimeSeriesInfo Info { get; set; }
        public List<int> EpisodeId { get; set; }
        public string PreferedQuality { get; set; }
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

        private async Task<MediaSourceInfo> GetSource(Dictionary<string, MediaSourceInfo> filmSources, string preferedQuality)
        {
            int index = DownloadManager.PresetQuality.IndexOf(preferedQuality);
            if (index < 0)
            {
                return null;
            }

            for (int i = index; i >= 0; i--)
            {
                var result = await AttachedManager.GetCommonQuality(filmSources, DownloadManager.PresetQuality[i]);
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

    /// <summary>
    /// Use to access UAD Notification system
    /// </summary>
    public class NotificationManager
    {
        public static ObservableCollection<NotificationItem> Notifications { get; private set; } = new ObservableCollection<NotificationItem>();
        public static void Add(NotificationItem item)
        {
            for (int i = Notifications.Count - 1; i >= 0; i--)
            {
                var tmp = Notifications[i];
                if (i + 1 == Notifications.Count)
                {
                    Notifications.Add(tmp);
                }
                else
                {
                    Notifications[i + 1] = tmp;
                }
            }

            if (Notifications.Count == 0)
            {
                Notifications.Add(item);
            }
            else
            {
                Notifications[0] = item;
            }

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
        public static void RemoveAll()
        {
            Notifications.Clear();
            OnItemRemoved(null);
        }


        public static string Serialize() => JsonConvert.SerializeObject(Notifications);

        public static void Deserialize(string value)
        {
            if (value != null)
            {
                var tmp = JsonConvert.DeserializeObject<ObservableCollection<NotificationItem>>(value);
                Notifications = new ObservableCollection<NotificationItem>(tmp);
            }
            else
            {
                Notifications = new ObservableCollection<NotificationItem>();
            }
        }

        public static event EventHandler<NotificationEventArgs> ItemAdded;
        public static event EventHandler<NotificationEventArgs> ItemRemoved;
        public static void OnItemAdded(NotificationItem item) => ItemAdded?.Invoke(Notifications, new NotificationEventArgs() { AffectededItem = item });
        public static void OnItemRemoved(NotificationItem item) => ItemRemoved?.Invoke(Notifications, new NotificationEventArgs() { AffectededItem = item });
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
        /// Will be invoke if user click the button, Method must be public 
        /// <para>Format: {Type in full name} Example: UADEx.HelperClass</para>
        /// <para>You should create a static class for any method link to this property, Nested class is not avaible</para>
        /// </summary>
        public string ButtonActionStringClass { get; set; }
        /// <summary>
        /// Will be invoke if user click the button. Method must be public 
        /// <para>Format: {Method name without bracket} Example: CreateMethod</para>
        /// <para>You should create a static class for any method link to this property, Nested class is not avaible</para>
        /// </summary>
        public string ButtonActionStringMethod { get; set; }

        public async Task InvokeAsync()
        {
            return;
            var type = Type.GetType(ButtonActionStringClass);
            var method = type.GetMethod(ButtonActionStringMethod);
            await Task.Run(() => method.Invoke(null, null));
        }
    }


    public class UserInterestMananger
    {
        public static List<ModGenresInterests> UserInterest { get; set; } = new List<ModGenresInterests>();

        public static async Task AddInterest(string queryModFullname, GenreItem genre)
        {
            if (string.IsNullOrEmpty(queryModFullname))
                return;

            //Check if the mod is already define in the list
            var mod = UserInterest.FirstOrDefault(f => f.QueryModFullName == queryModFullname);
            if(mod == null)
            {
                mod = new ModGenresInterests();
                mod.QueryModFullName = queryModFullname;
                mod.GenresInterest = new List<GenreItemInterest>();
                var querier = ApiHelpper.CreateQueryAnimeObjectByClassName(queryModFullname);
                var genres = await querier.GetAnimeGenres();
                foreach (var item in genres)
                    mod.GenresInterest.Add(new GenreItemInterest() { Genre = item, DownloadCount = 0 });

                UserInterest.Add(mod);
            }

            //check if the last download genres is not the same as this download
            if(mod.FirstRecentDownload != null)
            {
                if(genre.Name != mod.FirstRecentDownload.Name)
                {
                    mod.ThirdRecentDownload = mod.SecondRecentDownload;
                    mod.SecondRecentDownload = mod.FirstRecentDownload;
                    mod.FirstRecentDownload = genre;
                }
            }

            var interest = mod.GenresInterest.FirstOrDefault(f => f.Genre.Name == genre.Name);
            //Add if the interest in not found in our list
            if(interest == null)
            {
                var newInterest = new GenreItemInterest();
                newInterest.Genre = new GenreItem() { Name = genre.Name, ID = genre.ID, Slug = genre.Slug };
                newInterest.DownloadCount = 0;

                mod.GenresInterest.Add(newInterest);
                interest = newInterest;
            }

            interest.DownloadCount++;
        }

        public static async Task<List<AnimeSeriesInfo>> GetSuggestion(string queryModFullname, int offset, int count)
        {
            var mod = UserInterest.FirstOrDefault(f => f.QueryModFullName == queryModFullname);
            var querier = ApiHelpper.CreateQueryAnimeObjectByClassName(queryModFullname);
            var rand = new Random();
            if(mod == null)
            {
                int clacFrom = offset;
                var max = await querier.GetSearchLimit(string.Empty, false);
                if (max > 0)
                    clacFrom = offset % max;

                return await querier.GetAnime(offset, count);
            }

            List<GenreItemInterest> interest = new List<GenreItemInterest>();

            //Get total genres download
            int totaGenreDownload = 0;
            for (int i = 0; i < mod.GenresInterest.Count; i++)
            {
                totaGenreDownload += mod.GenresInterest[i].DownloadCount;
                var thisGenres = mod.GenresInterest[i].Genre;
                interest.Add(new GenreItemInterest() { Genre = new GenreItem() { Name = thisGenres.Name, ID = thisGenres.ID, Slug = thisGenres.Slug}});
            }

            //Calcuate pool quantites
            for (int i = 0; i < interest.Count; i++)
            {
                int bonus = 0;
                if (mod.FirstRecentDownload != null)
                    if (mod.FirstRecentDownload.Name == interest[i].Genre.Name)
                        bonus += 15;
                if (mod.SecondRecentDownload != null)
                    if (mod.SecondRecentDownload.Name == interest[i].Genre.Name)
                        bonus += 10;
                if (mod.ThirdRecentDownload != null)
                    if (mod.ThirdRecentDownload.Name == interest[i].Genre.Name)
                        bonus += 5;

                interest[i].DownloadCount = (int)Math.Ceiling((mod.GenresInterest[i].DownloadCount / (double)totaGenreDownload + bonus) / 10d);
            }


            List<AnimeSeriesInfo> info = new List<AnimeSeriesInfo>();
            //Get the anime series
            while(info.Count < count)
            {
                for (int i = 0; i < interest.Count; i++)
                {
                    //get the anime pool
                    var calcFrom = offset;
                    var maxOffset = await querier.GetSearchLimit(interest[i].Genre.Slug, false);
                    var poolCount = interest[i].DownloadCount * 10;

                    if (maxOffset > 0)
                        calcFrom = offset % (maxOffset - count);

                    var pool = await querier.GetAnime(calcFrom, poolCount);
                    for (int j = 0; j < poolCount; j++)
                    {
                        AnimeSeriesInfo first = null;
                        AnimeSeriesInfo second = null;
                        AnimeSeriesInfo third = null;

                        for (int k = j * 10; k < (j + 1) * 10; k++)
                        {
                            if (first == null)
                                pool[k] = first;
                            else if (first.Views < pool[k].Views)
                            {
                                third = second;
                                second = first;
                                first = pool[k];
                            }
                            else if (second == null)
                                second = pool[k];
                            else if (second.Views < pool[k].Views)
                            {
                                third = second;
                                second = pool[k];
                            }
                            else if (third == null)
                                third = pool[k];
                            else if (third.Views < pool[k].Views)
                                third = pool[k];
                        }

                        switch (rand.Next(1,3))
                        {
                            case 1:
                                info.Add(first);
                                break;
                            case 2:
                                info.Add(second);
                                break;
                            default:
                                info.Add(third);
                                break;
                        }
                    }
                }
            }

            return info.Shuffle().ToList().GetRange(0, count);
        }
    }

    public class ModGenresInterests
    {
        public string QueryModFullName { get; set; }
        public List<GenreItemInterest> GenresInterest { get; set; }

        public GenreItem FirstRecentDownload { get; set; } = null;
        public GenreItem SecondRecentDownload { get; set; } = null;
        public GenreItem ThirdRecentDownload { get; set; } = null;
    }

    public class GenreItemInterest
    {
        public GenreItem Genre { get; set; }
        public int DownloadCount { get; set; }
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

        private static void Serialize() => JsonConvert.SerializeObject(HistoricalRequests);

        private static void Deserialize(string value) => HistoricalRequests = JsonConvert.DeserializeObject<List<RequestCacheItem>>(value);
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

            var queryRes = ManagerTypes.Where(query => className.Contains(className)).ToList();
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

            var queryRes = QueryTypes.Where(
                query => className.Contains(query.FullName)).ToList();
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
