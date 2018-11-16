using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversalAnimeDownloader.ViewModel
{
    public class OnlineEpisodesListViewModel : ViewModelBase
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

        private string progress;
        public string Progress
        {
            get { return progress; }
            set
            {
                if (progress != value)
                    progress = value;
                OnPropertyChanged("Progress");
            }
        }

        private PackIconKind buttonKind;
        public PackIconKind ButtonKind
        {
            get { return buttonKind; }
            set
            {
                if (value != buttonKind)
                {
                    buttonKind = value;
                    OnPropertyChanged("ButtonKind");
                }
            }
        }
    }
}
