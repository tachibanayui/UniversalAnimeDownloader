using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using UADAPI;
using UniversalAnimeDownloader.UADSettingsPortal;
using ErrorEventArgs = Newtonsoft.Json.Serialization.ErrorEventArgs;

namespace UniversalAnimeDownloader.ViewModels
{
    class MyAnimeLibraryViewModel : BaseViewModel, IPageContent
    {
        #region Commmands
        public ICommand ReloadAnimeCommand { get; set; }
        public ICommand ShowAnimeDetailCommand { get; set; }
        public ICommand PageLoadedCommand { get; set; }
        public ICommand AnimeListSizeChangedCommand { get; set; }
        public ICommand DeleteAnimeCommand { get; set; }
        public ICommand OpenCustomSeriesEditorCommand { get; set; }
        public ICommand CustomSeriesEditorCancelCommand { get; set; }
        public ICommand CustomSeriesEditorSaveCommand { get; set; }
        #endregion

        #region Fields / Properties
        public string LastSearchedKeyWord { get; set; }
        #endregion


        public ObservableWrapedCollection<AnimeSeriesInfo> AnimeLibrary { get; set; } = new ObservableWrapedCollection<AnimeSeriesInfo>(725, 210);
        public List<AnimeSeriesInfo> NoDelayAnimeLib { get; } = new List<AnimeSeriesInfo>();

        private Visibility _OverlayNoModVisibility = Visibility.Collapsed;
        public Visibility OverlayNoModVisibility
        {
            get
            {
                return _OverlayNoModVisibility;
            }
            set
            {
                if (_OverlayNoModVisibility != value)
                {
                    _OverlayNoModVisibility = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _IsCustomSeriesEditorOpen;
        public bool IsCustomSeriesEditorOpen
        {
            get
            {
                return _IsCustomSeriesEditorOpen;
            }
            set
            {
                if (_IsCustomSeriesEditorOpen != value)
                {
                    _IsCustomSeriesEditorOpen = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _HostWidth;
        public double HostWidth
        {
            get
            {
                return _HostWidth;
            }
            set
            {
                if (_HostWidth != value)
                {
                    _HostWidth = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _HostHeight;
        public double HostHeight
        {
            get
            {
                return _HostHeight;
            }
            set
            {
                if (_HostHeight != value)
                {
                    _HostHeight = value;
                    OnPropertyChanged();
                }
            }
        }

        private UADSettingsData _SettingsData;
        public UADSettingsData SettingsData
        {
            get
            {
                return _SettingsData;
            }
            set
            {
                if (_SettingsData != value)
                {
                    _SettingsData = value;
                    OnPropertyChanged();
                }
            }
        }





        public MyAnimeLibraryViewModel()
        {
            MiscClass.UserSearched += (s, e) =>
            {
                LastSearchedKeyWord = e.Keyword;
                ICollectionView view = CollectionViewSource.GetDefaultView(AnimeLibrary);
                view.Filter += SearchAnimeLibrary;
                view.Refresh();
            };

            SettingsData = (Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings;
            ReloadAnimeLibrary(true);
            ReloadAnimeLibrary(false);

            ReloadAnimeCommand = new RelayCommand<object>(null, async p => await ReloadAnimeLibrary(false));
            ShowAnimeDetailCommand = new RelayCommand<AnimeSeriesInfo>(null, async p =>
            {
                MiscClass.NavigationHelper.AddNavigationHistory(5);
                IAnimeSeriesManager manager = p.ModInfo != null
                    ? ApiHelpper.CreateAnimeSeriesManagerObjectByClassName(p.ModInfo.ModTypeString)
                    : ApiHelpper.CreateAnimeSeriesManagerObjectByClassName("Fake");
                if (manager == null)
                {
                    OverlayNoModVisibility = Visibility.Visible;
                    return;
                }
                manager.AttachedAnimeSeriesInfo = p;
                var viewModel = Application.Current.FindResource("OfflineAnimeDetailViewModel") as OfflineAnimeDetailViewModel;
                viewModel.CurrentSeries = manager;

                viewModel.IsOnlineVersionBtnEnable = await ApiHelpper.CheckForInternetConnection() && !p.IsCustomSeries;
            });
            DeleteAnimeCommand = new RelayCommand<AnimeSeriesInfo>(null, async p =>
            {
                Directory.Delete(p.AnimeSeriesSavedDirectory, true);
                await ReloadAnimeLibrary(false);
            });
            PageLoadedCommand = new RelayCommand<object>(null, p =>
            {
                if (ApiHelpper.QueryTypes.Count == 0 || ApiHelpper.ManagerTypes.Count == 0)
                {
                    OverlayNoModVisibility = Visibility.Visible;
                }
            });
            AnimeListSizeChangedCommand = new RelayCommand<ListBox>(null, p => AnimeLibrary.ContainerWidth = p.ActualWidth);
            OpenCustomSeriesEditorCommand = new RelayCommand<object>(null, p =>
            {
                (Application.Current.FindResource("CustomAnimeSeriesEditorViewModel") as CustomAnimeSeriesEditorViewModel).CurrentSeries = new AnimeSeriesInfo();
                IsCustomSeriesEditorOpen = true;
            });
            CustomSeriesEditorCancelCommand = new RelayCommand<object>(null, p => IsCustomSeriesEditorOpen = false);
            CustomSeriesEditorSaveCommand = new RelayCommand<AnimeSeriesInfo>(null, async p =>
            {
                var result = await MiscClass.CreateCustomAnimeSeries(p);

                await ReloadAnimeLibrary(false);
                IsCustomSeriesEditorOpen = false;

                if( result.Item2)
                {
                    var dialogResult = await MessageDialog.ShowAsync("Custom anime series creation failed!", "We unable to create this custom series. Please verify all your file is valid and try again.\r\nClick yes to send us a bug report, no to close this dialog without sending bug report", MessageDialogButton.YesNoButton);
                    if(dialogResult == MessageDialogResult.Yes)
                    {
                        ReportErrorHelper.ReportError(result.Item3);
                    }
                }
                else
                {
                    if (result.Item1)
                        await MessageDialog.ShowAsync("Some file are exist!", "When we create and consolidate all file into one folder, we can't move the file. But don't worry, you can edit this playlist anytime", MessageDialogButton.OKOnlyButton);
                }
            });
        }

        private bool SearchAnimeLibrary(object obj)
        {
            if (string.IsNullOrEmpty(LastSearchedKeyWord))
            {
                return true;
            }
            else
            {
                return (obj as AnimeSeriesInfo).Name.ToLower().Contains(LastSearchedKeyWord.ToLower());
            }
        }

        public async Task ReloadAnimeLibrary(bool isRealtimeList)
        {
            AnimeLibrary.Clear();
            string lib = SettingsData.AnimeLibraryLocation;
            foreach (var item in Directory.EnumerateDirectories(lib))
            {
                if (File.Exists(Path.Combine(item , "Manager.json")))
                {
                    string content = File.ReadAllText(item + "\\Manager.json");
                    var jsonSetting = new JsonSerializerSettings()
                    {
                        Error = new EventHandler<ErrorEventArgs>((s, e) => e.ErrorContext.Handled = true),
                        TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full,
                    };
                    var info = JsonConvert.DeserializeObject<AnimeSeriesInfo>(content, jsonSetting);
                    if (isRealtimeList)
                    {
                        NoDelayAnimeLib.Add(info);
                    }
                    else
                    {
                        await AnimeLibrary.AddAsyncTask(info);
                    }
                }
            }
            if (!isRealtimeList)
            {
                NoDelayAnimeLib.Clear();
                NoDelayAnimeLib.AddRange(AnimeLibrary);
            }
        }

        public void OnShow()
        {
        }

        public void OnHide()
        {

        }
    }
}
