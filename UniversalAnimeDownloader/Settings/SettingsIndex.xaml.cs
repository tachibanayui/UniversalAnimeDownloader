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

namespace UniversalAnimeDownloader.Settings
{
    /// <summary>
    /// Interaction logic for SettingsIndex.xaml
    /// </summary>
    public partial class SettingsIndex : Page
    {
        public Frame FrameHost
        {
            get { return (Frame)GetValue(FrameHostProperty); }
            set { SetValue(FrameHostProperty, value); }
        }
        public static readonly DependencyProperty FrameHostProperty =
            DependencyProperty.Register("FrameHost", typeof(Frame), typeof(SettingsIndex), new PropertyMetadata());

        public SettingsIndex()
        {
            InitializeComponent();
        }

        private void NavigateToSettings(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            string settingName = (((btn.Content as Grid).Children[0] as Grid).Children[1] as TextBlock).Text;
            switch (settingName)
            {
                case "General Settings":
                    FrameHost.Content = new SettingsGeneral() { FrameHost = FrameHost };
                    break;
                case "Account Settings":
                    FrameHost.Content = new SettingsAccount() { FrameHost = FrameHost };
                    break;
                case "Appearance Settings":
                    FrameHost.Content = new SettingsAppearance() { FrameHost = FrameHost };
                    break;
                case "Download Settings":
                    FrameHost.Content = new SettingsDownload() { FrameHost = FrameHost };
                    break;
                case "Playback Settings":
                    FrameHost.Content = new SettingsPlayback() { FrameHost = FrameHost };
                    break;
                default:
                    break;
            }
            Common.FadeInTransition(FrameHost, .5);
        }
    }
}
