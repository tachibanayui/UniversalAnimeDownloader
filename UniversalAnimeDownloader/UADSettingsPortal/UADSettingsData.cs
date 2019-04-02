using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using UADAPI;
using UniversalAnimeDownloader.ViewModels;

namespace UniversalAnimeDownloader.UADSettingsPortal
{
    class UADSettingsData : INotifyPropertyChanged
    {
        public string SaveLocation { get; set; } = AppDomain.CurrentDomain.BaseDirectory + "Settings\\UserSetting.json";

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
                        new PaletteHelper().ReplacePrimaryColor(value.Name);
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
                    //new PaletteHelper().ReplaceAccentColor(value);
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
                    try { (Application.Current.FindResource("MainWindowViewModel") as MainWindowViewModel).DisableAnimation = value; } catch { }
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



        private bool _UseVirtalizingWrapPanel;
        public bool UseVirtalizingWrapPanel
        {
            get
            {
                return _UseVirtalizingWrapPanel;
            }
            set
            {
                if (_UseVirtalizingWrapPanel != value)
                {
                    _UseVirtalizingWrapPanel = value;
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
}