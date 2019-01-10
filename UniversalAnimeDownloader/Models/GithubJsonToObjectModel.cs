using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversalAnimeDownloader.Models
{
    public class GitHubData
    {
        public string tag_name { get; set; }
        public string name { get; set; }
        public Asset[] assets { get; set; }
        public string body { get; set; }
    }

    public class Asset
    {
        public string name { get; set; }
        public int size { get; set; }
        public DateTime updated_at { get; set; }
        public string browser_download_url { get; set; }
    }
}
