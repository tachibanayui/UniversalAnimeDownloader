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


        //These properties and fields only be used for segmented downloader to report data
        private string downloadRate;
        public string DownloadRate
        {
            get { return downloadRate; }
            private set
            {
                if (value != downloadRate)
                {
                    downloadRate = value;
                    OnPropertyChanged("DownloadRate");
                }
            }
        }
        public void SetDownloadRate(double byteRate)
        {
            double convertedValue = byteRate;
            string unitType = "Byte/s";

            
            if(byteRate < Math.Pow(2,20))
            {
                convertedValue /= Math.Pow(2, 10);
                unitType = "KB/s";
            }
            else if (byteRate < Math.Pow(2, 30))
            {
                convertedValue /= Math.Pow(2, 20);
                unitType = "MB/s";
            }

            DownloadRate = string.Format("{0:N2}", convertedValue) + unitType;
        }

        private string eta;
        public string Eta
        {
            get { return eta; }
            private set
            {
                if (value != eta)
                {
                    eta = value;
                    OnPropertyChanged("Eta");
                }
            }
        }
        public void SetEta(TimeSpan eta) => Eta = eta.ToString("h'h 'm'm 's's'");

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
            DownloadRate = "N/A";
            Eta = "N/A";
        }
    }
}
