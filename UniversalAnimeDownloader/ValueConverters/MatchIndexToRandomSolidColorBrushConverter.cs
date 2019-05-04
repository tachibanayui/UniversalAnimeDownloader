using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace UniversalAnimeDownloader.ValueConverters
{
    class MatchIndexToRandomSolidColorBrushConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            int parsedVal = (int)values[0];

            for (int i = 1; i < values.Length; i++)
                if ((int)values[i] != parsedVal)
                    return new SolidColorBrush(Colors.Transparent);

            return new SolidColorBrush(PresetColors.GetRandomColor());
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class MatchIndexToSpecificSolidColorBrushConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            int parsedVal = (int)values[2];

            for (int i = 3; i < values.Length; i++)
                if ((int)values[i] != parsedVal)
                    return values[1];

            return values[0];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class MatchIndexToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Count(p => p == DependencyProperty.UnsetValue) > 0)
                return Visibility.Collapsed;

            int parsedVal = (int)values[0];

            for (int i = 1; i < values.Length; i++)
                if ((int)values[i] != parsedVal)
                    return Visibility.Collapsed;

            return Visibility.Visible;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
