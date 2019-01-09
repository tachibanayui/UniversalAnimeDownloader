using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversalAnimeDownloader.Settings;

namespace UniversalAnimeDownloader.ViewModel
{
    public class SettingsAppearanceViewModel : ViewModelBase
    {
        public double Heading
        {
            get => SettingsManager.Current.FontSizeHeading;
            set => SettingsManager.Current.FontSizeHeading = value;
        }

        public double Heading2
        {
            get => SettingsManager.Current.FontSizeHeading2;
            set => SettingsManager.Current.FontSizeHeading2 = value;
        }

        public double Heading3
        {
            get => SettingsManager.Current.FontSizeHeading3;
            set => SettingsManager.Current.FontSizeHeading3 = value;
        }

        public double Heading4
        {
            get => SettingsManager.Current.FontSizeHeading4;
            set => SettingsManager.Current.FontSizeHeading4 = value;
        }

        public bool IsBGMenubarEnabled
        {
            get => SettingsManager.Current.EnableBGMenubarImage;
            set => SettingsManager.Current.EnableBGMenubarImage = value;
        }

        public string BGMenubarImage
        {
            get => SettingsManager.Current.BGMenubarImageLocation;
            set => SettingsManager.Current.BGMenubarImageLocation = value;
        }

        public bool IsBGViewerEnabled
        {
            get => SettingsManager.Current.EnableBGViewerImage;
            set => SettingsManager.Current.EnableBGViewerImage = value;
        }

        public string BGViewerImage
        {
            get => SettingsManager.Current.BGViewerImageLocation;
            set => SettingsManager.Current.BGViewerImageLocation = value;
        }
    }
}
