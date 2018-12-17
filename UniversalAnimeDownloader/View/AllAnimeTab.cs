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
            InitializingFilmList();
        }

        private async void InitializingFilmList()
        {
            FilmListModel filmList = await new BaseVuigheHost().GetFilmListTaskAsync(0, 50);
            AddCard(filmList);

            //Event
            cbxGenre.SelectionChanged += ChangeGenre;
            SearchEvent += (s, ee) => SearchAnime((string)s);
            LoadMoreEvent += (s, ee) => LoadMore(s, ee);
        }

        /// <summary>
        /// Code behind version for No Connection overlay used to implement in xaml
        /// </summary>
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

        private async void AddCard(FilmListModel filmList, bool removeOldCard = true, string genre = null)
        {
            if (!isAvaible)
                return;

            isAvaible = false;
            //Remove the old anime card
            if (removeOldCard)
                animeCardContainer.Children.RemoveRange(0, animeCardContainer.Children.Count);

            if (filmList == null)
            {
                VM.IsLoading = false;
                return;
            }
            VuigheAnimeCard[] cards = null;
            cards = new VuigheAnimeCard[filmList.data.Length];

            await Task.Delay(10);

            for (int i = 0; i < filmList.data.Length; i++)
            {
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
                await Task.Delay(10);
            }
            VM.IsLoading = false;
            isAvaible = true;
        }

        private async void SearchAnime(string text)
        {
            FilmListModel searchedFilmList = null;
            searchedFilmList = await new BaseVuigheHost().SearchFilmTaskAsync(text, 50);
            AddCard(searchedFilmList);
        }

        private async void ChangeGenre(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cbx = sender as ComboBox;
            VuigheGenreModel model = cbx.SelectedValue as VuigheGenreModel;

            FilmListModel filmList = null;

            filmList = await new BaseVuigheHost().GetFilmListTaskAsync(0, 50, model.Slug);
            AddCard(filmList);
        }

        private async void LoadMore(object sender, ScrollChangedEventArgs e)
        {
            if (VM.IsLoading || animeCardContainer.Children.Count == 0)
                return;

            if (!string.IsNullOrEmpty(searchText.Text))
                return;

            ScrollViewer scroll = sender as ScrollViewer;

            if (scroll.VerticalOffset > scroll.ScrollableHeight - 100)
            {
                int cardCount = 0;
                string genre = string.Empty;
                cardCount = animeCardContainer.Children.Count;
                await Task.Delay(10);
                genre = ((VuigheGenreModel)cbxGenre.SelectedItem).Slug;
                FilmListModel list = await new BaseVuigheHost().GetFilmListTaskAsync(cardCount, 50, genre);
                AddCard(list, false);
                VM.IsLoading = true;
            }
        }
    }
}
