using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using UADAPI;

namespace UniversalAnimeDownloader.ValueConverters
{
    class EpisodeDownloadStateToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var data = (EpisodeDownloadState)value;
            switch (data)
            {
                case EpisodeDownloadState.NotDownloaded:
                    return "This episode is not in download queue";
                case EpisodeDownloadState.InDownloadQueue:
                    return "Getting information...";
                case EpisodeDownloadState.FinishedDownloading:
                    return "Finished downloading";
                case EpisodeDownloadState.FailedDownloading:
                    return "Download Failed!";
                default:
                    return "Other";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
