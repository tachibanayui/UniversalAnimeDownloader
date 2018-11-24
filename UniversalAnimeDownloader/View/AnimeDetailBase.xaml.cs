using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using uadcorelib;
using uadcorelib.Models;
using UniversalAnimeDownloader.CustomControl;
using UniversalAnimeDownloader.ViewModel;

namespace UniversalAnimeDownloader.View
{
    /// <summary>
    /// Interaction logic for OnlineAnimeDetail.xaml
    /// </summary>
    public partial class AnimeDetailBase : Page
    {
        public Frame HostFrame
        {
            get { return (Frame)GetValue(HostFrameProperty); }
            set { SetValue(HostFrameProperty, value); }
        }
        public static readonly DependencyProperty HostFrameProperty =
            DependencyProperty.Register("HostFrame", typeof(Frame), typeof(AnimeDetailBase), new PropertyMetadata());
        
        public AnimeDetailBase()
        {
            InitializeComponent();
        }
    }
}
