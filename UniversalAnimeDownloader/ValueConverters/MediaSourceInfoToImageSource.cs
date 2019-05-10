using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using UADAPI;

namespace UniversalAnimeDownloader.ValueConverters
{
    class MediaSourceInfoToImageSource : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Task<Stream> task = null;
            if (!(value is MediaSourceInfo info))
            {
                return new BitmapImage();
            }

            if (!string.IsNullOrEmpty(info.LocalFile))
            {
                if (File.Exists(info.LocalFile))
                {
                    task = Task.Run(() => GetOfflineImage(info.LocalFile));
                }
                else if (!string.IsNullOrEmpty(info.Url))
                {
                    task = Task.Run(() => GetOnlineImage(info));
                }
            }
            else if (!string.IsNullOrEmpty(info.Url))
            {
                task = Task.Run(() => GetOnlineImage(info));
            }
            else
            {
                return new BitmapImage();
            }

            return new TaskCompletionNotifier<Stream>(task);
        }

        private Stream GetOnlineImage(MediaSourceInfo info)
        {
            try
            {
                var stream =  AsyncHelpers.RunSync<Stream>(() => AnimeInformationRequester.GetStreamAsync(info.Url, info.Headers));
                return stream;
            }
            catch { return null; }
        }

        private Stream GetOfflineImage(string path)
        {
            return File.OpenRead(path);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
