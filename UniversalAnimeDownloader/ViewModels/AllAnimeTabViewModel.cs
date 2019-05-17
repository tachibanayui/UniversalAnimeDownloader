using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UADAPI;
using UniversalAnimeDownloader.UADSettingsPortal;

namespace UniversalAnimeDownloader.ViewModels
{
    public class AllAnimeTabViewModel : BaseViewModel, IPageContent
    {
        #region Properties
        /// <summary>
        /// Temp task to retrive the result of async call
        /// </summary>
        public Task TempTask { get; set; }
        public IQueryAnimeSeries Querier { get; set; }
        public ObservableWrapedCollection<AnimeSeriesInfo> AnimeInfos { get; set; }
        public ObservableCollection<GenreItem> Genres { get; set; }
        public ObservableCollection<SeasonItem> Seasons { get; set; }
        public CancellationTokenSource LoadAnimeCancelToken { get; set; } = new CancellationTokenSource();
        public bool IsLoadOngoing { get; set; }
        public bool IsLoadedAnime { get; set; }
        #endregion

        #region RelayCommand
        public ICommand SearchAnimeCommand { get; set; }
        public ICommand ReloadInternetCommand { get; set; }
        public ICommand DetailTooltipOpenedCommand { get; set; }
        public ICommand AnimeListScrollingCommand { get; set; }
        public ICommand ShowAnimeDetailCommand { get; set; }
        public ICommand PageLoaded { get; set; }
        public ICommand AnimeListSizeChangedCommand { get; set; }
        #endregion

        #region BindableProperties
        private int _SelectedQueryModIndex = 1;
        public int SelectedQueryModIndex
        {
            get
            {
                return _SelectedQueryModIndex;
            }
            set
            {
                if (_SelectedQueryModIndex != value)
                {
                    _SelectedQueryModIndex = value;
                    if (ApiHelpper.QueryTypes.Count > value)
                    {
                        Querier = ApiHelpper.CreateQueryAnimeObjectByType(ApiHelpper.QueryTypes[value]);
                        TempTask = LoadGenresAndSeasons();
                    }
                    OnPropertyChanged();
                }
            }
        }

        private int _SelectedGenresIndex = 1;
        public int SelectedGenresIndex
        {
            get
            {
                return _SelectedGenresIndex;
            }
            set
            {
                if (_SelectedGenresIndex != value)
                {
                    _SelectedGenresIndex = value;
                    SearchAnimeCommand.Execute(null);
                    OnPropertyChanged();
                }
            }
        }

        private int _SelectedSeasonIndex = 1;
        public int SelectedSeasonIndex
        {
            get
            {
                return _SelectedSeasonIndex;
            }
            set
            {
                if (_SelectedSeasonIndex != value)
                {
                    _SelectedSeasonIndex = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _SearchAnime;
        public string SearchAnime
        {
            get
            {
                return _SearchAnime;
            }
            set
            {
                if (_SearchAnime != value)
                {
                    _SearchAnime = value;
                    OnPropertyChanged();
                }
            }
        }

        private Visibility _OverlayNoInternetVisibility = Visibility.Collapsed;
        public Visibility OverlayNoInternetVisibility
        {
            get
            {
                return _OverlayNoInternetVisibility;
            }
            set
            {
                if (_OverlayNoInternetVisibility != value)
                {
                    _OverlayNoInternetVisibility = value;
                    OnPropertyChanged();
                }
            }
        }

        private Visibility _OverlayErrorOccuredVisibility = Visibility.Collapsed;
        public Visibility OverlayErrorOccuredVisibility
        {
            get
            {
                return _OverlayErrorOccuredVisibility;
            }
            set
            {
                if (_OverlayErrorOccuredVisibility != value)
                {
                    _OverlayErrorOccuredVisibility = value;
                    OnPropertyChanged();
                }
            }
        }

        private Visibility _OverlayActiityIndicatorVisibility = Visibility.Collapsed;
        public Visibility OverlayActiityIndicatorVisibility
        {
            get
            {
                return _OverlayActiityIndicatorVisibility;
            }
            set
            {
                if (_OverlayActiityIndicatorVisibility != value)
                {
                    _OverlayActiityIndicatorVisibility = value;
                    OnPropertyChanged();
                }
            }
        }

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



        private ItemsPanelTemplate _AnimeCardPanel = Application.Current.FindResource("WrapPanelItemPanel") as ItemsPanelTemplate;
        public ItemsPanelTemplate AnimeCardPanel
        {
            get
            {
                return _AnimeCardPanel;
            }
            set
            {
                if (_AnimeCardPanel != value)
                {
                    _AnimeCardPanel = value;
                    OnPropertyChanged();
                }
            }
        }

        private Exception _CurrentException;
        public Exception CurrentException
        {
            get
            {
                return _CurrentException;
            }
            set
            {
                if (_CurrentException != value)
                {
                    _CurrentException = value;
                    OnPropertyChanged();
                }
            }
        }



        #endregion


        public AllAnimeTabViewModel()
        {
            SearchAnimeCommand = new RelayCommand<object>(null, async (p) => await LoadAnime(0, 50));
            ReloadInternetCommand = new RelayCommand<object>(p => OverlayNoInternetVisibility == Visibility.Visible, async (p) => await LoadAnime(0, 50));
            AnimeListScrollingCommand = new RelayCommand<object>(null, async (p) =>
            {
                if (!AnimeInfos.IsReCalculatingItem)
                {
                    ScrollViewer scr = MiscClass.FindVisualChild<ScrollViewer>(p as ListBox);
                    if( scr.VerticalOffset > scr.ScrollableHeight - 100 && !IsLoadOngoing && scr.ScrollableHeight > 100)
                        await LoadAnime(AnimeInfos.Count, 50, false);
                }
            });
            ShowAnimeDetailCommand = new RelayCommand<AnimeSeriesInfo>(null, async (p) =>
            {
                if (p != null)
                {
                    MiscClass.NavigationHelper.AddNavigationHistory(1);
                    IAnimeSeriesManager manager = ApiHelpper.CreateAnimeSeriesManagerObjectByClassName(p.ModInfo.ModTypeString);
                    manager.AttachedAnimeSeriesInfo = p;
                    await manager.GetPrototypeEpisodes();
                    (Application.Current.FindResource("AnimeDetailsViewModel") as AnimeDetailsViewModel).CurrentSeries = manager;
                }
            });
            MiscClass.UserSearched += async (s, e) =>
            {
                if (MiscClass.NavigationHelper.Current != 0)
                {
                    return;
                }

                SearchAnime = e.Keyword;
                await LoadAnime(0, 50);
            };

            AnimeInfos = new ObservableWrapedCollection<AnimeSeriesInfo>(725, 210);
            Genres = new ObservableCollection<GenreItem>();
            Seasons = new ObservableCollection<SeasonItem>();

            //When the setting is loaded, all viewmodels haven't loaded yet, the setting can't get the property to change, so we need to change here
            ItemsPanelTemplate appliedPanel = null;
            if ((Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.UseVirtalizingWrapPanel)
            {
                appliedPanel = Application.Current.FindResource("VirtualizingWrapPanelItemPanel") as ItemsPanelTemplate;
            }
            else
            {
                appliedPanel = Application.Current.FindResource("WrapPanelItemPanel") as ItemsPanelTemplate;
            }
            AnimeCardPanel = appliedPanel;

            PageLoaded = new RelayCommand<object>(null, async p => { if (!(Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.IsOnlyLoadWhenHostShow) { await InitAnimeList(); } });

            AnimeListSizeChangedCommand = new RelayCommand<ListBox>(null, p => AnimeInfos.ContainerWidth = p.ActualWidth);
        }

        private async Task InitAnimeList()
        {
            SelectedQueryModIndex = 0;
            SelectedGenresIndex = 0;
            try
            {
                if (ApiHelpper.QueryTypes.Count == 0)
                {
                    OverlayNoModVisibility = Visibility.Visible;
                    return;
                }

                await TempTask;
                await LoadAnime(0, 50);
            }
            catch (Exception e)
            {
                ShowErrorOcuredOverlay(e);
                //Add an error in UADAPI.OutputLogHelper class
            }
        }

        public async Task LoadAnime(int offset, int count, bool clearPreviousCard = true)
        {
            IsLoadOngoing = true;
            try
            {
                HideAllOverlay();
                OverlayActiityIndicatorVisibility = Visibility.Visible;
                if (await ApiHelpper.CheckForInternetConnection())
                {
                    if (Querier != null)
                    {
                        LoadAnimeCancelToken?.Cancel();
                        if (clearPreviousCard)
                        {
                            AnimeInfos.Clear();
                        }

                        LoadAnimeCancelToken = new CancellationTokenSource();
                        var currentToken = LoadAnimeCancelToken.Token;
                        string strGenres = string.Empty;
                        string strSeason = string.Empty;
                        if (Genres.Count > SelectedGenresIndex)
                        {
                            strGenres = Genres[SelectedGenresIndex]?.Slug;
                        }

                        if (Seasons.Count > SelectedSeasonIndex)
                        {
                            strSeason = Seasons[SelectedSeasonIndex]?.Slug;
                        }

                        var animes = await Querier.GetAnime(offset, count, SearchAnime, strGenres, strSeason);
                        if (clearPreviousCard)
                        {
                            AnimeInfos.Clear();
                        }
                        try
                        {
                            AnimeInfos.AddRangeAsync(animes);
                        }
                        catch { }
                    }
                    HideAllOverlay();
                }
                else
                {
                    HideAllOverlay();
                    ShowNoInternetOverlay();
                }
            }
            catch (Exception e)
            {
                //Add a error message in UADAPI.OutputLogHelper class
                HideAllOverlay();
                ShowErrorOcuredOverlay(e);
            }
            IsLoadOngoing = false;
        }

        private async Task LoadGenresAndSeasons()
        {
            try
            {
                var genres = await Querier.GetAnimeGenres();
                Genres.Clear();
                foreach (var item in genres)
                {
                    Genres.Add(item);
                }

                var season = await Querier.GetAnimeSeasons();
                Seasons.Clear();
                if (season != null)
                {
                    foreach (var item in season)
                    {
                        Seasons.Add(item);
                    }
                }
            }
            catch { }
        }

        private void ShowNoInternetOverlay()
        {
            if (OverlayErrorOccuredVisibility == Visibility.Visible)
            {
                OverlayErrorOccuredVisibility = Visibility.Collapsed;
            }

            if (OverlayNoInternetVisibility == Visibility.Collapsed)
            {
                OverlayNoInternetVisibility = Visibility.Visible;
            }

            OverlayActiityIndicatorVisibility = Visibility.Collapsed;
        }

        private void ShowErrorOcuredOverlay(Exception e)
        {
            if (OverlayNoInternetVisibility == Visibility.Visible)
            {
                OverlayNoInternetVisibility = Visibility.Collapsed;
            }

            if (OverlayErrorOccuredVisibility == Visibility.Collapsed)
            {
                OverlayErrorOccuredVisibility = Visibility.Visible;
            }
            OverlayActiityIndicatorVisibility = Visibility.Collapsed;

            CurrentException = e;
        }

        private void HideAllOverlay()
        {
            if (OverlayNoInternetVisibility == Visibility.Visible)
            {
                OverlayNoInternetVisibility = Visibility.Collapsed;
            }

            if (OverlayErrorOccuredVisibility == Visibility.Visible)
            {
                OverlayErrorOccuredVisibility = Visibility.Collapsed;
            }

            OverlayActiityIndicatorVisibility = Visibility.Collapsed;
        }

        public async void OnShow()
        {
            if ((Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.IsOnlyLoadWhenHostShow && !IsLoadedAnime)
            {
                IsLoadedAnime = true;
                await InitAnimeList();
            }
        }

        public void OnHide()
        {
        }
    }
}
