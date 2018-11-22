using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using uadcorelib;
using uadcorelib.Models;
using UniversalAnimeDownloader.CustomControl;
using UniversalAnimeDownloader.ViewModel;

namespace UniversalAnimeDownloader.View
{
    /// <summary>
    /// Interaction logic for OnlineAnimeDetail.xaml
    /// </summary>
    public partial class OnlineAnimeDetail : Page
    {
        public Frame HostFrame
        {
            get { return (Frame)GetValue(HostFrameProperty); }
            set { SetValue(HostFrameProperty, value); }
        }
        public static readonly DependencyProperty HostFrameProperty =
            DependencyProperty.Register("HostFrame", typeof(Frame), typeof(OnlineAnimeDetail), new PropertyMetadata());
        
        public OnlineAnimeDetailViewModel VM;
        public VuigheAnimeManager Data;
        private Thread LoaderThread;
        private bool hasLoadGeneralData = false;

        public OnlineAnimeDetail(VuigheAnimeManager data)
        {
            VM = new OnlineAnimeDetailViewModel(Dispatcher);
            DataContext = VM;

            InitializeComponent();
            cbxQuality.SelectedIndex = 3;

            cbxQuality.SelectionChanged += ChangeQuality;

            Data = data;

            LoaderThreadManageer(ReceiveData);
        }

        public OnlineAnimeDetail()
        {
            VM = new OnlineAnimeDetailViewModel(Dispatcher);
            DataContext = VM;

            InitializeComponent();
            cbxQuality.SelectedIndex = 3;

        }

        private void ChangeQuality(object sender, SelectionChangedEventArgs e) => LoaderThreadManageer(ReceiveData);

        private void LoaderThreadManageer(Action action)
        {
            //Kill the last running thread
            if(LoaderThread != null)
                if (LoaderThread.ThreadState == ThreadState.Background)
                    LoaderThread.Abort();

            LoaderThread = new Thread(new ThreadStart(action))
            {
                Name = "Loader Thread", IsBackground = true
            };
            LoaderThread.SetApartmentState(ApartmentState.STA);
            LoaderThread.Start();
        }

        private void ReceiveData()
        {
            if(!hasLoadGeneralData)
            {
                ReceiveGeneralData();
                hasLoadGeneralData = true;
            }

            //Delete The Itemsources
            Dispatcher.Invoke(() => VM.AnimeEpisodes.Clear());
            Thread.Sleep(10); //Updating UI

            foreach (VideoSource item in Data.GetVideoSourcesAsync(VM.Quality))
            {
                if (item != null)
                {
                    OnlineEpisodesListViewModel model = new OnlineEpisodesListViewModel();
                    model.EpisodeName = item.EpisodeName;
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
            DownloadManager mng = new DownloadManager(Data, "AnimeLibrary");
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
