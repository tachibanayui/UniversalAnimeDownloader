using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace UADAPI
{
    class PlaceholderManager : IAnimeSeriesManager
    {
        public AnimeSeriesInfo AttachedAnimeSeriesInfo { get; set; }
        public ModificatorInformation ModInfo { get; set; }
        public ModificatorInformation RelativeQueryInfo { get; set; }
        public List<string> Qualities { get; set; }

        public PlaceholderManager()
        {
            ModInfo = new ModificatorInformation("PlaceholderManager", typeof(PlaceholderManager)) { Description = "A place holder contains fake information getter. Use for custom anime series"};
            RelativeQueryInfo = new ModificatorInformation();
        }

        public async Task<MediaSourceInfo> GetCommonQuality(Dictionary<VideoQuality, MediaSourceInfo> filmSources, string requestedQuality)
        {
            return new MediaSourceInfo();
        }

        public async Task<EpisodeInfo> GetEpisodeByID(int id)
        {
            return new EpisodeInfo();
        }

        public async Task<IEnumerable<EpisodeInfo>> GetEpisodes(List<int> id = null)
        {
            return new List<EpisodeInfo>();
        }

        public async Task<IEnumerable<EpisodeInfo>> GetPrototypeEpisodes()
        {
            return new List<EpisodeInfo>();
        }

        public async Task<List<VideoQuality>> GetQualities()
        {
            return new List<VideoQuality>();
        }
    }
}
