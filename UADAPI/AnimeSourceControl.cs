using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace UADAPI
{
    /// <summary>
    /// Handle anime sources such as download, update, delete,...
    /// </summary>
    public class AnimeSourceControl
    {

        /// <summary>
        /// Init anime control with a anime manager
        /// </summary>
        /// <param name="manager">The mananger use to get this anime series information</param>
        public AnimeSourceControl(IAnimeSeriesManager manager) => CurrentAnimeSeries = manager;

        public string PreferedQuality { get; set; } = "480p";


        /// <summary>
        /// Init a anime control with a manager file
        /// </summary>
        /// <param name="managerFileLocation">The file which store anime series data</param>
        public AnimeSourceControl(string managerFileLocation)
        {
            AnimeSeriesInfo info = JsonConvert.DeserializeObject<AnimeSeriesInfo>(File.ReadAllText(managerFileLocation));

            var manager = ApiHelpper.CreateAnimeSeriesManagerObjectByClassName(info.ModInfo.ModTypeString);
            manager.AttachedAnimeSeriesInfo = info;

            CurrentAnimeSeries = manager;
        }

        /// <summary>
        /// Current manager to do operation
        /// </summary>
        public IAnimeSeriesManager CurrentAnimeSeries { get; set; }

        /// <summary>
        /// Download anime with selected episodes
        /// </summary>
        /// <param name="episodeIndexes">Anime indexes</param>
        /// <param name="isSelective">If user select to download selective this series, return false when download all, download missing episdes, update</param>
        public DownloadInstance DownloadAnimeByIndexes(List<int> episodeIndexes, bool isSelective = true)
        {
            CurrentAnimeSeries.AttachedAnimeSeriesInfo.IsSelectiveDownload = isSelective;
            var instance = DownloadManager.CreateNewDownloadInstance(CurrentAnimeSeries, episodeIndexes, PreferedQuality, true);
            NotificationManager.Add(new NotificationItem() { Title = "Download started!", Detail = $"{instance.AttachedManager.AttachedAnimeSeriesInfo.Name} has been started!. Prefer quality: {instance.PreferedQuality}" });
            instance.FinishedDownloading += FinishedDownloading;
            return instance;
        }



        /// <summary>
        /// Sort hand to download all anime indexes
        /// </summary>
        public void DownloadAll()
        {
            List<int> result = new List<int>();

            foreach (EpisodeInfo item in CurrentAnimeSeries.AttachedAnimeSeriesInfo.Episodes)
            {
                result.Add(item.Index);
            }

            DownloadAnimeByIndexes(result, false);
        }

        /// <summary>
        /// Check for new episodes of this CurrentAnimeSeries
        /// </summary>
        public async Task<bool> Update()
        {
            if (CurrentAnimeSeries.AttachedAnimeSeriesInfo.IsSelectiveDownload || CurrentAnimeSeries.AttachedAnimeSeriesInfo.HasEnded)
            {
                return false;
            }

            await CurrentAnimeSeries.GetPrototypeEpisodes();

            List<int> episodesIndex = new List<int>();
            foreach (EpisodeInfo item in CurrentAnimeSeries.AttachedAnimeSeriesInfo.Episodes)
            {
                if (!item.AvailableOffline)
                {
                    episodesIndex.Add(item.Index);
                    await CurrentAnimeSeries.GetEpisodes(new List<int>() { item.EpisodeID });
                }
            }
            if (episodesIndex.Count != 0)
            {
                DownloadAnimeByIndexes(episodesIndex, false);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Delete episode(s) in the anime series. This will change IsSelectiveDownload Property of the AnimeSeries to true
        /// </summary>
        /// <param name="EpisodeIndexes">Episodes to delete</param>
        public async Task DeleteEpisodes(List<int> id)
        {
            await Task.Run(() =>
            {
                CurrentAnimeSeries.AttachedAnimeSeriesInfo.IsSelectiveDownload = true;

                foreach (int item in id)
                {
                    EpisodeInfo info = CurrentAnimeSeries.AttachedAnimeSeriesInfo.Episodes.Where(query => query.Index == item).ToArray()[0];
                    info.AvailableOffline = false;

                    foreach (var item2 in info.FilmSources.Keys)
                    {
                        if (File.Exists(info.FilmSources[item2].LocalFile.ToString()))
                        {
                            File.Delete(info.FilmSources[item2].LocalFile.ToString());
                        }
                        info.FilmSources[item2].LocalFile = string.Empty;
                    }

                    SerializeAndSaveToFile();
                }
            });
        }

        public async Task DeleteEpisodes(EpisodeInfo info)
        {
            await Task.Run(() =>
            {
                info.AvailableOffline = false;

                foreach (var item2 in info.FilmSources.Keys)
                {
                    if (File.Exists(info.FilmSources[item2].LocalFile.ToString()))
                    {
                        File.Delete(info.FilmSources[item2].LocalFile.ToString());
                    }
                    info.FilmSources[item2].LocalFile = string.Empty;
                }

                SerializeAndSaveToFile();
            });
        }

        /// <summary>
        /// Redownload all missing episodes. This will change IsSelectiveDownload Property of the AnimeSeries to false
        /// </summary>
        public async Task DownloadMissingEpisodes()
        {
            List<int> episodeIndex = new List<int>();
            foreach (EpisodeInfo item in await CurrentAnimeSeries.GetEpisodes())
            {
                if (!item.AvailableOffline)
                {
                    episodeIndex.Add(item.Index);
                }
            }

            DownloadAnimeByIndexes(episodeIndex, false);
        }

        public void SerializeAndSaveToFile()
        {
            string maangerFileContent = JsonConvert.SerializeObject(CurrentAnimeSeries.AttachedAnimeSeriesInfo);
            File.WriteAllText(CurrentAnimeSeries.AttachedAnimeSeriesInfo.ManagerFileLocation, maangerFileContent);
        }


        #region EventHanlers
        private void FinishedDownloading(object sender, EventArgs e)
        {
            var instance = sender as DownloadInstance;
            string episodeName = instance.AttachedManager.AttachedAnimeSeriesInfo.Name;
            string quality = VideoQualityHelper.GetQualityStringFromEnum(instance.PreferedQuality);
            int epCount = instance.EpisodeToDownload.Count;
            int epAllCount = instance.AttachedManager.AttachedAnimeSeriesInfo.Episodes.Count;
            var nof = new NotificationItem()
            {
                Title = $"{episodeName} has finished downloading!",
                Detail = $"{episodeName} has been downloaded. {epCount}/{epAllCount} were requested to download. Preferer quality: {quality}",
                ShowActionButton = false
            };
            NotificationManager.Add(nof);
        }
        #endregion
    }
}
