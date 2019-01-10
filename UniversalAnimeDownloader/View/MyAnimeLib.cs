using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using uadcorelib.Models;
using UniversalAnimeDownloader.CustomControl;
using UniversalAnimeDownloader.Settings;

namespace UniversalAnimeDownloader.View
{
    class MyAnimeLib : AnimeList
    {
        public MyAnimeLib() : base()
        {
            progressIndicator.Visibility = Visibility.Collapsed;

            AddAnimeCardAsync();
            Title = "My anime library";
        }

        private async void AddAnimeCardAsync()
        {
            string animeLibDir = SettingsManager.Current.AnimeLibraryDirectory;
            
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

            if(animeCardContainer.Children.Count == 0)
                ShowsNothingToShow();
        }

        private void ShowsNothingToShow()
        {
            StackPanel stackPanel = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
            PackIcon packIcon = new PackIcon { Kind = PackIconKind.EmoticonSad, Width = 100, Height = double.NaN};
            packIcon.SetResourceReference(ForegroundProperty, "ForeGroundColor");
            stackPanel.Children.Add(packIcon);
            TextBlock textBlock = new TextBlock { Text = "Nothing to show here", HorizontalAlignment = HorizontalAlignment.Center};
            textBlock.SetResourceReference(FontSizeProperty, "Heading");
            stackPanel.Children.Add(textBlock);
            TextBlock txblDescription = new TextBlock { Text = "Your downloaded anime will show here, so go and download some", HorizontalAlignment = HorizontalAlignment.Center };
            txblDescription.SetResourceReference(FontSizeProperty, "Heading2");
            stackPanel.Children.Add(txblDescription);
            ((animeCardContainer.Parent as StackPanel).Parent as Grid).Children.Add(stackPanel);
        }
    }
}
