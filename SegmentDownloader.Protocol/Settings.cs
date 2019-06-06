using System;
using System.Collections.Generic;
using System.Text;

namespace SegmentDownloader.Protocol
{
    class Settings
    {
        private static Settings _Default;
        public static Settings Default { get => _Default ?? new Settings(); set => _Default = value; }

        public string ProxyAddress { get; set; }
        public string ProxyUserName { get; set; }
        public string ProxyPassword { get; set; }
        public string ProxyDomain { get; set; }
        public bool UseProxy { get; set; } = false;
        public bool ProxyByPassOnLocal { get; set; } = true;
        public int ProxyPort { get; set; } = 80;
    }
}
