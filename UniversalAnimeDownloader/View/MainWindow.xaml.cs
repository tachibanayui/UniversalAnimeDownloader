using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace UniversalAnimeDownloader.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void PageTransition(object sender, EventArgs e)
        {
            while (pageViewer.CanGoBack)
                pageViewer.RemoveBackEntry();
            GC.Collect();
            Common.FadeInTransition(pageViewer, .5);
        }

        //Handle the Custom Window Action
        private void MinimizedWindow(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;
        private void MaximizeWindow(object sender, RoutedEventArgs e) => WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        private void ExitApplication(object sender, RoutedEventArgs e) => Environment.Exit(0);

        //Navigation
        private void NavigateToExplore(object sender, MouseButtonEventArgs e) => pageViewer.Content = new Explore { HostFrame = pageViewer };

        private void NavigateToFeatured(object sender, MouseButtonEventArgs e)
        {
        }

        private void NavigateToYouMayLike(object sender, MouseButtonEventArgs e)
        {

        }

        private void NavigateToAllAnime(object sender, MouseButtonEventArgs e) => pageViewer.Content = new AllAnimeTab { FrameHost = pageViewer };
    }
}
