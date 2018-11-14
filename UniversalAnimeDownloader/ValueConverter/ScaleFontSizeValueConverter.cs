using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace UniversalAnimeDownloader.ValueConverter
{
    class ScaleFontSizeValueConverter : IMultiValueConverter
    {
        public double multiplyer { get; set; }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(targetType == typeof(int) || targetType == typeof(double)))
                return 0;



            double[] partNumbers = new double[values.Length];
            for (int i = 0; i < partNumbers.Length; i++)
            {
                partNumbers[i] = System.Convert.ToDouble(values[i]);
            }

            double result = 1;

            if (partNumbers.Length == 1)
                return partNumbers[0] * multiplyer;

           
            return partNumbers[1] / partNumbers[0];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            double doubleVal = (double)value;
            double res = doubleVal/ multiplyer;
            double[] arrayVal = new double[] { res, multiplyer };
            return arrayVal.Cast<object>().ToArray();
        }
    }
}
