using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;

namespace UniversalAnimeDownloader.ViewModel
{
    class OfflineAnimeDetailViewModel : AnimeDetailViewModelBase
    {
        public ObservableCollection<OfflineEpisodesListViewModel> AnimeEpisodes { get; set; }

        private ImageSource animeThumbnail;
        public ImageSource AnimeThumbnail
        {
            get => animeThumbnail;
            set
            {
                if (animeThumbnail != value)
                {
                    animeThumbnail = value;
                    OnPropertyChanged("AnimeThumbnail");
                }
            }
        }

        public OfflineAnimeDetailViewModel(Dispatcher dis)
        {
            AnimeEpisodes = new ObservableCollection<OfflineEpisodesListViewModel>();
            currentDispatcher = dis;
        }
    }
}
