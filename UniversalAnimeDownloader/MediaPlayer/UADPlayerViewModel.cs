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
                if (UADSettingsManager.Instance.CurrentSettings != null)
                {
                    MediaElementVolume = UADSettingsManager.Instance.CurrentSettings.PlaybackVolume / 100;
                    return UADSettingsManager.Instance.CurrentSettings.PlaybackVolume;
                }
                else
                {
                    return 1;
                }
            }
            set
            {
                if (UADSettingsManager.Instance.CurrentSettings.PlaybackVolume != value)
                {
                    UADSettingsManager.Instance.CurrentSettings.PlaybackVolume = value;
                    MediaElementVolume = UADSettingsManager.Instance.CurrentSettings.PlaybackVolume / 100;
                    OnPropertyChanged("PlayerVolume");
                }
            }
        }

        private bool _IsDrawingEnabled;
        public bool IsDrawingEnabled
        {
            get
            {
                if (UADSettingsManager.Instance.CurrentSettings != null)
                {
                    return UADSettingsManager.Instance.CurrentSettings.IsDrawingEnabled;
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
                UADSettingsManager.Instance.CurrentSettings.PrimaryBurshThickness = value.Height;
                UADSettingsManager.Instance.CurrentSettings.PrimaryPenColor = value.Color;
                OnPropertyChanged();
            }
        }

        private DrawingAttributes _SecondaryPen;
        public DrawingAttributes SecondaryPen
        {
            get { return _SecondaryPen; }
            set
            {
                UADSettingsManager.Instance.CurrentSettings.SecondaryBurshThickness = value.Height;
                UADSettingsManager.Instance.CurrentSettings.SecondaryPenColor = value.Color;
            }
        }

        private DrawingAttributes _HighlighterPen;
        public DrawingAttributes HighlighterPen
        {
            get { return _HighlighterPen; }
            set
            {
                UADSettingsManager.Instance.CurrentSettings.HighlighterBurshThickness = value.Height;
                UADSettingsManager.Instance.CurrentSettings.HighlighterPenColor = value.Color;
            }
        }

        public double PlayerBorderThickness
        {
            get
            {
                if (UADSettingsManager.Instance.CurrentSettings != null)
                {
                    if (UADSettingsManager.Instance.CurrentSettings.IsSneakyWatcherEnabled && UADSettingsManager.Instance.CurrentSettings.IsSneakyWatcherBorderEnabled)
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
                if (UADSettingsManager.Instance.CurrentSettings != null)
                {
                    if (UADSettingsManager.Instance.CurrentSettings.IsSneakyWatcherEnabled)
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

        public SolidColorBrush ScreenBlockerColor => UADSettingsManager.Instance.CurrentSettings != null ? new SolidColorBrush(UADSettingsManager.Instance.CurrentSettings.BlockerColor) : new SolidColorBrush(Colors.Black);

        public bool ShowBlockerImage => UADSettingsManager.Instance.CurrentSettings != null ? IsBlockerActive && UADSettingsManager.Instance.CurrentSettings.IsBlockerImageEnabled : false;

        public ImageSource BlockerImageSource
        {
            get
            {
                if (UADSettingsManager.Instance.CurrentSettings != null)
                {
                    if (!string.IsNullOrEmpty(UADSettingsManager.Instance.CurrentSettings.BlockerImageLocation))
                    {
                        return new BitmapImage(new Uri(UADSettingsManager.Instance.CurrentSettings.BlockerImageLocation));
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

        public Stretch BlockerStretchMode => UADSettingsManager.Instance.CurrentSettings != null ? UADSettingsManager.Instance.CurrentSettings.BlockerStretchMode : Stretch.Fill;



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
