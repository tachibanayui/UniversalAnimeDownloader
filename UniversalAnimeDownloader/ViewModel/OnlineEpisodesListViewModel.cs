using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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

        private double progress;
        public double Progress
        {
            get { return progress; }
            set
            {
                if (progress != value)
                    progress = value;
                OnPropertyChanged("Progress");
            }
        }

        private double byteReceived;
        public double ByteReceived
        {
            get { return byteReceived; }
            set
            {
                if(byteReceived != value)
                {
                    byteReceived = value;
                    OnPropertyChanged("ByteReceived");
                }
            }
        }

        private double byteToReceive;
        public double ByteToReceive
        {
            get { return byteToReceive; }
            set
            {
                if (byteToReceive != value)
                {
                    byteToReceive = value;
                    OnPropertyChanged("ByteToReceive");
                }
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

        private Visibility detailVisibility;
        public Visibility DetailVisibility
        {
            get { return detailVisibility; }
            set
            {
                if (detailVisibility != value)
                {
                    detailVisibility = value;
                    OnPropertyChanged("DetailVisibility");
                }
            }
        }

        public OnlineEpisodesListViewModel()
        {
            DetailVisibility = Visibility.Collapsed;
        }
    }
}
