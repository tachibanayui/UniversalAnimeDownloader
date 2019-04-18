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
            get { return UADSettingsManager.Instance.CurrentSettings.PlaybackVolume; }
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
        public bool IsDrawingEnabled
        {
            get { return UADSettingsManager.Instance.CurrentSettings.IsDrawingEnabled; }
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

        public DrawingAttributes PrimaryPen
        {
            get { return new DrawingAttributes() { Color = UADSettingsManager.Instance.CurrentSettings.PrimaryPenColor, Width = UADSettingsManager.Instance.CurrentSettings.PrimaryBurshThickness, Height = UADSettingsManager.Instance.CurrentSettings.PrimaryBurshThickness }; }
            set
            {
                UADSettingsManager.Instance.CurrentSettings.PrimaryBurshThickness = value.Height;
                UADSettingsManager.Instance.CurrentSettings.PrimaryPenColor = value.Color;
            }
        }

        public DrawingAttributes SecondaryPen
        {
            get { return new DrawingAttributes() { Color = UADSettingsManager.Instance.CurrentSettings.SecondaryPenColor, Width = UADSettingsManager.Instance.CurrentSettings.SecondaryBurshThickness, Height = UADSettingsManager.Instance.CurrentSettings.SecondaryBurshThickness }; }
            set
            {
                UADSettingsManager.Instance.CurrentSettings.SecondaryBurshThickness = value.Height;
                UADSettingsManager.Instance.CurrentSettings.SecondaryPenColor = value.Color;
            }
        }

        public DrawingAttributes HighlighterPen
        {
            get { return new DrawingAttributes() { IsHighlighter = true, Color = UADSettingsManager.Instance.CurrentSettings.HighlighterPenColor, Width = UADSettingsManager.Instance.CurrentSettings.HighlighterBurshThickness, Height = UADSettingsManager.Instance.CurrentSettings.HighlighterBurshThickness }; }
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
                if (UADSettingsManager.Instance.CurrentSettings.IsSneakyWatcherEnabled && UADSettingsManager.Instance.CurrentSettings.IsSneakyWatcherBorderEnabled)
                {
                    return 2;
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
                if (UADSettingsManager.Instance.CurrentSettings.IsSneakyWatcherEnabled)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public SolidColorBrush ScreenBlockerColor => new SolidColorBrush(UADSettingsManager.Instance.CurrentSettings.BlockerColor);

        public bool ShowBlockerImage => IsBlockerActive && UADSettingsManager.Instance.CurrentSettings.IsBlockerImageEnabled;

        public ImageSource BlockerImageSource
        {
            get
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
        }

        public Stretch BlockerStretchMode => UADSettingsManager.Instance.CurrentSettings.BlockerStretchMode;


        public UADPlayerViewModel()
        {
            MediaElementVolume = UADSettingsManager.Instance.CurrentSettings.PlaybackVolume / 100;
            InkCanvasVisibility = Visibility.Collapsed;
        }

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
