using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using UADAPI;

namespace UniversalAnimeDownloader.MediaPlayer
{
    class OnlineUADMediaPlayer : UADMediaPlayer
    {
        static OnlineUADMediaPlayer()
        {
            PlayIndexProperty.OverrideMetadata(typeof(OnlineUADMediaPlayer), new PropertyMetadata(ChangeIndexCallback));
        }

        private static async void ChangeIndexCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ins = d as OnlineUADMediaPlayer;
            var newVal = (int)e.NewValue;
            if (newVal == -1)
                return;

            //Base class "UADMediaPlyayer" make Playlist return null when call Reset()
            if (ins.Playlist == null)
                return;

            var currentEpisode = ins.Playlist.Episodes[(int)e.NewValue];

            //if the IAnimeSeriesManager.GetEpisode() is not call yet
            if (currentEpisode.FilmSources == null)
            {
                var manager = ApiHelpper.CreateAnimeSeriesManagerObjectByClassName(ins.Playlist.ModInfo.ModTypeString);
                manager.AttachedAnimeSeriesInfo = ins.Playlist;
                await manager.GetEpisodes(new List<int> { currentEpisode.EpisodeID });
            }
            if(currentEpisode.FilmSources.Count != 0)
            {
                ins.VideoUri = new Uri(currentEpisode.FilmSources.Last().Value.Url.Replace("https","http"));
                var episodeInfo = ins.Playlist.Episodes[(int)e.NewValue];

                ins.AnimeThumbnail = new BitmapImage(new Uri(episodeInfo.Thumbnail.Url));

                ins.Title = ins.Playlist.Name;
                ins.SubbedTitle = episodeInfo.Name;
            }
            else
                ins.PlayIndex++;
        }


        public OnlineUADMediaPlayer() : base() { }


    }
}
