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

namespace UniversalAnimeDownloader.View
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    /// 
    public partial class Explore : Page
    {


        public Frame HostFrame
        {
            get { return (Frame)GetValue(HostFrameProperty); }
            set { SetValue(HostFrameProperty, value); }
        }
        public static readonly DependencyProperty HostFrameProperty =
            DependencyProperty.Register("HostFrame", typeof(Frame), typeof(Explore), new PropertyMetadata());


        public Explore()
        {
            InitializeComponent();
        }

        private void BrowseAllAnime(object sender, RoutedEventArgs e) => HostFrame.Content = new AnimeList();
    }
}
