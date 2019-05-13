using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UniversalAnimeDownloader.UcContentPages
{
    /// <summary>
    /// Interaction logic for UserFeedback.xaml
    /// </summary>
    public partial class UserFeedback : UserControl, INotifyPropertyChanged
    {
        #region Commands
        public ICommand SendCommand { get; set; }
        public ICommand ViewErrorCommand { get; set; }
        public ICommand CloseCommand { get; set; }
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

        private Visibility _ViewErrorVisibility;
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

        private Visibility _CloseVisibility;
        public Visibility CloseVisibility
        {
            get
            {
                return _CloseVisibility;
            }
            set
            {
                if (_CloseVisibility != value)
                {
                    _CloseVisibility = value;
                    OnPropertyChanged();
                }
            }
        }

        private Visibility _DescribeProblemVisibility;
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
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
