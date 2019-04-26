
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using UniversalAnimeDownloader.ViewModels;

namespace UniversalAnimeDownloader.UADSettingsPortal
{
    class UADSettingsManager : BaseViewModel
    {
        public UADSettingsManager()
        {
            Init();
        }

        public void Init()
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

        private UADSettingsData _CurrentSettings;
        public UADSettingsData CurrentSettings
        {
            get => _CurrentSettings;
            set
            {
                if (_CurrentSettings != value)
                {
                    _CurrentSettings = value;
                    OnPropertyChanged();
                }
            }
        }

        public static void ResetCurrentSettings()
        {
            string settingFileLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings", "UserSetting.json");
            string newSettingFileContent = JsonConvert.SerializeObject(new UADSettingsData(), MiscClass.IgnoreConverterErrorJson);
            File.WriteAllText(settingFileLocation, newSettingFileContent);

            //Restart UAD
            string batchContent = "timeout /t 3 /nobreak\r\nstart UniversalAnimeDownloader.exe";
            File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UADScript.bat"), batchContent);
            Process process = new Process();
            process.StartInfo = new ProcessStartInfo()
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                FileName = "cmd.exe",
                Arguments = $"/C start UADScript.bat"
            };
            process.Start();
            Application.Current.Shutdown();
        }
    }
}
