
using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Animation;

namespace UniversalAnimeDownloader.UADSettingsPortal
{
    static class UADSettingsManager
    {
        public static void Init()
        {
            if (CurrentSettings == null)
            {
                var settingFolder = AppDomain.CurrentDomain.BaseDirectory + "Settings\\";
                if (!Directory.Exists(settingFolder))
                {
                    Directory.CreateDirectory(settingFolder);
                }

                var settingsFile = settingFolder + "UserSetting.json";
                if (File.Exists(settingsFile))
                {
                    var settingFileContent = File.ReadAllText(settingsFile);
                    var tmp = JsonConvert.DeserializeObject<UADSettingsData>(settingFileContent);
                    CurrentSettings = tmp;
                }
                else
                {
                    CurrentSettings = new UADSettingsData();
                }
                Timeline.DesiredFrameRateProperty.OverrideMetadata(typeof(Timeline), new FrameworkPropertyMetadata { DefaultValue = (int)CurrentSettings.AnimationFrameRate });
            }

        }
        public static UADSettingsData CurrentSettings { get; set; }

    }
}
