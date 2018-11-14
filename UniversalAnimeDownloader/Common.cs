using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace UniversalAnimeDownloader
{
    class Common
    {
        public static bool InternetAvaible = false;

        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (var stream = client.OpenRead("http://www.google.com"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public static void FadeInTransition(Control target, double interVal)
        {
            target.Margin = new Thickness(0, 50, 0, 0);
            target.Opacity = 0;
            DoubleAnimation fadeIn = new DoubleAnimation(1, TimeSpan.FromSeconds(interVal), FillBehavior.Stop);
            ThicknessAnimation slideUp = new ThicknessAnimation(new Thickness(0), TimeSpan.FromSeconds(interVal), FillBehavior.Stop);

            fadeIn.Completed += (s, e) =>
            {
                target.Margin = new Thickness(0);
                target.Opacity = 1;
            };

            target.BeginAnimation(UIElement.OpacityProperty, fadeIn);
            target.BeginAnimation(FrameworkElement.MarginProperty, slideUp);
        }

    }



}
