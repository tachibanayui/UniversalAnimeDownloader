using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UADAPI;

namespace UniversalAnimeDownloader.ViewModels
{
    class OfflineAnimeDetailViewModel : BaseViewModel
    {
        #region Commands
        public ICommand WatchEpisodeCommand { get; set; }
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
            WatchEpisodeCommand = new RelayCommand<EpisodeInfo>(p => p.AvailableOffline, p => 
            Process.Start(p.FilmSources.Last(query => query.Value.IsFinishedRequesting).Value.LocalFile));
        }

        public void OnShow()
        {

        }
    }
}
