using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
                if (seekLocation != value)
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
                if (SettingsValues.PlaybackVolume != value)
                {
                    SettingsValues.PlaybackVolume = value;
                    MediaElementVolume = SettingsValues.PlaybackVolume / 100;
                    OnPropertyChanged("PlayerVolume");
                }
            }
        }
        public bool IsDrawingEnabled
        {
            get { return SettingsValues.IsDrawingEnabled; }
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
            get { return new DrawingAttributes() { Color = SettingsValues.PrimaryPenColor, Width = SettingsValues.PrimaryBurshThickness, Height = SettingsValues.PrimaryBurshThickness }; }
            set
            {
                SettingsValues.PrimaryBurshThickness = value.Height;
                SettingsValues.PrimaryPenColor = value.Color;
            }
        }

        public DrawingAttributes SecondaryPen
        {
            get { return new DrawingAttributes() { Color = SettingsValues.SecondaryPenColor, Width = SettingsValues.SecondaryBurshThickness, Height = SettingsValues.SecondaryBurshThickness }; }
            set
            {
                SettingsValues.SecondaryBurshThickness = value.Height;
                SettingsValues.SecondaryPenColor = value.Color;
            }
        }

        public DrawingAttributes HighlighterPen
        {
            get { return new DrawingAttributes() { IsHighlighter = true, Color = SettingsValues.HighlighterPenColor, Width = SettingsValues.HighlighterBurshThickness, Height = SettingsValues.HighlighterBurshThickness }; }
            set
            {
                SettingsValues.HighlighterBurshThickness = value.Height;
                SettingsValues.HighlighterPenColor = value.Color;
            }
        }

        public double PlayerBorderThickness
        {
            get
            {
                if (SettingsValues.IsSneakyWatcherEnabled && SettingsValues.IsSneakyWatcherBorderEnabled)
                    return 2;
                else
                    return 0;
            }
        }

        public bool IsSneakyWatcherEnabled
        {
            get
            {
                if (SettingsValues.IsSneakyWatcherEnabled)
                    return true;
                else
                    return false;
            }
        }

        public SolidColorBrush ScreenBlockerColor => new SolidColorBrush(SettingsValues.BlockerColor);

        public bool ShowBlockerImage => IsBlockerActive && SettingsValues.IsBlockerImageEnabled;

        public ImageSource BlockerImageSource
        {
            get
            {
                if(!string.IsNullOrEmpty(SettingsValues.BlockerImageLocation))
                    return new BitmapImage(new Uri(SettingsValues.BlockerImageLocation));
                else
                    return new BitmapImage();
            }
        }

        public Stretch BlockerStretchMode => SettingsValues.BlockerStretchMode;

        public UADPlayerViewModel()
        {
            MediaElementVolume = SettingsValues.PlaybackVolume / 100;
            InkCanvasVisibility = Visibility.Collapsed;
        }
    }
}
