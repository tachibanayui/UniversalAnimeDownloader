using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace UniversalAnimeDownloader
{
    public class RequestingWindowStateEventArgs : EventArgs
    {
        public WindowState RequestState { get; set; }
    }

}
