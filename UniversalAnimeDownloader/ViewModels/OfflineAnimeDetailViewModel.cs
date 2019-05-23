using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using UADAPI;

namespace UniversalAnimeDownloader.ViewModels
{
    class OfflineAnimeDetailViewModel : BaseViewModel
    {
        #region Commands
        public ICommand WatchEpisodeCommand { get; set; }
        public ICommand OnlineVersionCommand { get; set; }
        public ICommand PlayAllButtonCommand { get; set; }
        public ICommand CopyDescriptionCommand { get; set; }
        public ICommand DeleteEpisodeCommand { get; set; }
        public ICommand EditPlaylistCommand { get; set; }
        public ICommand CustomSeriesEditorCancelCommand { get; set; }
        public ICommand CustomSeriesEditorSaveCommand { get; set; }

        #endregion

        #region BindableProperties
        private double _HostWidth;
        public double HostWidth
        {
            get
            {
                return _HostWidth;
            }
            set
            {
                if (_HostWidth != value)
                {
                    _HostWidth = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _HostHeight;
        public double HostHeight
        {
            get
            {
                return _HostHeight;
            }
            set
            {
                if (_HostHeight != value)
                {
                    _HostHeight = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _IsCustomSeriesEditorOpen;
        public bool IsCustomSeriesEditorOpen
        {
            get
            {
                return _IsCustomSeriesEditorOpen;
            }
            set
            {
                if (_IsCustomSeriesEditorOpen != value)
                {
                    _IsCustomSeriesEditorOpen = value;
                    OnPropertyChanged();
                }
            }
        }



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
                    SourceControl = new AnimeSourceControl(value);
                    OnPropertyChanged();
                }
            }
        }

        private bool _IsOnlineVersionBtnEnable = true;
        public bool IsOnlineVersionBtnEnable
        {
            get
            {
                return _IsOnlineVersionBtnEnable;
            }
            set
            {
                if (_IsOnlineVersionBtnEnable != value)
                {
                    _IsOnlineVersionBtnEnable = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        private AnimeSourceControl SourceControl = null;


        public OfflineAnimeDetailViewModel()
        {
            MiscClass.UserSearched += (s, e) =>
            {
                if (MiscClass.NavigationHelper.Current != 5)
                {
                    return;
                }

                ICollectionView view = CollectionViewSource.GetDefaultView(_CurrentSeries.AttachedAnimeSeriesInfo.Episodes);
                view.Filter = (p) =>
                {
                    var info = p as EpisodeInfo;
                    return info.Name.ToLower().Contains(e.Keyword.ToLower());
                };
                view.Refresh();
            };

            CopyDescriptionCommand = new RelayCommand<object>(null, p => Clipboard.SetText(CurrentSeries.AttachedAnimeSeriesInfo.Description ?? ""));

            PlayAllButtonCommand = new RelayCommand<object>(null, p =>
            {
                UADMediaPlayerHelper.Play(CurrentSeries.AttachedAnimeSeriesInfo);
            });

            WatchEpisodeCommand = new RelayCommand<EpisodeInfo>(p => p.AvailableOffline, p =>
            {
                UADMediaPlayerHelper.Play(CurrentSeries.AttachedAnimeSeriesInfo, CurrentSeries.AttachedAnimeSeriesInfo.Episodes.FindIndex(pp => pp == p));
            });

            OnlineVersionCommand = new RelayCommand<object>(null, async p =>
             {
                 //var online = await ApiHelpper.CreateQueryAnimeObjectByClassName(CurrentSeries.RelativeQueryInfo.ModTypeString).GetAnimeByID(CurrentSeries.AttachedAnimeSeriesInfo.AnimeID);
                 //var onlineManager = ApiHelpper.CreateAnimeSeriesManagerObjectByClassName(online.ModInfo.ModTypeString);
                 //onlineManager.AttachedAnimeSeriesInfo = online;
                 //await onlineManager.GetPrototypeEpisodes();
                 (Application.Current.FindResource("AnimeDetailsViewModel") as AnimeDetailsViewModel).CurrentSeries = CurrentSeries;
                 (Application.Current.FindResource("MainWindowViewModel") as MainWindowViewModel).NavigateProcess("AnimeDetails");
             });

            DeleteEpisodeCommand = new RelayCommand<EpisodeInfo>(null, async p =>
            {
                await SourceControl.DeleteEpisodes(p);
                (Application.Current.FindResource("MyAnimeLibraryViewModel") as MyAnimeLibraryViewModel).ReloadAnimeCommand.Execute(false);
                ICollectionView view = CollectionViewSource.GetDefaultView(CurrentSeries.AttachedAnimeSeriesInfo.Episodes);
                view.Refresh();
                await MessageDialog.ShowAsync("Episode deleted successfully", $"This episode: {p.Name} as successfully removed from your library!", MessageDialogButton.OKOnlyButton);
            });

            EditPlaylistCommand = new RelayCommand<object>(null, p =>
            {
                (Application.Current.FindResource("CustomAnimeSeriesEditorViewModel") as CustomAnimeSeriesEditorViewModel).CurrentSeries = CurrentSeries.AttachedAnimeSeriesInfo;
                IsCustomSeriesEditorOpen = true;
            });

            CustomSeriesEditorCancelCommand = new RelayCommand<object>(null, p => IsCustomSeriesEditorOpen = false);

            CustomSeriesEditorSaveCommand = new RelayCommand<AnimeSeriesInfo>(null, async p =>
            {
                var result = await MiscClass.CreateCustomAnimeSeries(p);

                await (Application.Current.FindResource("MyAnimeLibraryViewModel") as MyAnimeLibraryViewModel).ReloadAnimeLibrary(false);
                var manager = ApiHelpper.CreateAnimeSeriesManagerObjectByClassName("Fake");
                manager.AttachedAnimeSeriesInfo = p;
                CurrentSeries = manager;

                IsCustomSeriesEditorOpen = false;

                if (result.Item2)
                {
                    var dialogResult = await MessageDialog.ShowAsync("Custom anime series creation failed!", "We unable to create this custom series. Please verify all your file is valid and try again.\r\nClick yes to send us a bug report, no to close this dialog without sending bug report", MessageDialogButton.YesNoButton);
                    if (dialogResult == MessageDialogResult.Yes)
                    {
                        ReportErrorHelper.ReportError(result.Item3);
                    }
                }
                else
                {
                    if (result.Item1)
                    {
                        await MessageDialog.ShowAsync("Some file are exist!", "When we create and consolidate all file into one folder, we can't move the file. But don't worry, you can edit this playlist anytime", MessageDialogButton.OKOnlyButton);
                    }
                }
            });
        }

        public void OnShow()
        {

        }
    }
}
