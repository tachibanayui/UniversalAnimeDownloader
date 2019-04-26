using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
        public ICommand ChangeWindowStateRequestOnlineCommand { get; set; }
        public ICommand UADMediaPlayerClosedCommand { get; set; }
        public ICommand WindowStateChangedCommand { get; set; }
        public ICommand PageLoaded { get; set; }

        public ICommand NavigateCommand { get; set; }
        #endregion

        #region BindableProperties
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

        private Visibility _UADOnlineMediaPlayerVisibility = Visibility.Collapsed;
        public Visibility UADOnlineMediaPlayerVisibility
        {
            get
            {
                return _UADOnlineMediaPlayerVisibility;
            }
            set
            {
                if (_UADOnlineMediaPlayerVisibility != value)
                {
                    _UADOnlineMediaPlayerVisibility = value;
                    OnPropertyChanged();
                }
            }
        }




        #endregion

        #region Fields and Properties
        public bool IsPlayButtonEnable { get; set; }
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

            CloseWindowCommand = new RelayCommand<object>(p => true, p => Application.Current.Shutdown());
            ChangeWindowStateCommand = new RelayCommand<Button>(p => true, p =>
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
            MinimizeWindowCommand = new RelayCommand<Window>(p => true, p => p.WindowState = WindowState.Minimized);
            DragMoveWindowCommand = new RelayCommand<Window>(p => true, p => p.DragMove());
            ToggleNavSideBarCommand = new RelayCommand<Window>(p => true, AnimateMenuBar);
            BlackOverlayMouseDownCommand = new RelayCommand<Rectangle>(p => p.IsHitTestVisible = true, p => { IsExpandSidePanel = !IsExpandSidePanel; AnimateMenuBar(Window.GetWindow(p)); });
            DeleteSearchBoxCommand = new RelayCommand<TextBox>(p => true, p => { p.Clear(); MiscClass.OnUserSearched(this, p.Text); });
            CheckForEnterKeyCommand = new RelayCommand<TextBox>(p => true, p =>
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
            SearchButtonClickCommand = new RelayCommand<TextBox>(p => true, p => MiscClass.OnUserSearched(this, p.Text));
            GoBackNavigationCommand = new RelayCommand<object>(p => MiscClass.NavigationHelper.CanGoBack, p =>
            {
                TransisionerIndex = MiscClass.NavigationHelper.Back();
                var cnt = Pages[TransisionerIndex].DataContext as IPageContent;
                if (cnt != null)
                {
                    cnt.OnShow();
                }
            });
            GoForwardNavigationCommand = new RelayCommand<object>(p => MiscClass.NavigationHelper.CanGoForward, p => TransisionerIndex = MiscClass.NavigationHelper.Forward());
            ResetNotifyBadgeCommand = new RelayCommand<object>(p => true, p => NotifycationBadgeCount = 0);
            NavigateCommand = new RelayCommand<string>(p => true, NavigateProcess);
            ChangeWindowStateRequestCommand = new RelayCommand<RequestingWindowStateEventArgs>(p => true, ChangeUADMediaPlayerWindowState);
            ChangeWindowStateRequestOnlineCommand = new RelayCommand<RequestingWindowStateEventArgs>(p => true, ChangeUADOnlineMediaPlayerWindowState);
            OpenUADMediaPlayerCommand = new RelayCommand<object>(p => IsPlayButtonEnable, p =>
            {
                if (UADMediaPlayerHelper.IsOnlineMediaPlayerPlaying)
                {
                    UADOnlineMediaPlayerVisibility = Visibility.Visible;
                }
                else
                {
                    UADMediaPlayerVisibility = Visibility.Visible;
                }
            });
            UADMediaPlayerClosedCommand = new RelayCommand<string>(p => true, p =>
            {
                if (p == "Offline")
                {
                    UADMediaPlayerVisibility = Visibility.Collapsed;
                }
                else
                {
                    UADOnlineMediaPlayerVisibility = Visibility.Collapsed;
                }

                IsPlayButtonEnable = false;
            });
            WindowStateChangedCommand = new RelayCommand<Window>(p => true, WindowStateChangedAction);
            PageLoaded = new RelayCommand<Window>(p => true, async p =>
            {
                p.Visibility = Visibility.Collapsed;
                (Pages[0].DataContext as IPageContent).OnShow();
                CheckForAnimeSeriesUpdate();
                if((Application.Current.FindResource("Settings")as UADSettingsManager).CurrentSettings.IsLoadPageInBackground)
                {
                    p.Visibility = Visibility.Visible;
                    await Task.Delay(5000);
                    await LoadPagesToMemory(true);
                }
                else
                {
                    await LoadPagesToMemory(false);
                    p.Visibility = Visibility.Visible;
                }

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

        private void ChangeUADOnlineMediaPlayerWindowState(RequestingWindowStateEventArgs obj)
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
                    UADOnlineMediaPlayerVisibility = Visibility.Collapsed;
                    IsPlayButtonEnable = true;
                    break;
                default:
                    break;
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

                DoubleAnimation opacityAnim = new DoubleAnimation(fadeOverlay.Opacity, 0.5, TimeSpan.FromSeconds(0.5)) { DecelerationRatio = 0.1 };
                fadeOverlay.IsHitTestVisible = true;

                if (!IsExpandSidePanel)
                {
                    transitionAnim.To = 65;
                    opacityAnim.To = 0;
                    fadeOverlay.IsHitTestVisible = false;
                }
                scroll.BeginAnimation(FrameworkElement.WidthProperty, transitionAnim);
                fadeOverlay.BeginAnimation(UIElement.OpacityProperty, opacityAnim);
            }
        }

        private async void CheckForAnimeSeriesUpdate()
        {
            var offlineList = (Application.Current.FindResource("MyAnimeLibraryViewModel") as MyAnimeLibraryViewModel).NoDelayAnimeLib;
            int updatedSeries = 0;
            for (int i = 0; i < offlineList.Count; i++)
            {
                AnimeSeriesInfo item = offlineList[i];
                var manager = ApiHelpper.CreateAnimeSeriesManagerObjectByClassName(item.ModInfo.ModTypeString);
                manager.AttachedAnimeSeriesInfo = item;
                AnimeSourceControl source = new AnimeSourceControl(manager);
                if (await source.Update())
                {
                    updatedSeries++;
                }
            }

            NotificationManager.Add(new NotificationItem() { Title = "Check for anime updates completed", Detail = $"Found {updatedSeries} anime series need to updated out of {offlineList.Count} in your library. See download center for mode detail." });
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
                    default:
                        break;
                }

                if (pageIndex != -1)
                {
                    if (Pages[TransisionerIndex].DataContext is IPageContent cont)
                    {
                        cont.OnHide();
                    }
                    if(pageIndex != TransisionerIndex)
                        Pages[pageIndex].Visibility = Visibility.Visible;
                    await Task.Delay(50);
                    if (Pages[pageIndex].DataContext is IPageContent pageContent)
                    {
                        pageContent.OnShow();
                    }
                    int previousIndex = TransisionerIndex;
                    TransisionerIndex = pageIndex;
                    MiscClass.NavigationHelper.AddNavigationHistory(pageIndex);
                    await Task.Delay(250);
                    if(pageIndex != previousIndex && pageIndex != 1 && pageIndex != 5)
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
            new UcContentPages.Explore(){ Visibility = Visibility.Collapsed}
        };
    }
}
