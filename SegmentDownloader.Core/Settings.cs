using System;
using System.Collections.Generic;
using System.Text;

namespace SegmentDownloader.Core
{
    class Settings
    {
        private static Settings _Default;
        public static Settings Default { get => _Default ?? new Settings(); set => _Default = value; }

        public long MinSegmentSize { get; set; } = 200000;
        public int MinSegmentLeftToStartNewSegment { get; set; } = 30;
        public int InitialRetryDelay { get; set; } = 5;
        public int InitialMaxRetries { get; set; } = 5;
        public int RetryDelay { get; set; } = 5;
        public int MaxRetries { get; set; } = 10;
        public int MaxSegments { get; set; } = 8;
        public string DownloadFolder { get; set; }
    }
}
