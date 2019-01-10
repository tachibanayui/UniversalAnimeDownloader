using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using uadcorelib.Models;
using UniversalAnimeDownloader.ViewModel;

namespace UniversalAnimeDownloader.View
{
    class OfflineAnimeDetail : AnimeDetailBase
    {
        public OfflineAnimeDetailViewModel VM;
        public AnimeInformation OfflineInfo;
        public ListView episodeContainer;

        public OfflineAnimeDetail(AnimeInformation info) : base()
        {
            VM = new OfflineAnimeDetailViewModel(Dispatcher);
            OfflineInfo = info;
            DataContext = VM;

            Title = OfflineInfo.AnimeName;
            GetInfomationAsync();
            AddEpisodeListControl();
        }

        private void AddEpisodeListControl()
        {
            Grid grdRoot = new Grid() { Margin = new Thickness(15) };
            grdRoot.ColumnDefinitions.Add(new ColumnDefinition());
            grdRoot.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50, GridUnitType.Pixel)});

            grdRoot.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50) });
            grdRoot.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(15) });
            grdRoot.RowDefinitions.Add(new RowDefinition());

            TextBlock txbTitle = new TextBlock() { Text = "Episodes: ", FontWeight = FontWeights.Bold };
            txbTitle.SetResourceReference(FontSizeProperty, "Heading");
            grdRoot.Children.Add(txbTitle);

            PopupBox popupBox = new PopupBox();
            Grid.SetColumn(popupBox, 2);
            grdRoot.Children.Add(popupBox);

            episodeContainer = new ListView();
            episodeContainer.Style = Application.Current.Resources["listViewEpsiodesOfflineTemplate"] as Style;
            
            grdRoot.Children.Add(episodeContainer);

            animeEpisodes.Content = grdRoot;

        }

        private async void GetInfomationAsync()
        {
            VM.AnimeTitle = OfflineInfo.AnimeName;
            VM.AnimeDescription = OfflineInfo.Description;
            VM.AnimeGemres = OfflineInfo.AnimeGenres;


            foreach (MediaSource item in OfflineInfo.Episodes)
            {
                EpisodeLibrary castedItem = item as EpisodeLibrary;
                OfflineEpisodesListViewModel episode = new OfflineEpisodesListViewModel();
                episode.EpisodeName = item.EpisodeName;
                episode.IconKind = PackIconKind.Play;
                episode.MediaLocation = item.VideoSource;
                episode.CurrentThumbnail = item.DefaultThumbnailSource;
                VM.AnimeEpisodes.Add(episode);
                await Task.Delay(10);
            }
        }
    }
}
