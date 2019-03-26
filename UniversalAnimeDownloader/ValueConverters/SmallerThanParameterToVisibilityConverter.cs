using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace UniversalAnimeDownloader.ValueConverters
{
    class SmallerThanParameterToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var parsedValue = double.Parse(value.ToString());
            var parsedParameter = double.Parse(parameter.ToString());

            return parsedValue < parsedParameter ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
