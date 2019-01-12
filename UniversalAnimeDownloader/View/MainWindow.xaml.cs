using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using UniversalAnimeDownloader.Models;
using UniversalAnimeDownloader.Settings;
using UniversalAnimeDownloader.UserControls;

namespace UniversalAnimeDownloader.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        UADPlayer uadEmbededPlayer = null;
        GitHubData updateData;
        bool updateWhenExit;

        public MainWindow()
        {
            new SettingsManager();
            uadEmbededPlayer = Application.Current.FindResource("uadEmbededPlayer") as UADPlayer;
            InitializeComponent();
            uadEmbededPlayer.RequestWindowState += HandlePlayerWindowStateRequest;
            uadEmbededPlayer.RequestIconChange += IconChange;
            UADEmbededPlayerContainer.Children.Add(uadEmbededPlayer);
            Common.MainWin = this;
            InitializingApplication();
        }

        private async void InitializingApplication()
        {
            bool askToUpdate = false;

            await Task.Delay(4250);
            txblInitStatus.Text = "Checking for updates...";
            await Task.Run(() => Common.CheckForInternetConnection());
            if (Common.IsInternetAvaible)
            {
                try
                {
                    updateData = await Common.GetLatestUpdate();
                    if (updateData.assets[0].updated_at > Common.CurrentVersionReleaseDate)
                    {
                        if (SettingsManager.Current.DownloadUpdateWithoutAsking)
                        {
                            Common.DownloadUpdateContent(updateData, new AsyncCompletedEventHandler(ApplyUpdate));
                        }
                        else
                        {
                            askToUpdate = true;
                        }
                    }
                }
                catch(Exception e) { }
            }
            //Checking updates here
            txblInitStatus.Text = "Loading settings...";
            await Task.Delay(500);
            txblInitStatus.Text = "Finalizing...";
            await Task.Delay(500);
            welcomeScreenRoot.Focus();

            //Close Animation:
            CloseWelcomeAnimation();

            if(askToUpdate)
            {
                
                 OpenUpdateDialog(true);
            }
        }

        private void OpenUpdateDialog(bool isDownload)
        {
            if(isDownload)
            {
                txblUpdateDialogTitle.Text = "New update is avaiable!";
                updateDownloadPnl.Visibility = Visibility.Visible;
                updateRestartPnl.Visibility = Visibility.Collapsed;
            }
            else
            {
                txblUpdateDialogTitle.Text = "New update is ready to install!";
                updateDownloadPnl.Visibility = Visibility.Collapsed;
                updateRestartPnl.Visibility = Visibility.Visible;
            }
            txblUpdateVersion.Text = updateData.tag_name;
            txblUpdateReleaseDate.Text = updateData.assets[0].updated_at.ToLongDateString();
            txblUpdateSize.Text = (updateData.assets[0].size / 1024d).ToString() + "KB";
            txblUpdateTitle.Text = updateData.name;
            newVersionAvaiable.IsOpen = true;
            Color color = (Application.Current.Resources["ForeGroundColor"] as SolidColorBrush).Color;
            string rgbValue = $"rgb({color.R}, {color.G}, {color.B})";
            webUpdateDescription.Text = $"<body style=\"color: {rgbValue}\" >" + updateData.body + "</body>";
        }

        private void ApplyUpdate(object sender, AsyncCompletedEventArgs e) => OpenUpdateDialog(false);
        // color: rgba(255, 255, 255, 255)
        private void CloseWelcomeAnimation()
        {
            Binding bd = new Binding();
            bd.ElementName = "rootWindow";
            bd.Path = new PropertyPath(ActualWidthProperty);
            DoubleAnimation dbAnimation = new DoubleAnimation { To = 275, DecelerationRatio = 0.5 };
            dbAnimation.Duration = TimeSpan.FromSeconds(1);
            dbAnimation.BeginTime = TimeSpan.FromSeconds(0);
            Storyboard.SetTargetName(dbAnimation, "welcomeScreenRoot");
            Storyboard.SetTargetProperty(dbAnimation, new PropertyPath(WidthProperty));
            BindingOperations.SetBinding(dbAnimation, DoubleAnimation.FromProperty, bd);

            DoubleAnimation dbAnimation2 = new DoubleAnimation { To = 0 };
            dbAnimation2.Duration = TimeSpan.FromSeconds(0.5);
            dbAnimation2.BeginTime = TimeSpan.FromSeconds(0.75);
            Storyboard.SetTargetName(dbAnimation2, "welcomeScreenRoot");
            Storyboard.SetTargetProperty(dbAnimation2, new PropertyPath(OpacityProperty));

            DoubleAnimation dbAnimation3 = new DoubleAnimation { To = 0 };
            dbAnimation3.Duration = TimeSpan.FromSeconds(0.001);
            dbAnimation3.BeginTime = TimeSpan.FromSeconds(1.25);
            Storyboard.SetTargetName(dbAnimation3, "welcomeScreenRoot");
            Storyboard.SetTargetProperty(dbAnimation3, new PropertyPath(WidthProperty));

            Storyboard strBoard = new Storyboard();
            strBoard.Children.Add(dbAnimation);
            strBoard.Children.Add(dbAnimation2);
            strBoard.Children.Add(dbAnimation3);

            strBoard.Begin(welcomeScreenRoot);
        }

        private void IconChange(object sender, RequestWindowIconChangeEventArgs e)
        {
            Icon = new BitmapImage(e.IconLocation);
        }

        private void PageTransition(object sender, EventArgs e)
        {
            Frame frm = sender as Frame;
            string pageName = (frm.Content as Page).Title;
            txblNavigationTitle.Text = pageName;
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

        private void HandlePlayerWindowStateRequest(object sender, RequestingWindowStateEventArgs e)
        {
            WindowState = e.RequestState;

            if (e.RequestState == WindowState.Maximized)
                Grid.SetRow(uadEmbededPlayer, 0);
            else
                Grid.SetRow(uadEmbededPlayer, 1);
        }

        private void ExitUADPlayer(object sender, RoutedEventArgs e)
        {
            uadEmbededPlayer.mediaPlayer.Pause();
            UADEmbededPlayerContainer.Visibility = Visibility.Collapsed;
            UADEmbededPlayerContainer.Opacity = 0;
        }

        private void NotImplementedYet(object sender, MouseButtonEventArgs e) => notImplementDialog.IsOpen = true;

        private void Event_CloseNotImplementDialog(object sender, RoutedEventArgs e) => notImplementDialog.IsOpen = false;

        private void Event_CloseUpdateDialog(object sender, RoutedEventArgs e)
        {
            newVersionAvaiable.IsOpen = false;
            if ((sender as Button).Content.ToString() == "Restart later")
            {
                updateWhenExit = true;
                btnApplyUpdate.Visibility = Visibility.Visible;
            }
        }

        private void Event_DownloadUpdate(object sender, RoutedEventArgs e)
        {
            Common.DownloadUpdateContent(updateData, new AsyncCompletedEventHandler(ApplyUpdate));
            newVersionAvaiable.IsOpen = false;
        }

        private void Event_ApplyUpdate(object sender, RoutedEventArgs e)
        {
            string updaterLocation = AppDomain.CurrentDomain.BaseDirectory + "Updates\\" + updateData.assets[0].name;
            Application.Current.Shutdown();
            Process.Start(updaterLocation, "update " + Common.CurrentVersionName);
        }

        private void NavigateToAbout(object sender, MouseButtonEventArgs e)
        {
            pageViewer.Content = new About() { FrameHost = pageViewer };
            ClearPageHistory();
        }
    }
}
