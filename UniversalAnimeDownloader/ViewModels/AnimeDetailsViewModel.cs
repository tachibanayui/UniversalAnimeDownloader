using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using UADAPI;

namespace UniversalAnimeDownloader.ViewModels
{
    public class AnimeDetailsViewModel : BaseViewModel
    {
        #region BindableProperties
        private IAnimeSeriesManager _CurrentSeries;
        public IAnimeSeriesManager CurrentSeries
        {
            get
            {
                return _CurrentSeries;
            }
            set
            {
                if (_CurrentSeries != value)
                {
                    _CurrentSeries = value;
                    OnDetailAnimeChanged(value);
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<SelectableEpisodeInfo> EpisodeInfo { get; set; } = new ObservableCollection<SelectableEpisodeInfo>();

        private List<int> _SelectedEpisodeIndex;
        public List<int> SelectedEpisodeIndex
        {
            get
            {
                return _SelectedEpisodeIndex;
            }
            set
            {
                if (_SelectedEpisodeIndex != value)
                {
                    _SelectedEpisodeIndex = value;
                    OnPropertyChanged();
                }
            }
        }


        #endregion

        #region Commands
        public ICommand CopyDescriptionCommand { get; set; }
        #endregion


        public AnimeDetailsViewModel()
        {
            CopyDescriptionCommand = new RelayCommand<object>(p => true, p => Clipboard.SetText(CurrentSeries.AttachedAnimeSeriesInfo.Description));



            //Test();
        }

        //private void Test()
        //{
        //    string res = string.Empty;
        //    bool isDash = false;
        //    int st = EpisodeInfo.IndexOf(EpisodeInfo.Find(p => p.IsSelected));
        //    res += (st + 1).ToString();
        //    for (int i = st + 1; i < EpisodeInfo.Count; i++)
        //    {
        //        if (EpisodeInfo[i].IsSelected)
        //        {
        //            if (EpisodeInfo[i - 1].IsSelected)
        //            {
        //                if(i + 1 == EpisodeInfo.Count)
        //                {
        //                    res += (i + 1).ToString();
        //                    break;
        //                }
        //                if (EpisodeInfo[i + 1].IsSelected)
        //                {
        //                    if (!isDash)
        //                    {
        //                        res += '-';
        //                        isDash = true;
        //                    }
        //                }
        //                else
        //                {
        //                    if (!isDash)
        //                    {
        //                        res += ',';
        //                    }
        //                    res += (i + 1).ToString();
        //                    isDash = false;
        //                }
        //            }
        //            else
        //            {
        //                res += ',' + (i + 1).ToString();
        //                isDash = false;
        //            }
        //        }
        //    }
        //    if (res == $"1-{EpisodeInfo.Count}")
        //        res = "all";
        //    System.Console.WriteLine(res);
        //}

        private void OnDetailAnimeChanged(IAnimeSeriesManager value)
        {
            EpisodeInfo.Clear();
            foreach (var item in CurrentSeries.AttachedAnimeSeriesInfo.Episodes)
            {
                var obj = new SelectableEpisodeInfo() { Data = item, IsSelected = false, };
                EpisodeInfo.Add(obj);
            }
        }


    }
}
