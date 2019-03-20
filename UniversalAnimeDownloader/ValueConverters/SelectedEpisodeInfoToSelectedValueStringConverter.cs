using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace UniversalAnimeDownloader.ValueConverters
{
    class SelectedEpisodeInfoToSelectedValueStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var EpisodeInfo = value as List<SelectableEpisodeInfo>;
            if (EpisodeInfo == null)
                return string.Empty;

            string res = string.Empty;
            bool isDash = false;
            int st = EpisodeInfo.IndexOf(EpisodeInfo.Find(p => p.IsSelected));
            if (st == -1)
                return string.Empty;
            res += (st + 1).ToString();
            for (int i = st + 1; i < EpisodeInfo.Count; i++)
            {
                if (EpisodeInfo[i].IsSelected)
                {
                    if (EpisodeInfo[i - 1].IsSelected)
                    {
                        if (i + 1 == EpisodeInfo.Count)
                        {
                            res += (i + 1).ToString();
                            break;
                        }
                        if (EpisodeInfo[i + 1].IsSelected)
                        {
                            if (!isDash)
                            {
                                res += '-';
                                isDash = true;
                            }
                        }
                        else
                        {
                            if (!isDash)
                            {
                                res += ',';
                            }
                            res += (i + 1).ToString();
                            isDash = false;
                        }
                    }
                    else
                    {
                        res += ',' + (i + 1).ToString();
                        isDash = false;
                    }
                }
            }
            if (res == $"1-{EpisodeInfo.Count}")
                res = "all";

            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
