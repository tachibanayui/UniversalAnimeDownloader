using MaterialDesignThemes.Wpf;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using UniversalAnimeDownloader.Animations;

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
        private GridLength _SideBarWidth = new GridLength(75);
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
                if(p != null)
                {
                    ColumnDefinition column = (p.Content as Grid).ColumnDefinitions[0];
                    Grid grid = (p.Content as Grid).Children[2] as Grid;

                    GridLengthAnimation anim = new GridLengthAnimation()
                    {
                        From = column.Width,
                        To = new GridLength(400),
                        Duration = TimeSpan.FromSeconds(0.35),
                        DecelerationRatio = 0.1
                    };

                    //DoubleAnimation opacityAnim = new DoubleAnimation()
                    //{
                    //    From = grid.Opacity,
                    //    To = 0.5,
                    //    Duration = TimeSpan.FromSeconds(0.35),
                    //    DecelerationRatio = 0.1
                    //};

                    if(!IsExpandSidePanel)
                    {
                        anim.To = new GridLength(75);
                        //opacityAnim.To = 1;
                    }

                    column.BeginAnimation(ColumnDefinition.WidthProperty, anim);
                    //grid.BeginAnimation(UIElement.OpacityProperty, opacityAnim);
                }
            });
            TestCommand = new RelayCommand<object>(p => true, p => { (Application.Current.FindResource("PaletteHelper") as PaletteHelper).SetLightDark(!IsDark); IsDark = !IsDark; });
        }
    }
}
