using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using UADAPI;

namespace UniversalAnimeDownloader.Models
{
    public class AnimeInformationModel
    {
        /// <summary>
        /// The name of this anime series
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The ID of this anime series in the remote host
        /// </summary>
        public int AnimeID { get; set; }

        /// <summary>
        /// Current quality of the video after getting information. 
        /// API Dev: Change this inside GetQualities();
        /// </summary>
        public string CurrentQuality { get; set; }

        /// <summary>
        /// If this anime series/season is ended. If true, it wil ignore update check
        /// </summary>
        public bool HasEnded { get; set; }

        /// <summary>
        /// The thumbnail of this anime series
        /// </summary>
        public ImageSource ThumbnailSource { get; set; }

        /// <summary>
        /// The genres of this anime
        /// </summary>
        public List<GenreItem> Genres { get; set; }

        /// <summary>
        /// Indicate if the user not download all the episdes of this anime series/season, true : will ignore update check
        /// </summary>
        public bool IsSelectiveDownload { get; set; }

        /// <summary>
        /// The description of this anime series
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The epsides of this anime series. Only save the source
        /// </summary>
        //public List<EpisodeInfo> Episodes { get; set; }

        /// <summary>
        /// Indicate wether the episodes got don't have enough detail
        /// </summary>
        public bool IsPrototypeEpisdodes { get; set; }

        /// <summary>
        /// The extractor used to get this anime. Use to invoke the extractor in manager file
        /// </summary>
        public ModificatorInformation ModInfo { get; set; }

        /// <summary>
        /// Where is the Manager file saved location
        /// </summary>
        public string ManagerFileLocation { get; set; }

        /// <summary>
        /// The directory of this anime series
        /// </summary>
        public string AnimeSeriesSavedDirectory { get; set; }
    }
}
