using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UADAPI;
using UniversalAnimeDownloader.Models;

namespace UniversalAnimeDownloader.ViewModels
{
    public class AllAnimeTabViewModel : BaseViewModel
    {
        public ObservableCollection<AnimeInformationModel> AnimeInfos { get; set; }
        public AllAnimeTabViewModel()
        {
            AnimeInfos = new ObservableCollection<AnimeInformationModel>();
            AnimeInfos.Add(new AnimeInformationModel() { Name = "Cancer Test", Description = "This is too cancer", Genres = new List<GenreItem>() { new GenreItem() { Name = "Cancer" } } });
        }
    }
}
