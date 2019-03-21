using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
        public AnimeSourceControl SourceControl { get; private set; }
        public ICommand CopyDescriptionCommand { get; set; }
        public ICommand SelectedEpisodeCommand { get; set; }
        public ICommand WatchEpisodeOnline { get; set; }
        public ICommand DownloadAnimeCommand { get; set; }
        #endregion


        public AnimeDetailsViewModel()
        {
            CopyDescriptionCommand = new RelayCommand<object>(p => true, p => Clipboard.SetText(CurrentSeries.AttachedAnimeSeriesInfo.Description));
            WatchEpisodeOnline = new RelayCommand<SelectableEpisodeInfo>(p => true, async (p) =>
            {
                if (p != null)
                {
                    var episodeDetail = (await CurrentSeries.GetEpisodes(new List<int>() { p.Data.EpisodeID })).ToList()[0];
                    Process.Start(episodeDetail.FilmSources[episodeDetail.FilmSources.Keys.ToList()[0]].Url);
                }
            });
            SelectedEpisodeCommand = new RelayCommand<TextBox>(p => true, async (p) => await SelectEpisodeIndex(p));
        }

        private async Task SelectEpisodeIndex(TextBox obj)
        {

            string res = obj.Text.ToLower();
            var dispatcher = Window.GetWindow(obj).Dispatcher;
            await Task.Run(async () =>
            {
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
                                        FindEpisodeByIndex(j).IsSelected = true;
                                    }
                                }
                                else
                                {
                                    FindEpisodeByIndex(int.Parse(parts[i])).IsSelected = true;
                                }
                            }
                        }
                        catch
                        {
                            await dispatcher.InvokeAsync(() => obj.Foreground = new SolidColorBrush(Colors.Red));
                            await dispatcher.InvokeAsync(() => obj.Text = res.Trim('\r', '\n'));
                            return;
                        }
                    }
                    await dispatcher.InvokeAsync(() => obj.SetResourceReference(Control.ForegroundProperty, "MaterialDesignBody"));
                }
            });
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
            for (int i = 0; i < EpisodeInfo.Last().Data.Index + 1; i++)
            {
                var currentSelection = FindEpisodeByIndex(i);
                if (currentSelection != null)
                {
                    if (currentSelection.IsSelected)
                    {
                        st = i;
                        break;
                    }
                }
            }
            res += st.ToString();
            for (int i = st + 1; i < EpisodeInfo.Last().Data.Index + 1; i++)
            {
                SelectableEpisodeInfo tempInfo = FindEpisodeByIndex(i);
                if (tempInfo != null)
                {
                    if (FindEpisodeByIndex(i).IsSelected)
                    {
                        tempInfo = FindEpisodeByIndex(i - 1);
                        if(tempInfo != null)
                        {
                            if (FindEpisodeByIndex(i - 1).IsSelected)
                            {
                                if (i + 1 == EpisodeInfo.Count)
                                {
                                    res += i.ToString();
                                    break;
                                }
                                tempInfo = FindEpisodeByIndex(i + 1);
                                if (tempInfo != null)
                                {
                                    if (FindEpisodeByIndex(i + 1).IsSelected)
                                    {
                                        if (!isDash)
                                        {
                                            res += '-';
                                            isDash = true;
                                        }
                                    }
                                }
                                else
                                {
                                    if (!isDash)
                                    {
                                        res += ',';
                                    }
                                    res += i.ToString();
                                    isDash = false;
                                }
                            }
                        }
                        else
                        {
                            res += ',' + i.ToString();
                            isDash = false;
                        }
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
                obj.SelectedIndexChanged += (s, e) => { SelectedIndexString = SelectionToText(); };
                EpisodeInfo.Add(obj);
            }

            SourceControl = new AnimeSourceControl(value);
        }

        private SelectableEpisodeInfo FindEpisodeByIndex(int desiredIndex)
        {
            foreach (var item in EpisodeInfo)
            {
                if (desiredIndex == item.Data.Index)
                {
                    return item;
                }
            }

            return null;
        }
    }
}
