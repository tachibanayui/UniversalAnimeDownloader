using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            //Migrate Settings Checker
            if(Current.SettingsVersion < 1)
            {
                //Do something
            }
        }

        public static void ResetCurrentSettings()
        {
            File.Delete("Settings\\CurrentSettings.json");
            string restartBatchFileContent = "start UniversalAnimeDownloader.exe";
            File.WriteAllText("RestartUAD.bat", restartBatchFileContent);
            System.Windows.Application.Current.Shutdown();
            ProcessStartInfo processStartInfo = new ProcessStartInfo("RestartUAD.bat");
            processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            Process.Start(processStartInfo);
        }
    }
}
