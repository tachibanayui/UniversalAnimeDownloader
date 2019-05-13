using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

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

    }
}
