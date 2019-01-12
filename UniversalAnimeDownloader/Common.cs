using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using UniversalAnimeDownloader.Models;

namespace UniversalAnimeDownloader
{
    static class Common
    {
        #region a
        private static string AccessToken = "af1a2d0dfe65224918e1259f79a2c55a677dbf00";
        #endregion
        public static string SourceVersions { get; set; } = $"https://api.github.com/repos/quangaming2929/UniversalAnimeDownloader/releases/latest?access_token={AccessToken}";
        public static string CurrentVersionName { get; set; } = "v0.8.2_beta";
        public static DateTime CurrentVersionReleaseDate { get; set; } = DateTime.Parse("1 Jan, 2019");

        public static async Task<GitHubData> GetLatestUpdate()
        {
            string apiResult = string.Empty;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(SourceVersions);
                request.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/64.0.3282.140 Safari/537.36 Edge/17.17134";
                using (WebResponse resp = await request.GetResponseAsync())
                {
                    using (Stream stream = resp.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream);
                        apiResult = await reader.ReadToEndAsync();
                    }
                }
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Can't get latest version!", e);
            }

            return JsonConvert.DeserializeObject<GitHubData>(apiResult);
        }

        public static void DownloadUpdateContent(GitHubData data, AsyncCompletedEventHandler callback = null)
        {
            WebClient client = new WebClient();

            string path = AppDomain.CurrentDomain.BaseDirectory + "Updates\\";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            client.DownloadFileAsync(new Uri(data.assets[0].browser_download_url), path + data.assets[0].name);

            if(callback != null)
                client.DownloadFileCompleted += callback;
        }

        public static string HtmlWrapWithHtml(string content)
        {
            string start = @"<html><body bgcolor=""#424242"">";
            string end = @"</body></html>";
            return start + content + end;
        }

        private static bool isInternetAvaible = false;

        public static bool IsInternetAvaible
        {
            get => isInternetAvaible;
            set
            {
                if(isInternetAvaible != value)
                {
                    isInternetAvaible = value;
                    OnInternetConnectionChanged();
                }
            }
        }

        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (var stream = client.OpenRead("http://www.google.com"))
                {
                    IsInternetAvaible = true;
                    return true;
                }
            }
            catch (Exception e)
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

        public static void FadeInAnimation(UIElement target, Duration duration, bool removePreviousAnimation = true, EventHandler completedCallback = null)
        {
            DoubleAnimation animation = new DoubleAnimation(1, duration);
            if(completedCallback != null)
                animation.Completed += completedCallback;
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

        private static void OnInternetConnectionChanged() => InternetStateChanged?.Invoke(null, e: EventArgs.Empty);

        public static event EventHandler<EventArgs> InternetStateChanged;
        


        public static void VerionCheck()
        {

        }

    }
}
