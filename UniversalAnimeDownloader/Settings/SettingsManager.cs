using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace UniversalAnimeDownloader.Settings
{
    public class SettingsManager
    {
        public static SettingValues Default { get; } = new SettingValues() { SettingName = "Default.json"};
        public static SettingValues Current { get; set; }

        //Not Implemented Yet
        public static List<SettingValues> Profiles { get; set; }

        public SettingsManager()
        {
            if (!Directory.Exists("Settings"))
                Directory.CreateDirectory("Settings");
            try
            {
                string currentSettingContent = File.ReadAllText("Settings\\CurrentSettings.json");
                Current = JsonConvert.DeserializeObject<SettingValues>(currentSettingContent);
            }
            catch
            {
                Current = Default;
                Current.SettingName = "CurrentSettings.json";
                string saveContent = JsonConvert.SerializeObject(Current);
                File.WriteAllText("Settings\\CurrentSettings.json", saveContent);
            }
        }
    }
}
