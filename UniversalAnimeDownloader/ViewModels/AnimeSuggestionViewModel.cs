using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using UADAPI;
using UniversalAnimeDownloader.UADSettingsPortal;

namespace UniversalAnimeDownloader.ViewModels
{
    class AnimeSuggestionViewModel : BaseViewModel, IPageContent
    {
        #region Commands
        public ICommand RefreshCommand { get; set; }
        public ICommand ShowAnimeDetailCommand { get; set; }
        public ICommand AnimeListScrollingCommand { get; set; }
        public ICommand PageLoaded { get; set; }
        // public ICommand OverlayNoInternetVisibility { get; set; }
        #endregion

        #region Properties
        public DelayedObservableCollection<AnimeSeriesInfo> SuggestedAnimeInfos { get; set; } = new DelayedObservableCollection<AnimeSeriesInfo>();
        public CancellationTokenSource LoadAnimeCancelToken { get; set; } = new CancellationTokenSource();
        public Random Rand { get; set; } = new Random();
        public bool IsLoadOngoing { get; private set; }
        public IQueryAnimeSeries Querier { get; set; }
        public Exception LastError { get; set; }
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
                        Querier = ApiHelpper.CreateQueryAnimeObjectByType(ApiHelpper.QueryTypes[value]);
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
        #endregion


        public AnimeSuggestionViewModel()
        {
            SelectedQueryModIndex = 0;
            RefreshCommand = new RelayCommand<object>(null, async (p) => await LoadSuggestedAnime(Rand.Next(1, 1000000), 50));
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
                await LoadSuggestedAnime(Rand.Next(1,1000000), 50, false);
            });
            ShowAnimeDetailCommand = new RelayCommand<AnimeSeriesInfo>(null, async (p) =>
            {
                if (p != null)
                {
                    IAnimeSeriesManager manager = ApiHelpper.CreateAnimeSeriesManagerObjectByClassName(p.ModInfo.ModTypeString);
                    manager.AttachedAnimeSeriesInfo = p;
                    await manager.GetPrototypeEpisodes();
                    (Application.Current.FindResource("AnimeDetailsViewModel") as AnimeDetailsViewModel).CurrentSeries = manager;
                }
            });
            PageLoaded = new RelayCommand<object>(null, async p => 
            {
                string userInterestString = (Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.UserInterest;
                await UserInterestMananger.Deserialize(userInterestString); // 16 ms
                if (!(Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.IsOnlyLoadWhenHostShow)
                    await InitAnimeList();
            });
        }

        private async Task InitAnimeList()
        {
            IsLoadOngoing = true;
            if(UserInterestMananger.Data.LastSuggestion != null)
            {
                HideAllOverlay();
                OverlayActiityIndicatorVisibility = Visibility.Visible;
                await SuggestedAnimeInfos.AddRange(UserInterestMananger.Data.LastSuggestion, LoadAnimeCancelToken.Token);
                HideAllOverlay();
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
            IsLoadOngoing = false;
        }

        public async Task LoadSuggestedAnime(int offset, int count, bool clearPreviousCard = true)
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
                        var animes = await Task.Run(async () => 
                        {
                            LoadAnimeCancelToken?.Cancel();
                            LoadAnimeCancelToken = new CancellationTokenSource();

                            var tmp = await UserInterestMananger.GetSuggestion(Querier.GetType().FullName, offset, count);
                            (Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.UserInterest = UserInterestMananger.Serialize();
                            return tmp;
                        });

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
