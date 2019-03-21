﻿using MaterialDesignThemes.Wpf;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

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
        public ICommand TestCommand { get; set; }
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


        #endregion

        public bool IsDark { get; set; }
        public MainWindowViewModel()
        {
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
            ToggleNavSideBarCommand = new RelayCommand<Window>(p => true, p =>
            {
                if (p != null)
                {
                    ScrollViewer scroll = (p.Content as Grid).Children[3] as ScrollViewer;
                    Rectangle fadeOverlay = (p.Content as Grid).Children[2] as Rectangle;

                    DoubleAnimation transitionAnim = new DoubleAnimation(scroll.Width, 255, TimeSpan.FromSeconds(0.5)) { DecelerationRatio = 0.1, EasingFunction = new BackEase() { EasingMode = EasingMode.EaseOut } };

                    DoubleAnimation opacityAnim = new DoubleAnimation(fadeOverlay.Opacity, 0.5, TimeSpan.FromSeconds(0.5)) { DecelerationRatio = 0.1 };

                    if (!IsExpandSidePanel)
                    {
                        transitionAnim.To = 65;
                        opacityAnim.To = 0;
                    }

                    scroll.BeginAnimation(FrameworkElement.WidthProperty, transitionAnim);
                    fadeOverlay.BeginAnimation(UIElement.OpacityProperty, opacityAnim);
                }
            });
            DeleteSearchBoxCommand = new RelayCommand<TextBox>(p => true, p => p.Clear());
            CheckForEnterKeyCommand = new RelayCommand<TextBox>(p => true, p =>
            {
                if (p != null)
                {
                    if (p.Text.Contains("\n") || p.Text.Contains("\r"))
                    {
                        p.Text = p.Text.Trim('\r', '\n');
                        MiscClass.OnUserSearched(this, p.Text);
                    }
                }

            });
            SearchButtonClickCommand = new RelayCommand<TextBox>(p => true, p => MiscClass.OnUserSearched(this, p.Text));
            GoBackNavigationCommand = new RelayCommand<object>(p => MiscClass.NavigationHelper.CanGoBack, p => TransisionerIndex = MiscClass.NavigationHelper.Back());
            GoForwardNavigationCommand = new RelayCommand<object>(p => MiscClass.NavigationHelper.CanGoForward, p => TransisionerIndex = MiscClass.NavigationHelper.Forward());
            TestCommand = new RelayCommand<object>(p => true, p => { (Application.Current.FindResource("PaletteHelper") as PaletteHelper).SetLightDark(!IsDark); IsDark = !IsDark; });
        }
    }
}