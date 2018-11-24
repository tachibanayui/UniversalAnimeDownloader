using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace UniversalAnimeDownloader.ViewModel
{
    class OfflineAnimeDetailViewModel : AnimeDetailViewModelBase
    {
        public ObservableCollection<OfflineEpisodesListViewModel> AnimeEpisodes { get; set; }

        public OfflineAnimeDetailViewModel(Dispatcher dis)
        {
            AnimeEpisodes = new ObservableCollection<OfflineEpisodesListViewModel>();
            currentDispatcher = dis;
        }
    }
}
