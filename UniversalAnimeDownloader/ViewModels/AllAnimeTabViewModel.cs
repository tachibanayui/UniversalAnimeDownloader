using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using UADAPI;

namespace UniversalAnimeDownloader.ViewModels
{
    public class AllAnimeTabViewModel : BaseViewModel
    {
        #region Properties
        /// <summary>
        /// Temp task to retrive the result of async call
        /// </summary>
        public Task TempTask { get; set; }
        public IQueryAnimeSeries Querier { get; set; }
        public DelayedObservableCollection<AnimeSeriesInfo> AnimeInfos { get; set; }
        public ObservableCollection<GenreItem> Genres { get; set; }
        public ObservableCollection<SeasonItem> Seasons { get; set; }
        public CancellationTokenSource LoadAnimeCancelToken { get; set; } = new CancellationTokenSource();
        public Exception LastError { get; set; }
        #endregion

        #region RelayCommand
        public ICommand SearchAnimeCommand { get; set; }
        public ICommand ReloadInternetCommand { get; set; }
        public ICommand ShowErrorCommand { get; set; }
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


        #endregion


        public AllAnimeTabViewModel()
        {
            SearchAnimeCommand = new RelayCommand<object>(p => true, async (p) => await LoadAnime(0, 50));

            ReloadInternetCommand = new RelayCommand<object>(p => OverlayNoInternetVisibility == Visibility.Visible, async (p) => await LoadAnime(0, 50));

            ApiHelpper.LoadAssembly();
            AnimeInfos = new DelayedObservableCollection<AnimeSeriesInfo>();
            AnimeInfos.DelayInterval = TimeSpan.FromSeconds(0.1);
            Genres = new ObservableCollection<GenreItem>();
            Seasons = new ObservableCollection<SeasonItem>();

            InitAnimeList();
        }

        private async void InitAnimeList()
        {
            SelectedQueryModIndex = 0;
            SelectedGenresIndex = 0;
            try
            {
                //await TempTask;

                //await LoadAnime(0, 50);
                for (int i = 0; i < 50; i++)
                {
                    await AnimeInfos.AddAndWait(new AnimeSeriesInfo() { Name = "Test: " + i, Genres = new List<GenreItem>() { new GenreItem() { Name = "Cancer" } } });
                }
            }
            catch (Exception e)
            {
                ShowErrorOcuredOverlay(e);
                //Add an error in UADAPI.OutputLogHelper class
            }
        }

        private async Task LoadAnime(int offset, int count, bool clearPreviousCard = true)
        {
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
                            AnimeInfos.RemoveAll();
                        }
                        try
                        {
                            await AnimeInfos.AddRange(animes, LoadAnimeCancelToken.Token);
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
        }
        

        private async Task LoadGenresAndSeasons()
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
        }
    }
}
