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
using UniversalAnimeDownloader.UADSettingsPortal;

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

        private string _SelectedQuality;
        public string SelectedQuality
        {
            get
            {
                return _SelectedQuality;
            }
            set
            {
                if (_SelectedQuality != value)
                {
                    _SelectedQuality = value;
                    SourceControl.PreferedQuality = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _IsFlipperFliped;
        public bool IsFlipperFliped
        {
            get
            {
                return _IsFlipperFliped;
            }
            set
            {
                if (_IsFlipperFliped != value)
                {
                    _IsFlipperFliped = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _DownloadButtonString = "Download Seleted Episodes";
        public string DownloadButtonString
        {
            get
            {
                return _DownloadButtonString;
            }
            set
            {
                if (_DownloadButtonString != value)
                {
                    _DownloadButtonString = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _IsDownloadButtonStringEnable = true;
        public bool IsDownloadButtonStringEnable
        {
            get
            {
                return _IsDownloadButtonStringEnable;
            }
            set
            {
                if (_IsDownloadButtonStringEnable != value)
                {
                    _IsDownloadButtonStringEnable = value;
                    OnPropertyChanged();
                }
            }
        }

        private Visibility _OfflineNavigationButtonVisibility;
        public Visibility OfflineNavigationButtonVisibility
        {
            get
            {
                return _OfflineNavigationButtonVisibility;
            }
            set
            {
                if (_OfflineNavigationButtonVisibility != value)
                {
                    _OfflineNavigationButtonVisibility = value;
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
        public ICommand OfflineVerionCommand { get; set; }
        #endregion


        #region Properties
        public Task TempTask { get; set; }
        public bool isDoneStringSeleting { get; set; } = true;
        public bool ManualSelectEpisodeOnDownloaded { get; set; } = false;
        #endregion

        public AnimeDetailsViewModel()
        {
            CopyDescriptionCommand = new RelayCommand<object>(p => true, p => Clipboard.SetText(CurrentSeries.AttachedAnimeSeriesInfo.Description ?? ""));
            WatchEpisodeOnline = new RelayCommand<SelectableEpisodeInfo>(p => true, async (p) =>
            {
                if (p != null)
                {
                    var episodeDetail = (await CurrentSeries.GetEpisodes(new List<int>() { p.Data.EpisodeID })).ToList()[0];
                    Process.Start(episodeDetail.FilmSources[episodeDetail.FilmSources.Keys.ToList()[0]].Url);
                }
            });
            SelectedEpisodeCommand = new RelayCommand<TextBox>(p => true, p => { TempTask = SelectEpisodeIndex(p); });
            DownloadAnimeCommand = new RelayCommand<object>(p => true, async p =>
            {
                MessageDialogResult result = 0;
                if (ManualSelectEpisodeOnDownloaded)
                    result = await MessageDialog.ShowAsync("You might trying to redownload some episodes!", "We detected that you have change the selection list. If you attempt to redownload any episodes, it will be overwritten. Do you want to continue download?", MessageDialogButton.YesNoButton);
                if(result == MessageDialogResult.Yes)
                {
                    IsFlipperFliped = true;
                    DownloadButtonString = "Getting Information...";
                    IsDownloadButtonStringEnable = false;
                    var selectedIndexList = new List<int>();
                    var selectedIDList = new List<int>();
                    foreach (var item in EpisodeInfo)
                    {
                        if (item.IsSelected)
                        {
                            selectedIndexList.Add(item.Data.Index);
                            selectedIDList.Add(item.Data.EpisodeID);
                        }
                    }

                    await CurrentSeries.GetEpisodes(selectedIDList);
                    SourceControl.PreferedQuality = string.IsNullOrEmpty(SelectedQuality) ? "480p" : SelectedQuality;
                    var downloader = SourceControl.DownloadAnimeByIndexes(selectedIndexList);
                    DownloadButtonString = "Downloading...";
                    UADSettingsManager.Instance.CurrentSettings.Download = DownloadManager.Serialize();
                    downloader.EpisodeDownloadCompleted += (s,e) => UADSettingsManager.Instance.CurrentSettings.Download = DownloadManager.Serialize();
                }
            });
            OfflineVerionCommand = new RelayCommand<object>(p => true, p =>
            {
                var offline = Application.Current.FindResource("OfflineAnimeDetailViewModel") as OfflineAnimeDetailViewModel;
                var lib = Application.Current.FindResource("MyAnimeLibraryViewModel") as MyAnimeLibraryViewModel;
                var offlineSeries = lib.AnimeLibrary.FirstOrDefault(f => f.AnimeID == CurrentSeries.AttachedAnimeSeriesInfo.AnimeID && f.ModInfo.ModTypeString.Contains(CurrentSeries.AttachedAnimeSeriesInfo.ModInfo.ModTypeString));
                if (offlineSeries != null)
                {
                    var manage = ApiHelpper.CreateAnimeSeriesManagerObjectByClassName(offlineSeries.ModInfo.ModTypeString);
                    manage.AttachedAnimeSeriesInfo = offlineSeries;
                    offline.CurrentSeries = manage;
                    (Application.Current.FindResource("MainWindowViewModel") as MainWindowViewModel).NavigateProcess("OfflineAnimeDetail");
                }
            });
        }

        private async Task SelectEpisodeIndex(TextBox obj)
        {
            isDoneStringSeleting = false;
            string res = obj.Text.ToLower();
            var dispatcher = Window.GetWindow(obj).Dispatcher;
            await Task.Run(async () =>
            {
                if (res.Contains("\n"))
                {
                    res = res.Trim('\r', '\n');
                    await dispatcher.InvokeAsync(() => obj.Text = res);
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
            if(DownloadButtonString == "Download missing episodes")
            {
                DownloadButtonString = "Download Seleted Episodes";
                ManualSelectEpisodeOnDownloaded = true;
            }
            isDoneStringSeleting = true;
        }

        private void ResetSelected(bool resetValue)
        {
            foreach (var item in EpisodeInfo)
            {
                item.IsSelected = resetValue;
            }
        }

        private async Task<string> SelectionToText()
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
                        if (tempInfo != null)
                        {
                            if (FindEpisodeByIndex(i - 1).IsSelected)
                            {
                                if (i + 1 == EpisodeInfo.Last().Data.Index + 1)
                                {
                                    if (!isDash)
                                    {
                                        res += ',';
                                    }

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
                            else
                            {
                                res += ',' + i.ToString();
                                isDash = false;
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

        private async void OnDetailAnimeChanged(IAnimeSeriesManager value)
        {
            var downloader = DownloadManager.Instances.FirstOrDefault(p => p.AttachedManager.AttachedAnimeSeriesInfo.AnimeID == value.AttachedAnimeSeriesInfo.AnimeID && (p.State == UADDownloaderState.Paused || p.State == UADDownloaderState.Working));
            var localList = (Application.Current.FindResource("MyAnimeLibraryViewModel") as MyAnimeLibraryViewModel).AnimeLibrary;
            var offline = localList.FirstOrDefault(f => f.AnimeID == value.AttachedAnimeSeriesInfo.AnimeID);

            //if the episode is being download?
            if (downloader != null)
            {
                if (downloader.AttachedManager.ModInfo.ModTypeString.Contains(value.AttachedAnimeSeriesInfo.ModInfo.ModTypeString))
                {
                    DownloadButtonString = "Downloading";
                    IsDownloadButtonStringEnable = false;
                    IsFlipperFliped = false;
                    OfflineNavigationButtonVisibility = Visibility.Collapsed;
                }
            }
            //if the episode avaible offline?
            else if (offline != null)
            {
                if (offline.ModInfo.ModTypeString.Contains(value.AttachedAnimeSeriesInfo.ModInfo.ModTypeString))
                {
                    DownloadButtonString = "Download missing episodes";
                    IsDownloadButtonStringEnable = true;
                    IsFlipperFliped = true;
                    OfflineNavigationButtonVisibility = Visibility.Visible;
                }
            }
            else
            {
                DownloadButtonString = "Download Seleted Episodes";
                IsDownloadButtonStringEnable = true;
                IsFlipperFliped = false;
                OfflineNavigationButtonVisibility = Visibility.Collapsed;
            }


            //Convert List to Selectible list
            EpisodeInfo.Clear();
            foreach (var item in CurrentSeries.AttachedAnimeSeriesInfo.Episodes)
            {
                var obj = new SelectableEpisodeInfo() { Data = item, IsSelected = true };
                if (offline != null)
                    if (offline.Episodes.Any(p => p.EpisodeID == item.EpisodeID && p.AvailableOffline == true))
                        obj.IsSelected = false;

                obj.SelectedIndexChanged += async (s, e) =>
                {
                    if (!isDoneStringSeleting)
                    {
                        return;
                    }

                    if (TempTask != null)
                    {
                        await TempTask;
                    }

                    SelectedIndexString = await SelectionToText();
                };
                EpisodeInfo.Add(obj);
            }


            ManualSelectEpisodeOnDownloaded = false;
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
