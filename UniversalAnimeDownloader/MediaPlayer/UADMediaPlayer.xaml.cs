using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using UADAPI;
using UniversalAnimeDownloader.Animations;
using UniversalAnimeDownloader.UADSettingsPortal;

namespace UniversalAnimeDownloader.MediaPlayer
{
    /// <summary>
    /// Interaction logic for UADMediaPlayer.xaml
    /// </summary>
    public partial class UADMediaPlayer : UserControl
    {
        #region Fields and Properties
        public UADPlayerViewModel VM { get; set; }
        public bool IsSeekSliderLocked = false;

        private Random rand = new Random();
        private bool isToolboxBarHold = false;
        private bool isColorBarHold = false;
        private Point mouseOffsetToBar = new Point();
        private string lastCapImgLocation;
        private int currentHideControllerTimeOutID;
        private DateTime LastClick;
        public bool NoPlayableMedia { get; set; }

        public bool IsBackgroundPlayerActive { get; set; }
        public bool IsFakeCrashActive { get; set; }

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

        private bool isSidePanelOpen = false;
        public bool IsSidePanelOpen
        {
            get => isSidePanelOpen;
            set
            {
                if(isSidePanelOpen != value)
                {
                    DoubleAnimation transitionAnim = new DoubleAnimation(sideBar.Width, 350, TimeSpan.FromSeconds(0.5)) { DecelerationRatio = 0.1, EasingFunction = new QuarticEase() { EasingMode = EasingMode.EaseOut } };
                    if (!value)
                        transitionAnim.To = 0;

                    sideBar.BeginAnimation(WidthProperty ,transitionAnim);

                    isSidePanelOpen = value;
                }
                
            }
        }

       
        public FakeNotRespondingDialog FakeHost { get; set; }
        private DispatcherTimer _Timer;
        #endregion

        #region Dependency Properties
        public double PlayProgress
        {
            get { return (double)GetValue(PlayProgressProperty); }
            set { SetValue(PlayProgressProperty, value); }
        }
        public static readonly DependencyProperty PlayProgressProperty =
            DependencyProperty.Register("PlayProgress", typeof(double), typeof(UADMediaPlayer), new PropertyMetadata(0d));

        public TimeSpan MediaDuration
        {
            get { return (TimeSpan)GetValue(MediaDurationProperty); }
            set { SetValue(MediaDurationProperty, value); }
        }
        public static readonly DependencyProperty MediaDurationProperty =
            DependencyProperty.Register("MediaDuration", typeof(TimeSpan), typeof(UADMediaPlayer), new PropertyMetadata());

        public TimeSpan MediaPosition
        {
            get { return (TimeSpan)GetValue(MediaPositionProperty); }
            set { SetValue(MediaPositionProperty, value); }
        }
        public static readonly DependencyProperty MediaPositionProperty =
            DependencyProperty.Register("MediaPosition", typeof(TimeSpan), typeof(UADMediaPlayer), new PropertyMetadata());

        public int PlayIndex
        {
            get { return (int)GetValue(PlayIndexProperty); }
            set { SetValue(PlayIndexProperty, value); }
        }
        public static readonly DependencyProperty PlayIndexProperty =
            DependencyProperty.Register("PlayIndex", typeof(int), typeof(UADMediaPlayer), new PropertyMetadata(-1, UpdateMediaElementSource));

        private static void UpdateMediaElementSource(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ins = d as UADMediaPlayer;
            if (ins.IsPlayOnline)
                OnlineUpdateMediaElementSource(d, e);
            else
                OfflineUpdateMediaElementSource(d, e);
        }

        private static async void OnlineUpdateMediaElementSource(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ins = d as UADMediaPlayer;
            var newVal = (int)e.NewValue;
            if (newVal == -1)
                return;

            //Base class "UADMediaPlyayer" make Playlist return null when call Reset()
            if (ins.Playlist == null)
                return;

            var currentEpisode = ins.Playlist.Episodes[(int)e.NewValue];

            //if the IAnimeSeriesManager.GetEpisode() is not call yet
            if (currentEpisode.FilmSources == null || currentEpisode.FilmSources.Count == 0)
            {
                var manager = ApiHelpper.CreateAnimeSeriesManagerObjectByClassName(ins.Playlist.ModInfo.ModTypeString);
                manager.AttachedAnimeSeriesInfo = ins.Playlist;
                await manager.GetEpisodes(new List<int> { currentEpisode.EpisodeID });
            }
            //Add video quality
            ins.VM.VideoQuality.Clear();
            foreach (var item in currentEpisode.FilmSources)
            {
                ins.VM.VideoQuality.Add(item);
            }
            (ins.popupQuality.PopupContent as ListBox).SelectedIndex = 0;

            var episodeInfo = ins.Playlist.Episodes[(int)e.NewValue];
            ins.AnimeThumbnail = new BitmapImage(new Uri(episodeInfo.Thumbnail.Url));
            ins.Title = ins.Playlist.Name;
            ins.SubbedTitle = episodeInfo.Name;
        }

        private static async void OfflineUpdateMediaElementSource(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ins = d as UADMediaPlayer;
            if ((int)e.NewValue == -1)
            {
                ins.NoPlayableMedia = true;
                return;
            }

            ins.NoPlayableMedia = false;
            //Reset cause the playlist return null
            if (ins.Playlist == null)
                return;

            if (ins.Playlist.Episodes[(int)e.NewValue].FilmSources == null)
            {
                ins.Next();
                return;
            }

            var source = ins.Playlist.Episodes[(int)e.NewValue].FilmSources.Where(p => !string.IsNullOrEmpty(p.Value.LocalFile));
            if (source.Count() != 0)
            {
                ins.VideoUri = new Uri(source.First().Value.LocalFile);
                var episodeInfo = ins.Playlist.Episodes[(int)e.NewValue];

                string localThumbnailSrc = episodeInfo.Thumbnail.LocalFile;
                if (!string.IsNullOrEmpty(localThumbnailSrc))
                    ins.AnimeThumbnail = new BitmapImage(new Uri(localThumbnailSrc));
                else
                {
                    if (await ApiHelpper.CheckForInternetConnection())
                        ins.AnimeThumbnail = new BitmapImage(new Uri(episodeInfo.Thumbnail.Url));
                    else
                        ins.AnimeThumbnail = new BitmapImage();
                }

                ins.Title = ins.Playlist.Name;
                ins.SubbedTitle = episodeInfo.Name;
            }
            else
            {
                //ins.Next();

                ins.NoPlayableMedia = true;
                return;
            }
        }

        public AnimeSeriesInfo Playlist
        {
            get { return (AnimeSeriesInfo)GetValue(PlaylistProperty); }
            set { SetValue(PlaylistProperty, value); }
        }
        public static readonly DependencyProperty PlaylistProperty =
            DependencyProperty.Register("Playlist", typeof(AnimeSeriesInfo), typeof(UADMediaPlayer), new PropertyMetadata(null, PreparePlayList));

        private Uri _VideoUri;
        public Uri VideoUri
        {
            get => _VideoUri;
            set
            {
                if(_VideoUri != value)
                {
                    _VideoUri = value;
                    mediaPlayer.Source = value;
                }
            }
        }

        public Thickness CaptureOffset
        {
            get { return (Thickness)GetValue(CaptureOffsetProperty); }
            set { SetValue(CaptureOffsetProperty, value); }
        }
        public static readonly DependencyProperty CaptureOffsetProperty =
            DependencyProperty.Register("CaptureOffset", typeof(Thickness), typeof(UADMediaPlayer), new PropertyMetadata(new Thickness(10, 70, 20, 90)));

        public bool ShowWatermask
        {
            get { return (bool)GetValue(ShowWatermaskProperty); }
            set { SetValue(ShowWatermaskProperty, value); }
        }
        public static readonly DependencyProperty ShowWatermaskProperty =
            DependencyProperty.Register("ShowWatermask", typeof(bool), typeof(UADMediaPlayer), new PropertyMetadata(true));

        public ImageSource AnimeThumbnail
        {
            get { return (ImageSource)GetValue(AnimeThumbnailProperty); }
            set
            {
                SetValue(AnimeThumbnailProperty, value);
                imgAnimeThumbnail.Source = value;
            }
        }
        public static readonly DependencyProperty AnimeThumbnailProperty =
            DependencyProperty.Register("AnimeThumbnail", typeof(ImageSource), typeof(UADMediaPlayer), new PropertyMetadata());

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set
            {
                SetValue(TitleProperty, value);
                txblTitle.Text = value;
            }
        }
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(UADMediaPlayer), new PropertyMetadata("Title"));

        public string SubbedTitle
        {
            get { return (string)GetValue(SubbedTitleProperty); }
            set
            {
                SetValue(SubbedTitleProperty, value);
                txblDes.Text = value;
            }
        }
        public static readonly DependencyProperty SubbedTitleProperty =
            DependencyProperty.Register("SubbedTitle", typeof(string), typeof(UADMediaPlayer), new PropertyMetadata("Description"));
        #endregion

        private static void PreparePlayList(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
                return;
            var ins = d as UADMediaPlayer;
            ins.mediaPlayer.Stop();
            ins.PlayIndex = 0;
            if ((Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.PlayMediaFullScreen)
            {
                ins.OnRequestWindowState(WindowState.Maximized);
                (ins.btnFullScreenToggle.Content as PackIcon).Kind = PackIconKind.ArrowCollapse;
            }
            ins.strokeThicknessSlider.Value = (Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.PrimaryBurshThickness;
        }

        /// <summary>
        /// Get the nearestt episode
        /// </summary>
        /// <param name="media">The <see cref="UADMediaPlayer"/> instance to search</param>
        /// <param name="skipCurrent">Skip the current index by +1 or -1 the index</param>
        /// <param name="isBack">Search in forward or backward order</param>
        /// <returns>The index of the <see cref="AnimeSeriesInfo.Episodes"/> in the currnt <see cref="UADMediaPlayer"/> insntance </returns>
        public static int GetNearestEpisode(UADMediaPlayer media, bool skipCurrent, bool isBack)
        {
            if (media.Playlist == null)
                return -1;
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
                if (episode != null)
                {
                    if (episode.AvailableOffline)
                        return currentIndex;
                }

                if (isBack)
                    currentIndex = currentIndex < 1 ? media.Playlist.Episodes.Count - 1 : currentIndex - 1;
                else
                    currentIndex = media.Playlist.Episodes.Count - 1 > currentIndex ? currentIndex + 1 : 0;
            } while (currentIndex != media.PlayIndex);
            if (skipCurrent)
                return media.PlayIndex;
            else
                return -1;
        }



        public bool IsPlaying
        {
            get { return (bool)GetValue(IsPlayingProperty); }
            set { SetValue(IsPlayingProperty, value); }
        }
        private static readonly DependencyProperty IsPlayingProperty =
            DependencyProperty.Register("IsPlaying", typeof(bool), typeof(UADMediaPlayer), new PropertyMetadata(true));

        public bool IsPause
        {
            get { return (bool)GetValue(IsPauseProperty); }
            set { SetValue(IsPauseProperty, value); }
        }
        public static readonly DependencyProperty IsPauseProperty =
            DependencyProperty.Register("IsPause", typeof(bool), typeof(UADMediaPlayer), new PropertyMetadata(false));

        public bool IsPlayOnline
        {
            get { return (bool)GetValue(IsPlayOnlineProperty); }
            set { SetValue(IsPlayOnlineProperty, value); }
        }
        public static readonly DependencyProperty IsPlayOnlineProperty =
            DependencyProperty.Register("IsPlayOnline", typeof(bool), typeof(UADMediaPlayer), new PropertyMetadata(false, PlayOnlinePropertyChanged));

        private static void PlayOnlinePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ins = d as UADMediaPlayer;

            ins.popupQuality.Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Hidden;
        }

        public UADMediaPlayer()
        {
            VM = new UADPlayerViewModel();
            InitializeComponent();

            //Online media player
            _Timer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(1) };
            _Timer.Tick += UpdateBufferingStatus;
        }

        private void UpdateBufferingStatus(object sender, EventArgs e)
        {
            bufferingIndicator.Visibility = mediaPlayer.IsBuffering ? Visibility.Visible : Visibility.Collapsed;
        }

        private void Event_MediaPlayerHostLoaded(object sender, RoutedEventArgs e)
        {
           // (Content as FrameworkElement).DataContext = VM;
            mediaPlayer.Source = VideoUri;
            isControllerVisible = true;
            string tt = AppDomain.CurrentDomain.BaseDirectory + "unnamed.ico";
            mediaPlayer.Pause();

            if ((Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.PlayMediaFullScreen)
            {
                OnRequestWindowState(WindowState.Maximized);
                (btnFullScreenToggle.Content as PackIcon).Kind = PackIconKind.ArrowCollapse;
            }
            strokeThicknessSlider.Value = (Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.PrimaryBurshThickness;

            //Set the property previously add in the VM
            VM.MediaElementVolume = (Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.PlaybackVolume / 100;
            VM.InkCanvasVisibility = Visibility.Collapsed;

            VM.PrimaryPen = new DrawingAttributes()
            {
                Color = (Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.PrimaryPenColor,
                Width = (Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.PrimaryBurshThickness,
                Height = (Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.PrimaryBurshThickness
            };

            VM.SecondaryPen = new DrawingAttributes()
            {
                Color = (Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.SecondaryPenColor,
                Width = (Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.SecondaryBurshThickness,
                Height = (Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.SecondaryBurshThickness
            };

            VM.HighlighterPen = new DrawingAttributes()
            {
                IsHighlighter = true,
                Color = (Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.HighlighterPenColor,
                Width = (Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.HighlighterBurshThickness,
                Height = (Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.HighlighterBurshThickness
            };
        
            //Change the fullsrceen icon when the host window state changed
            AssignProperty();
            var hostWindow = Window.GetWindow(this);
            var winState = hostWindow.WindowState;
            switch (winState)
            {
                case WindowState.Normal:
                    (btnFullScreenToggle.Content as PackIcon).Kind = PackIconKind.ArrowExpand;
                    (controlBarMaximize.Content as PackIcon).Kind = PackIconKind.WindowMaximize;
                    break;
                case WindowState.Minimized:
                    break;
                case WindowState.Maximized:
                    (btnFullScreenToggle.Content as PackIcon).Kind = PackIconKind.ArrowCollapse;
                    (controlBarMaximize.Content as PackIcon).Kind = PackIconKind.WindowRestore;
                    break;
                default:
                    break;
            }

            hostWindow.StateChanged += (s, ee) =>
            {
                var state = (s as Window).WindowState;
                switch (state)
                {
                    case WindowState.Normal:
                        (btnFullScreenToggle.Content as PackIcon).Kind = PackIconKind.ArrowExpand;
                        (controlBarMaximize.Content as PackIcon).Kind = PackIconKind.WindowMaximize;
                        break;
                    case WindowState.Minimized:
                        break;
                    case WindowState.Maximized:
                        (btnFullScreenToggle.Content as PackIcon).Kind = PackIconKind.ArrowCollapse;
                        (controlBarMaximize.Content as PackIcon).Kind = PackIconKind.WindowRestore;
                        break;
                    default:
                        break;
                }
            };
             
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
                    IsControllerVisible = false;
                    controller.IsHitTestVisible = false;
                    return;
                }
            }
        }

        #region PlayerGeneralControler
        private void AssignProperty()
        {
            imgAnimeThumbnail.Source = AnimeThumbnail;
            txblTitle.Text = Title;
            txblDes.Text = SubbedTitle;
        }

        private void AlwaysCanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;

        private void Command_Forward30Sec(object sender, ExecutedRoutedEventArgs e) => mediaPlayer.Position += TimeSpan.FromSeconds(30);

        private void Command_Previous10Sec(object sender, ExecutedRoutedEventArgs e) => mediaPlayer.Position -= TimeSpan.FromSeconds(10);

        private void Command_VolumeDown(object sender, ExecutedRoutedEventArgs e) => VM.PlayerVolume -= 5;

        private void Command_VolumeUp(object sender, ExecutedRoutedEventArgs e) => VM.PlayerVolume += 5;

        private void Command_QuitFullScreen(object sender, ExecutedRoutedEventArgs e)
        {
            OnRequestWindowState(WindowState.Normal);
            (btnFullScreenToggle.Content as PackIcon).Kind = PackIconKind.ArrowExpand;
        }

        private void PlayPauseButton_Click(object sender, RoutedEventArgs e) => PlayPauseMedia();

        private async void Command_PlayPauseMedia(object sender, ExecutedRoutedEventArgs e)
        {
            await PlayPauseMedia();
            MiscClass.FadeInAnimation(controller, TimeSpan.FromSeconds(.5), false);
            controller.IsHitTestVisible = true;
            IsControllerVisible = true;
        }

        public async Task PlayPauseMedia()
        {
            PackIcon pkIcon = btnPlayPause.Content as PackIcon;

            if (!IsPause)
            {
                pkIcon.Kind = PackIconKind.Play;
                MiscClass.FadeInAnimation(grdFilmProperty, TimeSpan.FromSeconds(0.25), false);
                PointAnimation animation = new PointAnimation(new Point(0, 1), TimeSpan.FromSeconds(0.25));
                controllerMask.BeginAnimation(LinearGradientBrush.EndPointProperty, animation);

                mediaPlayer.Pause();
            }
            else
            {
                pkIcon.Kind = PackIconKind.Pause;
                MiscClass.FadeOutAnimation(grdFilmProperty, TimeSpan.FromSeconds(0.25), false);

                PointAnimation animation = new PointAnimation(new Point(0, .5), TimeSpan.FromSeconds(0.25));
                controllerMask.BeginAnimation(LinearGradientBrush.EndPointProperty, animation);

                await Task.Delay(260);
                Play();
            }

            IsPause = !IsPause;
        }

        private void Back10Sec(object sender, RoutedEventArgs e) => mediaPlayer.Position -= TimeSpan.FromSeconds(10);

        private void Forward30Sec(object sender, RoutedEventArgs e) => mediaPlayer.Position += TimeSpan.FromSeconds(30);

        private void mediaPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            MediaDuration = mediaPlayer.NaturalDuration.TimeSpan;
            txblMediaLength.Text = MediaDuration.ToString(@"hh\:mm\:ss");
            UpdatePosition();
        }

        private async void UpdatePosition()
        {
            while (true)
            {
                txblMediaPos.Text = mediaPlayer.Position.ToString(@"hh\:mm\:ss");
                if (!IsSeekSliderLocked)
                {
                    var progressVal = MiscClass.GetTimeSpanRatio(mediaPlayer.Position, MediaDuration);
                    seekSlider.Value = progressVal;
                    PlayProgress = progressVal;
                    MediaPosition = mediaPlayer.Position;
                }

                await Task.Delay(500);
            }
        }

        private void ChangePosition(object sender, MouseButtonEventArgs e) => ChangePositionProgress(seekSlider.Value);

        /// <summary>
        /// Change the MediaPlayer Position based on the specified progress
        /// </summary>
        public void ChangePositionProgress(double progress)
        {
            mediaPlayer.Position = MiscClass.MutiplyTimeSpan(MediaDuration, progress);
            IsSeekSliderLocked = false;
            Focus();
        }

        private void LockSeekSlider(object sender, MouseButtonEventArgs e) => IsSeekSliderLocked = true;

        private void VolumnChange(object sender, RoutedEventArgs e)
        {
            VolumeChanger.IsOpen = true;
            Focus();
        }

        private void CloseVolumePopup(object sender, MouseEventArgs e)
        {
            VolumeChanger.IsOpen = false;
            Focus();
        }

        private void ChangeWindowState(object sender, RoutedEventArgs e)
        {
            ChangeWindowState();
            Focus();
        }

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

        private void ToggleDrawing(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            PackIcon packIcon = btn.Content as PackIcon;

            VM.InkCanvasVisibility = Visibility.Visible;
            inkCanvas.DefaultDrawingAttributes = VM.PrimaryPen;
        }

        private void ToggleCotrollerBar(object sender, MouseButtonEventArgs e)
        {
            ToggleControllerBar();
            CalculateDoubleClick();
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

        private void VideoSpeedChange_Popup(object sender, RoutedEventArgs e) => VideoSpeedChanger.IsOpen = true;

        private void CloseVideoSpeedPopup(object sender, MouseEventArgs e) => VideoSpeedChanger.IsOpen = false;

        private void ChangePlaySpeedSlider(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            mediaPlayer.SpeedRatio = speedSlider.Value;
            mediaPlayer.Position -= TimeSpan.FromMilliseconds(0.001);
        }

        private void FocusToThis(object sender, MouseButtonEventArgs e) => Focus();
        #endregion

        #region On-screen Drawing

        private void ChangePen(object sender, MouseButtonEventArgs e)
        {
            PackIcon icon = sender as PackIcon;
            inkCanvas.EditingMode = InkCanvasEditingMode.Ink;
            switch (icon.Kind)
            {
                case PackIconKind.Pencil:
                    inkCanvas.DefaultDrawingAttributes = VM.PrimaryPen ?? new DrawingAttributes();
                    break;
                case PackIconKind.Pen:
                    inkCanvas.DefaultDrawingAttributes = VM.SecondaryPen ?? new DrawingAttributes();
                    break;
                case PackIconKind.GreasePencil:
                    inkCanvas.DefaultDrawingAttributes = VM.HighlighterPen ?? new DrawingAttributes();
                    break;
                default:
                    throw new InvalidOperationException("Pen Not found!");
            }

            colorCanvas.SelectedColor = inkCanvas.DefaultDrawingAttributes.Color;
            strokeThicknessSlider.Value = inkCanvas.DefaultDrawingAttributes.Height;
        }

        private void ToggleEraser(object sender, MouseButtonEventArgs e)
        {
            PackIcon icon = sender as PackIcon;

            inkCanvas.EditingMode = icon.Kind == PackIconKind.Eraser ? InkCanvasEditingMode.EraseByPoint : InkCanvasEditingMode.EraseByStroke;
        }

        private void ToggleSelect(object sender, MouseButtonEventArgs e) => inkCanvas.EditingMode = InkCanvasEditingMode.Select;

        private void ChangeColor(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            DrawingAttributes tempAttib = null;

            if (inkCanvas.DefaultDrawingAttributes == VM.PrimaryPen)
            {
                tempAttib = VM.PrimaryPen;
                tempAttib.Color = (Color)e.NewValue;
                VM.PrimaryPen = tempAttib;
                inkCanvas.DefaultDrawingAttributes = VM.PrimaryPen;
            }
            else if (inkCanvas.DefaultDrawingAttributes == VM.SecondaryPen)
            {
                tempAttib = VM.SecondaryPen;
                tempAttib.Color = (Color)e.NewValue;
                VM.SecondaryPen = tempAttib;
                inkCanvas.DefaultDrawingAttributes = VM.SecondaryPen;
            }
            else if (inkCanvas.DefaultDrawingAttributes == VM.HighlighterPen)
            {
                tempAttib = VM.HighlighterPen;
                tempAttib.Color = (Color)e.NewValue;
                VM.HighlighterPen = tempAttib;
                inkCanvas.DefaultDrawingAttributes = VM.HighlighterPen;
            }
        }

        private void ChangePenBrushThickness(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            DrawingAttributes tempAttib = null;

            if (inkCanvas.DefaultDrawingAttributes == VM.PrimaryPen)
            {
                tempAttib = VM.PrimaryPen;
                tempAttib.Width = e.NewValue;
                tempAttib.Height = e.NewValue;
                VM.PrimaryPen = tempAttib;
                inkCanvas.DefaultDrawingAttributes = VM.PrimaryPen;
            }
            else if (inkCanvas.DefaultDrawingAttributes == VM.SecondaryPen)
            {
                tempAttib = VM.SecondaryPen;
                tempAttib.Width = e.NewValue;
                tempAttib.Height = e.NewValue;
                VM.SecondaryPen = tempAttib;
                inkCanvas.DefaultDrawingAttributes = VM.SecondaryPen;
            }
            else if (inkCanvas.DefaultDrawingAttributes == VM.HighlighterPen)
            {
                tempAttib = VM.HighlighterPen;
                tempAttib.Width = e.NewValue;
                tempAttib.Height = e.NewValue;
                VM.HighlighterPen = tempAttib;
                inkCanvas.DefaultDrawingAttributes = VM.HighlighterPen;
            }
        }

        private void CloseDrawing(object sender, MouseButtonEventArgs e)
        {
            VM.InkCanvasVisibility = Visibility.Collapsed;
            inkCanvas.Strokes.Clear();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isToolboxBarHold = true;
            mouseOffsetToBar = Mouse.GetPosition(toolBoxBar);
        }

        private void HandleDrag(object sender, MouseEventArgs e)
        {
            Point windowPointRelative = e.GetPosition(this);

            if (isToolboxBarHold)
            {
                Canvas.SetLeft(drawingToolbox, windowPointRelative.X - mouseOffsetToBar.X);
                Canvas.SetTop(drawingToolbox, windowPointRelative.Y - mouseOffsetToBar.Y);
            }
            else if (isColorBarHold)
            {
                Canvas.SetLeft(colorSelector, windowPointRelative.X - mouseOffsetToBar.X);
                Canvas.SetTop(colorSelector, windowPointRelative.Y - mouseOffsetToBar.Y);
            }
        }

        private void HandleFinishDrag(object sender, MouseButtonEventArgs e)
        {
            isToolboxBarHold = false;
            isColorBarHold = false;
            colorCanvas.IsHitTestVisible = true;
        }

        private void ColorSelectorBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isColorBarHold = true;
            mouseOffsetToBar = Mouse.GetPosition(colorSelector);
            Canvas.SetRight(colorSelector, 0);
            colorCanvas.IsHitTestVisible = false;
        }

        private async void TakePicture(object sender, MouseButtonEventArgs e)
        {
            colorSelector.Visibility = Visibility.Collapsed;
            drawingToolbox.Visibility = Visibility.Collapsed;
            controller.Visibility = Visibility.Collapsed;
            IsControllerVisible = false;
            grdControlBar.Visibility = Visibility.Collapsed;
            if (ShowWatermask)
                uadWatermark.Visibility = Visibility.Visible;
            await Task.Delay(10);
            await CaptureWindow();
            colorSelector.Visibility = Visibility.Visible;
            controller.Visibility = Visibility.Visible;
            drawingToolbox.Visibility = Visibility.Visible;
            IsControllerVisible = true;
            grdControlBar.Visibility = Visibility.Visible;
            if (ShowWatermask)
                uadWatermark.Visibility = Visibility.Collapsed;
            snackBar.IsActive = true;
        }

        private async Task CaptureWindow()
        {
            //Capture
            var windowHost = Window.GetWindow(this);
            await Task.Delay(50);
            string fileName = "Captured Image " + DateTime.Now.ToLongDateString() + ", " + DateTime.Now.ToLongTimeString().Replace(':', '-') + ".png";
            string fileLocation = System.IO.Path.Combine((Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.ScreenShotLocation, fileName);
            if (!Directory.Exists((Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.ScreenShotLocation))
                Directory.CreateDirectory((Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.ScreenShotLocation);

            var picture = MiscClass.SaveFrameworkElementToPng(this, (int)windowHost.Width, (int)windowHost.Height, fileLocation);
            capturedImg.Source = picture;
            showCapturedImg.Visibility = Visibility.Visible;

            lastCapImgLocation = fileLocation;
        }

        private void ShowImageExternal(object sender, RoutedEventArgs e) => Process.Start(lastCapImgLocation);

        private void CloseImagePreview(object sender, MouseButtonEventArgs e) => showCapturedImg.Visibility = Visibility.Collapsed;
        #endregion

        #region Sneaky Watcher

        private void CheckAvaibility(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = VM.IsSneakyWatcherEnabled;

        private void AcivateScreenBlocker(object sender, ExecutedRoutedEventArgs e) => ActivateBlocker();

        private void AcivateFakeAppCrash(object sender, ExecutedRoutedEventArgs e) => ActivateFakeCrash();

        private void AcivateBackgroundPlayer(object sender, ExecutedRoutedEventArgs e) => ActivateBGPlayer();

        private void ScreenBlocker_Click(object sender, RoutedEventArgs e)
        {
            if (!VM.IsSneakyWatcherEnabled)
                return;

            ActivateBlocker();
        }

        private void FakeAppCrash_Click(object sender, RoutedEventArgs e)
        {
            if (!VM.IsSneakyWatcherEnabled)
                return;

            ActivateFakeCrash();
        }

        private void BackgroundPlayer_Click(object sender, RoutedEventArgs e)
        {
            if (!VM.IsSneakyWatcherEnabled)
                return;

            ActivateBGPlayer();
        }

        public void ActivateBlocker()
        {
            InitSneakyWatcher();
            if (VM.IsBlockerActive)
                if ((Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.IsEnableMasterPassword)
                    if (new SneakyWatcherPasswordBox().ValidatePassword((Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.SneakyWatcherMasterPassword, (Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.IsRandomizePasswordBox))
                    {
                        VM.IsBlockerActive = false;

                        if ((Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.ChangeAppIconWhenSneakyWatcherActive)
                            OnRequestIconChange(new Uri("pack://application:,,,/Resources/UADIcon.ico"));
                    }
                    else
                        return;
                else
                {
                    VM.IsBlockerActive = false;

                    if ((Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.ChangeAppIconWhenSneakyWatcherActive)
                        OnRequestIconChange(new Uri("pack://application:,,,/Resources/UADIcon.ico"));
                }
            else
                VM.IsBlockerActive = true;
        }

        public void ActivateFakeCrash()
        {
            InitSneakyWatcher();
            var WinHost = Window.GetWindow(this);

            if (IsFakeCrashActive)
                if ((Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.IsEnableMasterPassword)
                    if (new SneakyWatcherPasswordBox().ValidatePassword((Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.SneakyWatcherMasterPassword, (Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.IsRandomizePasswordBox))
                        UnActivateFakeCrash(WinHost);
                    else
                        return;
                else
                    UnActivateFakeCrash(WinHost);
            else
            {
                if ((Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.MakeWindowTopMost)
                    WinHost.Topmost = true;
                if ((Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.DisableAltF4)
                    WinHost.Closing += MiscClass.CancelCloseWindow;
                FakeAppCrashFill.Visibility = Visibility.Visible;
                FakeHost = new FakeNotRespondingDialog();
                IsFakeCrashActive = true;
                FakeHost.ShowDialog();
            }
        }

        public void ActivateBGPlayer()
        {
            if ((Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.IsPauseWhenSneakyWactherActive)
            {
                mediaPlayer.Pause();
                IsPlaying = false;
                (btnPlayPause.Content as PackIcon).Kind = PackIconKind.Play;
            }

            Focus();
            Window wdHost = Window.GetWindow(this);
            wdHost.Hide();
            IsBackgroundPlayerActive = true;
        }

        private void ReopenCrashDialog(object sender, MouseButtonEventArgs e) => FakeHost.ShowDialog();

        private void UnActivateFakeCrash(Window WinHost)
        {
            FakeHost.Close();
            FakeAppCrashFill.Visibility = Visibility.Collapsed;
            IsFakeCrashActive = false;
            if ((Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.MakeWindowTopMost)
                WinHost.Topmost = false;
            if ((Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.DisableAltF4)
                WinHost.Closing -= MiscClass.CancelCloseWindow;

            if ((Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.ChangeAppIconWhenSneakyWatcherActive)
                OnRequestIconChange(new Uri("pack://application:,,,/Resources/UADIcon.ico"));
        }

        private void InitSneakyWatcher()
        {
            if ((Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.IsPauseWhenSneakyWactherActive)
            {
                mediaPlayer.Pause();
                IsPlaying = false;
                (btnPlayPause.Content as PackIcon).Kind = PackIconKind.Play;
            }

            if ((Application.Current.FindResource("Settings") as UADSettingsManager).CurrentSettings.ChangeAppIconWhenSneakyWatcherActive)
                OnRequestIconChange(new Uri("pack://application:,,,/Resources/WinDefaultIcon.png"));

            Focus();
        }
        #endregion


        public event EventHandler<RequestingWindowStateEventArgs> RequestWindowState;
        public event EventHandler<EventArgs> UADMediaPlayerClosed;
        public event EventHandler<RequestWindowIconChangeEventArgs> RequestIconChange;

        protected virtual void OnRequestWindowState(WindowState state) => RequestWindowState?.Invoke(this, new RequestingWindowStateEventArgs() { RequestState = state });
        protected virtual void OnRequestIconChange(Uri iconLocation) => RequestIconChange?.Invoke(this, new RequestWindowIconChangeEventArgs() { IconLocation = iconLocation });
        protected virtual void OnUADMediaPlayerClosed()
        {
            UADMediaPlayerClosed?.Invoke(this, EventArgs.Empty);

            //Online Media Player
            if(IsPlayOnline)
                _Timer.Stop();

            bufferingIndicator.Visibility = Visibility.Collapsed;
        }

        private void Event_WindowMinimize(object sender, RoutedEventArgs e) => OnRequestWindowState(WindowState.Minimized);

        private void Event_UADMediaPlayerClosed(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Stop();
            OnUADMediaPlayerClosed();
        }

        private void Event_Previous(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Stop();
            Previous();
            Play();
            PackIcon pkIcon = btnPlayPause.Content as PackIcon;
            pkIcon.Kind = PackIconKind.Pause;
        }

        private void Event_Next(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Stop();
            Next();
            Play();
            PackIcon pkIcon = btnPlayPause.Content as PackIcon;
            pkIcon.Kind = PackIconKind.Pause;
        }

        public void Previous() => PlayIndex = GetNearestEpisode(this, true, true);

        public void Next() => PlayIndex = GetNearestEpisode(this, true, false);

        private void OpenDetailSidePanel(object sender, RoutedEventArgs e) => IsSidePanelOpen = !IsSidePanelOpen;

        private void Event_EpisodeSelected(object sender, RoutedEventArgs e)
        {
            var info = sender as Button;
            PlayIndex = Playlist.Episodes.FindIndex(p => p == info.DataContext);
        }

        private void Event_OpenPlayListTab(object sender, RoutedEventArgs e) => VM.SidePanelTabIndex = 0;

        private void Event_OpenInfoTab(object sender, RoutedEventArgs e) => VM.SidePanelTabIndex = 1;

        public virtual async void Play()
        {
            if (NoPlayableMedia)
                await MessageDialog.ShowAsync("No playable media!", "We can't find any playable media of this series. You can switch to online version and download some episode.", MessageDialogButton.OKCancelButton);
            else
            {
                mediaPlayer.Play();
                (btnPlayPause.Content as PackIcon).Kind = PackIconKind.Pause;
                IsPlaying = true;

                //Online media player
                if (IsPlayOnline)
                    _Timer.Start();
                else
                    bufferingIndicator.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Clear all the data inside uadmediaplayer such as Playlist, Index,...
        /// </summary>
        public virtual void Reset()
        {
            mediaPlayer.Stop();
            IsPlaying = false;
            PlayIndex = -1;
            Playlist = null;
            GC.Collect();
        }

        private void Event_QualityChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = sender as ListBox;
            var selectedQuality = ((KeyValuePair<VideoQuality, MediaSourceInfo>)listBox.SelectedItem).Key;
            var currentEpisode = Playlist.Episodes[PlayIndex];

            if (currentEpisode.FilmSources.Count != 0)
            {
                // When the video uri changes the VideoPlayerPostion to 0. So we store the position before changes and set them back later
                var previousTime = mediaPlayer.Position;
                VideoUri = new Uri(currentEpisode.FilmSources[selectedQuality].Url.Replace("https", "http"));
                mediaPlayer.Position = previousTime;
            }
        }
    }
}
