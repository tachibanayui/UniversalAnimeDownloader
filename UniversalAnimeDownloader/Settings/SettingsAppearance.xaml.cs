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
                    Application.Current.Resources["ForeGroundColor"] = new SolidColorBrush((Color)colorPicker.SelectedColor); 
                    break;
                case "desColor":
                    Application.Current.Resources["FileSize"] = new SolidColorBrush((Color)colorPicker.SelectedColor);
                    break;
                case "menubarColor":
                    Application.Current.Resources["Primary"] = new SolidColorBrush((Color)colorPicker.SelectedColor);
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
    }
}
