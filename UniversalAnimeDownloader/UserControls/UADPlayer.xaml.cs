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
using System.Windows.Ink;
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

        private bool isToolboxBarHold = false;
        private bool isColorBarHold = false;
        private Point mouseOffsetToBar = new Point();

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
            VM = new UADPlayerViewModel();
            InitializeComponent();
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

        private void ToggleDrawing(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            PackIcon packIcon = btn.Content as PackIcon;

            VM.InkCanvasVisibility = Visibility.Visible;
            inkCanvas.DefaultDrawingAttributes = VM.PrimaryPen;
        }


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
        #endregion



        public event EventHandler<RequestingWindowStateEventArgs> RequestWindowState;
        protected virtual void OnRequestWindowState(WindowState state) => RequestWindowState?.Invoke(this, new RequestingWindowStateEventArgs() { RequestState = state });
    }
}
