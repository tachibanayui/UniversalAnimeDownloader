using SegmentDownloader.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UADAPI
{
    public class DownloadCompletedEventArgs : EventArgs
    {
        public bool IsSuccess { get; set; }
        public SegmentDownloader.Core.Downloader InnerDownloader { get; set; }
        public EpisodeInfo CompletedEpisode { get; set; }
    }

    public class DownloadProgressChangedEventArgs : EventArgs
    {
        public long FileSize { get; set; }
        public TimeSpan EstimatedTimeLeft { get; set; }
        public double Progress { get; set; }
        public double DownloadSpeed { get; set; }
        public long Transfered { get; set; }
        public DownloaderState State { get; set; }
        public List<Segment> Segments { get; set; }
    }
}
