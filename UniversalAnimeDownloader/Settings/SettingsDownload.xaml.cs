﻿using System;
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
        public Thickness defaultFilePerDownloadThickness;

        public SettingsDownload()
        {
            VM = new SettingsDownloadViewModel();
            Resources.Add("VM", VM);
            InitializeComponent();
            DataContext = VM;
            defaultFilePerDownloadThickness = fpd.Margin;
        }

        private void IntergerValidate(object sender, TextChangedEventArgs e)
        {
            TextBox txb = sender as TextBox;
            if (txb.Text.Length == 0)
                return;

            bool parseResult = int.TryParse(txb.Text, out int result);
            if (!parseResult)
                ValidationFailed(txb, filePerDownloadErrorMessage, "Invail character!", defaultFilePerDownloadThickness, TimeSpan.FromSeconds(0.25), 5);
            else if (result < 1 || result > 32)
                ValidationFailed(txb, filePerDownloadErrorMessage, $"Please type in a number that from {1} to {32}!", defaultFilePerDownloadThickness, TimeSpan.FromSeconds(0.25), 5);
            else
                ValidationPassed(txb, filePerDownloadErrorMessage, defaultFilePerDownloadThickness);
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
                animation.Children.Add(new ThicknessAnimation(defaultThickness, MulitplyThinkness(PlusThickness(defaultThickness, new Thickness(10, 0, -10, 0)), amplitude), TimeSpan.FromSeconds(step)) { BeginTime = TimeSpan.FromSeconds(step * stepMultiplier++) });
                animation.Children.Add(new ThicknessAnimation(MulitplyThinkness(PlusThickness(defaultThickness, new Thickness(10, 0, -10, 0)), amplitude), defaultThickness, TimeSpan.FromSeconds(step)) { BeginTime = TimeSpan.FromSeconds(step * stepMultiplier++) });
                animation.Children.Add(new ThicknessAnimation(defaultThickness, MulitplyThinkness(PlusThickness(defaultThickness, new Thickness(-10, 0, 10, 0)), amplitude), TimeSpan.FromSeconds(step)) { BeginTime = TimeSpan.FromSeconds(step * stepMultiplier++) });
                animation.Children.Add(new ThicknessAnimation(MulitplyThinkness(PlusThickness(defaultThickness, new Thickness(-10, 0, 10, 0)), amplitude), defaultThickness, TimeSpan.FromSeconds(step)) { BeginTime = TimeSpan.FromSeconds(step * stepMultiplier++) });

            }
            ctrl.BeginStoryboard(animation);
        }

        private Thickness PlusThickness (Thickness a, Thickness b)
        {
            return new Thickness(a.Left + b.Left, a.Top + b.Top, right: a.Right + b.Right, bottom: a.Bottom + b.Bottom);
        }

        private Thickness MulitplyThinkness(Thickness a, double b)
        {
            return new Thickness(a.Left * b, a.Top * b, a.Right * b, a.Bottom * b);
        }
    }

}
