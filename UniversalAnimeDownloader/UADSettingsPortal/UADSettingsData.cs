using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using UADAPI;

namespace UniversalAnimeDownloader.UADSettingsPortal
{
    public class UADSettingsData : INotifyPropertyChanged
    {
        public string SaveLocation { get; set; } = AppDomain.CurrentDomain.BaseDirectory + "Settings\\UserSetting.json";
        [JsonIgnore]
        public bool IsViewModelLoaded { get; set; } = false;

        #region GeneralSetting
        private string _AnimeLibraryLocation = AppDomain.CurrentDomain.BaseDirectory + "Anime Library";
        public string AnimeLibraryLocation
        {
            get
            {
                return _AnimeLibraryLocation;
            }
            set
            {
                if (_AnimeLibraryLocation != value)
                {
                    _AnimeLibraryLocation = value;
                    DownloadManager.DownloadDirectory = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _ScreenShotLocation = AppDomain.CurrentDomain.BaseDirectory + "Screenshots\\";
        public string ScreenShotLocation
        {
            get
            {
                return _ScreenShotLocation;
            }
            set
            {
                if (_ScreenShotLocation != value)
                {
                    _ScreenShotLocation = value;
                    OnPropertyChanged();
                }
            }
        }



        #endregion

        #region AppearanceSetting
        private bool _IsDarkTheme = true;
        public bool IsDarkTheme
        {
            get
            {
                return _IsDarkTheme;
            }
            set
            {
                if (_IsDarkTheme != value)
                {
                    _IsDarkTheme = value;
                    new PaletteHelper().SetLightDark(value);
                    OnPropertyChanged();
                }
            }
        }

        private string _PrimaryColorTheme;
        public Swatch PrimaryColorTheme
        {
            get
            {
                return new SwatchesProvider().Swatches.FirstOrDefault(f => f.Name == _PrimaryColorTheme);
            }
            set
            {
                if (value != null)
                {
                    if (_PrimaryColorTheme != value.Name)
                    {
                        _PrimaryColorTheme = value.Name;
                        Application.Current.Dispatcher.Invoke(() => new PaletteHelper().ReplacePrimaryColor(value.Name));
                        OnPropertyChanged();
                    }
                }
            }
        }

        private Swatch _AccentColorTheme;
        public Swatch AccentColorTheme
        {
            get
            {
                return _AccentColorTheme;
            }
            set
            {
                if (_AccentColorTheme != value)
                {
                    _AccentColorTheme = value;
                    Application.Current.Dispatcher.Invoke(() => new PaletteHelper().ReplaceAccentColor(value.Name));
                    OnPropertyChanged();
                }
            }
        }


        #endregion

        #region PerformanceSetting
        private bool _DisableAnimation = false;
        public bool DisableAnimation
        {
            get
            {
                return _DisableAnimation;
            }
            set
            {
                if (_DisableAnimation != value)
                {
                    _DisableAnimation = value;
                    //try { (Application.Current.FindResource("MainWindowViewModel") as MainWindowViewModel).DisableAnimation = value; } catch { }
                    OnPropertyChanged();
                }
            }
        }

        private int _AnimationFrameRate = 60;
        public int AnimationFrameRate
        {
            get
            {
                return _AnimationFrameRate;
            }
            set
            {
                if (_AnimationFrameRate != value)
                {
                    _AnimationFrameRate = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region PlayblackSetting
        private PlayerType preferedPlayer = PlayerType.Embeded;
        public PlayerType PreferedPlayer
        {
            get { return preferedPlayer; }
            set
            {
                if (preferedPlayer != value)
                {
                    preferedPlayer = value;
                    OnPropertyChanged();
                    Save();
                }
            }
        }

        private bool playMediaFullScreen = true;
        public bool PlayMediaFullScreen
        {
            get { return playMediaFullScreen; }
            set
            {
                if (playMediaFullScreen != value)
                {
                    playMediaFullScreen = value;
                    OnPropertyChanged();
                    Save();
                }
            }
        }

        private double playbackVolume = 100;
        public double PlaybackVolume
        {
            get { return playbackVolume; }
            set
            {
                if (playbackVolume != value)
                {
                    playbackVolume = value;
                    OnPropertyChanged();
                    Save();
                }
            }
        }

        private bool isDrawingEnabled = true;
        public bool IsDrawingEnabled
        {
            get { return isDrawingEnabled; }
            set
            {
                if (isDrawingEnabled != value)
                {
                    isDrawingEnabled = value;
                    OnPropertyChanged();
                    Save();
                }
            }
        }

        private bool isSneakyWatherEnabled = true;
        public bool IsSneakyWatcherEnabled
        {
            get { return isSneakyWatherEnabled; }
            set
            {
                if (isSneakyWatherEnabled != value)
                {
                    isSneakyWatherEnabled = value;
                    OnPropertyChanged();
                    Save();
                }
            }
        }

        private bool isSneakyWatcherBorderEnabled = false;
        public bool IsSneakyWatcherBorderEnabled
        {
            get { return isSneakyWatcherBorderEnabled; }

            set
            {
                if (isSneakyWatcherBorderEnabled != value)
                {
                    isSneakyWatcherBorderEnabled = value;
                    OnPropertyChanged();
                    Save();
                }
            }
        }

        #region Pen
        private Color primaryPenColor = Colors.OrangeRed;
        public Color PrimaryPenColor
        {
            get { return primaryPenColor; }
            set
            {
                if (primaryPenColor != value)
                {
                    primaryPenColor = value;
                    OnPropertyChanged();
                    Save();
                }
            }
        }

        private double primaryBurshThickness = 20;
        public double PrimaryBurshThickness
        {
            get { return primaryBurshThickness; }
            set
            {
                if (primaryBurshThickness != value)
                {
                    primaryBurshThickness = value;
                    OnPropertyChanged();
                    Save();
                }
            }
        }


        private Color secondaryPenColor = Colors.White;
        public Color SecondaryPenColor
        {
            get { return secondaryPenColor; }
            set
            {
                if (secondaryPenColor != value)
                {
                    secondaryPenColor = value;
                    OnPropertyChanged();
                    Save();
                }
            }
        }

        private double secondaryBurshThickness = 20;
        public double SecondaryBurshThickness
        {
            get { return secondaryBurshThickness; }
            set
            {
                if (secondaryBurshThickness != value)
                {
                    secondaryBurshThickness = value;
                    OnPropertyChanged();
                    Save();
                }
            }
        }

        private Color highlighterPenColor = Colors.YellowGreen;
        public Color HighlighterPenColor
        {
            get { return highlighterPenColor; }
            set
            {
                if (highlighterPenColor != value)
                {
                    highlighterPenColor = value;
                    OnPropertyChanged();
                    Save();
                }
            }
        }

        private double highlighterBurshThickness = 20;
        public double HighlighterBurshThickness
        {
            get { return highlighterBurshThickness; }
            set
            {
                if (highlighterBurshThickness != value)
                {
                    highlighterBurshThickness = value;
                    OnPropertyChanged();
                    Save();
                }
            }
        }
        #endregion

        #region Sneaky Watcher Settings

        #region Sneaky Watcher Hotkeys
        private char blockerToggleHotKeys = 'B';
        public char BlockerToggleHotKeys
        {
            get { return blockerToggleHotKeys; }
            set
            {
                if (blockerToggleHotKeys != value)
                {
                    blockerToggleHotKeys = value;
                    int enumNumber = value;
                    if (char.IsLetter(value))
                    {
                        enumNumber -= 21;
                    }

                    if (char.IsNumber(value))
                    {
                        enumNumber -= 14;
                    }

                    CustomCommands.ScreenBlockerHotkey.InputGestures[0] = new KeyGesture((Key)enumNumber, ModifierKeys.Control);
                    OnPropertyChanged();
                    Save();
                }
            }
        }

        private char appCrashToggleHotKeys = 'C';
        public char AppCrashToggleHotKeys
        {
            get { return appCrashToggleHotKeys; }
            set
            {
                if (appCrashToggleHotKeys != value)
                {
                    appCrashToggleHotKeys = value;
                    int enumNumber = value;
                    if (char.IsLetter(value))
                    {
                        enumNumber -= 21;
                    }

                    if (char.IsNumber(value))
                    {
                        enumNumber -= 14;
                    }

                    CustomCommands.FakeAppCrashHotkey.InputGestures[0] = new KeyGesture((Key)enumNumber, ModifierKeys.Control);
                    OnPropertyChanged();
                    Save();
                }
            }
        }

        private char bgPlayerToggleHotKeys = 'N';
        public char BgPlayerToggleHotKeys
        {
            get { return bgPlayerToggleHotKeys; }
            set
            {
                if (bgPlayerToggleHotKeys != value)
                {
                    bgPlayerToggleHotKeys = value;
                    int enumNumber = value;
                    if (char.IsLetter(value))
                    {
                        enumNumber -= 21;
                    }

                    if (char.IsNumber(value))
                    {
                        enumNumber -= 14;
                    }

                    CustomCommands.BackgroundPlayerHotkey.InputGestures[0] = new KeyGesture((Key)enumNumber, ModifierKeys.Control);
                    OnPropertyChanged();
                    Save();
                }
            }
        }
        #endregion

        private bool isPauseWhenSneakyWatcherActive = true;
        public bool IsPauseWhenSneakyWactherActive
        {
            get { return isPauseWhenSneakyWatcherActive; }
            set
            {
                if (isPauseWhenSneakyWatcherActive != value)
                {
                    isPauseWhenSneakyWatcherActive = value;
                    OnPropertyChanged();
                    Save();
                }
            }
        }

        private Color blockerColor = Colors.Black;
        public Color BlockerColor
        {
            get { return blockerColor; }
            set
            {
                if (blockerColor != value)
                {
                    blockerColor = value;
                    OnPropertyChanged();
                    Save();
                }
            }
        }

        private bool isBlockerImageEnabled = false;
        public bool IsBlockerImageEnabled
        {
            get { return isBlockerImageEnabled; }
            set
            {
                if (isBlockerImageEnabled != value)
                {
                    isBlockerImageEnabled = value;
                    OnPropertyChanged();
                    Save();
                }
            }
        }

        private string blockerImageLocation = string.Empty;
        public string BlockerImageLocation
        {
            get { return blockerImageLocation; }
            set
            {
                if (blockerImageLocation != value)
                {
                    blockerImageLocation = value;
                    OnPropertyChanged();
                    Save();
                }
            }
        }

        private Stretch blockerStretchMode = Stretch.Fill;
        public Stretch BlockerStretchMode
        {
            get { return blockerStretchMode; }
            set
            {
                if (blockerStretchMode != value)
                {
                    blockerStretchMode = value;
                    OnPropertyChanged();
                    Save();
                }
            }
        }

        private bool makeWindowTopMost = false;
        public bool MakeWindowTopMost
        {
            get { return makeWindowTopMost; }
            set
            {
                if (makeWindowTopMost != value)
                {
                    makeWindowTopMost = value;
                    OnPropertyChanged();
                    Save();
                }
            }
        }

        private bool disableAltF4 = false;
        public bool DisableAltF4
        {
            get { return disableAltF4; }
            set
            {
                if (disableAltF4 != value)
                {
                    disableAltF4 = value;
                    OnPropertyChanged();
                    Save();
                }
            }
        }

        private bool isEnableMasterPassword = true;
        public bool IsEnableMasterPassword
        {
            get { return isEnableMasterPassword; }
            set
            {
                if (isEnableMasterPassword != value)
                {
                    isEnableMasterPassword = value;
                    OnPropertyChanged();
                    Save();
                }
            }
        }

        private string sneakyWatcherMasterPassword = "12345";
        public string SneakyWatcherMasterPassword
        {
            get { return sneakyWatcherMasterPassword; }
            set
            {
                if (sneakyWatcherMasterPassword != value)
                {
                    sneakyWatcherMasterPassword = value;
                    OnPropertyChanged();
                    Save();
                }
            }
        }

        private bool isRandomizePasswordBox = true;
        public bool IsRandomizePasswordBox
        {
            get { return isRandomizePasswordBox; }
            set
            {
                if (isRandomizePasswordBox != value)
                {
                    isRandomizePasswordBox = value;
                    OnPropertyChanged();
                    Save();
                }
            }
        }

        private bool changeAppIconWhenSneakyWatcherActive = true;
        public bool ChangeAppIconWhenSneakyWatcherActive
        {
            get { return changeAppIconWhenSneakyWatcherActive; }
            set
            {
                if (changeAppIconWhenSneakyWatcherActive != value)
                {
                    changeAppIconWhenSneakyWatcherActive = value;
                    OnPropertyChanged();
                    Save();
                }
            }
        }
        #endregion

        #endregion

        #region Performance Settings
        private bool _IsOnlyLoadWhenHostShow = true;
        public bool IsOnlyLoadWhenHostShow
        {
            get
            {
                return _IsOnlyLoadWhenHostShow;
            }
            set
            {
                if (_IsOnlyLoadWhenHostShow != value)
                {
                    _IsOnlyLoadWhenHostShow = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _IsLoadPageInBackground = true;
        public bool IsLoadPageInBackground
        {
            get
            {
                return _IsLoadPageInBackground;
            }
            set
            {
                if (_IsLoadPageInBackground != value)
                {
                    _IsLoadPageInBackground = value;
                    OnPropertyChanged();
                }
            }
        }

        private BitmapScalingMode _BitmapScalingMode = BitmapScalingMode.HighQuality;
        public BitmapScalingMode BitmapScalingMode
        {
            get
            {
                return _BitmapScalingMode;
            }
            set
            {
                if (_BitmapScalingMode != value)
                {
                    _BitmapScalingMode = value;
                    OnPropertyChanged();
                }
            }
        }


        #endregion

        private string _Notification;
        public string Notification
        {
            get
            {
                return _Notification;
            }
            set
            {
                if (_Notification != value)
                {
                    _Notification = value;
                    Save();
                }
            }
        }

        private string _Download;
        public string Download
        {
            get
            {
                return _Download;
            }
            set
            {
                if (_Download != value)
                {
                    _Download = value;
                    Save();
                }
            }
        }

        private string _UserInterest;
        public string UserInterest
        {
            get
            {
                return _UserInterest;
            }
            set
            {
                if (_UserInterest != value)
                {
                    _UserInterest = value;
                    Save();
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;



        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Save()
        {
            string content = JsonConvert.SerializeObject(this);
            File.WriteAllText(SaveLocation, content);
        }
    }

    public enum PlayerType
    {
        Embeded, External
    }
}