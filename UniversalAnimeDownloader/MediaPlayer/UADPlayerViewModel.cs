using System;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using UniversalAnimeDownloader.UADSettingsPortal;
using UniversalAnimeDownloader.ViewModels;

namespace UniversalAnimeDownloader.MediaPlayer
{
    public class UADPlayerViewModel : BaseViewModel
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
            get
            {
                if ((Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings != null)
                {
                    MediaElementVolume = (Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.PlaybackVolume / 100;
                    return (Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.PlaybackVolume;
                }
                else
                {
                    return 1;
                }
            }
            set
            {
                if ((Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.PlaybackVolume != value)
                {
                    (Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.PlaybackVolume = value;
                    MediaElementVolume = (Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.PlaybackVolume / 100;
                    OnPropertyChanged("PlayerVolume");
                }
            }
        }

        private bool _IsDrawingEnabled;
        public bool IsDrawingEnabled
        {
            get
            {
                if ((Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings != null)
                {
                    return (Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.IsDrawingEnabled;
                }
                else
                {
                    return true;
                }
            }
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
                if (isBlockerActive != value)
                {
                    isBlockerActive = value;
                    OnPropertyChanged("IsBlockerActive");
                    OnPropertyChanged("ShowBlockerImage");
                }
            }
        }

        private DrawingAttributes _PrimaryPen;
        public DrawingAttributes PrimaryPen
        {
            get { return _PrimaryPen; }
            set
            {
                (Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.PrimaryBurshThickness = value.Height;
                (Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.PrimaryPenColor = value.Color;
                OnPropertyChanged();
            }
        }

        private DrawingAttributes _SecondaryPen;
        public DrawingAttributes SecondaryPen
        {
            get { return _SecondaryPen; }
            set
            {
                (Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.SecondaryBurshThickness = value.Height;
                (Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.SecondaryPenColor = value.Color;
            }
        }

        private DrawingAttributes _HighlighterPen;
        public DrawingAttributes HighlighterPen
        {
            get { return _HighlighterPen; }
            set
            {
                (Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.HighlighterBurshThickness = value.Height;
                (Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.HighlighterPenColor = value.Color;
            }
        }

        public double PlayerBorderThickness
        {
            get
            {
                if ((Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings != null)
                {
                    if ((Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.IsSneakyWatcherEnabled && (Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.IsSneakyWatcherBorderEnabled)
                    {
                        return 2;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return 0;
                }
            }
        }

        public bool IsSneakyWatcherEnabled
        {
            get
            {
                if ((Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings != null)
                {
                    if ((Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.IsSneakyWatcherEnabled)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
        }

        public SolidColorBrush ScreenBlockerColor => (Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings != null ? new SolidColorBrush((Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.BlockerColor) : new SolidColorBrush(Colors.Black);

        public bool ShowBlockerImage => (Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings != null ? IsBlockerActive && (Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.IsBlockerImageEnabled : false;

        public ImageSource BlockerImageSource
        {
            get
            {
                if ((Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings != null)
                {
                    if (!string.IsNullOrEmpty((Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.BlockerImageLocation))
                    {
                        return new BitmapImage(new Uri((Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.BlockerImageLocation));
                    }
                    else
                    {
                        return new BitmapImage();
                    }
                }
                else
                {
                    return new BitmapImage();
                }
            }
        }

        public Stretch BlockerStretchMode => (Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings != null ? (Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.BlockerStretchMode : Stretch.Fill;



        private int _SidePanelTabIndex = 0;
        public int SidePanelTabIndex
        {
            get
            {
                return _SidePanelTabIndex;
            }
            set
            {
                if (_SidePanelTabIndex != value)
                {
                    _SidePanelTabIndex = value;
                    OnPropertyChanged();
                }
            }
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
