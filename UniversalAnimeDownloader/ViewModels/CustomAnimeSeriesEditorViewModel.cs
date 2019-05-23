using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Linq;
using System.Windows.Input;
using Microsoft.WindowsAPICodePack.Dialogs;
using UADAPI;

namespace UniversalAnimeDownloader.ViewModels
{
    class CustomAnimeSeriesEditorViewModel : BaseViewModel
    {
        public ICommand ChooseAnimeSeriesThumbnailCommand { get; set; }
        public ICommand ChooseEpisodeThumbnailCommand { get; set; }
        public ICommand UpdateCollectionViewCommand { get; set; }
        public ICommand ChooseVideoLocatonCommand { get; set; }
        public ICommand AddEpisodeCommand { get; set; }
        public ICommand RemoveEpisodeCommand { get; set; }
        //public ICommand TestUpdateCommand { get; set; }

        public CustomAnimeSeriesEditorViewModel()
        {
            ChooseAnimeSeriesThumbnailCommand = new RelayCommand<object>(null, p => 
            {
                var dialog = new CommonOpenFileDialog("Select Anime Thumbail");
                dialog.EnsureFileExists = true;
                dialog.Multiselect = false;
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                dialog.Filters.Add(new CommonFileDialogFilter("Image file", "*.png;*.jpg;*.bmp;*.jpeg"));
                dialog.Filters.Add(new CommonFileDialogFilter("All file", "*.*"));
                if( dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    CurrentSeries.Thumbnail = new MediaSourceInfo()
                    {
                        LocalFile = dialog.FileName
                    };
                    OnPropertyChanged("CurrentSeries");
                }
            });

            ChooseEpisodeThumbnailCommand = new RelayCommand<EpisodeInfo>(null, p =>
            {
                var dialog = new CommonOpenFileDialog("Select Anime Thumbail");
                dialog.EnsureFileExists = true;
                dialog.Multiselect = false;
                dialog.Filters.Add(new CommonFileDialogFilter("Image file", "*.png;*.jpg;*.bmp;*.jpeg"));
                dialog.Filters.Add(new CommonFileDialogFilter("All file", "*.*"));
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    p.Thumbnail = new MediaSourceInfo() { LocalFile = dialog.FileName };
                    RefreshEpisodeCollection();
                }
            });

            UpdateCollectionViewCommand = new RelayCommand<object>(null, p =>
            {
                RefreshEpisodeCollection();
            });

            ChooseVideoLocatonCommand = new RelayCommand<EpisodeInfo>(null, p => 
            {
                var dialog = new CommonOpenFileDialog("Select Anime Source");
                dialog.EnsureFileExists = true;
                dialog.Multiselect = false;
                dialog.Filters.Add(new CommonFileDialogFilter("Video file", "*.mp4"));
                dialog.Filters.Add(new CommonFileDialogFilter("All file", "*.*"));
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    var sources = new Dictionary<VideoQuality, MediaSourceInfo>();
                    sources.Add(VideoQuality.Quality144p, new MediaSourceInfo() { LocalFile = dialog.FileName });
                    p.FilmSources = sources;

                    RefreshEpisodeCollection();
                }
            });

            AddEpisodeCommand = new RelayCommand<object>(null, p =>
            {
                CurrentSeries.Episodes.Add(new EpisodeInfo());
                RefreshEpisodeCollection();
            });

            RemoveEpisodeCommand = new RelayCommand<EpisodeInfo>(null, p =>
            {
                CurrentSeries.Episodes.Remove(p);
                RefreshEpisodeCollection();
            });

            //TestUpdateCommand = new RelayCommand<object>(null, p => 
            //{
            //    Console.WriteLine(CurrentSeries);
            //    MessageBox.Show("As");
            //});
        }

        private AnimeSeriesInfo _CurrentSeries;
        public AnimeSeriesInfo CurrentSeries
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
                    OnCurrentSeriesChanged(value);
                    OnPropertyChanged();
                }
            }
        }

        private void OnCurrentSeriesChanged(AnimeSeriesInfo info)
        {
            if (CurrentSeries == null)
                CurrentSeries = new AnimeSeriesInfo();
            if (CurrentSeries.Episodes == null)
            {
                CurrentSeries.Episodes = new List<EpisodeInfo>();
                CurrentSeries.Episodes.Add(new EpisodeInfo());
            }
        }

        private string _CurrentSeriesThumbnail;
        public string CurrentSeriesThumbnail
        {
            get
            {
                return _CurrentSeriesThumbnail;
            }
            set
            {
                if (_CurrentSeriesThumbnail != value)
                {
                    _CurrentSeriesThumbnail = value;
                    OnPropertyChanged();
                }
            }
        }


        private void RefreshEpisodeCollection()
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(CurrentSeries.Episodes);
            view.Refresh();
        }
    }
}