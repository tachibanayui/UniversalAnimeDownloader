using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace UniversalAnimeDownloader.MediaPlayer
{
    public class RequestingWindowStateEventArgs : EventArgs
    {
        public WindowState RequestState { get; set; }
    }

    public class RequestWindowIconChangeEventArgs : EventArgs
    {
        public Uri IconLocation { get; set; }
    }
}
