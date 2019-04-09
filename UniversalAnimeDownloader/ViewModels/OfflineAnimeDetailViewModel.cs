using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using UADAPI;
using UniversalAnimeDownloader.UcContentPages;

namespace UniversalAnimeDownloader.ViewModels
{
    class OfflineAnimeDetailViewModel : BaseViewModel
    {
        #region Commands
        public ICommand WatchEpisodeCommand { get; set; }
        public ICommand OnlineVersionCommand { get; set; }
        public ICommand PlayAllButtonCommand { get; set; }
        public ICommand CopyDescriptionCommand { get; set; }
        #endregion

        #region BindableProperties
        private IAnimeSeriesManager _CurrentSeries;
        public IAnimeSeriesManager CurrentSeries
        {
            get
            {
                return _CurrentSeries;
            }
            set
            {
                if (_CurrentSeries != value)
                {
                    _CurrentSeries = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        public OfflineAnimeDetailViewModel()
        {
            CopyDescriptionCommand = new RelayCommand<object>(p => true, p => Clipboard.SetText(CurrentSeries.AttachedAnimeSeriesInfo.Description ?? ""));
            
            PlayAllButtonCommand = new RelayCommand<object>(p => true, async p => {
                var a = await MessageDialog.ShowAsync("Test", "ajdadjoasd", MessageDialogButton.OKCancelButton);
                var b = await MessageDialog.ShowAsync("Test", "12e12e", MessageDialogButton.YesNoButton);
                var c = await MessageDialog.ShowAsync("Test", "ajdadjoasd", MessageDialogButton.OKCancelButton);
                MessageBox.Show("Yay");
            });

            WatchEpisodeCommand = new RelayCommand<EpisodeInfo>(p => p.AvailableOffline, p =>
            Process.Start(p.FilmSources.Last(query => query.Value.IsFinishedRequesting).Value.LocalFile));

            OnlineVersionCommand = new RelayCommand<object>(p => true,async p =>
            {
                //var online = await ApiHelpper.CreateQueryAnimeObjectByClassName(CurrentSeries.RelativeQueryInfo.ModTypeString).GetAnimeByID(CurrentSeries.AttachedAnimeSeriesInfo.AnimeID);
                //var onlineManager = ApiHelpper.CreateAnimeSeriesManagerObjectByClassName(online.ModInfo.ModTypeString);
                //onlineManager.AttachedAnimeSeriesInfo = online;
                //await onlineManager.GetPrototypeEpisodes();
                (Application.Current.FindResource("AnimeDetailsViewModel") as AnimeDetailsViewModel).CurrentSeries = CurrentSeries;
                (Application.Current.FindResource("MainWindowViewModel") as MainWindowViewModel).NavigateProcess("AnimeDetails");
            });
        }

        public void OnShow()
        {

        }
    }
}
