using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversalAnimeDownloader.ViewModel
{
    public class SettingsPlaybackViewModel : ViewModelBase
    {
        public ObservableCollection<string> Players { get; set; }

        public SettingsPlaybackViewModel()
        {
            Players = new ObservableCollection<string>();
            Players.Add("UAD Player");
            Players.Add("Exthernal Player");
        }
    }
}
