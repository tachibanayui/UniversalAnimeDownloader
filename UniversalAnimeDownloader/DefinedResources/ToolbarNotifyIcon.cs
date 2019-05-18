using Hardcodet.Wpf.TaskbarNotification;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace UniversalAnimeDownloader.DefinedResources
{
    public partial class ToolbarNotifyIcon : ResourceDictionary
    {
        public void Event_ToggleSneakyWatcher(object sender, RoutedEventArgs e)
        {
            var player = UADMediaPlayerHelper.MediaPlayer;
            switch ((sender as Button).Name)
            {
                case "blocker":
                    player.ActivateBlocker();
                    break;
                case "crash":
                    player.ActivateFakeCrash();
                    break;
                case "bg":
                    player.ActivateBGPlayer();
                    break;
            }
        }

        public void CloseBalloonEvent(object sender, RoutedEventArgs e)
        {
            var hostBalloon = MiscClass.FindParent<Card>(sender as DependencyObject);

            var stb = new Storyboard();
            DoubleAnimation slideOutAnim = new DoubleAnimation()
            {
                From = 0,
                To = 150,
                Duration = TimeSpan.FromSeconds(0.5),
                EasingFunction = new QuarticEase() { EasingMode = EasingMode.EaseOut },
                FillBehavior = FillBehavior.Stop
            };
            Storyboard.SetTargetProperty(slideOutAnim, new PropertyPath("RenderTransform.Children[3].Y"));
            stb.Children.Add(slideOutAnim);
            stb.Completed += (s, ee) =>
            {
                (Application.Current.FindResource("mainToolbarTray") as TaskbarIcon).CloseBalloon();
                ((hostBalloon.RenderTransform as TransformGroup).Children[3] as TranslateTransform).Y = 0;
            };
            hostBalloon.BeginStoryboard(stb);
        }
    }
}
