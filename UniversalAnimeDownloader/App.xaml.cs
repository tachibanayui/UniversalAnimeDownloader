using MaterialDesignThemes.Wpf;
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
            if(SettingsValues.PreferedPlayer == PlayerType.Embeded)
            {
                Common.MainWin.UADEmbededPlayerContainer.Visibility = Visibility.Visible;
                var player = Common.MainWin.UADEmbededPlayerContainer.Children[2] as UADPlayer;
                player.VideoUri = new Uri(mediaLocation);
                Common.FadeInAnimation(Common.MainWin.UADEmbededPlayerContainer, TimeSpan.FromSeconds(1), false, AnimationFinish);
            }
            else
                Process.Start(mediaLocation);
        }

        private void AnimationFinish(object sender, EventArgs e)
        {
            var player = Common.MainWin.UADEmbededPlayerContainer.Children[2] as UADPlayer;

            if (SettingsValues.PlayMediaFullScreen)
            {
                player.OnRequestWindowState(WindowState.Maximized);
                (player.btnFullScreenToggle.Content as PackIcon).Kind = PackIconKind.ArrowCollapse;
            }
            player.mediaPlayer.Play();
        }

        private void ScreenBlocker_Click(object sender, RoutedEventArgs e)
        {
            UADPlayer player = Current.FindResource("uadEmbededPlayer") as UADPlayer;

            if (SettingsValues.IsPauseWhenSneakyWactherActive)
            {
                player.mediaPlayer.Pause();
                player.isPlaying = false;
                (player.btnPlayPause.Content as PackIcon).Kind = PackIconKind.Play;
            }

            if(SettingsValues.ChangeAppIconWhenSneakyWatcherActive)
                player.OnRequestIconChange(new Uri("pack://application:,,,/Resources/WinDefaultIcon.png"));

            if (player.VM.IsBlockerActive)
                if (SettingsValues.IsEnableMasterPassword)
                    if (new SneakyWatcherPasswordBox().ValidatePassword(SettingsValues.SneakyWatcherMasterPassword, SettingsValues.IsRandomizePasswordBox))
                    {
                        player.VM.IsBlockerActive = false;
                        if (SettingsValues.ChangeAppIconWhenSneakyWatcherActive)
                            player.OnRequestIconChange(new Uri("pack://application:,,,/Resources/UADIcon.ico"));
                    }
                    else
                        return;
                else
                {
                    player.VM.IsBlockerActive = false;
                    if (SettingsValues.ChangeAppIconWhenSneakyWatcherActive)
                        player.OnRequestIconChange(new Uri("pack://application:,,,/Resources/UADIcon.ico"));
                }
            else
                player.VM.IsBlockerActive = true;
        }

        private void FakeAppCrash_Click(object sender, RoutedEventArgs e)
        {
            UADPlayer player = Current.FindResource("uadEmbededPlayer") as UADPlayer;
            if (SettingsValues.IsPauseWhenSneakyWactherActive)
            {
                player.mediaPlayer.Pause();
                player.isPlaying = false;
                (player.btnPlayPause.Content as PackIcon).Kind = PackIconKind.Play;
            }

            if (SettingsValues.ChangeAppIconWhenSneakyWatcherActive)
                player.OnRequestIconChange(new Uri("pack://application:,,,/Resources/WinDefaultIcon.png"));

            if (player.IsFakeCrashActive)
            {
                if (player.IsFakeCrashActive)
                    if (SettingsValues.IsEnableMasterPassword)
                        if (new SneakyWatcherPasswordBox().ValidatePassword(SettingsValues.SneakyWatcherMasterPassword, SettingsValues.IsRandomizePasswordBox))
                            UnActivateFakeAppCrash(player);
                        else
                            return;
                    else
                        UnActivateFakeAppCrash(player);
            }
            else
            {
                if (SettingsValues.MakeWindowTopMost)
                    Common.MainWin.Topmost = true;
                if (SettingsValues.DisableAltF4)
                    Common.MainWin.Closing += Common.CancelCloseWindow;
                player.FakeAppCrashFill.Visibility = Visibility.Visible;
                player.FakeHost = new FakeNotRespondingDialog();
                player.FakeHost.ShowDialog();
                player.IsFakeCrashActive = true;
            }
        }

        private void UnActivateFakeAppCrash(UADPlayer player)
        {
            player.FakeHost.Close();
            player.FakeAppCrashFill.Visibility = Visibility.Collapsed;
            player.IsFakeCrashActive = false;
            if (SettingsValues.MakeWindowTopMost)
                Common.MainWin.Topmost = false;
            if (SettingsValues.DisableAltF4)
                Common.MainWin.Closing -= Common.CancelCloseWindow;

            if (SettingsValues.ChangeAppIconWhenSneakyWatcherActive)
                player.OnRequestIconChange(new Uri("pack://application:,,,/Resources/UADIcon.ico"));
        }

        private void BackgroundPlayer_Click(object sender, RoutedEventArgs e)
        {
            UADPlayer player = Current.FindResource("uadEmbededPlayer") as UADPlayer;
            if (SettingsValues.IsPauseWhenSneakyWactherActive)
            {
                player.mediaPlayer.Pause();
                player.isPlaying = false;
                (player.btnPlayPause.Content as PackIcon).Kind = PackIconKind.Play;
            }

            if (SettingsValues.ChangeAppIconWhenSneakyWatcherActive)
                player.OnRequestIconChange(new Uri("pack://application:,,,/Resources/WinDefaultIcon.png"));

            if (player.IsBackgroundPlayerActive)
            {
                if (SettingsValues.IsEnableMasterPassword)
                    if (new SneakyWatcherPasswordBox().ValidatePassword(SettingsValues.SneakyWatcherMasterPassword, SettingsValues.IsRandomizePasswordBox))
                    {
                        Common.MainWin.Show();
                        Common.MainWin.Focus();
                        player.IsBackgroundPlayerActive = false;
                        if (SettingsValues.ChangeAppIconWhenSneakyWatcherActive)
                            player.OnRequestIconChange(new Uri("pack://application:,,,/Resources/UADIcon.ico"));
                    }
                    else
                        return;
                else
                {
                    Common.MainWin.Show();
                    Common.MainWin.Focus();
                    player.IsBackgroundPlayerActive = false;
                    if (SettingsValues.ChangeAppIconWhenSneakyWatcherActive)
                        player.OnRequestIconChange(new Uri("pack://application:,,,/Resources/UADIcon.ico"));
                }
            }
            else
            {
                Common.MainWin.Hide();
                player.IsBackgroundPlayerActive = true;
            }
        }

        private void FocusOnMainWindow(object sender, System.Windows.Input.MouseButtonEventArgs e) => MainWindow.Focus();
    }
}
