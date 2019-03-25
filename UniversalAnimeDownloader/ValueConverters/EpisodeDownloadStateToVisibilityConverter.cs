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
    /// <summary>
    /// Convert EpisodeDownloadState.InDownloadQueue to Visibility.Visible other value return Visibility.Collapsed
    /// </summary>
    class EpisodeDownloadStateToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string parameterVal = string.Empty;
            if (parameter != null)
                parameterVal = parameter.ToString();
           
            var parsedVal = (EpisodeDownloadState)value;

            switch (parameterVal)
            {
                case "SegmentProgressBar":
                case "DownloaderDetail":
                    return parsedVal == EpisodeDownloadState.Downloading ? Visibility.Visible : Visibility.Collapsed;
                case "IconTitle":
                    return parsedVal != EpisodeDownloadState.InDownloadQueue && parsedVal != EpisodeDownloadState.Downloading ? Visibility.Visible : Visibility.Collapsed;
                case "WaitingTextBlock":
                    return parsedVal != EpisodeDownloadState.Downloading ? Visibility.Visible : Visibility.Collapsed;
                default:
                    return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
