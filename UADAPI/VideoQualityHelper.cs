using System;
using System.Collections.Generic;
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

        public static VideoQuality GetEnumFromString(string value) => (VideoQuality)Enum.Parse(typeof(VideoQuality), value);
    }
}
