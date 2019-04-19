using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using UniversalAnimeDownloader.UADSettingsPortal;

namespace UniversalAnimeDownloader.ValueConverters
{
    class StringToPlayerTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((PlayerType)value == PlayerType.External)
                return MiscClass.MediaPlayerOptions[0];
            else
                return MiscClass.MediaPlayerOptions[1];

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.ToString() == MiscClass.MediaPlayerOptions[0])
                return PlayerType.External;
            else
                return PlayerType.Embeded;

        }
    }
}
