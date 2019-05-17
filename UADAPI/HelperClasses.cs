using Newtonsoft.Json;
using SegmentDownloader.Core;
using SegmentDownloader.Protocol;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UADAPI.PlatformSpecific;
using ErrorEventArgs = Newtonsoft.Json.Serialization.ErrorEventArgs;
using ResourceLocation = SegmentDownloader.Core.ResourceLocation;

namespace UADAPI
{

    /// <summary>
    /// Contain information abuot the modification or extractor
    /// </summary>
    public class ModificatorInformation
    {
        /// <summary>
        /// Create this type with modification name and extractor type
        /// </summary>
        /// <param name="name">The name of this type</param>
        /// <param name="currentType">The type of this extractor, pass in this.GetType() for simpilcity</param>
        public ModificatorInformation(string name, Type currentType)
        {
            ModName = name;
            ModTypeString = currentType.FullName;
        }

        public ModificatorInformation()
        {

        }

        /// <summary>
        /// Your modidication name, this will be displayed when user select extractor mod
        /// </summary>
        public string ModName { get; set; }

        /// <summary>
        /// Your modifiaction version.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Your modification description, will be displayed when user select modification
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The type of your extractor, use ModType = GetType() if you don't know much about refection
        /// </summary>
        //public Type ModType { get => string.IsNullOrEmpty(ModTypeString) ? Type.GetType(ModTypeString) : null; set => ModTypeString = value != null ? value.FullName : string.Empty; }

        /// <summary>
        /// Use to deserialize
        /// </summary>
        public string ModTypeString { get; set; }
    }

    public class ModGenresInterests
    {
        public string QueryModFullName { get; set; }
        public List<GenreItemInterest> GenresInterest { get; set; }

        public List<GenreItem> RecentDownloads { get; set; } = new List<GenreItem>();
    }

    public class GenreItemInterest
    {
        public GenreItem Genre { get; set; }
        public int DownloadCount { get; set; }
    }

    /// <summary>
    /// This class is provide option for UAD. 
    /// </summary>
    public class ApiHelpper
    {
        /// <summary>
        /// Indicate if the modification binary file (*.dll) is loaded
        /// </summary>
        public static bool IsLoadedAssembly { get; set; }

        /// <summary>
        /// Information about this class/modificator
        /// </summary>
        public static ModificatorInformation ModInfo
        {
            get
            {
                return new ModificatorInformation("BaseAPI", typeof(ApiHelpper))
                {
                    Description = "This is a base modification to comunicate between UAD itself and all the custom modification",
                    Version = "v1.0"
                };
            }
        }

        /// <summary>
        /// The release date of this mods
        /// </summary>
        public static DateTime VerionReleaseDate { get; set; }

        /// <summary>
        /// Contain all types which implement IQueryAnimeSeries
        /// </summary>
        public static List<Type> QueryTypes { get; set; } = new List<Type>();

        /// <summary>
        /// Contain all types which implement IAnimeSeriesManager
        /// </summary>
        public static List<Type> ManagerTypes { get; set; } = new List<Type>();

        /// <summary>
        /// Create a instance by class name that implement this interface IAnimeSeriesManager
        /// </summary>
        /// <param name="className">The name of the class</param>
        public static IAnimeSeriesManager CreateAnimeSeriesManagerObjectByClassName(string className)
        {
            if (!IsLoadedAssembly)
            {
                LoadAssembly();
            }

            var queryRes = ManagerTypes.Where(query => className.Contains(className)).ToList();
            if (queryRes.Count != 0)
            {
                return Activator.CreateInstance(queryRes[0]) as IAnimeSeriesManager;
            }
            else
            {
                return null;
            }
        }


        public static IAnimeSeriesManager CreateAnimeSeriesManagerObjectByType(Type type)
        {
            if (!IsLoadedAssembly)
            {
                LoadAssembly();
            }

            var queryRes = ManagerTypes.Where(query => query == type).ToList();
            if (queryRes.Count() != 0)
            {
                return Activator.CreateInstance(queryRes[0]) as IAnimeSeriesManager;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Create a instance by class name that implement this interface IQueryAnimeSeries
        /// </summary>
        /// <param name="className">The name of the class</param>
        public static IQueryAnimeSeries CreateQueryAnimeObjectByClassName(string className)
        {
            if (!IsLoadedAssembly)
            {
                LoadAssembly();
            }

            var queryRes = QueryTypes.Where(
                query => className.Contains(query.FullName)).ToList();
            if (queryRes.Count != 0)
            {
                return Activator.CreateInstance(queryRes[0]) as IQueryAnimeSeries;
            }
            else
            {
                return null;
            }
        }

        public static IQueryAnimeSeries CreateQueryAnimeObjectByType(Type type)
        {
            if (!IsLoadedAssembly)
            {
                LoadAssembly();
            }

            var queryRes = QueryTypes.Where(query => query == type).ToList();
            if (queryRes.Count() != 0)
            {
                return Activator.CreateInstance(queryRes[0]) as IQueryAnimeSeries;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Load the mod in the mod folder
        /// </summary>
        /// <param name="checkForUpdate">How to check for update, null will use the default way (PC, WPF version)</param>
        public static void LoadAssembly(CheckForUpdateAction checkForUpdate = null)
        {
            if (IsLoadedAssembly)
                return;

            bool updateAvaible = checkForUpdate == null ? CheckForUpdates() : checkForUpdate.Invoke();
            if (updateAvaible)
            {
                //throw new InvalidOperationException("This modification is out of date, update in our GitHub release!");
                IsLoadedAssembly = true;
                return;
            }

            string modDirectory = AppDomain.CurrentDomain.BaseDirectory + "Mods" + "\\";
            List<Assembly> modAssemblies = new List<Assembly>();

            if (!Directory.Exists(modDirectory))
            {
                Directory.CreateDirectory(modDirectory);
            }

            foreach (string item in Directory.EnumerateFiles(modDirectory))
            {
                try
                {
                    if (Path.GetExtension(item).Contains("dll"))
                    {
                        modAssemblies.Add(Assembly.LoadFrom(item));
                    }
                }
                catch { }
            }

            IsLoadedAssembly = true;

            for (int i = 0; i < modAssemblies.Count; i++)
            {
                try
                {
                    var assemblyTypes = modAssemblies[i].ExportedTypes;
                    ManagerTypes.AddRange(assemblyTypes.Where(SearchForManagerTypes));
                    QueryTypes.AddRange(assemblyTypes.Where(SearchForQueryTypes));
                }
                catch (Exception e) { Console.WriteLine(e); }
            }

            ExamineMods();
        }

        private static bool SearchForManagerTypes(Type arg)
        {
            foreach (var item in arg.GetInterfaces())
            {
                if (item.FullName == typeof(IAnimeSeriesManager).FullName)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool SearchForQueryTypes(Type arg)
        {
            foreach (var item in arg.GetInterfaces())
            {
                if (item.FullName == typeof(IQueryAnimeSeries).FullName)
                {
                    return true;
                }
            }
            return false;
        }

        static ApiHelpper()
        {
            _CheckInternetThread = new Thread(async () =>
            {
                while (true)
                {
                    _IsCheckForInternetOnGoing = true;
                    _CheckForInternetTask = Task.Run(() =>
                    {
                        try
                        {
                            using (var client = new WebClient())
                            using (client.OpenRead("http://clients3.google.com/generate_204"))
                            {
                                return true;
                            }
                        }
                        catch
                        {
                            return false;
                        }
                    });

                    await Task.WhenAny(_CheckForInternetTask);
                    _InternetAvaible = await _CheckForInternetTask;
                    _IsCheckForInternetOnGoing = false;

                    await Task.Delay(30000);
                }
            })
            { IsBackground = true, Name = "Check for internet thread", Priority = ThreadPriority.BelowNormal };
            _CheckInternetThread.Start();
        }

        /// <summary>
        /// Not implemented yet!
        /// </summary>
        private static void ExamineMods()
        {
        }

        private static string AccessToken = "5eba8827aa833401a98f06b9e367aede67444d44";
        public static string SourceVersions { get; set; } = $"https://api.github.com/repos/quangaming2929/UniversalAnimeDownloader/releases/latest";
        public static GitHubData NewestVersionInfo { get; set; }


        /// <summary>
        /// Check for updates on GitHubs
        /// </summary>
        public static bool CheckForUpdates()
        {
            return false;

            try
            {
                var version =  GetLatestUpdate(AccessToken);
                var versionFileContent = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "VersionManager.json"));
                var versionManager = JsonConvert.DeserializeObject<VersionManager>(versionFileContent);
                NewestVersionInfo = version;
                return CompareVersion(version.tag_name, versionManager.GlobalVersion);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Check for updates on Github as async operation
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> CheckForUpdateAsyncTask() => await Task.Run(() => CheckForUpdates());

        /// <summary>
        /// Return true if the v1 is greater than v2
        /// </summary>
        /// <param name="tag_name"></param>
        /// <returns></returns>
        private static bool CompareVersion(string v1, string v2)
        {
            var newVer = v1.Substring(1).Split('.');
            var oldVer = v2.Substring(1).Split('.');

            for (int i = 0; i < Math.Min(newVer.Length, oldVer.Length); i++)
            {
                if (int.Parse(newVer[i]) > int.Parse(oldVer[i]))
                    return true;

                if (int.Parse(newVer[i]) < int.Parse(oldVer[i]))
                    return false;
            }
            return false;
        }

        public static GitHubData GetLatestUpdate(string accessToken = null)
        {
            string apiResult = string.Empty;
            string req = SourceVersions;
            if (!string.IsNullOrEmpty(accessToken))
            {
                req += $"?access_token={accessToken}";
            }
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(SourceVersions);
                request.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/64.0.3282.140 Safari/537.36 Edge/17.17134";
                using (WebResponse resp = request.GetResponse())
                {
                    using (Stream stream = resp.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream);
                        apiResult = reader.ReadToEnd();
                    }
                }
            }
            catch
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(req);
                    request.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/64.0.3282.140 Safari/537.36 Edge/17.17134";
                    using (WebResponse resp = request.GetResponse())
                    {
                        using (Stream stream = resp.GetResponseStream())
                        {
                            StreamReader reader = new StreamReader(stream);
                            apiResult = reader.ReadToEnd();
                        }
                    }
                }
                catch (Exception e)
                {
                    throw new InvalidOperationException("Can't get latest version!", e);
                }
            }

            return JsonConvert.DeserializeObject<GitHubData>(apiResult);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns>Is internet connection avaible</returns>
        public static async Task<bool> CheckForInternetConnection()
        {
            if (_IsCheckForInternetOnGoing)
                return await _CheckForInternetTask;
            else
                return _InternetAvaible;
        }

        private static bool _InternetAvaible = true;
        private static bool _IsCheckForInternetOnGoing = false;
        private static Task<bool> _CheckForInternetTask;
        private static Thread _CheckInternetThread = null;
    }

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

    public class VersionManager
    {
        public List<FileVersion> FileVersion { get; set; } = new List<FileVersion>();
        public string GlobalVersion { get; set; }
    }

    public class FileVersion
    {
        public string FileName { get; set; }
        public int Version { get; set; }
    }

    public delegate bool CheckForUpdateAction();
}
