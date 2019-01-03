using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace UniversalAnimeDownloader
{
    static class SettingsValues
    {
        #region PlayblackSetting
        private static PlayerType preferedPlayer = PlayerType.Embeded;
        public static PlayerType PreferedPlayer
        {
            get { return preferedPlayer; }
            set
            {
                if(preferedPlayer != value)
                {
                    preferedPlayer = value;
                    UpdateSetting();
                }
            }
        }

        private static bool playMediaFullScreen = true;
        public static bool PlayMediaFullScreen
        {
            get { return playMediaFullScreen; }
            set
            {
                if(playMediaFullScreen != value)
                {
                    playMediaFullScreen = value;
                    UpdateSetting();
                }
            }
        }

        private static double playbackVolume = 100;
        public static double PlaybackVolume
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

        private static bool isDrawingEnabled = true;
        public static bool IsDrawingEnabled
        {
            get { return isDrawingEnabled; }
            set
            {
                if(isDrawingEnabled != value)
                {
                    isDrawingEnabled = value;
                    UpdateSetting();
                }
            }
        }

        private static bool isSneakyWatherEnabled = true;
        public static bool IsSneakyWatcherEnabled
        {
            get { return isSneakyWatherEnabled; }
            set
            {
                if(isDrawingEnabled != value)
                {
                    isDrawingEnabled = value;
                    UpdateSetting();
                }
            }
        }

        private static bool isSneakyWatcherBorderEnabled = false;
        public static bool IsSneakyWatcherBorderEnabled
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
        private static Color primaryPenColor = Colors.OrangeRed;
        public static Color PrimaryPenColor
        {
            get { return primaryPenColor; }
            set
            {
                if(primaryPenColor != value)
                {
                    primaryPenColor = value;
                    UpdateSetting();
                }
            }
        }

        private static double primaryBurshThickness = 20;
        public static double PrimaryBurshThickness
        {
            get { return primaryBurshThickness; }
            set
            {
                if(primaryBurshThickness != value)
                {
                    primaryBurshThickness = value;
                    UpdateSetting();
                }
            }
        }


        private static Color secondaryPenColor = Colors.White;
        public static Color SecondaryPenColor
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

        private static double secondaryBurshThickness = 20;
        public static double SecondaryBurshThickness
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

        private static Color highlighterPenColor = Colors.YellowGreen;
        public static Color HighlighterPenColor
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

        private static double highlighterBurshThickness = 20;
        public static double HighlighterBurshThickness
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
        private static bool isPauseWhenSneakyWatcherActive = true;
        public static bool IsPauseWhenSneakyWactherActive
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

        private static Color blockerColor = Colors.Black;
        public static Color BlockerColor
        {
            get { return blockerColor; }
            set
            {
                if(blockerColor != value)
                {
                    blockerColor = value;
                    UpdateSetting();
                }
            }
        }

        private static bool isBlockerImageEnabled = false;
        public static bool IsBlockerImageEnabled
        {
            get { return isBlockerImageEnabled; }
            set
            {
                if(isBlockerImageEnabled != value)
                {
                    isBlockerImageEnabled = value;
                    UpdateSetting();
                }
            }
        }

        private static string blockerImageLocation;
        public static string BlockerImageLocation
        {
            get { return blockerImageLocation; }
            set
            {
                if(blockerImageLocation != value)
                {
                    blockerImageLocation = value;
                    UpdateSetting();
                }
            }
        }

        private static Stretch blockerStretchMode = Stretch.Fill;
        public static Stretch BlockerStretchMode
        {
            get { return blockerStretchMode; }
            set
            {
                if(blockerStretchMode != value)
                {
                    blockerStretchMode = value;
                    UpdateSetting();
                }
            }
        }

        private static bool makeWindowTopMost = false;
        public static bool MakeWindowTopMost
        {
            get { return makeWindowTopMost; }
            set
            {
                if(makeWindowTopMost != value)
                {
                    makeWindowTopMost = value;
                    UpdateSetting();
                }
            }
        }

        private static bool disableAltF4 = false;
        public static bool DisableAltF4
        {
            get { return disableAltF4; }
            set
            {
                if(disableAltF4 != value)
                {
                    disableAltF4 = value;
                    UpdateSetting();
                }
            }
        }

        private static bool isEnableMasterPassword = true;
        public static bool IsEnableMasterPassword
        {
            get { return isEnableMasterPassword; }
            set
            {
                if(isEnableMasterPassword != value)
                {
                    isEnableMasterPassword = value;
                    UpdateSetting();
                }
            }
        }

        private static string sneakyWatcherMasterPassword = "12345";
        public static string SneakyWatcherMasterPassword
        {
            get { return sneakyWatcherMasterPassword; }
            set
            {
                if(sneakyWatcherMasterPassword != value)
                {
                    sneakyWatcherMasterPassword = value;
                    UpdateSetting();
                }
            }
        }

        private static bool isRandomizePasswordBox = true;
        public static bool IsRandomizePasswordBox
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

        private static bool changeAppIconWhenSneakyWatcherActive = true;
        public static bool ChangeAppIconWhenSneakyWatcherActive
        {
            get { return changeAppIconWhenSneakyWatcherActive; }
            set
            {
                if(changeAppIconWhenSneakyWatcherActive != value)
                {
                    changeAppIconWhenSneakyWatcherActive = value;
                    UpdateSetting();
                }
            }
        }
        #endregion

        #endregion

        private static void UpdateSetting()
        {
            
        }
    }

    public enum PlayerType
    {
        External, Embeded
    }
}
