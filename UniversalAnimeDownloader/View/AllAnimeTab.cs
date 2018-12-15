using MaterialDesignThemes.Wpf;
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
    public partial class AllAnimeTab : AnimeList
    {
        public AnimeListViewModel VM;
        private bool isAvaible = true;

        public AllAnimeTab() : base()
        {
            VM = new AnimeListViewModel(Dispatcher);
            DataContext = VM;

            VM.IsHaveConnection = Common.InternetAvaible;

            CreateNoConnectionOverlay();

            //Get the films data collection
            Thread thd = new Thread(() => {
                FilmListModel filmList = new BaseVuigheHost().GetFilmListTaskAsync(0, 50).Result;
                AddCard(filmList);
            });
            thd.Name = "GetDataForThe1stTime";
            thd.Start();

            //Event
            cbxGenre.SelectionChanged += ChangeGenre;
            SearchEvent += (s, ee) => SearchAnime((string)s);
            LoadMoreEvent += (s, ee) => LoadMore(s, ee);
        }

        private void CreateNoConnectionOverlay()
        {
            StackPanel pnl = new StackPanel() { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
            Binding visibilityBinding = new Binding();
            visibilityBinding.Source = VM.IsHaveConnection;
            visibilityBinding.Converter = Application.Current.Resources["boolToInvisConverter"] as IValueConverter;
            visibilityBinding.ConverterParameter = "Inverted";
            visibilityBinding.Mode = BindingMode.OneTime;
            BindingOperations.SetBinding(pnl, VisibilityProperty, visibilityBinding);

            PackIcon packIcon = new PackIcon() { Kind = PackIconKind.WifiOff, Height = 100, Width = double.NaN, Foreground = new SolidColorBrush(Colors.White), HorizontalAlignment = HorizontalAlignment.Center };
            TextBlock textBlock = new TextBlock() { Text = "No Internet Connection :(" };
            textBlock.SetResourceReference(FontSizeProperty, "Heading2");
            pnl.Children.Add(packIcon);
            pnl.Children.Add(textBlock);

            overlayContainer.Children.Add(pnl);
        }

        private void AddCard(FilmListModel filmList, bool removeOldCard = true, string genre = null)
        {
            if (!isAvaible)
                return;

            isAvaible = false;
            //Remove the old anime card
            if (removeOldCard)
                Dispatcher.Invoke(() => animeCardContainer.Children.RemoveRange(0, animeCardContainer.Children.Count));

            if (filmList == null)
            {
                VM.IsLoading = false;
                return;
            }
            VuigheAnimeCard[] cards = null;
            Dispatcher.Invoke(() => cards = new VuigheAnimeCard[filmList.data.Length]);

            Thread.Sleep(10);

            for (int i = 0; i < filmList.data.Length; i++)
            {
                Dispatcher.Invoke(() => {
                    cards[i] = new VuigheAnimeCard();
                    cards[i].Opacity = 0;
                    animeCardContainer.Children.Add(cards[i]);
                    cards[i].AnimeBG = new BitmapImage();
                    cards[i].Data = new VuigheAnimeManager(filmList.data[i]);
                    cards[i].BeginAnimation(OpacityProperty, new DoubleAnimation(1, TimeSpan.FromSeconds(.5)));
                    cards[i].WatchAnimeButtonClicked += (s, e) =>
                    {
                        VuigheAnimeCard animeCard = s as VuigheAnimeCard;
                        AnimeDetailBase animeDetail = new OnlineAnimeDetail(animeCard.Data);
                        FrameHost.Content = animeDetail;
                    };
                });
                Thread.Sleep(10);
            }
            VM.IsLoading = false;
            isAvaible = true;
        }

        private void SearchAnime(string text)
        {
            FilmListModel searchedFilmList = null;
            Thread thd = new Thread(async() =>
            {
                searchedFilmList = await new BaseVuigheHost().SearchFilmTaskAsync(text, 50);
                AddCard(searchedFilmList);
            })
            { IsBackground = true };
            thd.Start();
        }

        private void ChangeGenre(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cbx = sender as ComboBox;
            VuigheGenreModel model = cbx.SelectedValue as VuigheGenreModel;

            Thread thd = new Thread(() => {
                FilmListModel filmList = null;

                filmList = new BaseVuigheHost().GetFilmListTaskAsync(0, 50, model.Slug).Result;
                AddCard(filmList);
            });
            thd.Name = "ChangeGenre";
            thd.Start();
        }

        private void LoadMore(object sender, ScrollChangedEventArgs e)
        {
            if (VM.IsLoading || animeCardContainer.Children.Count == 0)
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
                    FilmListModel list = new BaseVuigheHost().GetFilmListTaskAsync(cardCount, 50, genre).Result;
                    AddCard(list, false);
                })
                { IsBackground = true, Name = "Add More Anime Cards" };
                VM.IsLoading = true;
                thd.Start();
            }
        }
    }
}