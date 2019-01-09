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

namespace UniversalAnimeDownloader.Settings
{
    /// <summary>
    /// Interaction logic for SettingsAppearance.xaml
    /// </summary>
    public partial class SettingsAppearance : Page
    {
        public SettingsAppearanceViewModel VM;
        private Button lastColorPicker;

        public Frame FrameHost
        {
            get { return (Frame)GetValue(FrameHostProperty); }
            set { SetValue(FrameHostProperty, value); }
        }
        public static readonly DependencyProperty FrameHostProperty =
            DependencyProperty.Register("FrameHost", typeof(Frame), typeof(SettingsAppearance), new PropertyMetadata());

        public SettingsAppearance()
        {
            VM = new SettingsAppearanceViewModel();
            InitializeComponent();
            DataContext = VM;
            headingSlider.ValueChanged += ChangeFontSize;
            heading2Slider.ValueChanged += ChangeFontSize;
            heading3Slider.ValueChanged += ChangeFontSize;
            heading4Slider.ValueChanged += ChangeFontSize;

            GetValuesFromSettings();
        }

        private void GetValuesFromSettings()
        {
            imgPreviewImageBGmenubar.Source = new BitmapImage(new Uri(SettingsManager.Current.BGMenubarImageLocation));
            txbBGmenubarLocation.Text = SettingsManager.Current.BGViewerImageLocation;
            imgPreviewImageBGviewer.Source = new BitmapImage(new Uri(SettingsManager.Current.BGViewerImageLocation));
            txbBGViewerLocation.Text = SettingsManager.Current.BGViewerImageLocation;
        }

        private void ChooseColor(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            lastColorPicker = btn;
            //get the current color
            colorPicker.SelectedColor = ((btn.Content as Rectangle).Fill as SolidColorBrush).Color;

            colorPickerHost.IsOpen = true;
        }

        private void CloseDialogHost(object sender, RoutedEventArgs e) => colorPickerHost.IsOpen = false;

        private void AssignColor(object sender, RoutedEventArgs e)
        {
            switch (lastColorPicker.Name)
            {
                case "textColor":
                    SettingsManager.Current.ForegroundColor = (Color)colorPicker.SelectedColor;
                    break;
                case "desColor":
                    SettingsManager.Current.FileSizeColor = (Color)colorPicker.SelectedColor;
                    break;
                case "menubarColor":
                    SettingsManager.Current.PrimaryColor = (Color)colorPicker.SelectedColor;
                    break;
                default:
                    break;
            }

            colorPickerHost.IsOpen = false;
        }

        private void ChangeFontSize(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider sld = sender as Slider;
            string text = ((sld.Parent as Grid).Children[0] as TextBlock).Text;
            Application.Current.Resources[text] = e.NewValue;
        }

        private void Event_OpenFileDialog(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.Multiselect = false;
            dialog.Filter = "Image Files(*.BMP;*.JPG;*.PNG;*.JPEG)|*.BMP;*.JPG;*.GIF;*.JPEG";

            var fileRes = dialog.ShowDialog();

            if (fileRes != System.Windows.Forms.DialogResult.OK)
                return;

            Button btn = sender as Button;
            string checkboxName = ((((btn.Parent as Grid).Parent as StackPanel).Parent as StackPanel).Children[0] as CheckBox).Name;

            switch (checkboxName)
            {
                case "useMenubar":
                    VM.BGMenubarImage = dialog.FileName;
                    txbBGmenubarLocation.Text = dialog.FileName;
                    imgPreviewImageBGmenubar.Source = new BitmapImage(new Uri(dialog.FileName));
                    break;
                case "useBackground":
                    VM.BGViewerImage = dialog.FileName;
                    txbBGViewerLocation.Text = dialog.FileName;
                    imgPreviewImageBGviewer.Source = new BitmapImage(new Uri(dialog.FileName));
                    break;
                default:
                    break;
            }
        }
    }
}
