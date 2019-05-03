using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace UniversalAnimeDownloader.ValueConverters
{
    class MultiplicationMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null)
                return 0;

            if (values.Length == 0)
                return 0d;

            double result = (double)values[0];
            for (int i = 1; i < values.Length; i++)
            {
                result *= (double)values[i];
            }
            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
