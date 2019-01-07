using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using UniversalAnimeDownloader.Settings;

namespace UniversalAnimeDownloader.ViewModel
{
    public class UADPlayerViewModel : ViewModelBase
    {
        private SettingValues AppSettings = SettingsManager.Current;

        private double seekLocation;
        public double SeekLocation
        {
            get { return seekLocation; }
            set
            {
                if (seekLocation != value)
                {
                    seekLocation = value;
                    OnPropertyChanged("SeekLocation");
                }
            }
        }

        public double PlayerVolume
        {
            get { return AppSettings.PlaybackVolume; }
            set
            {
                if (AppSettings.PlaybackVolume != value)
                {
                    AppSettings.PlaybackVolume = value;
                    MediaElementVolume = AppSettings.PlaybackVolume / 100;
                    OnPropertyChanged("PlayerVolume");
                }
            }
        }
        public bool IsDrawingEnabled
        {
            get { return AppSettings.IsDrawingEnabled; }
        }

        private double mediaElementVolume;
        public double MediaElementVolume
        {
            get { return mediaElementVolume; }
            set
            {
                if (mediaElementVolume != value)
                {
                    mediaElementVolume = value;
                    OnPropertyChanged("MediaElementVolume");
                }
            }
        }

        private Visibility inkCanvasVisibility;
        public Visibility InkCanvasVisibility
        {
            get { return inkCanvasVisibility; }
            set
            {
                if (inkCanvasVisibility != value)
                {
                    inkCanvasVisibility = value;
                    OnPropertyChanged("InkCanvasVisibility");
                }
            }
        }

        private bool isBlockerActive;
        public bool IsBlockerActive
        {
            get { return isBlockerActive; }
            set
            {
                if(isBlockerActive != value)
                {
                    isBlockerActive = value;
                    OnPropertyChanged("IsBlockerActive");
                    OnPropertyChanged("ShowBlockerImage");
                }
            }
        }

        public DrawingAttributes PrimaryPen
        {
            get { return new DrawingAttributes() { Color = AppSettings.PrimaryPenColor, Width = AppSettings.PrimaryBurshThickness, Height = AppSettings.PrimaryBurshThickness }; }
            set
            {
                AppSettings.PrimaryBurshThickness = value.Height;
                AppSettings.PrimaryPenColor = value.Color;
            }
        }

        public DrawingAttributes SecondaryPen
        {
            get { return new DrawingAttributes() { Color = AppSettings.SecondaryPenColor, Width = AppSettings.SecondaryBurshThickness, Height = AppSettings.SecondaryBurshThickness }; }
            set
            {
                AppSettings.SecondaryBurshThickness = value.Height;
                AppSettings.SecondaryPenColor = value.Color;
            }
        }

        public DrawingAttributes HighlighterPen
        {
            get { return new DrawingAttributes() { IsHighlighter = true, Color = AppSettings.HighlighterPenColor, Width = AppSettings.HighlighterBurshThickness, Height = AppSettings.HighlighterBurshThickness }; }
            set
            {
                AppSettings.HighlighterBurshThickness = value.Height;
                AppSettings.HighlighterPenColor = value.Color;
            }
        }

        public double PlayerBorderThickness
        {
            get
            {
                if (AppSettings.IsSneakyWatcherEnabled && AppSettings.IsSneakyWatcherBorderEnabled)
                    return 2;
                else
                    return 0;
            }
        }

        public bool IsSneakyWatcherEnabled
        {
            get
            {
                if (AppSettings.IsSneakyWatcherEnabled)
                    return true;
                else
                    return false;
            }
        }

        public SolidColorBrush ScreenBlockerColor => new SolidColorBrush(AppSettings.BlockerColor);

        public bool ShowBlockerImage => IsBlockerActive && AppSettings.IsBlockerImageEnabled;

        public ImageSource BlockerImageSource
        {
            get
            {
                if(!string.IsNullOrEmpty(AppSettings.BlockerImageLocation))
                    return new BitmapImage(new Uri(AppSettings.BlockerImageLocation));
                else
                    return new BitmapImage();
            }
        }

        public Stretch BlockerStretchMode => AppSettings.BlockerStretchMode;

        public UADPlayerViewModel()
        {
            MediaElementVolume = AppSettings.PlaybackVolume / 100;
            InkCanvasVisibility = Visibility.Collapsed;
        }
    }
}
