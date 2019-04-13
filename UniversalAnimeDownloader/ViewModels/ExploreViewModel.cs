using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using UADAPI;
using UniversalAnimeDownloader.UADSettingsPortal;

namespace UniversalAnimeDownloader.ViewModels
{
    class ExploreViewModel : BaseViewModel
    {
        public ICommand NavigateGetMoreCommand { get; set; }
        public ICommand LoadedCommand { get; set; }
        public ICommand ShowAnimeDetailCommand { get; set; }

        public IQueryAnimeSeries Querier { get; set; }

        public DelayedObservableCollection<AnimeSeriesInfo> FeaturedAnimeList { get; set; } = new DelayedObservableCollection<AnimeSeriesInfo>();
        public DelayedObservableCollection<AnimeSeriesInfo> SuggestedAnimeList { get; set; } = new DelayedObservableCollection<AnimeSeriesInfo>();
        public ObservableCollection<AnimeSeriesInfo> CarouselAnimeList { get; set; } = new DelayedObservableCollection<AnimeSeriesInfo>();
        public CancellationTokenSource LoadFeaturedAnimeCancelToken { get; set; } = new CancellationTokenSource();
        public CancellationTokenSource LoadSuggestedAnimeCancelToken { get; set; } = new CancellationTokenSource();

        public ExploreViewModel()
        {
            LoadedCommand = new RelayCommand<object>(p => true, Init);
            NavigateGetMoreCommand = new RelayCommand<string>(p => true, p =>
            {
                if (p == "Featured")
                    (Application.Current.FindResource("MainWindowViewModel") as MainWindowViewModel).NavigateProcess("FeaturedAnime");
                else if (p == "Suggestion")
                    (Application.Current.FindResource("MainWindowViewModel") as MainWindowViewModel).NavigateProcess("AnimeSuggestion");
            });

            ShowAnimeDetailCommand = new RelayCommand<AnimeSeriesInfo>(p => true,async p => 
            {
                IAnimeSeriesManager manager = ApiHelpper.CreateAnimeSeriesManagerObjectByClassName(p.ModInfo.ModTypeString);
                manager.AttachedAnimeSeriesInfo = p;
                await manager.GetPrototypeEpisodes();
                (Application.Current.FindResource("AnimeDetailsViewModel") as AnimeDetailsViewModel).CurrentSeries = manager;
                (Application.Current.FindResource("MainWindowViewModel") as MainWindowViewModel).NavigateProcess("AnimeDetails");
            });
        }

        private void Init(object obj)
        {
            
            Querier = ApiHelpper.CreateQueryAnimeObjectByType(ApiHelpper.QueryTypes[0]);
            InitAnimeList();
        }

        public void InitAnimeList()
        {
            //await LoadFeaturedAnime(0, 10);
            var featureInfo = (Application.Current.FindResource("FeaturedAnimeViewModel") as FeaturedAnimeViewModel).SuggestedAnimeInfos;
            featureInfo.CollectionReallyChanged += async (s, e) =>
            {
                LoadFeaturedAnimeCancelToken?.Cancel();
                LoadFeaturedAnimeCancelToken = new CancellationTokenSource();
                FeaturedAnimeList.RemoveAll();
                CarouselAnimeList.Clear();
                try
                {
                    for (int i = 0; i < 6; i++)
                    {
                        CarouselAnimeList.Add(featureInfo[i]);
                    }
                    await FeaturedAnimeList.AddRange(featureInfo.Where((f, i) => i > 6 &&  i < 18).ToList(), LoadFeaturedAnimeCancelToken.Token);
                }
                catch { }
            };
            var suggestInfo = (Application.Current.FindResource("AnimeSuggestionViewModel") as AnimeSuggestionViewModel).SuggestedAnimeInfos;
            suggestInfo.CollectionReallyChanged += async (s, e) =>
            {
                LoadSuggestedAnimeCancelToken?.Cancel();
                LoadSuggestedAnimeCancelToken = new CancellationTokenSource();
                FeaturedAnimeList.RemoveAll();
                try
                {
                    await SuggestedAnimeList.AddRange(suggestInfo.Where((f, i) => i < 10).ToList(), LoadSuggestedAnimeCancelToken.Token);
                }
                catch { }
            };
        }

        private async Task LoadFeaturedAnime(int offset, int count, bool clearPreviousCard = true)
        {
            if (Querier.SupportGetPopularSeries)
            {
                try
                {
                    if (ApiHelpper.CheckForInternetConnection())
                    {
                        if (Querier != null)
                        {
                            var animes = await Querier.GetFeaturedAnime(offset, count);

                            try
                            {
                                await SuggestedAnimeList.AddRange(animes, new System.Threading.CancellationToken());
                            }
                            catch { }
                        }
                    }
                }
                catch { }
            }
        }

        private async Task LoadSuggestedAnime(int offset, int count, bool clearPreviousCard = true)
        {
            try
            {
                if (ApiHelpper.CheckForInternetConnection())
                {
                    if (Querier != null)
                    {
                        var animes = await UserInterestMananger.GetSuggestion(Querier.GetType().FullName, offset, count);
                        UADSettingsManager.Instance.CurrentSettings.UserInterest = UserInterestMananger.Serialize();

                        try
                        {
                            await SuggestedAnimeList.AddRange(animes,new System.Threading.CancellationToken());
                        }
                        catch { }
                    }
                }
            }
            catch
            {
            }
        }
    }
}
