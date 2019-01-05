using System;
using System.Collections.Generic;
using System.IO;
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
            string animeLibDir = AppDomain.CurrentDomain.BaseDirectory + "AnimeLibrary";
            
            //Check if the animeLibDir exist
            if (!Directory.Exists(animeLibDir))
                Directory.CreateDirectory(animeLibDir);

            //animeCardContainer
            foreach (AnimeInformation item in AnimeInformation.GetAnimeInformationFromLib(animeLibDir))
            {
                VuigheAnimeCard card = new VuigheAnimeCard();
                card.Opacity = 0;
                animeCardContainer.Children.Add(card);
                card.AnimeBG = new BitmapImage();
                card.OfflineData = item;
                card.BeginAnimation(OpacityProperty, new DoubleAnimation(1, TimeSpan.FromSeconds(.5)));
                card.WatchAnimeButtonClicked += (s, e) =>
                {
                    VuigheAnimeCard vuigheCard = s as VuigheAnimeCard;
                    OfflineAnimeDetail offlineAnime = new OfflineAnimeDetail(vuigheCard.OfflineData) { HostFrame = FrameHost };
                    FrameHost.Content = offlineAnime;
                };
                await Task.Delay(20);
            }
        }
    }
}
