using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace UniversalAnimeDownloader.ViewModel
{
    public class OnlineAnimeDetailViewModel : AnimeDetailViewModelBase
    {
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

        private ImageSource animeThumbnail;
        public ImageSource AnimeThumbnail
        {
            get => animeThumbnail;
            set
            {
                if(animeThumbnail != value)
                {
                    animeThumbnail = value;
                    OnPropertyChanged("AnimeThumbnail");
                }
            }
        }

        public ObservableCollection<OnlineEpisodesListViewModel> AnimeEpisodes { get; set; }

        public OnlineAnimeDetailViewModel(Dispatcher currentDispatcher)
        {
            this.currentDispatcher = currentDispatcher;
            AnimeEpisodes = new ObservableCollection<OnlineEpisodesListViewModel>();
            IsDownloadButtonEnabled = true;

            AnimeEpisodes.CollectionChanged += (s, e) => OnPropertyChanged("IsEpisodeLoading");
        }
    }
}
