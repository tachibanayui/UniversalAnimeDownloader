using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversalAnimeDownloader.ViewModel
{
    public class SettingsGeneralViewModel : ViewModelBase
    {
        private double backGroundMusicVolume;
        public double BackGroundMusicVolume
        {
            get { return backGroundMusicVolume; }
            set
            {
                if(backGroundMusicVolume != value)
                {
                    backGroundMusicVolume = value;
                    ChangeVolumeIcon();
                    OnPropertyChanged("BackGroundMusicVolume");
                }
            }
        }

        private PackIconKind backgroundVolumeIcon;
        public PackIconKind BackgroundVolumeIcon
        {
            get { return backgroundVolumeIcon; }
            set
            {
                if(backgroundVolumeIcon != value)
                {
                    backgroundVolumeIcon = value;
                    OnPropertyChanged("BackgroundVolumeIcon");
                }
            }
        }

        private void ChangeVolumeIcon()
        {
            if (BackGroundMusicVolume == 0)
                BackgroundVolumeIcon = PackIconKind.VolumeMute;
            else if (BackGroundMusicVolume < 33)
                BackgroundVolumeIcon = PackIconKind.VolumeLow;
            else if (BackGroundMusicVolume < 66)
                BackgroundVolumeIcon = PackIconKind.VolumeMedium;
            else if (BackGroundMusicVolume < 100)
                BackgroundVolumeIcon = PackIconKind.VolumeHigh;
        }

        public SettingsGeneralViewModel()
        {
            BackgroundVolumeIcon = PackIconKind.VolumeMedium;
            BackGroundMusicVolume = 50;
        }
    }
}
