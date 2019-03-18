using System;
using UADAPI;

namespace UniversalAnimeDownloader
{
    class MiscClass
    {
        public static event EventHandler<SearchEventArgs> UserSearched;
        public static void OnUserSearched(object sender, string searchKeyword) => UserSearched?.Invoke(sender, new SearchEventArgs(searchKeyword));

    }


    public class SearchEventArgs : EventArgs
    {
        public SearchEventArgs()
        {

        }

        public SearchEventArgs(string keyword)
        {
            Keyword = keyword;
        }
        public string Keyword { get; set; }
    }
}
