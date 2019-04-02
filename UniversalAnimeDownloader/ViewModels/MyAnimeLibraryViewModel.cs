using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using UADAPI;

using ErrorEventArgs = Newtonsoft.Json.Serialization.ErrorEventArgs;

namespace UniversalAnimeDownloader.ViewModels
{
    class MyAnimeLibraryViewModel : BaseViewModel, IPageContent
    {
        #region Commmands
        public ICommand ReloadAnimeCommand { get; set; }
        public ICommand ShowAnimeDetailCommand { get; set; }
        #endregion


        public DelayedObservableCollection<AnimeSeriesInfo> AnimeLibrary { get; set; } = new DelayedObservableCollection<AnimeSeriesInfo>();
        public List<AnimeSeriesInfo> NoDelayAnimeLib { get; } = new List<AnimeSeriesInfo>();

        public MyAnimeLibraryViewModel()
        {
            ReloadAnimeLibrary(true);
            ReloadAnimeLibrary(false);

            ReloadAnimeCommand = new RelayCommand<object>(p => true, async p => await ReloadAnimeLibrary(false));
            ShowAnimeDetailCommand = new RelayCommand<AnimeSeriesInfo>(p => true, p =>
            {
                MiscClass.NavigationHelper.AddNavigationHistory(4);
                IAnimeSeriesManager manager = ApiHelpper.CreateAnimeSeriesManagerObjectByClassName(p.ModInfo.ModTypeString);
                manager.AttachedAnimeSeriesInfo = p;
                (Application.Current.FindResource("OfflineAnimeDetailViewModel") as OfflineAnimeDetailViewModel).CurrentSeries = manager;
            });
        }

        private async Task ReloadAnimeLibrary(bool isRealtimeList)
        {
            AnimeLibrary.Clear();
            string lib = AppDomain.CurrentDomain.BaseDirectory + "Anime Library\\";
            foreach (var item in Directory.EnumerateDirectories(lib))
            {
                if(File.Exists(item + "\\Manager.json"))
                {
                    string content = File.ReadAllText(item + "\\Manager.json");
                    var jsonSetting = new JsonSerializerSettings()
                    {
                        Error = new EventHandler<ErrorEventArgs>((s, e) => e.ErrorContext.Handled = true),
                        TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full,
                    };
                    var info = JsonConvert.DeserializeObject<AnimeSeriesInfo>(content, jsonSetting);
                    if (isRealtimeList)
                        NoDelayAnimeLib.Add(info);
                    else
                        await AnimeLibrary.AddAndWait(info);
                }
            }
            if(!isRealtimeList)
            {
                NoDelayAnimeLib.Clear();
                NoDelayAnimeLib.AddRange(AnimeLibrary);
            }
        }

        public void OnShow()
        {
        }

        public void OnHide()
        {
            
        }
    }
}
