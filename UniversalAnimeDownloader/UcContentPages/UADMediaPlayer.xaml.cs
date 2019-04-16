using MaterialDesignThemes.Wpf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UADAPI;

namespace UniversalAnimeDownloader.UcContentPages
{
    /// <summary>
    /// Interaction logic for UADMediaPlayer.xaml
    /// </summary>
    public partial class UADMediaPlayer : UserControl
    {
        public int PlayIndex
        {
            get { return (int)GetValue(PlayIndexProperty); }
            set { SetValue(PlayIndexProperty, value); }
        }
        public static readonly DependencyProperty PlayIndexProperty =
            DependencyProperty.Register("PlayIndex", typeof(int), typeof(UADMediaPlayer), new PropertyMetadata());

        public AnimeSeriesInfo Playlist
        {
            get { return (AnimeSeriesInfo)GetValue(PlaylistProperty); }
            set {SetValue(PlaylistProperty, value);}
        }
        public static readonly DependencyProperty PlaylistProperty =
            DependencyProperty.Register("Playlist", typeof(AnimeSeriesInfo), typeof(UADMediaPlayer), new PropertyMetadata(PlaylistChanged));

        public MediaPlayerState State
        {
            get { return (MediaPlayerState)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }
        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register("State", typeof(MediaPlayerState), typeof(UADMediaPlayer), new PropertyMetadata(MediaPlayerState.Stop, MediaPlayerStateChanged));


        public int PlayableMediaCount { get; set; }

        private static void PlaylistChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var info = e.NewValue as AnimeSeriesInfo;
            if (info.Episodes == null)
                return;
            for (int i = 0; i < info.Episodes.Count; i++)
            {
                if (info.Episodes[i].AvailableOffline)
                    (d as UADMediaPlayer).PlayIndex = i;
            }

            (d as UADMediaPlayer).PlayableMediaCount = info.Episodes.Count(p => p.AvailableOffline);
        }
        private static void MediaPlayerStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            switch ((MediaPlayerState)e.NewValue)
            {
                case MediaPlayerState.Pause:
                    break;
                case MediaPlayerState.Play:
                    Play(d, e);
                    break;
                case MediaPlayerState.Stop:
                    break;
                default:
                    break;
            }
        }

        private static void Play(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var media = d as UADMediaPlayer;
            if ((MediaPlayerState)e.OldValue == MediaPlayerState.Pause)
                return;
            var info = GetNearestEpisode(media, false, false);
            var loc = info.FilmSources.Last(query => query.Value.IsFinishedRequesting).Value.LocalFile;
            media.player.Source = new Uri(loc);
            string imageSrc = info.Thumbnail.Url;
            if (!string.IsNullOrEmpty(info.Thumbnail.LocalFile))
                imageSrc = info.Thumbnail.LocalFile;
            media.imgAnimeThumbnail.Source = new BitmapImage(new Uri(imageSrc));
            media.txblTitle.Text = media.Playlist.Name;
            media.txblDes.Text = info.Name;
            media.player.Play();
        }

        public static EpisodeInfo GetNearestEpisode(UADMediaPlayer media, bool skipCurrent, bool isBack )
        {
            if(media.Playlist == null)
                return null;
            EpisodeInfo episode = null;
            int currentIndex = media.PlayIndex;
            if (skipCurrent)
                if (isBack)
                    currentIndex = currentIndex == 0 ? media.Playlist.Episodes.Count - 1 : currentIndex - 1;
                else
                    currentIndex = currentIndex == media.Playlist.Episodes.Count - 1 ? 0 : currentIndex + 1;

            do
            {
                episode = media.Playlist.Episodes[currentIndex];
                if(episode != null)
                {
                    if(episode.AvailableOffline)
                    {
                        media.PlayIndex = currentIndex;
                        return episode;
                    }
                }

                if (isBack)
                    currentIndex = currentIndex < 2 ? media.Playlist.Episodes.Count - 1 : currentIndex - 1;
                else
                    currentIndex = media.Playlist.Episodes.Count - 1 > currentIndex ? currentIndex + 1 : 0;
            } while (currentIndex != media.PlayIndex);

            return null;
        }

        public TimeSpan MediaDuration;
        private Random rand = new Random();
        private DateTime LastClick;

        private int currentHideControllerTimeOutID;



        private double _Volume;
        private bool isSeekSliderLocked;

        public double Volume
        {
            get { return _Volume; }
            set
            {
                if(_Volume != value)
                {
                    _Volume = value;
                    player.Volume = value / 100d;
                }
            }
        }

        private bool isControllerVisible;
        private bool IsControllerVisible
        {
            get { return isControllerVisible; }
            set
            {
                if (isControllerVisible != value)
                {
                    isControllerVisible = value;
                    HideControllerTimeout(rand.Next(1, 100));
                }
            }
        }

        private async void HideControllerTimeout(int timeoutID)
        {
            currentHideControllerTimeOutID = timeoutID;
            int timeoutLeft = 5000;

            while (true)
            {
                await Task.Delay(100);
                if (timeoutID != currentHideControllerTimeOutID || controller.Opacity < 0.01)
                    return;
                else
                    timeoutLeft -= 100;

                if (timeoutLeft == 0)
                {
                    MiscClass.FadeOutAnimation(controller, TimeSpan.FromSeconds(.25), false);
                    return;
                }
            }
        }

        public UADMediaPlayer()
        {
            InitializeComponent();
            newTest.Value = player.Volume * 100;
        }

        private void PlayPauseButton_Click(object sender, RoutedEventArgs e) => PlayPauseMedia();

        private void PlayPauseMedia()
        {
            PackIcon pkIcon = btnPlayPause.Content as PackIcon;

            if (State == MediaPlayerState.Play)
            {
                player.Pause();
                pkIcon.Kind = PackIconKind.Play;
                MiscClass.FadeInAnimation(grdFilmProperty, TimeSpan.FromSeconds(0.25), false);
                PointAnimation animation = new PointAnimation(new Point(0, 1), TimeSpan.FromSeconds(0.25));
                controllerBackgroundGradient.BeginAnimation(LinearGradientBrush.EndPointProperty, animation);
                State = MediaPlayerState.Pause;
            }
            else if (State == MediaPlayerState.Pause)
            {
                player.Play();
                pkIcon.Kind = PackIconKind.Pause;
                MiscClass.FadeOutAnimation(grdFilmProperty, TimeSpan.FromSeconds(0.25), false);
                PointAnimation animation = new PointAnimation(new Point(0, .5), TimeSpan.FromSeconds(0.25));
                controllerBackgroundGradient.BeginAnimation(LinearGradientBrush.EndPointProperty, animation);
                State = MediaPlayerState.Play;
            }
        }

        private void VolumnChange(object sender, RoutedEventArgs e) => VolumeChanger.IsOpen = true;

        private void CloseVolumePopup(object sender, MouseEventArgs e) => VolumeChanger.IsOpen = false;

        private void VideoSpeedChange_Popup(object sender, RoutedEventArgs e) => VideoSpeedChanger.IsOpen = true;

        private void CloseVideoSpeedPopup(object sender, MouseEventArgs e) => VideoSpeedChanger.IsOpen = false;

        private void ChangePlaySpeedSlider(object sender, DragCompletedEventArgs e)
        {
            player.SpeedRatio = speedSlider.Value;
            player.Position -= TimeSpan.FromMilliseconds(0.001);
        }

        private void Back10Sec(object sender, RoutedEventArgs e) => player.Position -= TimeSpan.FromSeconds(10);

        private void Forward30Sec(object sender, RoutedEventArgs e) => player.Position += TimeSpan.FromSeconds(30);

        private void LockSeekSlider(object sender, MouseButtonEventArgs e) => isSeekSliderLocked = true;

        private void ChangePosition(object sender, MouseButtonEventArgs e)
        {
            player.Position = MiscClass.MutiplyTimeSpan(MediaDuration, seekSlider.Value);
            isSeekSliderLocked = false;
        }

        private void ChangeWindowState(object sender, RoutedEventArgs e)
        {

        }

        private void ScreenBlocker_Click(object sender, RoutedEventArgs e)
        {

        }

        private void FakeAppCrash_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BackgroundPlayer_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ToggleCotrollerBar(object sender, MouseButtonEventArgs e)
        {
            ToggleControllerBar();
            CalculateDoubleClick();
        }

        private void ToggleDrawing(object sender, RoutedEventArgs e)
        {
          
        }

        private void ToggleControllerBar()
        {
            if (IsControllerVisible)
            {
                MiscClass.FadeOutAnimation(controller, TimeSpan.FromSeconds(.5), false);
                controller.IsHitTestVisible = false;
            }
            else
            {
                MiscClass.FadeInAnimation(controller, TimeSpan.FromSeconds(.5), false);
                controller.IsHitTestVisible = true;
            }

            IsControllerVisible = !IsControllerVisible;
        }

        private void CalculateDoubleClick()
        {
            if (LastClick == null)
            {
                LastClick = DateTime.Now;
                return;
            }

            if (DateTime.Now - LastClick < TimeSpan.FromSeconds(0.5))
                DoubleClickTriggered();
            else
                LastClick = DateTime.Now;
        }

        private void DoubleClickTriggered() => ChangeWindowState();

        private void ChangeWindowState()
        {
            PackIcon icon = btnFullScreenToggle.Content as PackIcon;
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


        private void Event_ChangeVolume(object sender, DragCompletedEventArgs e)
        {
            var slider = sender as Slider;
            Volume = slider.Value;
        }

        private void Player_MediaOpened(object sender, RoutedEventArgs e)
        {
            MediaDuration = player.NaturalDuration.TimeSpan;
            txblMediaLength.Text = MediaDuration.ToString();
            UpdatePosition();
        }

        private async void UpdatePosition()
        {
            while (true)
            {
                txblMediaPos.Text = player.Position.ToString(@"hh\:mm\:ss");
                if (!isSeekSliderLocked)
                    seekSlider.Value = MiscClass.GetTimeSpanRatio(player.Position, MediaDuration);
                await Task.Delay(500);
            }
        }

        public event EventHandler<RequestingWindowStateEventArgs> RequestWindowState;
        public void OnRequestWindowState(WindowState state) => RequestWindowState?.Invoke(this, new RequestingWindowStateEventArgs() { RequestState = state });

        private void Event_MediaPlayerMinimize(object sender, RoutedEventArgs e) => OnRequestWindowState(WindowState.Minimized);
    }

    public enum MediaPlayerState
    {
        Pause, Play, Stop
    }

    public class RequestingWindowStateEventArgs : EventArgs
    {
        public WindowState RequestState { get; set; }
    }

    public class RequestWindowIconChangeEventArgs : EventArgs
    {
        public Uri IconLocation { get; set; }
    }
}
