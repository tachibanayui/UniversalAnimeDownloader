using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using UniversalAnimeDownloader.Settings;

namespace UniversalAnimeDownloader.ViewModel
{
    public class SettingsPlaybackViewModel : ViewModelBase
    {
        public ObservableCollection<string> Players { get; set; }

        public bool PlayFullScreen
        {
            get => SettingsManager.Current.PlayMediaFullScreen;
            set => SettingsManager.Current.PlayMediaFullScreen = value;
        }

        public bool IsSneakyWatcherEnabled
        {
            get => SettingsManager.Current.IsSneakyWatcherEnabled;
            set
            {
                SettingsManager.Current.IsSneakyWatcherEnabled = value;
                OnPropertyChanged("IsSneakyWatcherEnabled");
            }
        }

        public bool SneakyWatcherOrangeBorder
        {
            get => SettingsManager.Current.IsSneakyWatcherBorderEnabled;
            set => SettingsManager.Current.IsSneakyWatcherBorderEnabled = value;
        }

        public bool IsRandomizePasswordText
        {
            get => SettingsManager.Current.IsRandomizePasswordBox;
            set => SettingsManager.Current.IsRandomizePasswordBox = value;
        }

        public string MasterPassword
        {
            get => SettingsManager.Current.SneakyWatcherMasterPassword;
            set => SettingsManager.Current.SneakyWatcherMasterPassword = value;
        }

        public bool EnableMasterPassword
        {
            get => SettingsManager.Current.IsEnableMasterPassword;
            set => SettingsManager.Current.IsEnableMasterPassword = value;
        }

        public bool SneakyWatcherChangeAppIcon
        {
            get => SettingsManager.Current.ChangeAppIconWhenSneakyWatcherActive;
            set => SettingsManager.Current.ChangeAppIconWhenSneakyWatcherActive = value;
        }

        public bool SneakyWatcherPause
        {
            get => SettingsManager.Current.IsPauseWhenSneakyWactherActive;
            set => SettingsManager.Current.IsPauseWhenSneakyWactherActive = value;
        }

        public bool IsEnableOnScreenDrawing
        {
            get => SettingsManager.Current.IsDrawingEnabled;
            set => SettingsManager.Current.IsDrawingEnabled = value;
        }

        public bool IsWindowTopMost
        {
            get => SettingsManager.Current.MakeWindowTopMost;
            set => SettingsManager.Current.MakeWindowTopMost = value;
        }

        public bool DisableAltF4
        {
            get => SettingsManager.Current.DisableAltF4;
            set => SettingsManager.Current.DisableAltF4 = value;
        }

        public string BlockerImageLocation
        {
            get => SettingsManager.Current.BlockerImageLocation;
            set => SettingsManager.Current.BlockerImageLocation = value;
        }

        public Stretch BlockerImageStretchMode
        {
            get => SettingsManager.Current.BlockerStretchMode;
            set
            {
                SettingsManager.Current.BlockerStretchMode = value;
                OnPropertyChanged("BlockerImageStretchMode");
            }
        }

        public bool IsBlockerSolidColor
        {
            get => SettingsManager.Current.IsBlockerImageEnabled;
            set => SettingsManager.Current.IsBlockerImageEnabled = value;
        }

        public SettingsPlaybackViewModel()
        {
            Players = new ObservableCollection<string>();
            Players.Add("UAD Player");
            Players.Add("External Player");
        }
    }
}
