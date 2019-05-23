using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using UADAPI;

namespace UniversalAnimeDownloader.ValueConverters
{
    class CustomSeriesFilmSourceToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value == DependencyProperty.UnsetValue)
                return string.Empty;

            var parsedVal = value as Dictionary<VideoQuality, MediaSourceInfo>;
            if(parsedVal.Keys.Count != 0)
                return parsedVal.First().Value.LocalFile;
            else
                return string.Empty;
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value == DependencyProperty.UnsetValue)
                return null;

            var parsedVal = value.ToString();

            if (!string.IsNullOrEmpty(parsedVal))
            {
                var filmSource = new Dictionary<VideoQuality, MediaSourceInfo>();
                filmSource.Add(VideoQuality.Quality144p, new MediaSourceInfo() { LocalFile = parsedVal });

                return filmSource;
            }
            else
                return null;
        }
    }
}
