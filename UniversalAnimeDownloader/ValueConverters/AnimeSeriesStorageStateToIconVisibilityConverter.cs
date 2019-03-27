using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using UADAPI;
using UniversalAnimeDownloader.ViewModels;

namespace UniversalAnimeDownloader.ValueConverters
{
    class AnimeSeriesStorageStateToIconVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var parsedValue = value as AnimeSeriesInfo;
            var parameterString = parameter.ToString();
            if (parsedValue == null)
                return Visibility.Visible;

            var offlineViewModel = Application.Current.FindResource("MyAnimeLibraryViewModel") as MyAnimeLibraryViewModel;
            if (offlineViewModel.AnimeLibrary.Any(f => f.AnimeID == parsedValue.AnimeID))
                return parameterString == "Offline" ? Visibility.Visible : Visibility.Collapsed;
            else if (DownloadManager.Instances.Any(f => f.AttachedManager.AttachedAnimeSeriesInfo.AnimeID == parsedValue.AnimeID && (f.State == UADDownloaderState.Working || f.State == UADDownloaderState.Paused)))
                return parameterString == "Downloading" ? Visibility.Visible : Visibility.Collapsed;
            else
                return Visibility.Collapsed;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
