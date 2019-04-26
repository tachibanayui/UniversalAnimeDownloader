using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UADAPI;

namespace UniversalAnimeDownloader.Models
{
    class AffectedAnimesModel
    {
        public string ThumbnailLocation { get; set; }
        public string Name { get; set; }
        public string ManagerFileLocation { get; set; }
        public string FolderName { get; set; }

        public AffectedAnimesModel(string managerFileLocation)
        {
            ManagerFileLocation = managerFileLocation;
            var info = JsonConvert.DeserializeObject<AnimeSeriesInfo>(File.ReadAllText(managerFileLocation), MiscClass.IgnoreConverterErrorJson);
            Name = info.Name;
            if (!string.IsNullOrEmpty(info.Thumbnail.LocalFile))
                ThumbnailLocation = info.Thumbnail.LocalFile;
            else
                ThumbnailLocation = info.Thumbnail.Url;
            FolderName = new DirectoryInfo(info.AnimeSeriesSavedDirectory).Name;
        }
    }
}
