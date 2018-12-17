using MaterialDesignThemes.Wpf;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using uadcorelib;
using uadcorelib.Models;
using UniversalAnimeDownloader.CustomControl;
using UniversalAnimeDownloader.ViewModel;

namespace UniversalAnimeDownloader.View
{
    class OnlineAnimeDetail : AnimeDetailBase
    {
        public OnlineAnimeDetailViewModel VM;
        public VuigheAnimeManager Data;
        private bool hasLoadGeneralData = false;
        private ComboBox cbxQuality;
        ListView episodeContainer;

        public OnlineAnimeDetail(VuigheAnimeManager data) : base()
        {
            VM = new OnlineAnimeDetailViewModel(Dispatcher);

            AddEpisodeAndDownloadUsingCode();
            DataContext = VM;
            cbxQuality.SelectedIndex = 3;
            cbxQuality.SelectionChanged += ChangeQuality;
            Data = data;
            ReceiveData();
        }


        /// <summary>
        /// Code-behind for epsiode and download used to be define in xaml
        /// </summary>
        private void AddEpisodeAndDownloadUsingCode()
        {
            Grid grdRoot = new Grid() { Margin = new Thickness(10) };
            grdRoot.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });
            grdRoot.ColumnDefinitions.Add(new ColumnDefinition());
            grdRoot.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50, GridUnitType.Pixel) });

            grdRoot.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50) });
            grdRoot.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(100) });
            grdRoot.RowDefinitions.Add(new RowDefinition());

            TextBlock txblTitle = new TextBlock() { Text = "Episodes and Download: ", FontWeight = FontWeights.Bold };
            txblTitle.SetResourceReference(FontSizeProperty, "Heading");
            grdRoot.Children.Add(txblTitle);

            Button btnDownloadAll = new Button() { Height = double.NaN };
            btnDownloadAll.Style = FindResource("MaterialDesignFlatButton") as Style;
            Grid.SetColumn(btnDownloadAll, 1);
            btnDownloadAll.Click += DownloadAll;
            grdRoot.Children.Add(btnDownloadAll);

            StackPanel stackPnl = new StackPanel() { Orientation = Orientation.Horizontal };
            btnDownloadAll.Content = stackPnl;

            PackIcon downloadIcon = new PackIcon() { Kind = PackIconKind.Download, Foreground = new SolidColorBrush(Colors.White), Height = double.NaN };
            downloadIcon.SetResourceReference(WidthProperty, "Heading");
            stackPnl.Children.Add(downloadIcon);

            System.Windows.Shapes.Rectangle rectangle = new System.Windows.Shapes.Rectangle() { Width = 10 };
            stackPnl.Children.Add(rectangle);

            TextBlock txblBtnContent = new TextBlock() { Text = "Download All" };
            txblBtnContent.SetResourceReference(TextBlock.FontSizeProperty, "Heading2");
            stackPnl.Children.Add(txblBtnContent);

            PopupBox popupBox = new PopupBox();
            Grid.SetColumn(popupBox, 2);
            grdRoot.Children.Add(popupBox);

            cbxQuality = new ComboBox();
            cbxQuality.Style = Application.Current.FindResource("cbxQualitySelector") as Style;
            grdRoot.Children.Add(cbxQuality);

            episodeContainer = new ListView();
            episodeContainer.Style = Application.Current.Resources["listViewDownloadTemplate"] as Style;
            grdRoot.Children.Add(episodeContainer);

            ProgressBar progressBar = new ProgressBar();
            Grid.SetRow(progressBar, 2);
            Grid.SetColumnSpan(progressBar, 3);
            progressBar.VerticalAlignment = VerticalAlignment.Bottom;
            progressBar.Height = 5;
            progressBar.IsIndeterminate = true;
            Binding visBinding = new Binding();
            visBinding.Source = VM.IsEpisodeLoading;
            visBinding.Converter = FindResource("boolToInvisConverter") as ValueConverter.InvertableBooleanToVisibilityConverter;
            visBinding.ConverterParameter = "Normal";
            BindingOperations.SetBinding(progressBar, VisibilityProperty, visBinding);

            animeEpisodes.Content = grdRoot;
        }

        public OnlineAnimeDetail()
        {
            VM = new OnlineAnimeDetailViewModel(Dispatcher);
            AddEpisodeAndDownloadUsingCode();
            DataContext = VM;

            cbxQuality.SelectedIndex = 3;
        }

        private void ChangeQuality(object sender, SelectionChangedEventArgs e) => ReceiveData();

        private async void ReceiveData()
        {
            if (!hasLoadGeneralData)
            {
                ReceiveGeneralData();
                hasLoadGeneralData = true;
            }

            //Delete The Itemsources
            VM.AnimeEpisodes.Clear();
            await Task.Delay(10);

            EpisodeList epList = await Data.GetEpisodeList();

            foreach (EpisodeInfo item in epList.data)
            {
                if (item != null)
                {
                    OnlineEpisodesListViewModel model = new OnlineEpisodesListViewModel();
                    model.EpisodeName = item.Full_Name;
                    model.ButtonKind = PackIconKind.Download;
                    VM.AnimeEpisodes.Add(model);
                    await Task.Delay(10);
                }

            }
        }

        //This method will receive data such as Description and Name
        private void ReceiveGeneralData()
        {
            VM.AnimeTitle = Data.CurrentFilm.Name;
            VM.AnimeDescription = Data.CurrentFilm.Description;

            //Add Anime Genres From source.
            for (int i = 0; i < Data.CurrentFilm.Genres.Data.Length; i++)
            {
                VM.AnimeGemres += Data.CurrentFilm.Genres.Data[i].Name;
                if (i != (Data.CurrentFilm.Genres.Data.Length - 1))
                    VM.AnimeGemres += ", ";
            }
        }

        private async void DownloadAll(object sender, RoutedEventArgs e)
        {
            VM.IsDownloadButtonEnabled = false;

            //mng.ComponentProgressChanged += UpdateProgressToViewModel;
            //await mng.DownloadAllAnimeAsync();

            SegmentedDownloadManager segmentedDownload = (SegmentedDownloadManager)DownloadManagerBase.Create(Data, "AnimeLibrary", DownloadMethod.Segmented, 2, 8, 50);
            segmentedDownload.ReportProgressRate = 50;
            segmentedDownload.ComponentProgressChanged += ReportProgress;
            await segmentedDownload.DownloadAllAnimeAsync();

            TaskCompletePopup popup = new TaskCompletePopup()
            {
                PopupTitle = "Download Completed",
                PopupText = $"{VM.AnimeTitle} has finished downloading! To see your downloaded anime, visit your anime library or Window Explorer",
                Style = Application.Current.Resources["DarkThemePopup"] as Style,
            };
            Grid.SetRowSpan(popup, 2);

            root.Children.Add(popup);
            popup.DialogOpen = true;
        }

        private void ReportProgress(object sender, DownloadManagerProgressChangedEventArgs e)
        {
            var queryResult = VM.AnimeEpisodes.Where(query => e.VideoSource.EpisodeName == query.EpisodeName).ToList();
            if (queryResult.Count == 0)
                return;

            var episodeViewModel = queryResult[0];
            episodeViewModel.DetailVisibility = Visibility.Visible;
            episodeViewModel.Progress = e.Progress;
            episodeViewModel.ByteReceived = e.Transfered / 1048576d;
            episodeViewModel.ByteToReceive = e.FileSize / 1048576d;

            //Check the eta:
            //if eta return 0:00:00 so the download is not segmented, the event will end
            //else the downloader is segmeted, the event will report additional information

            if (e.Eta == TimeSpan.Zero)
                return;

            episodeViewModel.SetDownloadRate(e.DownloadRate);
            episodeViewModel.SetEta(e.Eta);
        }
    }
}
