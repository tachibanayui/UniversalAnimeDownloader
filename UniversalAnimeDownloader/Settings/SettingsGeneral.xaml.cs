using MaterialDesignThemes.Wpf;
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
    /// Interaction logic for SettingsGeneral.xaml
    /// </summary>
    public partial class SettingsGeneral : Page
    {
        public SettingsGeneralViewModel VM;

        public Frame FrameHost
        {
            get { return (Frame)GetValue(FrameHostProperty); }
            set { SetValue(FrameHostProperty, value); }
        }
        public static readonly DependencyProperty FrameHostProperty =
            DependencyProperty.Register("FrameHost", typeof(Frame), typeof(SettingsGeneral), new PropertyMetadata());

        public SettingsGeneral()
        {
            InitializeComponent();
            VM = new SettingsGeneralViewModel();
            DataContext = VM;
            GetSettingValue();
        }

        private void GetSettingValue()
        {
            txbAnimeDir.Text = SettingsManager.Current.AnimeLibraryDirectory;
            txbMusicDir.Text = SettingsManager.Current.BackgroundMusicDirectory;
        }

        private void Event_SelectFolder(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            Button btn = sender as Button;
            TextBox tb = (btn.Parent as Grid).Children[0] as TextBox;
            tb.Text = dialog.SelectedPath;
            switch (HintAssist.GetHint(tb).ToString())
            {
                case "Anime Library Directory: ":
                    SettingsManager.Current.AnimeLibraryDirectory = dialog.SelectedPath;
                    break;
                case "Music Playback Directory: ":
                    SettingsManager.Current.BackgroundMusicDirectory = dialog.SelectedPath;
                    break;
                default:
                    break;
            }
        }

        private void Event_BGMusicChanged(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            SettingsManager.Current.BackgroundMusicVolume = (sender as Slider).Value;
        }

        private void Event_ResetThisSettingProfile(object sender, RoutedEventArgs e)
        {
            SettingsManager.ResetCurrentSettings();
        }

        private void Event_SelectFolderManual(object sender, RoutedEventArgs e)
        {
            string value = (sender as TextBox).Text;
            switch ((sender as TextBox).Name)
            {
                case "txbAnimeDir":
                    SettingsManager.Current.AnimeLibraryDirectory = value;
                    break;
                case "txbMusicDir":
                    SettingsManager.Current.BackgroundMusicDirectory = value;
                    break;
                default:
                    break;
            }
        }
    }
}
