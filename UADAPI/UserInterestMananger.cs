using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ErrorEventArgs = Newtonsoft.Json.Serialization.ErrorEventArgs;

namespace UADAPI
{
    public class UserInterestMananger
    {
        public static UserInterestData Data { get; set; } = new UserInterestData();

        public static async Task AddInterest(string queryModFullname, GenreItem genre)
        {
            if (string.IsNullOrEmpty(queryModFullname))
                return;

            //Check if the mod is already define in the list
            var mod = Data.UserInterest.FirstOrDefault(f => f.QueryModFullName == queryModFullname);
            if(mod == null)
            {
                mod = new ModGenresInterests();
                mod.QueryModFullName = queryModFullname;
                mod.GenresInterest = new List<GenreItemInterest>();
                var querier = ApiHelpper.CreateQueryAnimeObjectByClassName(queryModFullname);
                var genres = await querier.GetAnimeGenres();
                foreach (var item in genres)
                    mod.GenresInterest.Add(new GenreItemInterest() { Genre = item, DownloadCount = 0 });

                Data.UserInterest.Add(mod);
            }

            //check if the last download genres is not the same as this download
            if(mod.RecentDownloads.Count > 0)
            {
                mod.RecentDownloads.Add(mod.RecentDownloads[mod.RecentDownloads.Count - 1]);
                for (int i = mod.RecentDownloads.Count - 2; i > 0; i--)
                {
                    mod.RecentDownloads[i] = mod.RecentDownloads[i - 1];
                }
                mod.RecentDownloads[0] = genre;
                if (mod.RecentDownloads.Count > 15)
                    mod.RecentDownloads = mod.RecentDownloads.GetRange(0, 15);
            }
            else
                mod.RecentDownloads.Add(genre);

            var interest = mod.GenresInterest.FirstOrDefault(f => f.Genre.Name == genre.Name);
            //Add if the interest in not found in our list
            if(interest == null)
            {
                var newInterest = new GenreItemInterest();
                newInterest.Genre = new GenreItem() { Name = genre.Name, ID = genre.ID, Slug = genre.Slug };
                newInterest.DownloadCount = 0;

                mod.GenresInterest.Add(newInterest);
                interest = newInterest;
            }

            interest.DownloadCount++;
            Data.LastSuggestion = null;
        }

        public static async Task<List<AnimeSeriesInfo>> GetSuggestion(string queryModFullname, int offset, int count)
        {
            var mod = Data.UserInterest.FirstOrDefault(f => f.QueryModFullName == queryModFullname);
            var querier = ApiHelpper.CreateQueryAnimeObjectByClassName(queryModFullname);
            var rand = new Random();
            if(mod == null)
            {
                int clacFrom = offset;
                var max = await querier.GetSearchLimit(string.Empty, false);
                if (max > 0)
                    clacFrom = clacFrom * rand.Next(1, 100) % max;

                var ress = await querier.GetAnime(clacFrom, count);
                Data.LastSuggestion = ress;
                return ress;
            }

            List<GenreItemInterest> interest = new List<GenreItemInterest>();

            //Get total genres download
            int totaGenreDownload = 0;
            for (int i = 0; i < mod.GenresInterest.Count; i++)
            {
                totaGenreDownload += mod.GenresInterest[i].DownloadCount;
                var thisGenres = mod.GenresInterest[i].Genre;
                interest.Add(new GenreItemInterest() { Genre = new GenreItem() { Name = thisGenres.Name, ID = thisGenres.ID, Slug = thisGenres.Slug}});
            }

            //Calcuate pool quantites
            for (int i = 0; i < interest.Count; i++)
            {
                int bonus = 0;
                for (int j = 0; j < mod.RecentDownloads.Count; j++)
                {
                    if (mod.RecentDownloads[j] != null)
                    {
                        if (mod.RecentDownloads[j].Name == interest[i].Genre.Name)
                        {
                            bonus = 15 - j;
                            break;
                        }
                    }
                }

                interest[i].DownloadCount = (int)Math.Ceiling((mod.GenresInterest[i].DownloadCount / (double)totaGenreDownload + bonus) / 10d);
            }


            List<AnimeSeriesInfo> info = new List<AnimeSeriesInfo>();
            int retryCount = 0;
            //Get the anime series
            while(info.Count < count && retryCount < 3)
            {
                try
                {
                    for (int i = 0; i < interest.Count; i++)
                    {
                        //get the anime pool
                        var calcFrom = offset + 1;
                        var poolCount = interest[i].DownloadCount * 10;

                        if(poolCount != 0)
                        {
                            var maxOffset = await querier.GetSearchLimit(interest[i].Genre.Slug, false);
                            if (maxOffset > 0)
                                calcFrom = calcFrom * rand.Next(1, 100) % (maxOffset - count);
                            var pool = await querier.GetAnime(calcFrom, poolCount, string.Empty, interest[i].Genre.Slug);
                            for (int j = 0; j < poolCount / 10; j++)
                            {
                                AnimeSeriesInfo first = null;
                                AnimeSeriesInfo second = null;
                                AnimeSeriesInfo third = null;

                                for (int k = j * 10; k < (j + 1) * 10; k++)
                                {
                                    if (first == null)
                                        first = pool[k];
                                    else if (first.Views < pool[k].Views)
                                    {
                                        third = second;
                                        second = first;
                                        first = pool[k];
                                    }
                                    else if (second == null)
                                        second = pool[k];
                                    else if (second.Views < pool[k].Views)
                                    {
                                        third = second;
                                        second = pool[k];
                                    }
                                    else if (third == null)
                                        third = pool[k];
                                    else if (third.Views < pool[k].Views)
                                        third = pool[k];
                                }

                                AnimeSeriesInfo choosen = null;

                                switch (rand.Next(1, 3))
                                {
                                    case 1:
                                        choosen = first;
                                        break;
                                    case 2:
                                        choosen = second;
                                        break;
                                    default:
                                        choosen = third;
                                        break;
                                }

                                if (info.FindIndex(p => p.Name == choosen.Name) == -1)
                                    info.Add(choosen);
                            }
                        }
                    }
                }
                catch
                {
                    retryCount++;
                }
            }
            var res = info.Shuffle().ToList();

            Data.LastSuggestion = res;

            if (res.Count > count)
                return res.GetRange(0, count);
            else
                return res;

        }

        public static string Serialize() => JsonConvert.SerializeObject(Data);

        public static async Task Deserialize(string val)
        {
            await Task.Run(() =>
            {
                var jsonSetting = new JsonSerializerSettings()
                {
                    Error = new EventHandler<ErrorEventArgs>((s, e) => e.ErrorContext.Handled = true),
                    TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full,
                };

                if (!string.IsNullOrEmpty(val))
                {
                    var tmp = JsonConvert.DeserializeObject<UserInterestData>(val, jsonSetting);
                    Data = tmp;
                }
            });
        }
    }
}
