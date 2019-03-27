using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using UADAPI;

namespace UniversalAnimeDownloader.ValueConverters
{
    class MediaSourceInfoToImageSource : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is MediaSourceInfo info))
                return null;

            if (!string.IsNullOrEmpty(info.LocalFile))
            {
                if(File.Exists(info.LocalFile))
                    return new BitmapImage(new Uri(info.LocalFile));
                else
                    return new BitmapImage(new Uri(info.Url));
            }
            else if (!string.IsNullOrEmpty(info.Url))
                return new BitmapImage(new Uri(info.Url));
            else
                return null;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
