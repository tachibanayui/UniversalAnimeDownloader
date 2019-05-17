using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UADAPI;
using UniversalAnimeDownloader.UADSettingsPortal;

namespace UniversalAnimeDownloader.ViewModels
{
    class FeaturedAnimeViewModel : BaseViewModel, IPageContent
    {
        #region Commands
        public ICommand RefreshCommand { get; set; }
        public ICommand ShowAnimeDetailCommand { get; set; }
        public ICommand AnimeListScrollingCommand { get; set; }
        public ICommand AnimeListSizeChangedCommand { get; set; }
        public ICommand PageLoaded { get; set; }
        // public ICommand OverlayNoInternetVisibility { get; set; }
        #endregion

        #region Properties
        public ObservableWrapedCollection<AnimeSeriesInfo> FeaturedAnimeInfos { get; set; } = new ObservableWrapedCollection<AnimeSeriesInfo>(725,210);
        public CancellationTokenSource LoadAnimeCancelToken { get; set; } = new CancellationTokenSource();
        public Random Rand { get; set; } = new Random();
        public bool IsLoadOngoing { get; private set; }
        public IQueryAnimeSeries Querier { get; set; }
        public bool IsLoadedAnime { get; set; }
        #endregion

        #region Bindable Properties
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
                    }

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

        private Visibility _OverlayNotSupported = Visibility.Collapsed;
        public Visibility OverlayNotSupported
        {
            get
            {
                return _OverlayNotSupported;
            }
            set
            {
                if (_OverlayNotSupported != value)
                {
                    _OverlayNotSupported = value;
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


        public FeaturedAnimeViewModel()
        {
            SelectedQueryModIndex = 0;
            RefreshCommand = new RelayCommand<object>(null, async (p) => await LoadFeaturedAnime(0, 50));
            AnimeListScrollingCommand = new RelayCommand<object>(null, async (p) =>
            {
                if (!FeaturedAnimeInfos.IsReCalculatingItem)
                {
                    ScrollViewer scr = MiscClass.FindVisualChild<ScrollViewer>(p as ListBox);
                    if (scr.VerticalOffset > scr.ScrollableHeight - 100 && !IsLoadOngoing && scr.ScrollableHeight > 100)
                        await LoadFeaturedAnime(FeaturedAnimeInfos.Count, 50, false);
                }
            });

            ShowAnimeDetailCommand = new RelayCommand<AnimeSeriesInfo>(null, async (p) =>
            {
                if (p != null)
                {
                    IAnimeSeriesManager manager = ApiHelpper.CreateAnimeSeriesManagerObjectByClassName(p.ModInfo.ModTypeString);
                    manager.AttachedAnimeSeriesInfo = p;
                    await manager.GetPrototypeEpisodes();
                    (Application.Current.FindResource("AnimeDetailsViewModel") as AnimeDetailsViewModel).CurrentSeries = manager;
                    (Application.Current.FindResource("MainWindowViewModel") as MainWindowViewModel).NavigateProcess("AnimeDetails");
                }
            });

            PageLoaded = new RelayCommand<object>(null, async p => { if (!(Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.IsOnlyLoadWhenHostShow) { await InitAnimeList(); } });

            AnimeListSizeChangedCommand = new RelayCommand<ListBox>(null, p => FeaturedAnimeInfos.ContainerWidth = p.ActualWidth);

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
        }

        private async Task InitAnimeList()
        {
            IsLoadOngoing = true;
            try
            {
                await LoadFeaturedAnime(0, 50);
            }
            catch (Exception e)
            {
                ShowErrorOcuredOverlay(e);
                //Add an error in UADAPI.OutputLogHelper class
            }
            IsLoadOngoing = false;
        }

        public async Task LoadFeaturedAnime(int offset, int count, bool clearPreviousCard = true)
        {
            if (Querier == null)
            {
                OverlayNoModVisibility = Visibility.Visible;
                return;
            }
            if (Querier.SupportGetPopularSeries)
            {
                HideNotSupportedOverlay();
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
                            LoadAnimeCancelToken = new CancellationTokenSource();

                            var animes = await Querier.GetFeaturedAnime(offset, count);

                            if (clearPreviousCard)
                            {
                                FeaturedAnimeInfos.Clear();
                            }
                            try
                            {
                                await FeaturedAnimeInfos.AddRangeAsyncTask(animes);
                            }
                            catch { }
                        }
                        HideAllOverlay();
                        OnLoadFeaturedAnimeCompleted();
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
            else
            {
                ShowNotSupportedOverlay();
            }
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

        private void ShowNotSupportedOverlay()
        {
            OverlayActiityIndicatorVisibility = Visibility.Collapsed;
            OverlayErrorOccuredVisibility = Visibility.Collapsed;
            OverlayNoInternetVisibility = Visibility.Collapsed;
            OverlayNotSupported = Visibility.Visible;
        }

        private void HideNotSupportedOverlay() => OverlayNotSupported = Visibility.Collapsed;

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

        public event EventHandler LoadFeaturedAnimeCompleted;
        protected virtual void OnLoadFeaturedAnimeCompleted() => LoadFeaturedAnimeCompleted?.Invoke(this, EventArgs.Empty);
    }
}
