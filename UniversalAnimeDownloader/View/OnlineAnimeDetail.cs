﻿using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
        private Thread LoaderThread;
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
            LoaderThreadManageer(ReceiveData);
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

        private void ChangeQuality(object sender, SelectionChangedEventArgs e) => LoaderThreadManageer(ReceiveData);

        private void LoaderThreadManageer(Action action)
        {
            //Kill the last running thread
            if (LoaderThread != null)
                if (LoaderThread.ThreadState == ThreadState.Background)
                    LoaderThread.Abort();

            LoaderThread = new Thread(new ThreadStart(action))
            {
                Name = "Loader Thread",
                IsBackground = true
            };
            LoaderThread.SetApartmentState(ApartmentState.STA);
            LoaderThread.Start();
        }

        private async void ReceiveData()
        {
            if (!hasLoadGeneralData)
            {
                ReceiveGeneralData();
                hasLoadGeneralData = true;
            }

            //Delete The Itemsources
            Dispatcher.Invoke(() => VM.AnimeEpisodes.Clear());
            Thread.Sleep(10); //Updating UI

            //Anti racing effect
            while (VM.Quality == null) { }
            EpisodeList epList = await Data.GetEpisodeList();

            foreach (EpisodeInfo item in epList.data)
            {
                if (item != null)
                {
                    OnlineEpisodesListViewModel model = new OnlineEpisodesListViewModel();
                    model.EpisodeName = item.Full_Name;
                    model.ButtonKind = PackIconKind.Download;
                    Dispatcher.Invoke(() => VM.AnimeEpisodes.Add(model));
                    Thread.Sleep(10);
                }

            }
        }

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
            MultipleFilesDownloadManager mng = new MultipleFilesDownloadManager(Data, "AnimeLibrary");
            mng.ComponentProgressChanged += UpdateProgressToViewModel;
            await mng.DownloadAllAnimeAsync();

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

        private void UpdateProgressToViewModel(object sender, DownloadProgressChangedEventArgs e)
        {
            VideoSourceWithWebClient src = sender as VideoSourceWithWebClient;
            var find = VM.AnimeEpisodes.Where(query => query.EpisodeName == src.VideoSource.EpisodeName).ToList();
            if (find.Count == 0)
                return;

            find[0].DetailVisibility = Visibility.Visible;

            find[0].Progress = e.ProgressPercentage;
            find[0].ByteReceived = e.BytesReceived / 1048576d;
            find[0].ByteToReceive = e.TotalBytesToReceive / 1048576d;
        }
    }
}
