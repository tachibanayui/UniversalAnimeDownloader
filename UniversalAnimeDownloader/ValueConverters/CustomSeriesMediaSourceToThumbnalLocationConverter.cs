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
    class CustomSeriesMediaSourceToThumbnalLocationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value == DependencyProperty.UnsetValue)
                return string.Empty;

            var parsedVal = value as MediaSourceInfo;
            return parsedVal.LocalFile;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value == DependencyProperty.UnsetValue)
                return null;
            var parsedVal = value.ToString();
            return new MediaSourceInfo()
            {
                LocalFile = parsedVal,
            };
        }
    }
}
