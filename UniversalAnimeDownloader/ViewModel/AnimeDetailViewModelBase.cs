using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace UniversalAnimeDownloader.ViewModel
{
    public class AnimeDetailViewModelBase : ViewModelBase
    {
        public Dispatcher currentDispatcher;


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
                    OnPropertyChanged("IsHeaderLoading");
                }
            }

        }

        private string animeDescription;
        public string AnimeDescription
        {
            get { return animeDescription; }
            set
            {
                if (animeDescription != value)
                {
                    animeDescription = value;
                    OnPropertyChanged("AnimeDescription");
                    OnPropertyChanged("IsDescriptionLoading");
                }
            }
        }

        private string animeGenres;
        public string AnimeGemres
        {
            get { return animeGenres; }
            set
            {
                if (animeGenres != value)
                {
                    animeGenres = value;
                    OnPropertyChanged("AnimeGemres");
                }
            }
        }
    }
}
