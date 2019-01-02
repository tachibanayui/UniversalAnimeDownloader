using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using UniversalAnimeDownloader.CustomControl;
using UniversalAnimeDownloader.UserControls;

namespace UniversalAnimeDownloader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void OfflineAnimeListviewItem_PlayButton_Clicked(object sender, RoutedEventArgs e)
        {
            OfflineAnimeListviewItem obj = sender as OfflineAnimeListviewItem;
            string mediaLocation = obj.MediaLocation;
            Process.Start(mediaLocation);
        }

        private void ScreenBlocker_Click(object sender, RoutedEventArgs e)
        {
            UADPlayer player = Current.FindResource("uadEmbededPlayer") as UADPlayer;
            player.VM.IsBlockerActive = !player.VM.IsBlockerActive;
        }

        private void FakeAppCrash_Click(object sender, RoutedEventArgs e)
        {
            UADPlayer player = Current.FindResource("uadEmbededPlayer") as UADPlayer;
            if(player.IsFakeCrashActive)
            {
                player.FakeHost.Close();
                player.FakeAppCrashFill.Visibility = Visibility.Collapsed;
                player.IsFakeCrashActive = false;
            }
            else
            {
                player.FakeAppCrashFill.Visibility = Visibility.Visible;
                player.FakeHost = new FakeNotRespondingDialog();
                player.FakeHost.ShowDialog();
                player.IsFakeCrashActive = true;
            }
        }

        private void BackgroundPlayer_Click(object sender, RoutedEventArgs e)
        {
            UADPlayer player = Current.FindResource("uadEmbededPlayer") as UADPlayer;
            if(player.IsBackgroundPlayerActive)
            {
                Common.MainWin.Show();
                Common.MainWin.Focus();
            }
            else
            {
                Common.MainWin.Hide();
            }
            player.IsBackgroundPlayerActive = !player.IsBackgroundPlayerActive;
        }

        private void FocusOnMainWindow(object sender, System.Windows.Input.MouseButtonEventArgs e) => MainWindow.Focus();
    }
}
