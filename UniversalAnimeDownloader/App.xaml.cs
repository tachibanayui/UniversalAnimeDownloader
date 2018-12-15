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
    }
}
