using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace UniversalAnimeDownloader.ValueConverters
{
    class AutomatedUnitConversionFromByteConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double byteRate = double.Parse(value.ToString());
            double convertedValue = byteRate;
            string unitType = "Byte";


            if (byteRate < Math.Pow(2, 20))
            {
                convertedValue /= Math.Pow(2, 10);
                unitType = "KB";
            }
            else if (byteRate < Math.Pow(2, 30))
            {
                convertedValue /= Math.Pow(2, 20);
                unitType = "MB";
            }

            return string.Format("{0:N2}", convertedValue) + unitType;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
