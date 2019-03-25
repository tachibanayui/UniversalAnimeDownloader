using MaterialDesignThemes.Wpf;
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
    class EpisodeDownloadStateToPackIconKindConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var parsedVal = (EpisodeDownloadState)value;
            switch (parsedVal)
            {
                case EpisodeDownloadState.NotDownloaded:
                    return PackIconKind.MinusCircleOutline;
                case EpisodeDownloadState.FinishedDownloading:
                    return PackIconKind.CheckboxMarkedCircleOutline;
                case EpisodeDownloadState.FailedDownloading:
                    return PackIconKind.CloseCircleOutline;
                default:
                    return PackIconKind.MinusCircleOutline;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
