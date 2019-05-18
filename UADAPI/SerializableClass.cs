using Newtonsoft.Json;
using SegmentDownloader.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using UADAPI.PlatformSpecific;

namespace UADAPI
{

    /// <summary>
    /// This class will contains all the information required to get resource from remote host
    /// </summary>
    public class MediaSourceInfo
    {
        private string _LocalFile;

        /// <summary>
        /// Tell the request method where to save the result. If you don't want to save to local file, change SaveLocalFile to false
        /// </summary>
        public string LocalFile
        {
            get { return _LocalFile; }
            set
            {
                if (_LocalFile != value)
                {
                    _LocalFile = value;
                    SaveLocalFile = !string.IsNullOrEmpty(_LocalFile);
                }
            }
        }

        /// <summary>
        /// Tell the request method whether to save to local file. If false, the LocalFile property will be ignored.
        /// </summary>
        public bool SaveLocalFile { get; set; } = false;

        /// <summary>
        /// The url will be requested
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// The header will be applied when requested. You can also add invalid header such as Referer, Host,...
        /// </summary>
        public WebHeaderCollection Headers { get; set; }

        /// <summary>
        /// Indicate whether the request finished, sometime refer to is it a successful request
        /// </summary>
        public bool IsFinishedRequesting { get; set; }

        /// <summary>
        /// Result of the request, if the SaveLocalFile is true, this will be null
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// String reprentation if the result request
        /// </summary>
        public string ValueString =>
            SaveLocalFile ? /*File.ReadAllText(LocalFile.ToString())*/null : Result;
    }

    /// <summary>
    /// This class will store information about the episode
    /// </summary>
    public class EpisodeInfo
    {
        /// <summary>
        /// Epsisode ID define in anime remote host
        /// </summary>
        public int EpisodeID { get; set; }

        /// <summary>
        /// The name of this episode
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The slug of this episode
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// This will define how to sort the episde list
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// The request to get the thumbnail
        /// </summary>
        public MediaSourceInfo Thumbnail { get; set; }

        /// <summary>
        /// The video source of this episode. string will represent the quality
        /// </summary>
        public Dictionary<VideoQuality, MediaSourceInfo> FilmSources { get; set; }

        /// <summary>
        /// This will tell the state of the download, mostly for downloader to report.
        /// </summary>
        public DownloaderProgress EpisodeDownloadState { get; set; } = new DownloaderProgress();

        /// <summary>
        /// If the episode is downloaded, can be able to play without internet connection
        /// </summary>
        public bool AvailableOffline { get; set; }

        /// <summary>
        /// The request information to get this episode
        /// </summary>
        public MediaSourceInfo EpisodeSource { get; set; }
    }

    /// <summary>
    /// This class contain information about the anime
    /// </summary>
    public class AnimeSeriesInfo
    {
        /// <summary>
        /// The name of this anime series
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The slug of this anime
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// Views
        /// </summary>
        public int Views { get; set; }

        /// <summary>
        /// The ID of this anime series in the remote host
        /// </summary>
        public int AnimeID { get; set; }

        /// <summary>
        /// Current quality of the video after getting information. 
        /// API Dev: Change this inside GetQualities();
        /// </summary>
        public string CurrentQuality { get; set; }

        /// <summary>
        /// If this anime series/season is ended. If true, it wil ignore update check
        /// </summary>
        public bool HasEnded { get; set; }

        /// <summary>
        /// The thumbnail of this anime series
        /// </summary>
        public MediaSourceInfo Thumbnail { get; set; }

        /// <summary>
        /// The genres of this anime
        /// </summary>
        public List<GenreItem> Genres { get; set; }

        /// <summary>
        /// Indicate if the user not download all the episdes of this anime series/season, true : will ignore update check
        /// </summary>
        public bool IsSelectiveDownload { get; set; }

        /// <summary>
        /// The description of this anime series
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The epsides of this anime series. Only save the source
        /// </summary>
        public List<EpisodeInfo> Episodes { get; set; }

        /// <summary>
        /// Indicate wether the episodes got don't have enough detail
        /// </summary>
        public bool IsPrototypeEpisdodes { get; set; }

        /// <summary>
        /// The request information to get this anime
        /// </summary>
        public MediaSourceInfo AnimeSource { get; set; }

        /// <summary>
        /// The extractor used to get this anime. Use to invoke the extractor in manager file
        /// </summary>
        public ModificatorInformation ModInfo { get; set; }

        /// <summary>
        /// Where is the Manager file saved location
        /// </summary>
        public string ManagerFileLocation { get; set; }

        /// <summary>
        /// The directory of this anime series
        /// </summary>
        public string AnimeSeriesSavedDirectory { get; set; }

        //public override bool Equals(object obj) => AnimeID == (obj as AnimeSeriesInfo).AnimeID;

        //public override int GetHashCode()
        //{
        //    return base.GetHashCode();
        //}
    }

    /// <summary>
    /// Store information about the historical request
    /// </summary>
    public class RequestCacheItem
    {
        /// <summary>
        /// When the data is requested
        /// </summary>
        public DateTime RequestedDateTime { get; set; }

        /// <summary>
        /// Historyical request url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Historical request headers
        /// </summary>
        public WebHeaderCollection Headers { get; set; }

        /// <summary>
        /// The result of the request
        /// </summary>
        public MemoryStream Result { get; set; }
    }

    /// <summary>
    /// Store both slug and name
    /// </summary>
    public class GenreItem
    {
        public string Name { get; set; }
        public string Slug { get; set; }
        public int ID { get; set; }

        //public static bool operator ==(GenreItem a, GenreItem b) => a.Name == b.Name;
        //public static bool operator !=(GenreItem a, GenreItem b)
        //{
        //    if (a != null || b != null)
        //        return a.Name != b.Name;
        //    else
        //        return false;
        //}

        //public override bool Equals(object obj) => (obj as GenreItem).Name == Name;
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }


    public class SeasonItem
    {
        public string Name { get; set; }
        public string Slug { get; set; }
        public string ID { get; set; }
    }

    public class DownloaderProgress : BaseViewModel
    {
        private long _FileSize;
        public long FileSize
        {
            get
            {
                return _FileSize;
            }
            set
            {
                if (_FileSize != value)
                {
                    _FileSize = value;
                    OnPropertyChanged();
                }
            }
        }

        private TimeSpan _EstimatedTimeLeft;
        public TimeSpan EstimatedTimeLeft
        {
            get
            {
                return _EstimatedTimeLeft;
            }
            set
            {
                if (_EstimatedTimeLeft != value)
                {
                    _EstimatedTimeLeft = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _Progress;
        public double Progress
        {
            get
            {
                return _Progress;
            }
            set
            {
                if (_Progress != value)
                {
                    _Progress = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _DownloadSpeed;
        public double DownloadSpeed
        {
            get
            {
                return _DownloadSpeed;
            }
            set
            {
                if (_DownloadSpeed != value)
                {
                    _DownloadSpeed = value;
                    OnPropertyChanged();
                }
            }
        }

        private long _Transfered;
        public long Transfered
        {
            get
            {
                return _Transfered;
            }
            set
            {
                if (_Transfered != value)
                {
                    _Transfered = value;
                    OnPropertyChanged();
                }
            }
        }

        private DownloaderState _State;
        public DownloaderState State
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

        private EpisodeDownloadState _EpisodeDownloadState;
        public EpisodeDownloadState EpisodeDownloadState
        {
            get
            {
                return _EpisodeDownloadState;
            }
            set
            {
                if (_EpisodeDownloadState != value)
                {
                    _EpisodeDownloadState = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<Segment> _Segments = new ObservableCollection<Segment>();
        [JsonIgnore]
        public ObservableCollection<Segment> Segments
        {
            get
            {
                return _Segments;
            }
            set
            {
                if (_Segments != value)
                {
                    _Segments = value;
                    OnPropertyChanged();
                }
            }
        }
    }

    public enum VideoQuality
    {
        Quality144p = 0,
        Quality240p = 1,
        Quality480p = 2,
        Quality720p = 3,
        Quality1080p = 4,
        Quality2160p = 5
    }
}
