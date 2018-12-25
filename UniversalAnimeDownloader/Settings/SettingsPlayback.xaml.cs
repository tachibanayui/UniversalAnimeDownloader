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
using System.Windows.Navigation;
using System.Windows.Shapes;
using UniversalAnimeDownloader.ViewModel;

namespace UniversalAnimeDownloader.Settings
{
    /// <summary>
    /// Interaction logic for SettingsPlayback.xaml
    /// </summary>
    public partial class SettingsPlayback : Page
    {
        public SettingsPlaybackViewModel VM;
        
        public SettingsPlayback()
        {
            VM = new SettingsPlaybackViewModel();
            DataContext = VM;
            InitializeComponent();
            stretchMode.ItemsSource = typeof(Stretch).GetEnumNames();
        }

        private void ChangeDescription(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cbx = sender as ComboBox;
            if (playerDescription == null)
                return;

            switch (cbx.SelectedValue as string)
            {
                case "UAD Player":
                    playerDescription.Text = "Use our build-in playback to help you get a better watching experience and support unique feature such as sneaky watcher";
                    break;
                case "Exthernal Player":
                    playerDescription.Text = "Use your default exthernal playback program such as Window Media Player or VLC to play anime.Does not support unique feature like sneaky watcher,...";
                    break;
                default:
                    break;
            }
        }
    }
}
