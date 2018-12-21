using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversalAnimeDownloader.ViewModel
{
    public class SettingsDownloadViewModel : ViewModelBase
    {
        private int filePerDownload;
        public int FilePerDownload
        {
            get { return filePerDownload; }
            set
            {
                if(filePerDownload != value)
                {
                    filePerDownload = value;
                    OnPropertyChanged("FilePerDownload");
                }
            }
        }
    }
}
