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

        private static bool isSneakyWatcherBorderEnabled = true;
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
        #endregion

        #endregion



        private static void UpdateSetting()
        {
            
        }
    }
}
