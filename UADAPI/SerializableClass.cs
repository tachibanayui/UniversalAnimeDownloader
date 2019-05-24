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
    public class EpisodeInfo : BaseViewModel
    {
        /// <summary>
        /// Epsisode ID define in anime remote host
        /// </summary>
        private int _EpisodeID;
        public int EpisodeID
        {
            get
            {
                return _EpisodeID;
            }
            set
            {
                if (_EpisodeID != value)
                {
                    _EpisodeID = value;
                    OnPropertyChanged();
                }
            }
        }



        /// <summary>
        /// The name of this episode
        /// </summary>
        private string _Name;
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    OnPropertyChanged();
                }
            }
        }


        /// <summary>
        /// The slug of this episode
        /// </summary>
        private string _Slug;
        public string Slug
        {
            get
            {
                return _Slug;
            }
            set
            {
                if (_Slug != value)
                {
                    _Slug = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// This will define how to sort the episde list
        /// </summary>
        private int _Index;
        public int Index
        {
            get
            {
                return _Index;
            }
            set
            {
                if (_Index != value)
                {
                    _Index = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// The request to get the thumbnail
        /// </summary>
        private MediaSourceInfo _Thumbnail;
        public MediaSourceInfo Thumbnail
        {
            get
            {
                return _Thumbnail;
            }
            set
            {
                if (_Thumbnail != value)
                {
                    _Thumbnail = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// The video source of this episode. string will represent the quality
        /// </summary>
        private Dictionary<VideoQuality, MediaSourceInfo> _FilmSources;
        public Dictionary<VideoQuality, MediaSourceInfo> FilmSources
        {
            get
            {
                return _FilmSources;
            }
            set
            {
                if (_FilmSources != value)
                {
                    _FilmSources = value;
                    OnPropertyChanged();
                }
            }
        }



        /// <summary>
        /// This will tell the state of the download, mostly for downloader to report.
        /// </summary>
        private DownloaderProgress _EpisodeDownloadState;
        public DownloaderProgress EpisodeDownloadState
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



        /// <summary>
        /// If the episode is downloaded, can be able to play without internet connection
        /// </summary>
        private bool _AvailableOffline;
        public bool AvailableOffline
        {
            get
            {
                return _AvailableOffline;
            }
            set
            {
                if (_AvailableOffline != value)
                {
                    _AvailableOffline = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// The request information to get this episode
        /// </summary>
        private MediaSourceInfo _EpisodeSource;
        public MediaSourceInfo EpisodeSource
        {
            get
            {
                return _EpisodeSource;
            }
            set
            {
                if (_EpisodeSource != value)
                {
                    _EpisodeSource = value;
                    OnPropertyChanged();
                }
            }
        }
    }

    /// <summary>
    /// This class contain information about the anime
    /// </summary>
    public class AnimeSeriesInfo : BaseViewModel
    {
        /// <summary>
        /// The name of this anime series
        /// </summary>
        private string _Name;
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// The slug of this anime
        /// </summary>
        private string _Slug;
        public string Slug
        {
            get
            {
                return _Slug;
            }
            set
            {
                if (_Slug != value)
                {
                    _Slug = value;
                    OnPropertyChanged();
                }
            }
        }



        /// <summary>
        /// Views
        /// </summary>
        private int _Views;
        public int Views
        {
            get
            {
                return _Views;
            }
            set
            {
                if (_Views != value)
                {
                    _Views = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// The ID of this anime series in the remote host
        /// </summary>
        private int _AnimeID;
        public int AnimeID
        {
            get
            {
                return _AnimeID;
            }
            set
            {
                if (_AnimeID != value)
                {
                    _AnimeID = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Current quality of the video after getting information. 
        /// API Dev: Change this inside GetQualities();
        /// Status: Not implemented
        /// </summary>
        private string _CurrentQuality;
        public string CurrentQuality
        {
            get
            {
                return _CurrentQuality;
            }
            set
            {
                if (_CurrentQuality != value)
                {
                    _CurrentQuality = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// If this anime series/season is ended. If true, it wil ignore update check
        /// </summary>
        private bool _HasEnded;
        public bool HasEnded
        {
            get
            {
                return _HasEnded;
            }
            set
            {
                if (_HasEnded != value)
                {
                    _HasEnded = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// The thumbnail of this anime series
        /// </summary>
        private MediaSourceInfo _Thumbnail;
        public MediaSourceInfo Thumbnail
        {
            get
            {
                return _Thumbnail;
            }
            set
            {
                if (_Thumbnail != value)
                {
                    _Thumbnail = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// The genres of this anime
        /// <para>Note: Please don't create a new instance of this, use <see cref="ObservableCollection{T}"/>.Clear() instead</para>
        /// </summary>
        public ObservableCollection<GenreItem> Genres { get; set; }

        /// <summary>
        /// Indicate if the user not download all the episdes of this anime series/season, true : will ignore update check
        /// 
        /// </summary>
        private bool _IsSelectiveDownload;
        public bool IsSelectiveDownload
        {
            get
            {
                return _IsSelectiveDownload;
            }
            set
            {
                if (_IsSelectiveDownload != value)
                {
                    _IsSelectiveDownload = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// The description of this anime series
        /// </summary>
        private string _Description;
        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                if (_Description != value)
                {
                    _Description = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// The epsides of this anime series. Only save the source
        /// <para>Note: Please don't create a new instance of this, use <see cref="ObservableCollection{T}"/>.Clear() instead</para>
        /// </summary>
        public ObservableCollection<EpisodeInfo> Episodes { get; set; } = new ObservableCollection<EpisodeInfo>();

        /// <summary>
        /// Indicate wether the episodes got don't have enough detail
        /// </summary>
        private bool _IsPrototypeEpisdodes;
        public bool IsPrototypeEpisdodes
        {
            get
            {
                return _IsPrototypeEpisdodes;
            }
            set
            {
                if (_IsPrototypeEpisdodes != value)
                {
                    _IsPrototypeEpisdodes = value;
                    OnPropertyChanged();
                }
            }
        }


        /// <summary>
        /// The request information to get this anime
        /// </summary>
        private MediaSourceInfo _AnimeSource;
        public MediaSourceInfo AnimeSource
        {
            get
            {
                return _AnimeSource;
            }
            set
            {
                if (_AnimeSource != value)
                {
                    _AnimeSource = value;
                    OnPropertyChanged();
                }
            }
        }



        /// <summary>
        /// The extractor used to get this anime. Use to invoke the extractor in manager file. Not notifying when changed
        /// </summary>
        public ModificatorInformation ModInfo { get; set; }

        /// <summary>
        /// Where is the Manager file saved location
        /// </summary>
        private string _ManagerFileLocation;
        public string ManagerFileLocation
        {
            get
            {
                return _ManagerFileLocation;
            }
            set
            {
                if (_ManagerFileLocation != value)
                {
                    _ManagerFileLocation = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// The directory of this anime series
        /// </summary>
        private string _AnimeSeriesSavedDirectory;
        public string AnimeSeriesSavedDirectory
        {
            get
            {
                return _AnimeSeriesSavedDirectory;
            }
            set
            {
                if (_AnimeSeriesSavedDirectory != value)
                {
                    _AnimeSeriesSavedDirectory = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Indicate that this series is created by a anime series editor
        /// </summary>
        private bool _IsCustomSeries;
        public bool IsCustomSeries
        {
            get
            {
                return _IsCustomSeries;
            }
            set
            {
                if (_IsCustomSeries != value)
                {
                    _IsCustomSeries = value;
                    OnPropertyChanged();
                }
            }
        }

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
        Quality360p = 2,
        Quality480p = 3,
        Quality720p = 4,
        Quality1080p = 5,
        Quality2160p = 6
    }
}
