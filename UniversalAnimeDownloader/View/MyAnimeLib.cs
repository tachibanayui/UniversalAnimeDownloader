using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using uadcorelib.Models;
using UniversalAnimeDownloader.CustomControl;

namespace UniversalAnimeDownloader.View
{
    class MyAnimeLib : AnimeList
    {
        public MyAnimeLib() : base()
        {
            progressIndicator.Visibility = Visibility.Collapsed;

            AddAnimeCardAsync();
        }

        private async void AddAnimeCardAsync()
        {

            //animeCardContainer
            foreach (AnimeInformation item in AnimeInformation.GetAnimeInformationFromLib(AppDomain.CurrentDomain.BaseDirectory + "AnimeLibrary"))
            {
                Dispatcher.Invoke(() => {
                    VuigheAnimeCard card = new VuigheAnimeCard();
                    card.Opacity = 0;
                    animeCardContainer.Children.Add(card);
                    card.AnimeBG = new BitmapImage();
                    card.OfflineData = item;
                    card.BeginAnimation(OpacityProperty, new DoubleAnimation(1, TimeSpan.FromSeconds(.5)));
                    card.WatchAnimeButtonClicked += (s, e) =>
                    {
                        MessageBox.Show("Sorry, not implemented yet :(");
                    };
                });
                await Task.Delay(20);
            }
        }
    }
}
