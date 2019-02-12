using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using UniversalAnimeDownloader.Models;

namespace UniversalAnimeDownloader
{
    static class Common
    {
        public static string SourceVersions { get; set; } = $"https://api.github.com/repos/quangaming2929/UniversalAnimeDownloader/releases/latest";
        public static string CurrentVersionName { get; set; } = "v0.8.1";
        public static DateTime CurrentVersionReleaseDate { get; set; } = DateTime.Parse("2/2/2019 6:46:04 PM");

        public static async Task<GitHubData> GetLatestUpdate(string accessToken = null)
        {
            string apiResult = string.Empty;
            string req = SourceVersions;
            if(!string.IsNullOrEmpty(accessToken))
            {
                req += $"?access_token={accessToken}";
            }
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
            catch
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(req);
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
                System.Drawing.Imaging.PixelFormat.Format32bppArgb))
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
        
        public static string AddHtmlColorBody(string content)
        {
            var color = (Application.Current.Resources["ForeGroundColor"] as SolidColorBrush).Color;
            string rgbValue = $"rgb({color.R}, {color.G}, {color.B})";
            return $"<body style=\"color: {rgbValue}\">" + content + " </body>";
        }

        public static string GetFolderName(string path) => path.Substring(path.LastIndexOf('\\') + 1);

        //Report error
        public static async void ReportError(Exception e)
        {
            string title = "Error received from user: " + e.ToString().Substring(0, e.ToString().IndexOf("\r\n"));

            string content = "Exception Message: \r\n" + e.ToString() + "\r\n\r\n";
            content += await View.ReportError.GetUserInformationDialog(e) + "\r\n\r\n";

            if (CheckForErrorDuplication(e))
                Environment.Exit(-1);

            content += "Additional Information: \r\n";
            content += "Program Dir: " + AppDomain.CurrentDomain.BaseDirectory + "\r\n";
            content += "Program Version: " + CurrentVersionName + "\r\n";
            content += "Program Release Date: " + CurrentVersionReleaseDate + "\r\n";
            content += "Local time: " + DateTime.Now + "\r\n";
            content += "UTC Local time: " + DateTime.UtcNow + "\r\n";
            content += "Current Setting Profile Json:" + File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "Settings\\CurrentSettings.json") + "\r\n\r\n"; 
            content += "Enviroment Detail: \r\n";
            content += $@"Current Directory: {Environment.CurrentDirectory}
Machine name: {Environment.MachineName}
OSVersion: {Environment.OSVersion}
Processor count: {Environment.ProcessorCount}
StackTrace: {Environment.StackTrace}
System Dir: {Environment.SystemDirectory}
System Page Size: {Environment.SystemPageSize}
TickCount: {Environment.TickCount}
UserDomainName: {Environment.UserDomainName}
Username: {Environment.UserName}
Version (or CLR Version): {Environment.Version}
Working set: {Environment.WorkingSet}
ProgramFileDir: {Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)}" + "\r\n\r\n";
            content += "AppDirectory Tree: \r\n";

            content += CreateDirectoryTree(AppDomain.CurrentDomain.BaseDirectory) + "\r\n\r\n";

            content += await GetVuigheResp();

            SendEmailToUADServices(title, content);

            SaveErrorLog(e);
            Application.Current.Shutdown();
        }

        private static void SaveErrorLog(Exception e)
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory + "ErrorLog";
            string compare = e.ToString();
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            string filePath = dir + "\\" + (Directory.GetFiles(dir).Length + 1) + ".txt";
            File.WriteAllText(filePath, compare);
        }

        private static bool CheckForErrorDuplication(Exception e)
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory + "ErrorLog";
            string compare = e.ToString();
            if (!Directory.Exists(dir))
                return false;

            foreach (string item in Directory.GetFiles(dir))
            {
                if (compare == File.ReadAllText(item))
                    return true;
            }

            return false;
        }

        private static async Task<string> GetVuigheResp()
        {
            HttpWebRequest resq = (HttpWebRequest)WebRequest.Create("https://vuighe.net/");
            using (var resp = await resq.GetResponseAsync())
            {
                using (var stream = resp.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream);
                    return await reader.ReadToEndAsync();
                }
            }
        }

        private static string CreateDirectoryTree(string baseDirectory)
        {
            string result = "-| " + System.IO.Path.GetDirectoryName(baseDirectory) + "\r\n";
            result = GetFolderContent(baseDirectory, result, 1);
            return result;
        }

        private static string GetFolderContent(string baseDirectory, string result, int recursiveLevel)
        {
            foreach (string item in Directory.GetFiles(baseDirectory))
            {
                result += DrawTreeItem(recursiveLevel, false);
                result += System.IO.Path.GetFileName(item) + "\r\n";
            }

            foreach (string item in Directory.GetDirectories(baseDirectory))
            {
                result += DrawTreeItem(recursiveLevel, true);
                result += GetFolderName(item) + "\r\n";
                result = GetFolderContent(item, result, recursiveLevel + 1);
            }

            return result;
        }

        private static string DrawTreeItem(int recursiveLevel, bool isFolder)
        {
            string result = " ";
            for (int i = 0; i < recursiveLevel; i++)
                result += "|--";
            result += isFolder ? "| " : "- ";
            return result;
        }

        public static void SendEmailToUADServices(string title, string content)
        {

            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress("uad.reportservices@gmail.com");
                mail.To.Add("uad.apiservices@gmail.com");
                mail.Subject = title;
                mail.Body = content;

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new NetworkCredential("uad.reportservices@gmail.com", "uadProject");
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);
            }
            catch { }
        }
    }
}
