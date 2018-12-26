using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using UniversalAnimeDownloader.ViewModel;

namespace UniversalAnimeDownloader.UserControls
{
    /// <summary>
    /// Interaction logic for UADPlayer.xaml
    /// </summary>
    public partial class UADPlayer : UserControl
    {
        public UADPlayerViewModel VM;
        public TimeSpan MediaDuration;
        private bool isSeekSliderLocked = false;

        public Uri VideoUri
        {
            get { return (Uri)GetValue(VideoUriProperty); }
            set {
                SetValue(VideoUriProperty, value);
                mediaPlayer.Source = value;
                mediaPlayer.Play();
            }
        }
        public static readonly DependencyProperty VideoUriProperty =
            DependencyProperty.Register("VideoUri", typeof(Uri), typeof(UADPlayer), new PropertyMetadata());


        private bool isPlaying = true;
        public UADPlayer()
        {
            InitializeComponent();
            VM = new UADPlayerViewModel();
            DataContext = VM;
            mediaPlayer.Source = VideoUri;
            mediaPlayer.Play();
        }

        private void PlayPauseButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            PackIcon pkIcon = btn.Content as PackIcon;

            if (isPlaying)
            {
                mediaPlayer.Pause();
                pkIcon.Kind = PackIconKind.Play;
            }
            else
            {
                mediaPlayer.Play();
                pkIcon.Kind = PackIconKind.Pause;
            }
            isPlaying = !isPlaying;
        }

        private void Back10Sec(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Position -= TimeSpan.FromSeconds(10);
        }

        private void Forward30Sec(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Position += TimeSpan.FromSeconds(30);
        }

        private void mediaPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            MediaDuration = mediaPlayer.NaturalDuration.TimeSpan;
            txblMediaLength.Text = MediaDuration.ToString();
            UpdatePosition();
        }

        private async void UpdatePosition()
        {
            while (true)
            {
                txblMediaPos.Text = mediaPlayer.Position.ToString(@"hh\:mm\:ss");
                if (!isSeekSliderLocked)
                    seekSlider.Value = Common.GetTimeSpanRatio(mediaPlayer.Position, MediaDuration);
                await Task.Delay(500);
            }
        }


        private void ChangePosition(object sender, MouseButtonEventArgs e)
        {
            mediaPlayer.Position = Common.MutiplyTimeSpan(MediaDuration, seekSlider.Value);
            isSeekSliderLocked = false;
        }

        private void LockSeekSlider(object sender, MouseButtonEventArgs e) => isSeekSliderLocked = true;

        private void VolumnChange(object sender, RoutedEventArgs e) => VolumeChanger.IsOpen = true;

        private void CloseVolumePopup(object sender, MouseEventArgs e) => VolumeChanger.IsOpen = false;

        private void ChangeWindowState(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            PackIcon icon = btn.Content as PackIcon;
            if (icon.Kind == PackIconKind.ArrowExpand)
            {
                OnRequestWindowState(WindowState.Maximized);
                icon.Kind = PackIconKind.ArrowCollapse;
            }
            else
            {
                OnRequestWindowState(WindowState.Normal);
                icon.Kind = PackIconKind.ArrowExpand;
            }
        }

        public event EventHandler<RequestingWindowStateEventArgs> RequestWindowState;
        protected virtual void OnRequestWindowState(WindowState state) => RequestWindowState?.Invoke(this, new RequestingWindowStateEventArgs() { RequestState = state });

    }
}
