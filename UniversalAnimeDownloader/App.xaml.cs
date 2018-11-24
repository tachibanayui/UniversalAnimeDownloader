using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace UniversalAnimeDownloader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void PlayFilm(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            string mediaLocation = btn.Tag as string;
            Process.Start(mediaLocation);
        }
    }
}
