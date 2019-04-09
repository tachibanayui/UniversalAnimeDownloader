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
    class FeaturedAnimeViewModel : BaseViewModel
    {
        #region Commands
        public ICommand RefreshCommand { get; set; }
        public ICommand ShowAnimeDetailCommand { get; set; }
        public ICommand AnimeListScrollingCommand { get; set; }
        // public ICommand OverlayNoInternetVisibility { get; set; }
        #endregion

        #region Properties
        public DelayedObservableCollection<AnimeSeriesInfo> SuggestedAnimeInfos { get; set; } = new DelayedObservableCollection<AnimeSeriesInfo>();
        public CancellationTokenSource LoadAnimeCancelToken { get; set; } = new CancellationTokenSource();
        public Random Rand { get; set; } = new Random();
        public bool IsLoadOngoing { get; private set; }
        public IQueryAnimeSeries Querier { get; set; }
        public Exception LastError { get; set; }
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

        private Visibility _OverlayNotSupported;
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


        #endregion


        public FeaturedAnimeViewModel()
        {

            SelectedQueryModIndex = 0;
            RefreshCommand = new RelayCommand<object>(p => true, async (p) => await LoadSuggestedAnime(0, 50));
            AnimeListScrollingCommand = new RelayCommand<object>(p =>
            {
                if (p != null)
                {
                    ScrollViewer scr = MiscClass.FindVisualChild<ScrollViewer>(p as ListBox);
                    return scr.VerticalOffset > scr.ScrollableHeight - 100 && !IsLoadOngoing && scr.ScrollableHeight != 0;
                }
                else
                {
                    return false;
                }
            }, async (p) =>
            {
                await LoadSuggestedAnime(SuggestedAnimeInfos.Count, 50, false);
            });

            ShowAnimeDetailCommand = new RelayCommand<AnimeSeriesInfo>(p => true, async (p) =>
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

            InitAnimeList();
        }

        private async void InitAnimeList()
        {
            if (UserInterestMananger.LastSuggestion != null)
            {
                await SuggestedAnimeInfos.AddRange(UserInterestMananger.LastSuggestion, LoadAnimeCancelToken.Token);
            }
            else
            {
                try
                {
                    await LoadSuggestedAnime(0, 50);
                }
                catch (Exception e)
                {
                    ShowErrorOcuredOverlay(e);
                    //Add an error in UADAPI.OutputLogHelper class
                }
            }
        }

        private async Task LoadSuggestedAnime(int offset, int count, bool clearPreviousCard = true)
        {
            IsLoadOngoing = true;
            try
            {
                if (ApiHelpper.CheckForInternetConnection())
                {
                    HideAllOverlay();
                    OverlayActiityIndicatorVisibility = Visibility.Visible;
                    if (Querier != null)
                    {
                        LoadAnimeCancelToken.Cancel();
                        LoadAnimeCancelToken = new CancellationTokenSource();

                        var animes = await UserInterestMananger.GetSuggestion(Querier.GetType().FullName, offset, count);
                        UADSettingsManager.Instance.CurrentSettings.UserInterest = UserInterestMananger.Serialize();

                        if (clearPreviousCard)
                        {
                            SuggestedAnimeInfos.RemoveAll();
                        }
                        try
                        {
                            await SuggestedAnimeInfos.AddRange(animes, LoadAnimeCancelToken.Token);
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

            LastError = e;
        }

        private void ShowNotSupportedOverlay()
        {
            OverlayActiityIndicatorVisibility = Visibility.Collapsed;
            OverlayErrorOccuredVisibility = Visibility.Collapsed;
            OverlayNoInternetVisibility = Visibility.Collapsed;
            OverlayNotSupported = Visibility.Visible;
        }

        private void HideNotSupportedOverlay() => OverlayNotSupported = Visibility.Visible;
    }
}
