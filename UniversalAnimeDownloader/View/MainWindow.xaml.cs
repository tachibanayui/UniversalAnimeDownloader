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
using UniversalAnimeDownloader.Settings;

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
            a.VideoUri = new Uri("F:\\Test.mp4");
        }

        private void PageTransition(object sender, EventArgs e)
        {
            //ClearPageHistory();
        }

        private void ClearPageHistory()
        {
            while (pageViewer.CanGoBack)
                pageViewer.RemoveBackEntry();
            GC.Collect();
            Common.FadeInTransition(pageViewer, .5);
        }

        //Handle the Custom Window Action
        private void MinimizedWindow(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;
        private void MaximizeWindow(object sender, RoutedEventArgs e) => WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        private void ExitApplication(object sender, RoutedEventArgs e) => Application.Current.Shutdown(0);

        //Navigation
        private void NavigateToExplore(object sender, MouseButtonEventArgs e)
        {
            pageViewer.Content = new Explore { HostFrame = pageViewer };
            ClearPageHistory();
        }

        private void NavigateToFeatured(object sender, MouseButtonEventArgs e)
        {
        }

        private void NavigateToYouMayLike(object sender, MouseButtonEventArgs e)
        {

        }

        private void NavigateToAllAnime(object sender, MouseButtonEventArgs e)
        {
            pageViewer.Content = new AllAnimeTab { FrameHost = pageViewer };
            ClearPageHistory();
        }

        private void NavigateToMyAnime(object sender, MouseButtonEventArgs e)
        {
            pageViewer.Content = new MyAnimeLib() { FrameHost = pageViewer };
            ClearPageHistory();
        }

        private void NavigateBack(object sender, RoutedEventArgs e)
        {
            if(pageViewer.CanGoBack)
                pageViewer.GoBack();
        }


        private void NavigateToSetting(object sender, MouseButtonEventArgs e)
        {
            Grid grid = sender as Grid;
            string settingName = ((grid.Children[1] as StackPanel).Children[1] as TextBlock).Text.Trim();
            switch (settingName)
            {
                case "Settings":
                    pageViewer.Content = new SettingsIndex() { FrameHost = pageViewer };
                    break;
                case "General":
                    pageViewer.Content = new SettingsGeneral() { FrameHost = pageViewer };
                    break;
                case "Account":
                    pageViewer.Content = new SettingsAccount() { FrameHost = pageViewer };
                    break;
                case "Appearance":
                    pageViewer.Content = new SettingsAppearance() { FrameHost = pageViewer };
                    break;
                case "Download":
                    pageViewer.Content = new SettingsDownload() { FrameHost = pageViewer };
                    break;
                case "Playback":
                    pageViewer.Content = new SettingsPlayback() { FrameHost = pageViewer };
                    break;
                default:
                    break;
            }
            ClearPageHistory();
        }
    }
}
