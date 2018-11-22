using System;
using System.Collections.Generic;
using System.Linq;
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
using UniversalAnimeDownloader.Models;
using UniversalAnimeDownloader.ViewModel;

namespace UniversalAnimeDownloader.View
{
    /// <summary>
    /// Base class for both online and offline anime list
    /// </summary>
    public partial class AnimeList : Page
    {
        public Frame FrameHost
        {
            get { return (Frame)GetValue(FrameHostProperty); }
            set { SetValue(FrameHostProperty, value); }
        }
        public static readonly DependencyProperty FrameHostProperty =
            DependencyProperty.Register("FrameHost", typeof(Frame), typeof(AnimeList), new PropertyMetadata());


        public AnimeList()
        {
            InitializeComponent();
        }

        private void LoadMore(object sender, ScrollChangedEventArgs e) => OnLoadMoreEvent(sender, e);

        private void DeleteSearch(object sender, RoutedEventArgs e) => searchText.Text = string.Empty;

        private void DetectReturn(object sender, KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (e.Key != Key.Enter)
                return;

            OnSearchEvent(textBox.Text);
            btnSearch.Focus();
        }

        public event EventHandler<ScrollChangedEventArgs> LoadMoreEvent;
        public event EventHandler<EventArgs> SearchEvent;

        protected virtual void OnLoadMoreEvent(object sender, ScrollChangedEventArgs e) => LoadMoreEvent?.Invoke(sender, e);
        protected virtual void OnSearchEvent(string text) => SearchEvent?.Invoke(text, EventArgs.Empty);
    }
}