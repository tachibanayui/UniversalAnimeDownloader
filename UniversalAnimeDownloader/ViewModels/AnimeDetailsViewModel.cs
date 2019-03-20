using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
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

        private string _SelectedIndexString;
        public string SelectedIndexString
        {
            get
            {
                return _SelectedIndexString;
            }
            set
            {
                if (_SelectedIndexString != value)
                {
                    _SelectedIndexString = value;
                    OnPropertyChanged();
                }
            }
        }



        #endregion

        #region Commands
        public ICommand CopyDescriptionCommand { get; set; }
        public ICommand SelectedEpisodeCommand { get; set; }
        #endregion


        public AnimeDetailsViewModel()
        {
            CopyDescriptionCommand = new RelayCommand<object>(p => true, p => Clipboard.SetText(CurrentSeries.AttachedAnimeSeriesInfo.Description));
            SelectedEpisodeCommand = new RelayCommand<TextBox>(p => true, SelectEpisodeIndex);


            //Test();
        }

        private void SelectEpisodeIndex(TextBox obj)
        {
            string res = obj.Text.ToLower();
            if (res.Contains("\n"))
            {
                res = res.Trim('\r', '\n');
                ResetSelected(false);
                if (res == "all")
                {
                    ResetSelected(true);
                }
                else if (!string.IsNullOrEmpty(res))
                {
                    string[] parts = res.Split(',');
                    try
                    {
                        for (int i = 0; i < parts.Length; i++)
                        {
                            if (parts[i].Contains("-"))
                            {
                                string[] vs = parts[i].Split('-');
                                int min = int.Parse(vs[0]);
                                int max = int.Parse(vs[1]);
                                for (int j = min; j < max + 1; j++)
                                {
                                    EpisodeInfo[j - 1].IsSelected = true;
                                }
                            }
                            else
                            {
                                EpisodeInfo[int.Parse(parts[i]) - 1].IsSelected = true;
                            }
                        }
                    }
                    catch
                    {

                    }
                }
            }
        }

        private void ResetSelected(bool resetValue)
        {
            foreach (var item in EpisodeInfo)
            {
                item.IsSelected = resetValue;
            }
        }

        private string SelectionToText()
        {
            string res = string.Empty;
            bool isDash = false;
            int st = -1;
            for (int i = 0; i < EpisodeInfo.Count; i++)
            {
                if(EpisodeInfo[i].IsSelected)
                {
                    st = i;
                    break;
                }
            }
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
            {
                res = "all";
            }

            return res;
        }

        private void OnDetailAnimeChanged(IAnimeSeriesManager value)
        {
            EpisodeInfo.Clear();
            foreach (var item in CurrentSeries.AttachedAnimeSeriesInfo.Episodes)
            {
                var obj = new SelectableEpisodeInfo() { Data = item, IsSelected = false, };
                obj.SelectedIndexChanged += (s, e) => SelectedIndexString = SelectionToText();
                EpisodeInfo.Add(obj);
            }
        }

    }
}
