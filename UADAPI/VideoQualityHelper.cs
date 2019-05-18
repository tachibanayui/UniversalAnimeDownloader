using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UADAPI
{
    /// <summary>
    /// Help convert <see cref="VideoQuality"/> to various types
    /// </summary>
    public class VideoQualityHelper
    {
        public static int GetValue(VideoQuality videoQuality) => (int)videoQuality;

        public static string GetQualityStringFromValue(int value) => Enum.GetName(typeof(VideoQuality), value).Replace("Quality", "");

        public static string GetQualityStringFromEnum (VideoQuality value) => value.ToString("g").Replace("Quality", "");

        public static VideoQuality GetEnumFromString(string value)
        {
            string fixedQualityString = GetCommonQualityFromInt(GetNumberFromString(value));
            return string.IsNullOrEmpty(fixedQualityString) ? 
                default(VideoQuality) : 
                (VideoQuality)Enum.Parse(typeof(VideoQuality), fixedQualityString);
        }

        private static List<int> GetNumberFromString(string input)
        {
            var res = new List<int>();
            int currentNum = 0;
            int digits = 0;
            
            foreach (var item in input)
            {
                if(char.IsDigit(item))
                {
                    digits++;
                    currentNum = currentNum * 10 + (item - '0');
                }
                else
                {
                    if(digits != 0)
                    {
                        digits = 0;
                        res.Add(currentNum);
                        currentNum = 0;
                    }
                }
            }

            return res;
        }

        private static string GetCommonQualityFromInt(List<int> input)
        {
            if (input != null)
            {
                foreach (var item in input)
                {
                    switch (item)
                    {
                        case 144:
                        case 240:
                        case 360:
                        case 480:
                        case 720:
                        case 1080:
                        case 2160:
                            return $"Quality{item}p";
                        case 2:
                            return $"Quality1080p";
                        case 4:
                            return $"Quality2160p";
                        default:
                            return string.Empty;
                    }
                }
            }

            return string.Empty;
        }
    }
}
