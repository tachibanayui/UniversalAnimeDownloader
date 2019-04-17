﻿using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
using UniversalAnimeDownloader.UADSettingsPortal;

namespace UniversalAnimeDownloader.MediaPlayer
{
    /// <summary>
    /// Interaction logic for UADMediaPlayer.xaml
    /// </summary>
    public partial class UADMediaPlayer : UserControl
    {
        #region Fields and Properties
        public UADPlayerViewModel VM;
        public TimeSpan MediaDuration;
        private bool isSeekSliderLocked = false;

        private Random rand = new Random();
        private bool isToolboxBarHold = false;
        private bool isColorBarHold = false;
        private Point mouseOffsetToBar = new Point();
        private string lastCapImgLocation;
        private int currentHideControllerTimeOutID;
        private DateTime LastClick;

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

        public FakeNotRespondingDialog FakeHost { get; set; }
        #endregion

        #region Dependency Properties
        public Uri VideoUri
        {
            get { return (Uri)GetValue(VideoUriProperty); }
            set
            {
                SetValue(VideoUriProperty, value);
                mediaPlayer.Source = value;
                Focus();
            }
        }
        public static readonly DependencyProperty VideoUriProperty =
            DependencyProperty.Register("VideoUri", typeof(Uri), typeof(UADMediaPlayer), new PropertyMetadata());

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

        public string ImageLibraryLocation
        {
            get { return (string)GetValue(ImageLibraryLocationProperty); }
            set { SetValue(ImageLibraryLocationProperty, value); }
        }
        public static readonly DependencyProperty ImageLibraryLocationProperty =
            DependencyProperty.Register("ImageLibraryLocation", typeof(string), typeof(UADMediaPlayer), new PropertyMetadata(string.Empty));

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

        public bool isPlaying = true;
        public UADMediaPlayer()
        {
            //SettingsManager.Current = SettingsManager.Current;
            VM = new UADPlayerViewModel();
            InitializeComponent();
            (Content as FrameworkElement).DataContext = VM;
            mediaPlayer.Source = VideoUri;
            mediaPlayer.Play();
            Loaded += (s, e) => AssignProperty();
            isControllerVisible = true;
            //var t = Application.Current.Resources["applicationTaskbarPopup"] as TaskbarIcon;
            string tt = AppDomain.CurrentDomain.BaseDirectory + "unnamed.ico";
            ////t.Icon = new System.Drawing.Icon(tt);
            //t.Visibility = Visibility.Visible;
            mediaPlayer.Pause();

            if (UADSettingsManager.Instance.CurrentSettings.PlayMediaFullScreen)
            {
                OnRequestWindowState(WindowState.Maximized);
                (btnFullScreenToggle.Content as PackIcon).Kind = PackIconKind.ArrowCollapse;
            }
            strokeThicknessSlider.Value = UADSettingsManager.Instance.CurrentSettings.PrimaryBurshThickness;
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

        private void Command_PlayPauseMedia(object sender, ExecutedRoutedEventArgs e)
        {
            PlayPauseMedia();
            MiscClass.FadeInAnimation(controller, TimeSpan.FromSeconds(.5), false);
            controller.IsHitTestVisible = true;
            IsControllerVisible = true;
        }

        private void PlayPauseMedia()
        {
            PackIcon pkIcon = btnPlayPause.Content as PackIcon;

            if (isPlaying)
            {
                mediaPlayer.Pause();
                pkIcon.Kind = PackIconKind.Play;
                MiscClass.FadeInAnimation(grdFilmProperty, TimeSpan.FromSeconds(0.25), false);
                PointAnimation animation = new PointAnimation(new Point(0, 1), TimeSpan.FromSeconds(0.25));
                controllerBackgroundGradient.BeginAnimation(LinearGradientBrush.EndPointProperty, animation);

            }
            else
            {
                mediaPlayer.Play();
                pkIcon.Kind = PackIconKind.Pause;
                MiscClass.FadeOutAnimation(grdFilmProperty, TimeSpan.FromSeconds(0.25), false);
                PointAnimation animation = new PointAnimation(new Point(0, .5), TimeSpan.FromSeconds(0.25));
                controllerBackgroundGradient.BeginAnimation(LinearGradientBrush.EndPointProperty, animation);
            }
            isPlaying = !isPlaying;
        }

        private void Back10Sec(object sender, RoutedEventArgs e) => mediaPlayer.Position -= TimeSpan.FromSeconds(10);

        private void Forward30Sec(object sender, RoutedEventArgs e) => mediaPlayer.Position += TimeSpan.FromSeconds(30);

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
                    seekSlider.Value = MiscClass.GetTimeSpanRatio(mediaPlayer.Position, MediaDuration);
                await Task.Delay(500);
            }
        }

        private void ChangePosition(object sender, MouseButtonEventArgs e)
        {
            mediaPlayer.Position = MiscClass.MutiplyTimeSpan(MediaDuration, seekSlider.Value);
            isSeekSliderLocked = false;
        }

        private void LockSeekSlider(object sender, MouseButtonEventArgs e) => isSeekSliderLocked = true;

        private void VolumnChange(object sender, RoutedEventArgs e) => VolumeChanger.IsOpen = true;

        private void CloseVolumePopup(object sender, MouseEventArgs e) => VolumeChanger.IsOpen = false;

        private void ChangeWindowState(object sender, RoutedEventArgs e) => ChangeWindowState();

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
                    inkCanvas.DefaultDrawingAttributes = VM.PrimaryPen;
                    break;
                case PackIconKind.Pen:
                    inkCanvas.DefaultDrawingAttributes = VM.SecondaryPen;
                    break;
                case PackIconKind.GreasePencil:
                    inkCanvas.DefaultDrawingAttributes = VM.HighlighterPen;
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
            if (ShowWatermask)
                uadWatermark.Visibility = Visibility.Visible;
            await Task.Delay(10);
            await CaptureWindow();
            colorSelector.Visibility = Visibility.Visible;
            drawingToolbox.Visibility = Visibility.Visible;
            if (ShowWatermask)
                uadWatermark.Visibility = Visibility.Collapsed;
            snackBar.IsActive = true;
        }

        private async Task CaptureWindow()
        {
            ////Capture
            //var screenBound = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            //var windowHost = Window.GetWindow(this);
            //Point winLoc = new Point(windowHost.Left + CaptureOffset.Left, windowHost.Top + CaptureOffset.Top);
            //Point winFin = new Point(windowHost.Width - CaptureOffset.Right, windowHost.Height - CaptureOffset.Bottom);
            //await Task.Delay(50);
            //var picture = MiscClass.CopyScreen(new System.Drawing.Rectangle((int)winLoc.X, (int)winLoc.Y, (int)winFin.X, (int)winFin.Y));

            //capturedImg.Source = picture;
            //showCapturedImg.Visibility = Visibility.Visible;
            ////Save img
            //string fileName = "Captured Image " + DateTime.Now.ToLongDateString() + ", " + DateTime.Now.ToLongTimeString().Replace(':', '-') + ".png";
            //string fileLocation = System.IO.Path.Combine(ImageLibraryLocation, fileName);

            //if (!Directory.Exists(ImageLibraryLocation))
            //    Directory.CreateDirectory(ImageLibraryLocation);
            //FileStream fs = new FileStream(fileLocation, FileMode.Create);
            //PngBitmapEncoder encoder = new PngBitmapEncoder();
            //encoder.Frames.Add(BitmapFrame.Create(picture));
            //encoder.Save(fs);
            //fs.Close();

            //lastCapImgLocation = fileLocation;
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

        private void ActivateBlocker()
        {
            InitSneakyWatcher();
            if (VM.IsBlockerActive)
                if (UADSettingsManager.Instance.CurrentSettings.IsEnableMasterPassword)
                    if (new SneakyWatcherPasswordBox().ValidatePassword(UADSettingsManager.Instance.CurrentSettings.SneakyWatcherMasterPassword, UADSettingsManager.Instance.CurrentSettings.IsRandomizePasswordBox))
                    {
                        VM.IsBlockerActive = false;

                        if (UADSettingsManager.Instance.CurrentSettings.ChangeAppIconWhenSneakyWatcherActive)
                            OnRequestIconChange(new Uri("pack://application:,,,/Resources/UADIcon.ico"));
                    }
                    else
                        return;
                else
                {
                    VM.IsBlockerActive = false;

                    if (UADSettingsManager.Instance.CurrentSettings.ChangeAppIconWhenSneakyWatcherActive)
                        OnRequestIconChange(new Uri("pack://application:,,,/Resources/UADIcon.ico"));
                }
            else
                VM.IsBlockerActive = true;
        }

        private void ActivateFakeCrash()
        {
            InitSneakyWatcher();
            var WinHost = Window.GetWindow(this);

            if (IsFakeCrashActive)
                if (UADSettingsManager.Instance.CurrentSettings.IsEnableMasterPassword)
                    if (new SneakyWatcherPasswordBox().ValidatePassword(UADSettingsManager.Instance.CurrentSettings.SneakyWatcherMasterPassword, UADSettingsManager.Instance.CurrentSettings.IsRandomizePasswordBox))
                        UnActivateFakeCrash(WinHost);
                    else
                        return;
                else
                    UnActivateFakeCrash(WinHost);
            else
            {
                if (UADSettingsManager.Instance.CurrentSettings.MakeWindowTopMost)
                    WinHost.Topmost = true;
                if (UADSettingsManager.Instance.CurrentSettings.DisableAltF4)
                    WinHost.Closing += MiscClass.CancelCloseWindow;
                FakeAppCrashFill.Visibility = Visibility.Visible;
                FakeHost = new FakeNotRespondingDialog();
                IsFakeCrashActive = true;
                FakeHost.ShowDialog();
            }
        }

        private void ActivateBGPlayer()
        {
            if (UADSettingsManager.Instance.CurrentSettings.IsPauseWhenSneakyWactherActive)
            {
                mediaPlayer.Pause();
                isPlaying = false;
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
            if (UADSettingsManager.Instance.CurrentSettings.MakeWindowTopMost)
                WinHost.Topmost = false;
            if (UADSettingsManager.Instance.CurrentSettings.DisableAltF4)
                WinHost.Closing -= MiscClass.CancelCloseWindow;

            if (UADSettingsManager.Instance.CurrentSettings.ChangeAppIconWhenSneakyWatcherActive)
                OnRequestIconChange(new Uri("pack://application:,,,/Resources/UADIcon.ico"));
        }

        private void InitSneakyWatcher()
        {
            if (UADSettingsManager.Instance.CurrentSettings.IsPauseWhenSneakyWactherActive)
            {
                mediaPlayer.Pause();
                isPlaying = false;
                (btnPlayPause.Content as PackIcon).Kind = PackIconKind.Play;
            }

            if (UADSettingsManager.Instance.CurrentSettings.ChangeAppIconWhenSneakyWatcherActive)
                OnRequestIconChange(new Uri("pack://application:,,,/Resources/WinDefaultIcon.png"));

            Focus();
        }
        #endregion


        public event EventHandler<RequestingWindowStateEventArgs> RequestWindowState;
        public event EventHandler<RequestWindowIconChangeEventArgs> RequestIconChange;

        public void OnRequestWindowState(WindowState state) => RequestWindowState?.Invoke(this, new RequestingWindowStateEventArgs() { RequestState = state });
        public void OnRequestIconChange(Uri iconLocation) => RequestIconChange?.Invoke(this, new RequestWindowIconChangeEventArgs() { IconLocation = iconLocation });
    }
}