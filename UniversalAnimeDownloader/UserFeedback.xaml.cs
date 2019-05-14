using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using UniversalAnimeDownloader.ViewModels;

namespace UniversalAnimeDownloader
{
    /// <summary>
    /// Interaction logic for UserFeedback.xaml
    /// </summary>
    public partial class UserFeedback : Window, INotifyPropertyChanged
    {
        #region Properties / Fields
        private const string NotReportMessage = "This is not a error report!";
        public Exception ExceptionDetail { get; set; }
        public ManualResetEvent Waiter { get; set; }
        public string UserInfo { get; set; }
        private bool isReport;

        public bool IsReport
        {
            get { return isReport; }
            set
            {
                isReport = value;
                if (value)
                {
                    FeedbackTitle = "Report error";
                    FeedbackDescription = "I am sorry because you have experience this issue. Leave your information below, and we will try and respone to you as fast as possible. You can skip any field if you want";
                    ReportInfoVisibility = Visibility.Visible;
                }
            }
        }

        #endregion

        #region Commands
        public ICommand SendCommand { get; set; }
        public ICommand ViewErrorCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand GithubReportErrorCommand { get; set; }
        #endregion

        #region BindableProperties

        #region Text
        private string _FeedbackTitle = "Feedback";
        public string FeedbackTitle
        {
            get
            {
                return _FeedbackTitle;
            }
            set
            {
                if (_FeedbackTitle != value)
                {
                    _FeedbackTitle = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _FeedbackDescription = "Thank you to share you experience. We will try our best to improve Universal Anime Downloader";
        public string FeedbackDescription
        {
            get
            {
                return _FeedbackDescription;
            }
            set
            {
                if (_FeedbackDescription != value)
                {
                    _FeedbackDescription = value;
                    OnPropertyChanged();
                }
            }
        }

        private Visibility _ReportInfoVisibility = Visibility.Collapsed;
        public Visibility ReportInfoVisibility
        {
            get
            {
                return _ReportInfoVisibility;
            }
            set
            {
                if (_ReportInfoVisibility != value)
                {
                    _ReportInfoVisibility = value;
                    OnPropertyChanged();
                }
            }
        }


        #endregion

        #region Information
        private string _FirstName;
        public string FirstName
        {
            get
            {
                return _FirstName;
            }
            set
            {
                if (_FirstName != value)
                {
                    _FirstName = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _LastName;
        public string LastName
        {
            get
            {
                return _LastName;
            }
            set
            {
                if (_LastName != value)
                {
                    _LastName = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _EmailAddress;
        public string EmailAddress
        {
            get
            {
                return _EmailAddress;
            }
            set
            {
                if (_EmailAddress != value)
                {
                    _EmailAddress = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _ProblemDescription;
        public string ProblemDescription
        {
            get
            {
                return _ProblemDescription;
            }
            set
            {
                if (_ProblemDescription != value)
                {
                    _ProblemDescription = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _CustomerFeedBack;
        public string CustomerFeedBack
        {
            get
            {
                return _CustomerFeedBack;
            }
            set
            {
                if (_CustomerFeedBack != value)
                {
                    _CustomerFeedBack = value;
                    OnPropertyChanged();
                }
            }
        }


        private bool _ReceiveUpdate;
        public bool ReceiveUpdate
        {
            get
            {
                return _ReceiveUpdate;
            }
            set
            {
                if (_ReceiveUpdate != value)
                {
                    _ReceiveUpdate = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        private Visibility _ViewErrorVisibility = Visibility.Visible;
        public Visibility ViewErrorVisibility
        {
            get
            {
                return _ViewErrorVisibility;
            }
            set
            {
                if (_ViewErrorVisibility != value)
                {
                    _ViewErrorVisibility = value;
                    OnPropertyChanged();
                }
            }
        }

        private Visibility _DescribeProblemVisibility = Visibility.Visible;
        public Visibility DescribeProblemVisibility
        {
            get
            {
                return _DescribeProblemVisibility;
            }
            set
            {
                if (_DescribeProblemVisibility != value)
                {
                    _DescribeProblemVisibility = value;
                    OnPropertyChanged();
                }
            }
        }




        #endregion

        public UserFeedback()
        {
            ViewErrorCommand = new RelayCommand<object>(null, p => MessageBox.Show(ExceptionDetail.ToString(), "Exception Detail"));
            CloseCommand = new RelayCommand<object>(null, p => Waiter.Set());
            SendCommand = new RelayCommand<object>(null, p =>
            {
                var reportMessage = IsReport ? ProblemDescription : NotReportMessage;
                UserInfo = $"First Name: {FirstName}\r\nLast Name: {LastName}\r\nEmail Address: {EmailAddress}\r\nProblem: {reportMessage}\r\nFeedback: {CustomerFeedBack}";
                Waiter.Set();
            });
            GithubReportErrorCommand = new RelayCommand<object>(null, p => Process.Start("https://github.com/quangaming2929/UniversalAnimeDownloader/issues/new"));
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Get the user input for this error
        /// </summary>
        /// <param name="e"></param>
        /// <returns>information user provided, empty string if user canceled</returns>
        public static async Task<string> GetReportInfo(Exception e)
        {
            UserFeedback feedBack = new UserFeedback();
            feedBack.ExceptionDetail = e;
            feedBack.IsReport = true;
            feedBack.Show();
            feedBack.Waiter = new ManualResetEvent(false);
            feedBack.Show();
            await Task.Run(() => feedBack.Waiter.WaitOne());
            feedBack.Close();
            return feedBack.UserInfo;
        }

        /// <summary>
        /// Get user input for feedback
        /// </summary>
        /// <returns>information user provided, empty string if user canceled</returns>
        public static async Task<string> GetFeedbackInfo()
        {
            UserFeedback feedBack = new UserFeedback();
            feedBack.ViewErrorVisibility = Visibility.Collapsed;
            feedBack.DescribeProblemVisibility = Visibility.Collapsed;
            feedBack.Show();
            feedBack.Waiter = new ManualResetEvent(false);
            feedBack.Show();
            await Task.Run(() => feedBack.Waiter.WaitOne());
            feedBack.Close();
            return feedBack.UserInfo;
        }
    }
}
