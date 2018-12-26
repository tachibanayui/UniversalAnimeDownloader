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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UniversalAnimeDownloader.ViewModel;

namespace UniversalAnimeDownloader.Settings
{
    /// <summary>
    /// Interaction logic for SettingsDownload.xaml
    /// </summary>
    public partial class SettingsDownload : Page
    {
        public SettingsDownloadViewModel VM;
        public Thickness defaultFpdThickness;
        public Thickness defaultSpdThickness;

        public Frame FrameHost
        {
            get { return (Frame)GetValue(FrameHostProperty); }
            set { SetValue(FrameHostProperty, value); }
        }
        public static readonly DependencyProperty FrameHostProperty =
            DependencyProperty.Register("FrameHost", typeof(Frame), typeof(SettingsDownload), new PropertyMetadata());

        public SettingsDownload()
        {
            VM = new SettingsDownloadViewModel();
            Resources.Add("VM", VM);
            InitializeComponent();
            spdTextBox.TextChanged += IntergerValidate;
            fpdTextBox.TextChanged += IntergerValidate;
            DataContext = VM;
            defaultFpdThickness = fpdTextBox.Margin;
            defaultSpdThickness = spdTextBox.Margin;
        }

        private void IntergerValidate(object sender, TextChangedEventArgs e)
        {
            TextBox txb = sender as TextBox;
            TextBlock ErrMessagetxbl = txb.Name == "fpdTextBox" ? fpdErrMessage : spdErrMessage;
            Thickness txbDefaultThickness = txb.Name == "fpdTextBox" ? defaultFpdThickness : defaultSpdThickness;
            IntergerValidation(txb, ErrMessagetxbl, txbDefaultThickness, 1, 64);
        }

        private void IntergerValidation(TextBox txb, TextBlock messageControl, Thickness defaultThickness, int min, int max)
        {
            if (txb.Text.Length == 0)
            {
                ValidationFailed(txb, messageControl, "You must specify a value!", defaultThickness, TimeSpan.FromSeconds(0.25), 5);
                return;
            }

            bool parseResult = int.TryParse(txb.Text, out int result);
            if (!parseResult)
                ValidationFailed(txb, messageControl, "Invail character!", defaultThickness, TimeSpan.FromSeconds(0.25), 5);
            else if (result < min || result > max)
                ValidationFailed(txb, messageControl, $"Please type in a number that from {min} to {max}!", defaultThickness, TimeSpan.FromSeconds(0.25), 5);
            else
                ValidationPassed(txb, messageControl, defaultThickness);
        }

        private void ValidationPassed(Control ctrl, TextBlock messageControl, Thickness defaultFilePerDownloadThickness)
        {
            messageControl.Text = string.Empty;
            ctrl.Foreground = Application.Current.Resources["ForeGroundColor"] as SolidColorBrush;
        }

        private void ValidationFailed(Control ctrl, TextBlock messageControl, string message, Thickness defaultThickness, TimeSpan duration, int repeatAnimation = 2, double amplitude = 1)
        {
            messageControl.Text = message;
            Thickness lastThickness = defaultThickness;

            //calculate the step interval for a given duration
            
            double secDuration = duration.TotalMilliseconds / 1000d;
            //4: 4 animation in one loop
            double step = secDuration / 4 / repeatAnimation;

            //do rung animation here!
            ctrl.Foreground = new SolidColorBrush(Colors.Red);
            var animation = new Storyboard() { FillBehavior = FillBehavior.Stop };
            Storyboard.SetTargetProperty(animation, new PropertyPath("Margin"));
            Storyboard.SetTarget(animation, ctrl);

            int stepMultiplier = 0;
            for (int i = 0; i < repeatAnimation; i++)
            {
                animation.Children.Add(new ThicknessAnimation(defaultThickness, Common.MulitplyThinkness(Common.PlusThickness(defaultThickness, new Thickness(10, 0, -10, 0)), amplitude), TimeSpan.FromSeconds(step)) { BeginTime = TimeSpan.FromSeconds(step * stepMultiplier++) });
                animation.Children.Add(new ThicknessAnimation(Common.MulitplyThinkness(Common.PlusThickness(defaultThickness, new Thickness(10, 0, -10, 0)), amplitude), defaultThickness, TimeSpan.FromSeconds(step)) { BeginTime = TimeSpan.FromSeconds(step * stepMultiplier++) });
                animation.Children.Add(new ThicknessAnimation(defaultThickness, Common.MulitplyThinkness(Common.PlusThickness(defaultThickness, new Thickness(-10, 0, 10, 0)), amplitude), TimeSpan.FromSeconds(step)) { BeginTime = TimeSpan.FromSeconds(step * stepMultiplier++) });
                animation.Children.Add(new ThicknessAnimation(Common.MulitplyThinkness(Common.PlusThickness(defaultThickness, new Thickness(-10, 0, 10, 0)), amplitude), defaultThickness, TimeSpan.FromSeconds(step)) { BeginTime = TimeSpan.FromSeconds(step * stepMultiplier++) });
            }
            ctrl.BeginStoryboard(animation);
        }
    }

}
