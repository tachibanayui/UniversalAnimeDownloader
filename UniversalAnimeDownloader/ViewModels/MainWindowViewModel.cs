using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using UADAPI;
using UniversalAnimeDownloader.MediaPlayer;
using UniversalAnimeDownloader.UADSettingsPortal;

namespace UniversalAnimeDownloader.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {

        #region Commands
        public ICommand CloseWindowCommand { get; set; }
        public ICommand ChangeWindowStateCommand { get; set; }
        public ICommand MinimizeWindowCommand { get; set; }
        public ICommand DragMoveWindowCommand { get; set; }
        public ICommand ToggleNavSideBarCommand { get; set; }
        public ICommand DeleteSearchBoxCommand { get; set; }
        public ICommand CheckForEnterKeyCommand { get; set; }
        public ICommand SearchButtonClickCommand { get; set; }
        public ICommand GoBackNavigationCommand { get; set; }
        public ICommand GoForwardNavigationCommand { get; set; }
        public ICommand ResetNotifyBadgeCommand { get; set; }
        public ICommand BlackOverlayMouseDownCommand { get; set; }
        public ICommand OpenUADMediaPlayerCommand { get; set; }
        public ICommand ChangeWindowStateRequestCommand { get; set; }
        public ICommand UADMediaPlayerClosedCommand { get; set; }
        public ICommand WindowStateChangedCommand { get; set; }
        public ICommand PageLoaded { get; set; }
        public ICommand MouseEnterCommand { get; set; }
        public ICommand MouseLeaveCommand { get; set; }
        public ICommand PauseMediaPlayerCommand { get; set; }
        public ICommand LookSliderCommand { get; set; }
        public ICommand ChangePositionCommand { get; set; }
        public ICommand WindowSizeChangedCommand { get; set; }
        public ICommand SelectPlayIndexCommand { get; set; }
        public ICommand PreviousMediaPlayerPopupCommand { get; set; }
        public ICommand NextMediaPlayerPopupCommand { get; set; }
        public ICommand OpenNowPlayingPopupCommand { get; set; }
        public ICommand ShowPlaylistButtonCommand { get; set; }
        public ICommand FilterPlaylistPopup { get; set; }
        public ICommand OpenUADInstaller { get; set; }
        public ICommand CloseUniversalAnimeDownloader { get; set; }

        public ICommand NavigateCommand { get; set; }
        #endregion

        #region BindableProperties
        private string _LoadingStatus = "Initizaling...";
        public string LoadingStatus
        {
            get
            {
                return _LoadingStatus;
            }
            set
            {
                if (_LoadingStatus != value)
                {
                    _LoadingStatus = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _UpdateDescription;
        public string UpdateDescription
        {
            get
            {
                return _UpdateDescription;
            }
            set
            {
                if (_UpdateDescription != value)
                {
                    _UpdateDescription = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _IsNewUpdatesDialogOpen;
        public bool IsNewUpdatesDialogOpen
        {
            get
            {
                return _IsNewUpdatesDialogOpen;
            }
            set
            {
                if (_IsNewUpdatesDialogOpen != value)
                {
                    _IsNewUpdatesDialogOpen = value;
                    OnPropertyChanged();
                }
            }
        }



        private GridLength _SideBarWidth = new GridLength(65);
        public GridLength SideBarWidth
        {
            get
            {
                return _SideBarWidth;
            }
            set
            {
                if (_SideBarWidth != value)
                {
                    _SideBarWidth = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _IsExpandSidePanel;
        public bool IsExpandSidePanel
        {
            get
            {
                return _IsExpandSidePanel;
            }
            set
            {
                if (_IsExpandSidePanel != value)
                {
                    _IsExpandSidePanel = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _TransisionerIndex = 0;
        public int TransisionerIndex
        {
            get
            {
                return _TransisionerIndex;
            }
            set
            {
                if (_TransisionerIndex != value)
                {
                    _TransisionerIndex = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _NotifycationBadgeCount = 0;
        public int NotifycationBadgeCount
        {
            get
            {
                return _NotifycationBadgeCount;
            }
            set
            {
                if (_NotifycationBadgeCount != value)
                {
                    _NotifycationBadgeCount = value;
                    OnPropertyChanged();
                    OnPropertyChanged("BadgeBackgroundBrush");
                    OnPropertyChanged("BadgeContentVisibility");
                }
            }
        }

        private int _PreTransitionerIndex;
        /// <summary>
        /// This property will change before the actual slide animation. Use for load the Page before transitioning
        /// </summary>
        public int PreTransitionerIndex
        {
            get
            {
                return _PreTransitionerIndex;
            }
            set
            {
                if (_PreTransitionerIndex != value)
                {
                    _PreTransitionerIndex = value;
                    OnPropertyChanged();
                }
            }
        }



        public Brush BadgeBackgroundBrush => NotifycationBadgeCount == 0 ? new SolidColorBrush(Colors.Transparent) : Application.Current.FindResource("PrimaryHueDarkBrush") as Brush;
        public Visibility BadgeContentVisibility { get => NotifycationBadgeCount == 0 ? Visibility.Collapsed : Visibility.Visible; }


        private MessageDialogViewModel _MessageDialogViewModel = new MessageDialogViewModel();
        public MessageDialogViewModel MessageDialogViewModel
        {
            get
            {
                return _MessageDialogViewModel;
            }
            set
            {
                if (_MessageDialogViewModel != value)
                {
                    _MessageDialogViewModel = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _DisableAnimation;
        public bool DisableAnimation
        {
            get
            {
                return _DisableAnimation;
            }
            set
            {
                if (_DisableAnimation != value)
                {
                    _DisableAnimation = value;
                    OnPropertyChanged();
                }
            }
        }

        private Visibility _UADMediaPlayerVisibility = Visibility.Collapsed;
        public Visibility UADMediaPlayerVisibility
        {
            get
            {
                return _UADMediaPlayerVisibility;
            }
            set
            {
                if (_UADMediaPlayerVisibility != value)
                {
                    _UADMediaPlayerVisibility = value;
                    OnPropertyChanged();
                }
            }
        }

        private EpisodeInfo _NowPlaying;
        public EpisodeInfo NowPlaying
        {
            get
            {
                return _NowPlaying;
            }
            set
            {
                if (_NowPlaying != value)
                {
                    _NowPlaying = value;
                    OnPropertyChanged();
                }
            }
        }

        private AnimeSeriesInfo _NowPlayingPlaylist;
        public AnimeSeriesInfo NowPlayingPlaylist
        {
            get
            {
                return _NowPlayingPlaylist;
            }
            set
            {
                if (_NowPlayingPlaylist != value)
                {
                    _NowPlayingPlaylist = value;
                    CreateClonePlaylist(value);
                    OnPropertyChanged();
                }
            }
        }

        private void CreateClonePlaylist(AnimeSeriesInfo info)
        {
            FilteredNowPlayingPlaylist.Clear();
            if (info == null)
            {
                return;
            }

            foreach (var item in info.Episodes)
            {
                FilteredNowPlayingPlaylist.Add(item);
            }
        }

        public ObservableCollection<EpisodeInfo> FilteredNowPlayingPlaylist { get; set; } = new ObservableCollection<EpisodeInfo>();

        private int _UADMediaPlayerPlayIndex;
        public int UADMediaPlayerPlayIndex
        {
            get
            {
                return _UADMediaPlayerPlayIndex;
            }
            set
            {
                if (_UADMediaPlayerPlayIndex != value)
                {
                    _UADMediaPlayerPlayIndex = value;
                    if (value != -1 && NowPlayingPlaylist != null)
                    {
                        NowPlaying = NowPlayingPlaylist.Episodes[value];
                    }
                    OnPropertyChanged();
                }
            }
        }

        private bool _UADMediaPlayerPlaying;
        public bool UADMediaPlayerPlaying
        {
            get
            {
                return _UADMediaPlayerPlaying;
            }
            set
            {
                if (_UADMediaPlayerPlaying != value)
                {
                    _UADMediaPlayerPlaying = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _IsMediaPlayerPause;
        public bool IsMediaPlayerPause
        {
            get
            {
                return _IsMediaPlayerPause;
            }
            set
            {
                if (_IsMediaPlayerPause != value)
                {
                    _IsMediaPlayerPause = value;
                    PlayPauseButtonIconKind = value ? PackIconKind.PlayCircleOutline : PackIconKind.PauseCircleOutline;
                    OnPropertyChanged();
                }
            }
        }

        private PackIconKind _PlayPauseButtonIconKind = PackIconKind.PauseCircleOutline;
        public PackIconKind PlayPauseButtonIconKind
        {
            get
            {
                return _PlayPauseButtonIconKind;
            }
            set
            {
                if (_PlayPauseButtonIconKind != value)
                {
                    _PlayPauseButtonIconKind = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _IsNowPlayingPopupOpen;
        public bool IsNowPlayingPopupOpen
        {
            get
            {
                return _IsNowPlayingPopupOpen;
            }
            set
            {
                if (_IsNowPlayingPopupOpen != value)
                {
                    _IsNowPlayingPopupOpen = value;
                    OnPropertyChanged();
                }
            }
        }

        private void AnimateNowPlayingPopupOpen(Popup popup, bool isOpen)
        {
            Storyboard stb = new Storyboard();
            var globalDuration = TimeSpan.FromSeconds(0.4);

            double opacityFrom = 0;
            double opacityTo = 1;
            double bounceInFrom = 0.7;
            double bounceInTo = 0.9;

            DoubleAnimation fadeInAnim = new DoubleAnimation()
            {
                From = opacityFrom,
                To = opacityTo,
                Duration = globalDuration,
                EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut },
                FillBehavior = FillBehavior.HoldEnd
            };
            Storyboard.SetTargetProperty(fadeInAnim, new PropertyPath("Opacity"));
            stb.Children.Add(fadeInAnim);

            DoubleAnimation bounceInScaleXAnim = new DoubleAnimation()
            {
                From = bounceInFrom,
                To = bounceInTo,
                Duration = globalDuration,
                EasingFunction = new ElasticEase() { EasingMode = EasingMode.EaseOut, Springiness = 6, Oscillations = 2 },
                FillBehavior = FillBehavior.HoldEnd
            };
            Storyboard.SetTargetProperty(bounceInScaleXAnim, new PropertyPath("RenderTransform.Children[0].ScaleX"));
            stb.Children.Add(bounceInScaleXAnim);

            DoubleAnimation bounceInScaleYAnim = new DoubleAnimation()
            {
                From = bounceInFrom,
                To = bounceInTo,
                Duration = globalDuration,
                EasingFunction = new ElasticEase() { EasingMode = EasingMode.EaseOut, Springiness = 6, Oscillations = 2 },
                FillBehavior = FillBehavior.HoldEnd
            };
            Storyboard.SetTargetProperty(bounceInScaleYAnim, new PropertyPath("RenderTransform.Children[0].ScaleY"));
            stb.Children.Add(bounceInScaleYAnim);
            (popup.Child as Grid).BeginStoryboard(stb);
        }

        private Storyboard GetNowPlayingPopupCloseStoryBoard()
        {
            Storyboard stb = new Storyboard();
            var globalDuration = TimeSpan.FromSeconds(0.4);

            double opacityFrom = 1;
            double opacityTo = 0;
            double bounceInFrom = 0.9;
            double bounceInTo = 0.7;

            DoubleAnimation fadeInAnim = new DoubleAnimation()
            {
                From = opacityFrom,
                To = opacityTo,
                Duration = globalDuration,
                EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut },
                FillBehavior = FillBehavior.Stop
            };
            Storyboard.SetTargetProperty(fadeInAnim, new PropertyPath("Opacity"));
            stb.Children.Add(fadeInAnim);

            DoubleAnimation bounceInScaleXAnim = new DoubleAnimation()
            {
                From = bounceInFrom,
                To = bounceInTo,
                Duration = globalDuration,
                EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut },
                FillBehavior = FillBehavior.Stop
            };
            Storyboard.SetTargetProperty(bounceInScaleXAnim, new PropertyPath("RenderTransform.Children[0].ScaleX"));
            stb.Children.Add(bounceInScaleXAnim);

            DoubleAnimation bounceInScaleYAnim = new DoubleAnimation()
            {
                From = bounceInFrom,
                To = bounceInTo,
                Duration = globalDuration,
                EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut },
                FillBehavior = FillBehavior.Stop
            };
            Storyboard.SetTargetProperty(bounceInScaleYAnim, new PropertyPath("RenderTransform.Children[0].ScaleY"));
            stb.Children.Add(bounceInScaleYAnim);
            return stb;
        }

        private double _NowPlayingPopupScale = 0.4;
        public double NowPlayingPopupScale
        {
            get
            {
                return _NowPlayingPopupScale;
            }
            set
            {
                if (_NowPlayingPopupScale != value)
                {
                    _NowPlayingPopupScale = value;
                    OnPropertyChanged();
                }
            }
        }

        private Brush _PlaylistOpenButtonIconBrush = Application.Current.FindResource("MaterialDesignBody") as Brush;
        public Brush PlaylistOpenButtonIconBrush
        {
            get
            {
                return _PlaylistOpenButtonIconBrush;
            }
            set
            {
                if (_PlaylistOpenButtonIconBrush != value)
                {
                    _PlaylistOpenButtonIconBrush = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Fields and Properties
        public bool IsPlayButtonEnable { get; set; }
        public bool MediaPlayerBtnMouseOver { get; set; }
        private bool _IsPlaylistToggled;
        public bool IsPlaylistToggled
        {
            get => _IsPlaylistToggled;
            set
            {
                if (_IsPlaylistToggled != value)
                {
                    _IsPlaylistToggled = value;
                    if (value)
                    {
                        PlaylistOpenButtonIconBrush = Application.Current.FindResource("PrimaryHueMidBrush") as Brush;
                    }
                    else
                    {
                        PlaylistOpenButtonIconBrush = Application.Current.FindResource("MaterialDesignBody") as Brush;
                    }
                }
            }
        }
        private CancellationTokenSource MediaPlayerMouseOverBtnToken = null;
        private bool IsMediaPlayerPopupMouseOver;
        #endregion

        public MainWindowViewModel()
        {
            string notificationString = (Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.Notification;
            NotificationManager.Deserialize(notificationString);
            string downloadString = (Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.Download;
            DownloadManager.Deserialize(downloadString);
            DownloadManager.Instances.CollectionChanged += (s, e) => (Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.Download = DownloadManager.Serialize();
            NotificationManager.ItemRemoved += (s, e) => (Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.Notification = NotificationManager.Serialize();
            NotificationManager.ItemAdded += (s, e) =>
            {
                NotifycationBadgeCount++;
                try { (Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.Notification = NotificationManager.Serialize(); } catch { }
            };

            //NotificationManager.Add(new NotificationItem() { Title = "Test notification", Detail = "This is a test notification!", ShowActionButton = true, ActionButtonContent = "Click here!", ButtonAction = new Action(() => { MessageBox.Show("Test"); }) });

            CloseWindowCommand = new RelayCommand<object>(null, p => Application.Current.Shutdown());
            ChangeWindowStateCommand = new RelayCommand<Button>(null, p =>
            {
                if (p != null)
                {
                    Window win = Window.GetWindow(p);
                    if (win.WindowState == WindowState.Maximized)
                    {
                        win.WindowState = WindowState.Normal;
                        (p.Content as PackIcon).Kind = PackIconKind.WindowMaximize;
                    }
                    else
                    {
                        win.WindowState = WindowState.Maximized;
                        (p.Content as PackIcon).Kind = PackIconKind.WindowRestore;
                    }
                }
            });
            MinimizeWindowCommand = new RelayCommand<Window>(null, p => p.WindowState = WindowState.Minimized);
            DragMoveWindowCommand = new RelayCommand<Window>(null, p => p.DragMove());
            ToggleNavSideBarCommand = new RelayCommand<Window>(null, AnimateMenuBar);
            BlackOverlayMouseDownCommand = new RelayCommand<Rectangle>(p => p.IsHitTestVisible = true, p => { IsExpandSidePanel = !IsExpandSidePanel; AnimateMenuBar(Window.GetWindow(p)); });
            DeleteSearchBoxCommand = new RelayCommand<TextBox>(null, p => { p.Clear(); MiscClass.OnUserSearched(this, p.Text); });
            CheckForEnterKeyCommand = new RelayCommand<TextBox>(null, p =>
            {
                if (p != null)
                {
                    if (p.Text.Contains("\n") || p.Text.Contains("\r"))
                    {
                        p.Text = p.Text.Trim('\r', '\n');
                        p.CaretIndex = p.Text.Length;
                        MiscClass.OnUserSearched(this, p.Text);
                    }
                }

            });
            SearchButtonClickCommand = new RelayCommand<TextBox>(null, p => MiscClass.OnUserSearched(this, p.Text));
            GoBackNavigationCommand = new RelayCommand<object>(p => MiscClass.NavigationHelper.CanGoBack, async p => await SwichPage(MiscClass.NavigationHelper.Back()));
            GoForwardNavigationCommand = new RelayCommand<object>(p => MiscClass.NavigationHelper.CanGoForward, async p => await SwichPage(MiscClass.NavigationHelper.Forward()));
            ResetNotifyBadgeCommand = new RelayCommand<object>(null, p => NotifycationBadgeCount = 0);
            NavigateCommand = new RelayCommand<string>(null, NavigateProcess);
            ChangeWindowStateRequestCommand = new RelayCommand<RequestingWindowStateEventArgs>(null, ChangeUADMediaPlayerWindowState);
            OpenUADMediaPlayerCommand = new RelayCommand<object>(p => IsPlayButtonEnable, p => UADMediaPlayerVisibility = Visibility.Visible);
            UADMediaPlayerClosedCommand = new RelayCommand<object>(null, p =>
            {
                UADMediaPlayerVisibility = Visibility.Collapsed;
                IsPlayButtonEnable = false;
            });
            WindowStateChangedCommand = new RelayCommand<Window>(null, WindowStateChangedAction);
            PageLoaded = new RelayCommand<Window>(null, async p =>
            {
                LoadInSplashScreen();

                (Pages[0].DataContext as IPageContent).OnShow();
                CheckForAnimeSeriesUpdate();
                if ((Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.IsLoadPageInBackground)
                {
                    await Task.Delay(5000);
                    await LoadPagesToMemory(true);
                }
                else
                {
                    await LoadPagesToMemory(false);
                }
                NowPlayingPopupScale = p.Width / 1920 * (1 + (1 - (p.Width / 1920)));

            });
            MouseEnterCommand = new RelayCommand<string>(null, p =>
            {
                switch (p)
                {
                    case "MediaPlayerBtn":
                        MediaPlayerBtnMouseOver = true;
                        if (UADMediaPlayerPlaying)
                        {
                            IsNowPlayingPopupOpen = true;
                            if (MediaPlayerMouseOverBtnToken != null)
                            {
                                MediaPlayerMouseOverBtnToken.Cancel();
                            }
                        }
                        break;
                    case "MediaPlayerPopup":
                        IsMediaPlayerPopupMouseOver = true;
                        if (MediaPlayerMouseOverBtnToken != null)
                        {
                            MediaPlayerMouseOverBtnToken.Cancel();
                        }

                        break;
                    default:
                        break;
                }

            });
            MouseLeaveCommand = new RelayCommand<string>(null, async p =>
            {
                switch (p)
                {
                    case "MediaPlayerBtn":
                        MediaPlayerBtnMouseOver = false;
                        MediaPlayerMouseOverBtnToken = new CancellationTokenSource();
                        if (!IsMediaPlayerPopupMouseOver)
                        {
                            await QueueToClosePopup();
                        }

                        break;
                    case "MediaPlayerPopup":
                        IsMediaPlayerPopupMouseOver = false;
                        MediaPlayerMouseOverBtnToken = new CancellationTokenSource();
                        if (!MediaPlayerBtnMouseOver)
                        {
                            await QueueToClosePopup();
                        }

                        break;
                    default:
                        break;
                }
            });
            PauseMediaPlayerCommand = new RelayCommand<object>(null, p => UADMediaPlayerHelper.TogglePlayPause());
            LookSliderCommand = new RelayCommand<object>(null, p => UADMediaPlayerHelper.LockSliderPosition());
            ChangePositionCommand = new RelayCommand<double>(null, p => UADMediaPlayerHelper.ChangePositionByProgress(p));
            WindowSizeChangedCommand = new RelayCommand<Window>(null, p => NowPlayingPopupScale = p.Width / 1920 * (1 + (1 - (p.Width / 1920))));
            SelectPlayIndexCommand = new RelayCommand<int>(null, p => UADMediaPlayerHelper.ChangeDirectIndex(p));
            PreviousMediaPlayerPopupCommand = new RelayCommand<object>(null, p => UADMediaPlayerHelper.Previous());
            NextMediaPlayerPopupCommand = new RelayCommand<object>(null, p => UADMediaPlayerHelper.Next());
            OpenNowPlayingPopupCommand = new RelayCommand<Popup>(null, p => AnimateNowPlayingPopupOpen(p, true));
            ShowPlaylistButtonCommand = new RelayCommand<object>(null, p =>
            {
                IsPlaylistToggled = !IsPlaylistToggled;
                var cardHost = (UADMediaPlayerHelper.NowPlayingPopup.Child as Grid).Children[0] as Grid;
                var card = cardHost.Children[1] as Card;

                if (IsPlaylistToggled)
                {
                    cardHost.Height = 789;
                }

                Storyboard stb = GetPlaylistPanelAnim(IsPlaylistToggled);
                stb.Completed += (s, e) =>
                {
                    cardHost.Height = IsPlaylistToggled ? 789 : 197;
                };

                card.BeginStoryboard(stb);
            });
            FilterPlaylistPopup = new RelayCommand<string>(null, p =>
            {
                ICollectionView view = CollectionViewSource.GetDefaultView(FilteredNowPlayingPlaylist);
                view.Filter = o =>
                {
                    if (string.IsNullOrEmpty(p))
                    {
                        return true;
                    }

                    return (o as EpisodeInfo).Name.ToLower().Contains(p.ToLower());
                };

                view.Refresh();
            });
            CloseUniversalAnimeDownloader = new RelayCommand<object>(null, p => Application.Current.Shutdown(0));
        }

        private async void LoadInSplashScreen()
        {
            await Task.Delay(1000);
            LoadingStatus = "Checking for updates...";

            if (ApiHelpper.CheckForUpdates())
            {
                IsNewUpdatesDialogOpen = true;
                UpdateDescription = ApiHelpper.NewestVersionInfo?.body;
            }

            await Task.Delay(1000);
            LoadingStatus = "Initizaling...";
            await Task.Delay(1000);

            TimeSpan animDuration = TimeSpan.FromSeconds(1);
            DoubleAnimation slideAnim = new DoubleAnimation()
            {
                To = 0,
                EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseOut },
                Duration = animDuration
            };
            Storyboard.SetTargetProperty(slideAnim, new PropertyPath("Width"));

            DoubleAnimation fadeAnim = new DoubleAnimation()
            {
                To = 0,
                Duration = animDuration
            };
            Storyboard.SetTargetProperty(fadeAnim, new PropertyPath("Opacity"));

            Storyboard stb = new Storyboard();
            stb.Children.Add(slideAnim);
            stb.Children.Add(fadeAnim);

            (Application.Current.MainWindow.FindName("splashScreen") as Grid).BeginStoryboard(stb);
        }

        private Storyboard GetPlaylistPanelAnim(bool isOpen)
        {
            Storyboard stb = new Storyboard();
            var globalDuration = TimeSpan.FromSeconds(0.7);
            IEasingFunction globalEasingFunc = new CubicEase { EasingMode = EasingMode.EaseOut };
            double opacityTo = 1;
            double heightTo = 592;

            if (!isOpen)
            {
                opacityTo = 0;
                heightTo = 0;
            }

            DoubleAnimation opacityAnim = new DoubleAnimation()
            {
                To = opacityTo,
                Duration = globalDuration,
                EasingFunction = globalEasingFunc
            };
            if (!isOpen)
            {
                opacityAnim.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut };
                opacityAnim.Duration = TimeSpan.FromSeconds(0.6);
            }

            Storyboard.SetTargetProperty(opacityAnim, new PropertyPath("Opacity"));
            stb.Children.Add(opacityAnim);

            DoubleAnimation heightAnim = new DoubleAnimation()
            {
                To = heightTo,
                Duration = globalDuration,
                EasingFunction = globalEasingFunc
            };
            Storyboard.SetTargetProperty(heightAnim, new PropertyPath("Height"));
            stb.Children.Add(heightAnim);
            return stb;
        }

        private async Task QueueToClosePopup()
        {
            await Task.Run(() =>
            {
                for (int i = 0; i < 10; i++)
                {
                    Thread.Sleep(50);
                    if (MediaPlayerMouseOverBtnToken.IsCancellationRequested)
                    {
                        return;
                    }
                }
                Storyboard stb = null;
                Application.Current.Dispatcher.Invoke(async () =>
                {
                    stb = GetNowPlayingPopupCloseStoryBoard();
                    stb.Completed += (s, e) =>
                    {
                        if (MediaPlayerMouseOverBtnToken.IsCancellationRequested)
                        {
                            Application.Current.Dispatcher.Invoke(() => AnimateNowPlayingPopupOpen(UADMediaPlayerHelper.NowPlayingPopup, true));
                            return;
                        }
                        else
                        {
                            IsNowPlayingPopupOpen = false;
                            Storyboard stbPlaylist = GetPlaylistPanelAnim(IsPlaylistToggled);

                        }
                    };
                    if (IsPlaylistToggled)
                    {
                        IsPlaylistToggled = false;

                        var cardHost = (UADMediaPlayerHelper.NowPlayingPopup.Child as Grid).Children[0] as Grid;
                        var card = cardHost.Children[1] as Card;

                        if (IsPlaylistToggled)
                        {
                            cardHost.Height = 789;
                        }

                        Storyboard stb2 = GetPlaylistPanelAnim(IsPlaylistToggled);
                        stb2.Completed += (s, e) =>
                        {
                            cardHost.Height = IsPlaylistToggled ? 789 : 197;
                        };


                        card.BeginStoryboard(stb2);
                        await Task.Delay(100);
                    }
                    (UADMediaPlayerHelper.NowPlayingPopup.Child as Grid).BeginStoryboard(stb);
                });
            });
        }

        private async Task LoadPagesToMemory(bool waitShorter)
        {
            for (int i = 0; i < Pages.Count; i++)
            {
                if (Pages[i].Visibility == Visibility.Collapsed)
                {
                    Pages[i].Visibility = Visibility.Visible;
                    await Task.Delay(waitShorter ? 100 : 10);
                    Pages[i].Visibility = Visibility.Collapsed;
                    await Task.Delay(waitShorter ? 1000 : 100);
                }
            }
        }

        private void WindowStateChangedAction(Window obj)
        {
            //When user use full screen, we apply a 7 uniform margin to the MainWindow content (Grid) to avoid some UIElement offscreen
            if (obj.WindowState == WindowState.Maximized)
            {
                (obj.Content as Grid).Margin = new Thickness(7);
            }
            else if (obj.WindowState == WindowState.Normal)
            {
                (obj.Content as Grid).Margin = new Thickness(0);
            }
        }

        private void ChangeUADMediaPlayerWindowState(RequestingWindowStateEventArgs obj)
        {
            switch (obj.RequestState)
            {
                case WindowState.Normal:
                    Application.Current.MainWindow.WindowState = WindowState.Normal;
                    break;
                case WindowState.Maximized:
                    Application.Current.MainWindow.WindowState = WindowState.Maximized;
                    break;
                case WindowState.Minimized:
                    UADMediaPlayerVisibility = Visibility.Collapsed;
                    IsPlayButtonEnable = true;
                    break;
                default:
                    break;
            }
        }

        private void AnimateMenuBar(Window p)
        {
            if (p != null)
            {
                ScrollViewer scroll = (p.Content as Grid).Children[3] as ScrollViewer;
                Rectangle fadeOverlay = (p.Content as Grid).Children[2] as Rectangle;

                DoubleAnimation transitionAnim = new DoubleAnimation(scroll.Width, 255, TimeSpan.FromSeconds(0.5)) { DecelerationRatio = 0.1, EasingFunction = new BackEase() { EasingMode = EasingMode.EaseOut } };

                DoubleAnimation opacityAnim = new DoubleAnimation(fadeOverlay.Fill.Opacity, 0.5, TimeSpan.FromSeconds(0.5)) { DecelerationRatio = 0.1 };
                fadeOverlay.IsHitTestVisible = true;

                if (!IsExpandSidePanel)
                {
                    transitionAnim.To = 65;
                    opacityAnim.To = 0;
                    fadeOverlay.IsHitTestVisible = false;
                }
                scroll.BeginAnimation(FrameworkElement.WidthProperty, transitionAnim);
                fadeOverlay.Fill.BeginAnimation(Brush.OpacityProperty, opacityAnim);
            }
        }

        private async void CheckForAnimeSeriesUpdate()
        {
            bool modMissing = false;
            List<string> modMissingNames = new List<string>();
            var offlineList = (Application.Current.FindResource("MyAnimeLibraryViewModel") as MyAnimeLibraryViewModel).NoDelayAnimeLib;
            int updatedSeries = 0;
            for (int i = 0; i < offlineList.Count; i++)
            {
                AnimeSeriesInfo item = offlineList[i];
                var manager = ApiHelpper.CreateAnimeSeriesManagerObjectByClassName(item.ModInfo.ModTypeString);
                if (manager != null)
                {
                    manager.AttachedAnimeSeriesInfo = item;
                    AnimeSourceControl source = new AnimeSourceControl(manager);
                    if (await source.Update())
                    {
                        updatedSeries++;
                    }
                }
                else
                {
                    modMissing = true;
                    if (!modMissingNames.Contains(item.ModInfo.ModName))
                    {
                        modMissingNames.Add(item.ModInfo.ModName);
                    }
                }
            }
            if (offlineList.Count > 0)
            {
                NotificationManager.Add(new NotificationItem() { Title = "Check for anime updates completed", Detail = $"Found {updatedSeries} anime series need to updated out of {offlineList.Count} in your library. See download center for mode detail." });
            }

            if (modMissing)
            {
                string missingModList = string.Empty;
                foreach (var item in modMissingNames)
                {
                    missingModList += item + ", ";
                }

                missingModList.Substring(0, missingModList.Length - 4);
                NotificationManager.Add(new NotificationItem() { Title = "Some series are ignored!", Detail = $"There are {modMissingNames.Count} mod(s) is/are missing, series depend on these mod will be ignored. Missing mod list: {missingModList}" });
            }
        }

        public async void NavigateProcess(string pageName)
        {
            if (!string.IsNullOrEmpty(pageName))
            {
                int pageIndex = -1;
                switch (pageName)
                {
                    case "AllAnimeTab":
                        pageIndex = 0;
                        break;
                    case "MyAnimeLibrary":
                        pageIndex = 4;
                        break;
                    case "DownloadCenter":
                        pageIndex = 3;
                        break;
                    case "OfflineAnimeDetail":
                        pageIndex = 5;
                        break;
                    case "AnimeDetails":
                        pageIndex = 1;
                        break;
                    case "UADSettings":
                        pageIndex = 6;
                        break;
                    case "AnimeSuggestion":
                        pageIndex = 2;
                        break;
                    case "FeaturedAnime":
                        pageIndex = 7;
                        break;
                    case "Explore":
                        //This case explore need suggestion and feature to be able to show content so we will init them here
                        pageIndex = 8;
                        (Pages[2].DataContext as IPageContent).OnShow();
                        (Pages[7].DataContext as IPageContent).OnShow();
                        break;
                    case "About":
                        pageIndex = 9;
                        break;
                    default:
                        break;
                }

                await SwichPage(pageIndex);
                MiscClass.NavigationHelper.AddNavigationHistory(pageIndex);
            }
        }

        private async Task SwichPage(int pageIndex)
        {
            if (pageIndex != -1)
            {
                if (Pages[TransisionerIndex].DataContext is IPageContent cont)
                {
                    cont.OnHide();
                }
                if (pageIndex != TransisionerIndex)
                {
                    Pages[pageIndex].Visibility = Visibility.Visible;
                }

                await Task.Delay(50);
                if (Pages[pageIndex].DataContext is IPageContent pageContent)
                {
                    pageContent.OnShow();
                }
                int previousIndex = TransisionerIndex;
                TransisionerIndex = pageIndex;
                await Task.Delay(250);
                if (pageIndex != previousIndex && previousIndex != 1 && previousIndex != 5)
                {
                    Pages[previousIndex].Visibility = Visibility.Collapsed;
                }
            }
        }


        //Test
        public List<UserControl> Pages { get; set; } = new List<UserControl>()
        {
            new UcContentPages.AllAnimeTab(){ Visibility = Visibility.Visible},
            new UcContentPages.AnimeDetails(){ Visibility = Visibility.Visible},
            new UcContentPages.AnimeSuggestion(){ Visibility = Visibility.Collapsed},
            new UcContentPages.DownloadCenter(){ Visibility = Visibility.Collapsed},
            new UcContentPages.MyAnimeLibrary(){ Visibility = Visibility.Collapsed},
            new UcContentPages.OfflineAnimeDetail(){ Visibility = Visibility.Visible},
            new UcContentPages.UADSettings(){ Visibility = Visibility.Collapsed},
            new UcContentPages.FeaturedAnime(){ Visibility = Visibility.Collapsed},
            new UcContentPages.Explore(){ Visibility = Visibility.Collapsed},
            new UcContentPages.About(){Visibility = Visibility.Collapsed},
        };
    }
}
