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
            get { return SettingsManager.Current.PlaybackVolume; }
            set
            {
                if (SettingsManager.Current.PlaybackVolume != value)
                {
                    SettingsManager.Current.PlaybackVolume = value;
                    MediaElementVolume = SettingsManager.Current.PlaybackVolume / 100;
                    OnPropertyChanged("PlayerVolume");
                }
            }
        }
        public bool IsDrawingEnabled
        {
            get { return SettingsManager.Current.IsDrawingEnabled; }
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
            get { return new DrawingAttributes() { Color = SettingsManager.Current.PrimaryPenColor, Width = SettingsManager.Current.PrimaryBurshThickness, Height = SettingsManager.Current.PrimaryBurshThickness }; }
            set
            {
                SettingsManager.Current.PrimaryBurshThickness = value.Height;
                SettingsManager.Current.PrimaryPenColor = value.Color;
            }
        }

        public DrawingAttributes SecondaryPen
        {
            get { return new DrawingAttributes() { Color = SettingsManager.Current.SecondaryPenColor, Width = SettingsManager.Current.SecondaryBurshThickness, Height = SettingsManager.Current.SecondaryBurshThickness }; }
            set
            {
                SettingsManager.Current.SecondaryBurshThickness = value.Height;
                SettingsManager.Current.SecondaryPenColor = value.Color;
            }
        }

        public DrawingAttributes HighlighterPen
        {
            get { return new DrawingAttributes() { IsHighlighter = true, Color = SettingsManager.Current.HighlighterPenColor, Width = SettingsManager.Current.HighlighterBurshThickness, Height = SettingsManager.Current.HighlighterBurshThickness }; }
            set
            {
                SettingsManager.Current.HighlighterBurshThickness = value.Height;
                SettingsManager.Current.HighlighterPenColor = value.Color;
            }
        }

        public double PlayerBorderThickness
        {
            get
            {
                if (SettingsManager.Current.IsSneakyWatcherEnabled && SettingsManager.Current.IsSneakyWatcherBorderEnabled)
                    return 2;
                else
                    return 0;
            }
        }

        public bool IsSneakyWatcherEnabled
        {
            get
            {
                if (SettingsManager.Current.IsSneakyWatcherEnabled)
                    return true;
                else
                    return false;
            }
        }

        public SolidColorBrush ScreenBlockerColor => new SolidColorBrush(SettingsManager.Current.BlockerColor);

        public bool ShowBlockerImage => IsBlockerActive && SettingsManager.Current.IsBlockerImageEnabled;

        public ImageSource BlockerImageSource
        {
            get
            {
                if(!string.IsNullOrEmpty(SettingsManager.Current.BlockerImageLocation))
                    return new BitmapImage(new Uri(SettingsManager.Current.BlockerImageLocation));
                else
                    return new BitmapImage();
            }
        }

        public Stretch BlockerStretchMode => SettingsManager.Current.BlockerStretchMode;

       
        public UADPlayerViewModel()
        {
            MediaElementVolume = SettingsManager.Current.PlaybackVolume / 100;
            InkCanvasVisibility = Visibility.Collapsed;
        }

        public void UpdateBindings()
        {
            OnPropertyChanged("SeekLocation");
            OnPropertyChanged("MediaElementVolume");
            OnPropertyChanged("InkCanvasVisibility");
            OnPropertyChanged("PlayerVolume");
            OnPropertyChanged("IsBlockerActive");
            OnPropertyChanged("ShowBlockerImage");
            OnPropertyChanged("IsDrawingEnabled");
            OnPropertyChanged("PrimaryPen");
            OnPropertyChanged("SecondaryPen");
            OnPropertyChanged("HighlighterPen");
            OnPropertyChanged("PlayerBorderThickness");
            OnPropertyChanged("IsSneakyWatcherEnabled");
            OnPropertyChanged("ScreenBlockerColor");
            OnPropertyChanged("BlockerImageSource");
            OnPropertyChanged("BlockerStretchMode");
        }
    }
}
