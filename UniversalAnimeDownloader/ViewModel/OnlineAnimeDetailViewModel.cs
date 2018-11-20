using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
                if(animeDescription != value)
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

        private string quality;
        public string Quality
        {
            get { return quality; }
            set
            {
                if(quality != value)
                {
                    quality = value;
                    OnPropertyChanged("Quality");
                }
            }
        }

        public bool IsHeaderLoading
        {
            get { return AnimeTitle == null || AnimeDescription == null; }
        }   
        public bool IsDescriptionLoading
        {
            get { return AnimeDescription == null; }
        }

        private bool isDownloadButtonEnabled;
        public bool IsDownloadButtonEnabled
        {
            get { return isDownloadButtonEnabled; }
            set
            {
                if (isDownloadButtonEnabled != value)
                {
                    isDownloadButtonEnabled = value;
                    OnPropertyChanged("IsDownloadButtonEnabled");
                }
            }
        }

        //Not implement yet
        public bool IsPictureLoading
        {
            get { return true; }
        }
        //

        private bool isEpisodeLoading;
        public bool IsEpisodeLoading
        {
            get { return isEpisodeLoading; }
            private set
            {
                if (isEpisodeLoading != value)
                {
                    isEpisodeLoading = value;
                    OnPropertyChanged("IsEpisodeLoading");
                }
            }
        }

        public ObservableCollection<OnlineEpisodesListViewModel> AnimeEpisodes { get; set; }
        private Dispatcher currentDispatcher;

        public OnlineAnimeDetailViewModel(Dispatcher currentDispatcher)
        {
            this.currentDispatcher = currentDispatcher;
            AnimeEpisodes = new ObservableCollection<OnlineEpisodesListViewModel>();
            IsDownloadButtonEnabled = true;

            AnimeEpisodes.CollectionChanged += (s, e) => OnPropertyChanged("IsEpisodeLoading");
        }
    }
}
