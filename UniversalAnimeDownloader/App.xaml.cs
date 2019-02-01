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
using uadcorelib.Models;
using UniversalAnimeDownloader.CustomControl;
using UniversalAnimeDownloader.UserControls;
using Newtonsoft.Json;
using System.IO;
using System.Windows.Media.Imaging;
using UniversalAnimeDownloader.Settings;

namespace UniversalAnimeDownloader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Current.DispatcherUnhandledException += (s, e) =>
            {
                e.Handled = true;
                Common.ReportError(e.Exception as Exception);
            };
        }


        private void OfflineAnimeListviewItem_PlayButton_Clicked(object sender, RoutedEventArgs e)
        {
            OfflineAnimeListviewItem obj = sender as OfflineAnimeListviewItem;
            string mediaLocation = obj.MediaLocation;
            if(SettingsManager.Current.PreferedPlayer == PlayerType.Embeded)
            {
                Common.MainWin.UADEmbededPlayerContainer.Visibility = Visibility.Visible;
                var player = Common.MainWin.UADEmbededPlayerContainer.Children[2] as UADPlayer;
                string managerFileLocation = mediaLocation.Substring(0, mediaLocation.LastIndexOf("\\")) + "\\Manager.json";
                string managerFileContent = File.ReadAllText(managerFileLocation);
                AnimeInformation info = JsonConvert.DeserializeObject<AnimeInformation>(managerFileContent);
                player.Title = info.AnimeName;
                var episode = info.Episodes.Where(query => query.VideoSource == mediaLocation).ToList()[0];
                player.AnimeThumbnail = new BitmapImage(new Uri(info.AnimeThumbnail));
                player.SubbedTitle = episode.EpisodeName;
                player.VideoUri = new Uri(mediaLocation);
                player.VM.UpdateBindings();
                Common.FadeInAnimation(Common.MainWin.UADEmbededPlayerContainer, TimeSpan.FromSeconds(1), false, AnimationFinish);
            }
            else
                Process.Start(mediaLocation);
        }

        private void AnimationFinish(object sender, EventArgs e)
        {
            
            var player = Common.MainWin.UADEmbededPlayerContainer.Children[2] as UADPlayer;

            if (SettingsManager.Current.PlayMediaFullScreen)
            {
                player.OnRequestWindowState(WindowState.Maximized);
                (player.btnFullScreenToggle.Content as PackIcon).Kind = PackIconKind.ArrowCollapse;
            }
            player.mediaPlayer.Play();
        }

        private void ScreenBlocker_Click(object sender, RoutedEventArgs e)
        {
            if (!SettingsManager.Current.IsSneakyWatcherEnabled)
                return;

            UADPlayer player = Current.FindResource("uadEmbededPlayer") as UADPlayer;

            if (SettingsManager.Current.IsPauseWhenSneakyWactherActive)
            {
                player.mediaPlayer.Pause();
                player.isPlaying = false;
                (player.btnPlayPause.Content as PackIcon).Kind = PackIconKind.Play;
            }

            if(SettingsManager.Current.ChangeAppIconWhenSneakyWatcherActive)
                player.OnRequestIconChange(new Uri("pack://application:,,,/Resources/WinDefaultIcon.png"));

            if (player.VM.IsBlockerActive)
                if (SettingsManager.Current.IsEnableMasterPassword)
                    if (new SneakyWatcherPasswordBox().ValidatePassword(SettingsManager.Current.SneakyWatcherMasterPassword, SettingsManager.Current.IsRandomizePasswordBox))
                    {
                        player.VM.IsBlockerActive = false;
                        if (SettingsManager.Current.ChangeAppIconWhenSneakyWatcherActive)
                            player.OnRequestIconChange(new Uri("pack://application:,,,/Resources/UADIcon.ico"));
                    }
                    else
                        return;
                else
                {
                    player.VM.IsBlockerActive = false;
                    if (SettingsManager.Current.ChangeAppIconWhenSneakyWatcherActive)
                        player.OnRequestIconChange(new Uri("pack://application:,,,/Resources/UADIcon.ico"));
                }
            else
                player.VM.IsBlockerActive = true;
        }

        private void FakeAppCrash_Click(object sender, RoutedEventArgs e)
        {
            if (!SettingsManager.Current.IsSneakyWatcherEnabled)
                return;

            UADPlayer player = Current.FindResource("uadEmbededPlayer") as UADPlayer;
            if (SettingsManager.Current.IsPauseWhenSneakyWactherActive)
            {
                player.mediaPlayer.Pause();
                player.isPlaying = false;
                (player.btnPlayPause.Content as PackIcon).Kind = PackIconKind.Play;
            }

            if (SettingsManager.Current.ChangeAppIconWhenSneakyWatcherActive)
                player.OnRequestIconChange(new Uri("pack://application:,,,/Resources/WinDefaultIcon.png"));

            if (player.IsFakeCrashActive)
            {
                if (player.IsFakeCrashActive)
                    if (SettingsManager.Current.IsEnableMasterPassword)
                        if (new SneakyWatcherPasswordBox().ValidatePassword(SettingsManager.Current.SneakyWatcherMasterPassword, SettingsManager.Current.IsRandomizePasswordBox))
                            UnActivateFakeAppCrash(player);
                        else
                            return;
                    else
                        UnActivateFakeAppCrash(player);
            }
            else
            {
                if (SettingsManager.Current.MakeWindowTopMost)
                    Common.MainWin.Topmost = true;
                if (SettingsManager.Current.DisableAltF4)
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
            if (SettingsManager.Current.MakeWindowTopMost)
                Common.MainWin.Topmost = false;
            if (SettingsManager.Current.DisableAltF4)
                Common.MainWin.Closing -= Common.CancelCloseWindow;

            if (SettingsManager.Current.ChangeAppIconWhenSneakyWatcherActive)
                player.OnRequestIconChange(new Uri("pack://application:,,,/Resources/UADIcon.ico"));
        }

        private void BackgroundPlayer_Click(object sender, RoutedEventArgs e)
        {
            if (!SettingsManager.Current.IsSneakyWatcherEnabled)
                return;

            UADPlayer player = Current.FindResource("uadEmbededPlayer") as UADPlayer;
            if (SettingsManager.Current.IsPauseWhenSneakyWactherActive)
            {
                player.mediaPlayer.Pause();
                player.isPlaying = false;
                (player.btnPlayPause.Content as PackIcon).Kind = PackIconKind.Play;
            }

            if (SettingsManager.Current.ChangeAppIconWhenSneakyWatcherActive)
                player.OnRequestIconChange(new Uri("pack://application:,,,/Resources/WinDefaultIcon.png"));

            if (player.IsBackgroundPlayerActive)
            {
                if (SettingsManager.Current.IsEnableMasterPassword)
                    if (new SneakyWatcherPasswordBox().ValidatePassword(SettingsManager.Current.SneakyWatcherMasterPassword, SettingsManager.Current.IsRandomizePasswordBox))
                    {
                        Common.MainWin.Show();
                        Common.MainWin.Focus();
                        player.IsBackgroundPlayerActive = false;
                        if (SettingsManager.Current.ChangeAppIconWhenSneakyWatcherActive)
                            player.OnRequestIconChange(new Uri("pack://application:,,,/Resources/UADIcon.ico"));
                    }
                    else
                        return;
                else
                {
                    Common.MainWin.Show();
                    Common.MainWin.Focus();
                    player.IsBackgroundPlayerActive = false;
                    if (SettingsManager.Current.ChangeAppIconWhenSneakyWatcherActive)
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
