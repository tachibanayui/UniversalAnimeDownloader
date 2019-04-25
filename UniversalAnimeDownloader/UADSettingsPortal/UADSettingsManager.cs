
using Newtonsoft.Json;
using System;
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
    }
}
