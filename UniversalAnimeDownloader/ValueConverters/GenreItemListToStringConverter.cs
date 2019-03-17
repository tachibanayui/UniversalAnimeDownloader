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
    class GenreItemListToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string res = string.Empty;
            if (!(value is IList<GenreItem> genreItems))
                return res;

            foreach (GenreItem item in genreItems)
            {
                res += $"{item.Name}, ";
            }

            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IList<GenreItem> genreItems = null;
            try
            {
                string orginalString = value as string;

                string[] genreName = orginalString.Split(',');

                genreItems = new List<GenreItem>();
                foreach (var item in genreName)
                {
                    genreItems.Add(new GenreItem() { Name = item.Trim() });
                }
            }
            catch
            {
            }

            return genreItems;
        }
    }
}
