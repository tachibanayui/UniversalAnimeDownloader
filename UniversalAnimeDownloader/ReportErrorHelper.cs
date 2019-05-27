using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Windows;
using UniversalAnimeDownloader.UADSettingsPortal;
using UADAPI;

namespace UniversalAnimeDownloader
{
    public static class ReportErrorHelper
    {
        public static async void FeedBack()
        {
            string title = "User feedback about UAD experience";
            var userInfo = await UserFeedback.GetFeedbackInfo();
            string content = $"User info: \r\n {userInfo} \r\n\r\n";

            if (!string.IsNullOrEmpty(userInfo))
                await GetAdditionalInfomationAndSend(title, content);
        }

        //Report error
        public static async void ReportError(Exception e, bool shutdownApp = false, string reportErrorTitle = null)
        {
            string title = "Error received from user: " + e.ToString().Substring(0, e.ToString().IndexOf("\r\n"));
            var userInfo = await UserFeedback.GetReportInfo(e, reportErrorTitle);
            string content = "Exception Message: \r\n" + e.ToString() + "\r\n\r\n";
            content += userInfo + "\r\n\r\n";

            await Task.Run(async () =>
            {
                if (string.IsNullOrEmpty(userInfo))
                {
                    if (shutdownApp)
                        Environment.Exit(-1);
                    else
                        return;
                }

                await GetAdditionalInfomationAndSend(title, content);

                SaveErrorLog(e);
                if (shutdownApp)
                    Application.Current.Shutdown();
            });
        }

        private static async Task GetAdditionalInfomationAndSend(string title, string content)
        {
            await Task.Run(() =>
            {

                content += "Additional Information: \r\n";
                try { content += "Program Dir: " + AppDomain.CurrentDomain.BaseDirectory + "\r\n"; } catch { }
                try { content += "Program Version: " + MiscClass.GlobalVersion + "\r\n"; } catch { }
                try { content += "Local time: " + DateTime.Now + "\r\n"; } catch { }
                try { content += "UTC Local time: " + DateTime.UtcNow + "\r\n"; } catch { }
                try { content += "Current Setting Profile Json:" + GetUsefulUserSetting() + "\r\n\r\n"; } catch { }
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
                try { content += CreateDirectoryTree(AppDomain.CurrentDomain.BaseDirectory) + "\r\n\r\n"; } catch { }
                //try { content += await GetVuigheResp(); } catch { }

                try { SendEmailToUADServices(title, content); } catch { }
            });
        }

        private static string GetUsefulUserSetting()
        {
            var content = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "Settings\\UserSetting.json");
            UADSettingsData data = JsonConvert.DeserializeObject<UADSettingsData>(content);
            data.UserInterest = string.Empty;
            data.Notification = string.Empty;
            return JsonConvert.SerializeObject(data);
        }

        private static void SaveErrorLog(Exception e)
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory + "ErrorLog";
            string compare = e.ToString();
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            //string filePath = dir + "\\" + (Directory.GetFiles(dir).Length + 1) + ".txt";
            string filePath = dir + "\\" + DateTime.Now.ToShortTimeString().RemoveInvalidChar() + ".txt";
            File.WriteAllText(filePath, compare);
        }

        public static bool CheckForErrorDuplication(Exception e)
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
                result += Path.GetFileName(item) + "\r\n";
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
            catch (Exception e) { }
        }

        public static string GetFolderName(string path) => path.Substring(path.LastIndexOf('\\') + 1);
    }
    
}
