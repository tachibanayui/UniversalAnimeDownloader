using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace UniversalAnimeDownloader.Settings
{
    public class SettingValues
    {
        public string SettingName { get; set; }

        #region PlayblackSetting
        private PlayerType preferedPlayer = PlayerType.Embeded;
        public PlayerType PreferedPlayer
        {
            get { return preferedPlayer; }
            set
            {
                if (preferedPlayer != value)
                {
                    preferedPlayer = value;
                    UpdateSetting();
                }
            }
        }

        private bool playMediaFullScreen = true;
        public bool PlayMediaFullScreen
        {
            get { return playMediaFullScreen; }
            set
            {
                if (playMediaFullScreen != value)
                {
                    playMediaFullScreen = value;
                    UpdateSetting();
                }
            }
        }

        private double playbackVolume = 100;
        public double PlaybackVolume
        {
            get { return playbackVolume; }
            set
            {
                if (playbackVolume != value)
                {
                    playbackVolume = value;
                    UpdateSetting();
                }
            }
        }

        private bool isDrawingEnabled = true;
        public bool IsDrawingEnabled
        {
            get { return isDrawingEnabled; }
            set
            {
                if (isDrawingEnabled != value)
                {
                    isDrawingEnabled = value;
                    UpdateSetting();
                }
            }
        }

        private bool isSneakyWatherEnabled = true;
        public bool IsSneakyWatcherEnabled
        {
            get { return isSneakyWatherEnabled; }
            set
            {
                if (isDrawingEnabled != value)
                {
                    isDrawingEnabled = value;
                    UpdateSetting();
                }
            }
        }

        private bool isSneakyWatcherBorderEnabled = false;
        public bool IsSneakyWatcherBorderEnabled
        {
            get { return isSneakyWatcherBorderEnabled; }

            set
            {
                if (isSneakyWatcherBorderEnabled != value)
                {
                    isSneakyWatcherBorderEnabled = value;
                    UpdateSetting();
                }
            }
        }

        #region Pen
        private Color primaryPenColor = Colors.OrangeRed;
        public Color PrimaryPenColor
        {
            get { return primaryPenColor; }
            set
            {
                if (primaryPenColor != value)
                {
                    primaryPenColor = value;
                    UpdateSetting();
                }
            }
        }

        private double primaryBurshThickness = 20;
        public double PrimaryBurshThickness
        {
            get { return primaryBurshThickness; }
            set
            {
                if (primaryBurshThickness != value)
                {
                    primaryBurshThickness = value;
                    UpdateSetting();
                }
            }
        }


        private Color secondaryPenColor = Colors.White;
        public Color SecondaryPenColor
        {
            get { return secondaryPenColor; }
            set
            {
                if (secondaryPenColor != value)
                {
                    secondaryPenColor = value;
                    UpdateSetting();
                }
            }
        }

        private double secondaryBurshThickness = 20;
        public double SecondaryBurshThickness
        {
            get { return secondaryBurshThickness; }
            set
            {
                if (secondaryBurshThickness != value)
                {
                    secondaryBurshThickness = value;
                    UpdateSetting();
                }
            }
        }

        private Color highlighterPenColor = Colors.YellowGreen;
        public Color HighlighterPenColor
        {
            get { return highlighterPenColor; }
            set
            {
                if (highlighterPenColor != value)
                {
                    highlighterPenColor = value;
                    UpdateSetting();
                }
            }
        }

        private double highlighterBurshThickness = 20;
        public double HighlighterBurshThickness
        {
            get { return highlighterBurshThickness; }
            set
            {
                if (highlighterBurshThickness != value)
                {
                    highlighterBurshThickness = value;
                    UpdateSetting();
                }
            }
        }
        #endregion

        #region Sneaky Watcher Settings
        private bool isPauseWhenSneakyWatcherActive = true;
        public bool IsPauseWhenSneakyWactherActive
        {
            get { return isPauseWhenSneakyWatcherActive; }
            set
            {
                if (isPauseWhenSneakyWatcherActive != value)
                {
                    isPauseWhenSneakyWatcherActive = value;
                    UpdateSetting();
                }
            }
        }

        private Color blockerColor = Colors.Black;
        public Color BlockerColor
        {
            get { return blockerColor; }
            set
            {
                if (blockerColor != value)
                {
                    blockerColor = value;
                    UpdateSetting();
                }
            }
        }

        private bool isBlockerImageEnabled = false;
        public bool IsBlockerImageEnabled
        {
            get { return isBlockerImageEnabled; }
            set
            {
                if (isBlockerImageEnabled != value)
                {
                    isBlockerImageEnabled = value;
                    UpdateSetting();
                }
            }
        }

        private string blockerImageLocation = string.Empty;
        public string BlockerImageLocation
        {
            get { return blockerImageLocation; }
            set
            {
                if (blockerImageLocation != value)
                {
                    blockerImageLocation = value;
                    UpdateSetting();
                }
            }
        }

        private Stretch blockerStretchMode = Stretch.Fill;
        public Stretch BlockerStretchMode
        {
            get { return blockerStretchMode; }
            set
            {
                if (blockerStretchMode != value)
                {
                    blockerStretchMode = value;
                    UpdateSetting();
                }
            }
        }

        private bool makeWindowTopMost = true;
        public bool MakeWindowTopMost
        {
            get { return makeWindowTopMost; }
            set
            {
                if (makeWindowTopMost != value)
                {
                    makeWindowTopMost = value;
                    UpdateSetting();
                }
            }
        }

        private bool disableAltF4 = false;
        public bool DisableAltF4
        {
            get { return disableAltF4; }
            set
            {
                if (disableAltF4 != value)
                {
                    disableAltF4 = value;
                    UpdateSetting();
                }
            }
        }

        private bool isEnableMasterPassword = true;
        public bool IsEnableMasterPassword
        {
            get { return isEnableMasterPassword; }
            set
            {
                if (isEnableMasterPassword != value)
                {
                    isEnableMasterPassword = value;
                    UpdateSetting();
                }
            }
        }

        private string sneakyWatcherMasterPassword = "12345";
        public string SneakyWatcherMasterPassword
        {
            get { return sneakyWatcherMasterPassword; }
            set
            {
                if (sneakyWatcherMasterPassword != value)
                {
                    sneakyWatcherMasterPassword = value;
                    UpdateSetting();
                }
            }
        }

        private bool isRandomizePasswordBox = true;
        public bool IsRandomizePasswordBox
        {
            get { return isRandomizePasswordBox; }
            set
            {
                if (isRandomizePasswordBox != value)
                {
                    isRandomizePasswordBox = value;
                    UpdateSetting();
                }
            }
        }

        private bool changeAppIconWhenSneakyWatcherActive = true;
        public bool ChangeAppIconWhenSneakyWatcherActive
        {
            get { return changeAppIconWhenSneakyWatcherActive; }
            set
            {
                if (changeAppIconWhenSneakyWatcherActive != value)
                {
                    changeAppIconWhenSneakyWatcherActive = value;
                    UpdateSetting();
                }
            }
        }
        #endregion

        #endregion

        private void UpdateSetting()
        {
            string saveContent = JsonConvert.SerializeObject(this);
            File.WriteAllText("Settings\\" + SettingName, saveContent);
        }
    }

    public enum PlayerType
    {
        Embeded, External
    }
}
