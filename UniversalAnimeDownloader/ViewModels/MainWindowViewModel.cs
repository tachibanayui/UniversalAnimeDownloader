using MaterialDesignThemes.Wpf;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

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
                    ScrollViewer scroll = (p.Content as Grid).Children[2] as ScrollViewer;
                    Grid grd = (p.Content as Grid).Children[1] as Grid;

                    DoubleAnimationUsingKeyFrames transitionAnim = new DoubleAnimationUsingKeyFrames();
                    transitionAnim.KeyFrames.Add(new EasingDoubleKeyFrame() { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0)), Value = scroll.Width });
                    transitionAnim.KeyFrames.Add(new EasingDoubleKeyFrame() { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.35)), Value = 255 });

                    //DoubleAnimation transitionAnim = new DoubleAnimation(scroll.Width, 255, TimeSpan.FromSeconds(0.35)) { DecelerationRatio = 0.1 };

                    DoubleAnimation opacityAnim = new DoubleAnimation(grd.Opacity, 0.5, TimeSpan.FromSeconds(0.35)) { DecelerationRatio = 0.1 };

                    if (!IsExpandSidePanel)
                    {
                        transitionAnim.KeyFrames[1].Value = 65;
                        opacityAnim.To = 1;
                    }

                    scroll.BeginAnimation(FrameworkElement.WidthProperty, transitionAnim);
                    grd.BeginAnimation(UIElement.OpacityProperty, opacityAnim);
                }
            });
            TestCommand = new RelayCommand<object>(p => true, p => { (Application.Current.FindResource("PaletteHelper") as PaletteHelper).SetLightDark(!IsDark); IsDark = !IsDark; });
        }
    }
}
