using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using UADAPI;
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


        #endregion

        public MainWindowViewModel()
        {
            string notificationString = UADSettingsManager.Instance.CurrentSettings.Notification;
            NotificationManager.Deserialize(notificationString);
            string downloadString = UADSettingsManager.Instance.CurrentSettings.Download;
            DownloadManager.Deserialize(downloadString);
            DownloadManager.Instances.CollectionChanged += (s,e) => UADSettingsManager.Instance.CurrentSettings.Download = DownloadManager.Serialize();
            NotificationManager.ItemRemoved += (s,e) => UADSettingsManager.Instance.CurrentSettings.Notification = NotificationManager.Serialize();
            NotificationManager.ItemAdded += (s, e) =>
            {
                NotifycationBadgeCount++;
                try { UADSettingsManager.Instance.CurrentSettings.Notification = NotificationManager.Serialize(); } catch { }
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
            CheckForAnimeSeriesUpdate();
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

        public void NavigateProcess(string pageName)
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
                    default:
                        break;
                }

                if (pageIndex != -1)
                {
                    if (Pages[TransisionerIndex].DataContext is IPageContent cont)
                    {
                        cont.OnHide();
                    }
                    TransisionerIndex = pageIndex;
                    MiscClass.NavigationHelper.AddNavigationHistory(pageIndex);
                    if (Pages[pageIndex].DataContext is IPageContent pageContent)
                    {
                        pageContent.OnShow();
                    }
                }
            }
        }


        //Test
        public List<UserControl> Pages { get; set; } = new List<UserControl>()
        {
            new UcContentPages.AllAnimeTab(),
            new UcContentPages.AnimeDetails(),
            new UcContentPages.AnimeSuggestion(),
            new UcContentPages.DownloadCenter(),
            new UcContentPages.MyAnimeLibrary(),
            new UcContentPages.OfflineAnimeDetail(),
            new UcContentPages.UADSettings(),
        };
    }
}
