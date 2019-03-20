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
    class HtmlTagToColoredTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string input = value as string;
            if (input == null)
                return string.Empty;

            return MiscClass.AddHtmlColorBody(input, (Application.Current.FindResource("MaterialDesignBody") as SolidColorBrush).Color);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
