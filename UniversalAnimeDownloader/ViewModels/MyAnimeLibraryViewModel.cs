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
        #endregion

        #region Fields / Properties
        public string LastSearchedKeyWord { get; set; }
        #endregion


        public ObservableWrapedCollection<AnimeSeriesInfo> AnimeLibrary { get; set; } = new ObservableWrapedCollection<AnimeSeriesInfo>(725,210);
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



        public MyAnimeLibraryViewModel()
        {
            MiscClass.UserSearched += (s, e) =>
            {
                LastSearchedKeyWord = e.Keyword;
                ICollectionView view = CollectionViewSource.GetDefaultView(AnimeLibrary);
                view.Filter += SearchAnimeLibrary;
                view.Refresh();
            };

            ReloadAnimeLibrary(true);
            ReloadAnimeLibrary(false);

            ReloadAnimeCommand = new RelayCommand<object>(null, async p => await ReloadAnimeLibrary(false));
            ShowAnimeDetailCommand = new RelayCommand<AnimeSeriesInfo>(null, async p =>
            {
                MiscClass.NavigationHelper.AddNavigationHistory(5);
                IAnimeSeriesManager manager = ApiHelpper.CreateAnimeSeriesManagerObjectByClassName(p.ModInfo.ModTypeString);
                if (manager == null)
                {
                    OverlayNoModVisibility = Visibility.Visible;
                    return;
                }
                manager.AttachedAnimeSeriesInfo = p;
                var viewModel = Application.Current.FindResource("OfflineAnimeDetailViewModel") as OfflineAnimeDetailViewModel;
                viewModel.CurrentSeries = manager;
                viewModel.IsOnlineVersionBtnEnable = await ApiHelpper.CheckForInternetConnection();
            });
            PageLoadedCommand = new RelayCommand<object>(null, p =>
            {
                if (ApiHelpper.QueryTypes.Count == 0 || ApiHelpper.ManagerTypes.Count == 0)
                    OverlayNoModVisibility = Visibility.Visible;
            });
            AnimeListSizeChangedCommand = new RelayCommand<ListBox>(null, p => AnimeLibrary.ContainerWidth = p.ActualWidth);
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
            string lib = AppDomain.CurrentDomain.BaseDirectory + "Anime Library\\";
            foreach (var item in Directory.EnumerateDirectories(lib))
            {
                if (File.Exists(item + "\\Manager.json"))
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
