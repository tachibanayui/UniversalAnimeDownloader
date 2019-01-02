using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace UniversalAnimeDownloader
{
    static class Common
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

        public static string[] GetQualities = new string[] { "144p", "240p", "360p", "480p", "720p", "1080p" };

        public static string AnimeLibraryDirectory;

        public static Thickness PlusThickness(Thickness a, Thickness b)
        {
            return new Thickness(a.Left + b.Left, a.Top + b.Top, right: a.Right + b.Right, bottom: a.Bottom + b.Bottom);
        }

        public static Thickness MulitplyThinkness(Thickness a, double b)
        {
            return new Thickness(a.Left * b, a.Top * b, a.Right * b, a.Bottom * b);
        }

        public static double GetTimeSpanRatio(TimeSpan a, TimeSpan b) => a.TotalMilliseconds / b.TotalMilliseconds;

        public static TimeSpan MutiplyTimeSpan(TimeSpan a, double b)
        {
            double milli = a.TotalMilliseconds * b;
            return TimeSpan.FromMilliseconds(milli);
        }

        public static BitmapSource CopyScreen(Rectangle cropRect)
        {
            using (var screenBmp = new Bitmap(
                (int)SystemParameters.PrimaryScreenWidth,
                (int)SystemParameters.PrimaryScreenHeight,
                PixelFormat.Format32bppArgb))
            {
                using (var bmpGraphics = Graphics.FromImage(screenBmp))
                {
                    bmpGraphics.CopyFromScreen(0, 0, 0, 0, screenBmp.Size);

                    Bitmap target = new Bitmap(cropRect.Width, cropRect.Height);

                    using (Graphics g = Graphics.FromImage(target))
                    {
                        g.DrawImage(screenBmp, new Rectangle(0, 0, target.Width, target.Height),
                                         cropRect,
                                         GraphicsUnit.Pixel);
                    }
                    return Imaging.CreateBitmapSourceFromHBitmap(
                        target.GetHbitmap(),
                        IntPtr.Zero,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());
                }
            }
        }

        public static void FadeInAnimation(UIElement target, Duration duration, bool removePreviousAnimation = true)
        {
            DoubleAnimation animation = new DoubleAnimation(1, duration);
            if(removePreviousAnimation)
                target.BeginAnimation(UIElement.OpacityProperty, null);
            target.BeginAnimation(UIElement.OpacityProperty, animation);
        }

        public static void FadeOutAnimation(UIElement target, Duration duration, bool removePreviousAnimation = true)
        {
            DoubleAnimation animation = new DoubleAnimation(0, duration);
            if(removePreviousAnimation)
                target.BeginAnimation(UIElement.OpacityProperty, null);
            target.BeginAnimation(UIElement.OpacityProperty, animation);
        }

        public static View.MainWindow MainWin;

        public static void CancelCloseWindow(object sender, CancelEventArgs e) => e.Cancel = true;

    }
}
