using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using uadcorelib;
using uadcorelib.Models;
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

        public OnlineAnimeDetail(VuigheAnimeManager data)
        {
            VM = new OnlineAnimeDetailViewModel(Dispatcher);
            DataContext = VM;

            InitializeComponent();

            Data = data;
            Thread thd = new Thread(ReceiveData);
            thd.IsBackground = true;
            thd.SetApartmentState(ApartmentState.STA);
            thd.Start();
        }

        public OnlineAnimeDetail()
        {
            VM = new OnlineAnimeDetailViewModel(Dispatcher);
            DataContext = VM;

            InitializeComponent();
        }

        private void ReceiveData()
        {
            //Jus hard code for now, will change later.
            Data.GetDetailData(QualityOption.Qualitym720p);

            VM.AnimeTitle = Data.CurrentFilm.Name;
            VM.AnimeDescription = Data.CurrentFilm.Description;

            //Add Anime Genres From source.
            for (int i = 0; i < Data.CurrentFilm.Genres.Data.Length; i++)
            {
                VM.AnimeGemres += Data.CurrentFilm.Genres.Data[i].Name;
                if (i != (Data.CurrentFilm.Genres.Data.Length - 1))
                    VM.AnimeGemres += ", ";
            }

            foreach (VideoSource item in Data.VideoSources)
            {
                if (item != null)
                {
                    OnlineEpisodesListViewModel model = new OnlineEpisodesListViewModel();
                    model.EpisodeName = item.Name;
                    model.ButtonKind = PackIconKind.Download;
                    Dispatcher.Invoke(() => VM.AnimeEpisodes.Add(model));
                    Thread.Sleep(10);
                }
                
            }
        }
    }
}
