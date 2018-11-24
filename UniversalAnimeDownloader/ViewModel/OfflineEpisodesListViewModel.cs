using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversalAnimeDownloader.ViewModel
{
    class OfflineEpisodesListViewModel : ViewModelBase
    {
        private string episodeName;
        public string EpisodeName
        {
            get { return episodeName; }
            set
            {
                if (episodeName != value)
                {
                    episodeName = value;
                    OnPropertyChanged("EpisodeName");
                }
            }
        }

        private string mediaLocation;
        public string MediaLocation
        {
            get { return mediaLocation; }
            set
            {
                if (mediaLocation != value)
                {
                    mediaLocation = value;
                    OnPropertyChanged("MediaLocation");
                }
            }
        }

        private PackIconKind iconKind;
        public PackIconKind IconKind
        {
            get { return iconKind; }
            set
            {
                if(iconKind != value)
                {
                    iconKind = value;
                    OnPropertyChanged("IconKind");
                }
            }
        }

    }
}
