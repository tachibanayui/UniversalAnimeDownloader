using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversalAnimeDownloader.ViewModel
{
    public class UADPlayerViewModel : ViewModelBase
    {
        private double seekLocation;
        public double SeekLocation
        {
            get { return seekLocation; }
            set
            {
                if(seekLocation != value)
                {
                    seekLocation = value;
                    OnPropertyChanged("SeekLocation");
                }
            }
        }

        public double PlayerVolume
        {
            get { return SettingsValues.PlaybackVolume; }
            set
            {
                if(SettingsValues.PlaybackVolume != value)
                {
                    SettingsValues.PlaybackVolume = value;
                    MediaElementVolume = SettingsValues.PlaybackVolume / 100;
                    OnPropertyChanged("PlayerVolume");
                }
            }
        }

        private double mediaElementVolume;
        public double MediaElementVolume
        {
            get { return mediaElementVolume; }
            set
            {
                if(mediaElementVolume != value)
                {
                    mediaElementVolume = value;
                    OnPropertyChanged("MediaElementVolume");
                }
            }
        }

        public UADPlayerViewModel()
        {
            MediaElementVolume = SettingsValues.PlaybackVolume / 100;
        }
    }
}
