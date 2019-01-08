using System;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UniversalAnimeDownloader.ViewModel;

namespace UniversalAnimeDownloader.Settings
{
    /// <summary>
    /// Interaction logic for SettingsPlayback.xaml
    /// </summary>
    public partial class SettingsPlayback : Page
    {
        public SettingsPlaybackViewModel VM;
        private Border LastKeyChange;
        private StackPanel LastColorChange;

        public Frame FrameHost
        {
            get { return (Frame)GetValue(FrameHostProperty); }
            set { SetValue(FrameHostProperty, value); }
        }
        public static readonly DependencyProperty FrameHostProperty =
            DependencyProperty.Register("FrameHost", typeof(Frame), typeof(SettingsPlayback), new PropertyMetadata());

        public SettingsPlayback()
        {
            VM = new SettingsPlaybackViewModel();
            DataContext = VM;
            InitializeComponent();
            stretchMode.ItemsSource = typeof(Stretch).GetEnumNames();

            GetSettingFromCurrentSetting();
        }

        private void GetSettingFromCurrentSetting()
        {
            //Get Prefered Player
            if (SettingsManager.Current.PreferedPlayer == PlayerType.Embeded)
                cbxPlayerSelect.SelectedIndex = 0;

            switch (cbxPlayerSelect.SelectedValue as string)
            {
                case "UAD Player":
                    playerDescription.Text = "Use our build-in playback to help you get a better watching experience and support unique feature such as sneaky watcher";
                    break;
                case "External Player":
                    playerDescription.Text = "Use your default external playback program such as Window Media Player or VLC to play anime.Does not support unique feature like sneaky watcher.";
                    break;
                default:
                    break;
            }

            //Get Slider
            sldPlaybackVolume.Value = SettingsManager.Current.PlaybackVolume;
            BrushPrimaryThickness.Value = SettingsManager.Current.PrimaryBurshThickness;
            BrushSecondaryThickness.Value = SettingsManager.Current.SecondaryBurshThickness;
            BrushHighlighterThickness.Value = SettingsManager.Current.HighlighterBurshThickness;

            //Get Hotkeys
            ((BlockerKeyBox.Child as Grid).Children[0] as TextBlock).Text = SettingsManager.Current.BlockerToggleHotKeys.ToString();
            ((FakeCrashKeyBox.Child as Grid).Children[0] as TextBlock).Text = SettingsManager.Current.AppCrashToggleHotKeys.ToString();
            ((BGKeyBox.Child as Grid).Children[0] as TextBlock).Text = SettingsManager.Current.BgPlayerToggleHotKeys.ToString();

            //Get Colors
            ((BlockerColor.Children[0] as Button).Content as Rectangle).Fill = new SolidColorBrush(SettingsManager.Current.BlockerColor);
            ((BrushPrimary.Children[0] as Button).Content as Rectangle).Fill = new SolidColorBrush(SettingsManager.Current.PrimaryPenColor);
            ((BrushSecondary.Children[0] as Button).Content as Rectangle).Fill = new SolidColorBrush(SettingsManager.Current.SecondaryPenColor);
            ((BrushHighlighter.Children[0] as Button).Content as Rectangle).Fill = new SolidColorBrush(SettingsManager.Current.HighlighterPenColor);

            //Get Blocker Image
            imgPreviewImage.Source = new BitmapImage(new Uri(SettingsManager.Current.BlockerImageLocation));
            stretchMode.SelectedIndex = (int)SettingsManager.Current.BlockerStretchMode;
        }

        private void ChangeDescription(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cbx = sender as ComboBox;
            if (playerDescription == null)
                return;

            switch (cbx.SelectedValue as string)
            {
                case "UAD Player":
                    playerDescription.Text = "Use our build-in playback to help you get a better watching experience and support unique feature such as sneaky watcher";
                    SettingsManager.Current.PreferedPlayer = PlayerType.Embeded;
                    break;
                case "External Player":
                    playerDescription.Text = "Use your default external playback program such as Window Media Player or VLC to play anime.Does not support unique feature like sneaky watcher.";
                    SettingsManager.Current.PreferedPlayer = PlayerType.External;
                    break;
                default:
                    break;
            }
        }

        private void Event_ChangePlayBackVolume(object sender, DragCompletedEventArgs e) => SettingsManager.Current.PlaybackVolume = sldPlaybackVolume.Value;

        private void Event_CloseDialog(object sender, RoutedEventArgs e) => KeyCaptureDialog.IsOpen = false;

        private void Event_SetKey(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (LastKeyChange == null)
                return;

            ((LastKeyChange.Child as Grid).Children[0] as TextBlock).Text = textBox.Text.ToUpper();
            KeyCaptureDialog.IsOpen = false;
            ChangeHotkey();
            LastKeyChange = null;
            textBox.Text = string.Empty;
        }

        private void ChangeHotkey()
        {
            char current = ((LastKeyChange.Child as Grid).Children[0] as TextBlock).Text[0];

            switch (LastKeyChange.Name)
            {
                case "BlockerKeyBox":
                    SettingsManager.Current.BlockerToggleHotKeys = current;
                    break;
                case "FakeCrashKeyBox":
                    SettingsManager.Current.AppCrashToggleHotKeys = current;
                    break;
                case "BGKeyBox":
                    SettingsManager.Current.BgPlayerToggleHotKeys = current;
                    break;
                default:
                    break;
            }
        }

        private void Event_ChangeKeyBinding(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            KeyCaptureDialog.IsOpen = true;
            LastKeyChange = ((btn.Parent as Grid).Parent) as Border;
        }

        private void CloseDialogHost(object sender, RoutedEventArgs e) => colorPickerHost.IsOpen = false;

        private void AssignColor(object sender, RoutedEventArgs e)
        {
            ((LastColorChange.Children[0] as Button).Content as Rectangle).Fill = new SolidColorBrush((Color)colorPicker.SelectedColor);

            switch (LastColorChange.Name)
            {
                case "BlockerColor":
                    SettingsManager.Current.BlockerColor = (Color)colorPicker.SelectedColor;
                    break;
                case "BrushPrimary":
                    SettingsManager.Current.PrimaryPenColor = (Color)colorPicker.SelectedColor;
                    break;
                case "BrushSecondary":
                    SettingsManager.Current.SecondaryPenColor = (Color)colorPicker.SelectedColor;
                    break;
                case "BrushHighlighter":
                    SettingsManager.Current.HighlighterPenColor = (Color)colorPicker.SelectedColor;
                    break;
                default:
                    break;
            }
            colorPickerHost.IsOpen = false;
        }

        private void Event_ChooseColor(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            colorPickerHost.IsOpen = true;
            LastColorChange = btn.Parent as StackPanel;
        }

        private void Event_ChangeBrushThickness(object sender, DragCompletedEventArgs e)
        {
            Slider sld = sender as Slider;
            switch (sld.Name)
            {
                case "BrushPrimaryThickness":
                    SettingsManager.Current.PrimaryBurshThickness = sld.Value;
                    break;
                case "BrushSecondaryThickness":
                    SettingsManager.Current.SecondaryBurshThickness = sld.Value;
                    break;
                case "BrushHighlighterThickness":
                    SettingsManager.Current.HighlighterBurshThickness = sld.Value;
                    break;
                default:
                    break;
            }
        }

        private void Event_OpenFileDialog(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.Multiselect = false;
            dialog.Filter = "Image Files(*.BMP;*.JPG;*.PNG;*.JPEG)|*.BMP;*.JPG;*.GIF;*.JPEG";

            var fileRes = dialog.ShowDialog();

            if (fileRes != System.Windows.Forms.DialogResult.OK)
                return;

            VM.BlockerImageLocation = dialog.FileName;
            txbBlockerImageLocation.Text = dialog.FileName;

            //Update Image:
            imgPreviewImage.Source = new BitmapImage(new Uri(dialog.FileName));
        }
    }
}
