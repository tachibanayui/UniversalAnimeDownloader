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
    /// Interaction logic for AnimeList.xaml
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


        public AnimeListViewModel VM;
        public int cardCount;

        public AnimeList()
        {
            InitializeComponent();
            VM = new AnimeListViewModel(Dispatcher);
            DataContext = VM;

            VM.IsHaveConnection = Common.InternetAvaible;

            //Get the films data collection
            Thread thd = new Thread(() => {
                FilmListModel filmList = BaseLibraryClass.GetFilmList(cardCount, 50);
                cardCount += 50;
                AddCard(filmList);
            });
            thd.Name = "GetDataForThe1stTime";
            thd.Start();

            //Event
            cbxGenre.SelectionChanged += ChangeGenre;
        }

        private void AddCard(FilmListModel filmList, bool removeOldCard = true, string genre = null)
        {

            //Remove the old anime card
            if(removeOldCard)
                Dispatcher.Invoke(() => animeCardContainer.Children.RemoveRange(0, animeCardContainer.Children.Count - 1));

            if (filmList == null)
            {
                VM.IsLoading = false;
                return;
            }
            VuigheAnimeCard[] cards = null;
            Dispatcher.Invoke(() => cards = new VuigheAnimeCard[filmList.data.Length]);

            Thread.Sleep(750);

            for (int i = 0; i < filmList.data.Length; i++)
            {
                Dispatcher.Invoke(() => {
                    cards[i] = new VuigheAnimeCard();
                    cards[i].Opacity = 0;
                    animeCardContainer.Children.Add(cards[i]);
                    cards[i].AnimeBG = new BitmapImage();
                    cards[i].Data = new VuigheAnimeManager(filmList.data[i]);
                    cards[i].BeginAnimation(OpacityProperty, new DoubleAnimation(1, TimeSpan.FromSeconds(.5)));
                });
                Thread.Sleep(10);
            }
            VM.IsLoading = false;
        }

        private void DeleteSearch(object sender, RoutedEventArgs e) => searchText.Text = string.Empty;

        private void DetectReturn(object sender, KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (e.Key != Key.Enter)
                return;

            SearchAnime(textBox.Text);
            btnSearch.Focus();
            cardCount = 0;
        }

        private void SearchAnime(string text)
        {
            FilmListModel searchedFilmList = null;
            Thread thd = new Thread(() =>
            {
                searchedFilmList = BaseLibraryClass.SearchFilm(text, 50);
                AddCard(searchedFilmList);
            }) { IsBackground = true };
            thd.Start();
        }
        private void LoadMoreAnime(object sender, RoutedEventArgs e)
        {
            if (searchText.Text != null)
                return;
        }

        private void ChangeGenre(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cbx = sender as ComboBox;
            VuigheGenreModel model = cbx.SelectedValue as VuigheGenreModel;

            Thread thd = new Thread(() => {
                FilmListModel filmList = null;

                filmList = BaseLibraryClass.GetFilmList(0, 50, model.Slug);
                cardCount += 50;
                //AddCard(filmList);
            });
            thd.Name = "ChangeGenre";
            thd.Start();
        }

        private void LoadMore(object sender, ScrollChangedEventArgs e)
        {
            if (VM.IsLoading)
                return;

            if (!string.IsNullOrEmpty(searchText.Text))
                return;

            ScrollViewer scroll = sender as ScrollViewer;

            if (scroll.VerticalOffset > scroll.ScrollableHeight - 100)
            {
                Thread thd = new Thread(() =>
                {
                    int cardCount = 0;
                    string genre = string.Empty;
                    Dispatcher.Invoke(() => cardCount = animeCardContainer.Children.Count);
                    Thread.Sleep(10);
                    Dispatcher.Invoke(() => genre = ((VuigheGenreModel)cbxGenre.SelectedItem).Slug);
                    FilmListModel list = BaseLibraryClass.GetFilmList(cardCount, 50, genre);
                    AddCard(list, false);
                })
                { IsBackground = true, Name = "Add More Anime Cards" };
                VM.IsLoading = true;
                thd.Start();
            }
        }   
    }
}