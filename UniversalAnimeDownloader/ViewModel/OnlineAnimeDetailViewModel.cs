using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace UniversalAnimeDownloader.ViewModel
{
    public class OnlineAnimeDetailViewModel : ViewModelBase
    {
        private string animeTitle;
        public string AnimeTitle
        {
            get { return animeTitle; }
            set
            {
                if (value != animeTitle)
                {
                    animeTitle = value;
                    OnPropertyChanged("AnimeTitle");
                }
            }

        }

        private string animeDescription;
        public string AnimeDescription
        {
            get { return animeDescription; }
            set
            {
                if(animeDescription != value)
                {
                    animeDescription = value;
                    OnPropertyChanged("AnimeDescription");
                }
            }
        }

        public ObservableCollection<OnlineAnimeDetailViewModel> AnimeEpisodes { get; set; }
        private Dispatcher currentDispatcher;

        public OnlineAnimeDetailViewModel(Dispatcher currentDispatcher)
        {
            this.currentDispatcher = currentDispatcher;
            AnimeEpisodes = new ObservableCollection<OnlineAnimeDetailViewModel>();
            
            
        }
    }
}
