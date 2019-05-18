using Newtonsoft.Json;
using SegmentDownloader.Core;
using SegmentDownloader.Protocol;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ErrorEventArgs = Newtonsoft.Json.Serialization.ErrorEventArgs;

namespace UADAPI
{
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

            DownloadInstance ins = new DownloadInstance() { AttachedManager = manager, EpisodeId = episodeId, PreferedQuality = (VideoQuality)Enum.Parse(typeof(VideoQuality), "Quality" + quality) };

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

                Instances.Clear();
                foreach (var item in tmpIns)
                    Instances.Add(item);
            }
        }
    }
}
