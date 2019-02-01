using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace UniversalAnimeDownloader.View
{
    /// <summary>
    /// Interaction logic for ReportError.xaml
    /// </summary>
    public partial class ReportError : Window
    {
        public Exception ExceptionDetail { get; set; }
        public ManualResetEvent TaskReset { get; set; }
        public string UserInfo { get; set; }

        public ReportError()
        {
            InitializeComponent();
        }

        private void ShowException(object sender, RoutedEventArgs e) => MessageBox.Show(ExceptionDetail.ToString(), "Error detail");

        private void Window_Closing(object sender, CancelEventArgs e) => SubmitError();

        private void Button_Click(object sender, RoutedEventArgs e) => SubmitError();

        private void SubmitError()
        {
            string name = string.Empty, email = string.Empty, feedback = string.Empty, isUrgent = string.Empty;
            Dispatcher.Invoke(() => {
                name = txbName.Text;
                email = txbAddress.Text;
                feedback = txbFeedback.Text;
                isUrgent = (bool)ckbIsUrgent.IsChecked ? "Yes" : "No";
            });

            UserInfo += $@"Name: {name}
Email: {email}
Feedback: {feedback}
IsUrgent: {isUrgent}";

            TaskReset.Set();
        }

        public static async Task<string> GetUserInformationDialog(Exception e)
        {
            ReportError errorDialog = new ReportError();
            errorDialog.ExceptionDetail = e;
            ManualResetEvent resetEvent = new ManualResetEvent(false);
            errorDialog.TaskReset = resetEvent;
            errorDialog.Show();

            await Task.Run(() => 
            {
                resetEvent.WaitOne();
            });

            errorDialog.Close();
            return errorDialog.UserInfo;
        }
    }
}
