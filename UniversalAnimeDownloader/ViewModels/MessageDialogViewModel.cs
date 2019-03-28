using System;
using System.Windows;
using System.Windows.Input;

namespace UniversalAnimeDownloader.ViewModels
{
    public class MessageDialogViewModel : BaseViewModel
    {
        public ICommand ActionButtonCommand { get; set; }

        public event EventHandler<ButtonActionEventArgs> ActionButtonClicked;

        private void OnActionButtonClicked(MessageDialogResult e) => ActionButtonClicked?.Invoke(this, new ButtonActionEventArgs() { Result = e });


        public MessageDialogViewModel()
        {
            ActionButtonCommand = new RelayCommand<string>(p => true, p =>
            {
                IsDialogOpen = false;
                switch (p)
                {
                    case "Yes":
                        OnActionButtonClicked(MessageDialogResult.Yes);
                        break;
                    case "No":
                        OnActionButtonClicked(MessageDialogResult.No);
                        break;
                    case "OK":
                        OnActionButtonClicked(MessageDialogResult.OK);
                        break;
                    case "Cancel":
                        OnActionButtonClicked(MessageDialogResult.Cancel);
                        break;
                    default:
                        break;
                }
            });
        }


        private string _MessageTitle = "Message Title";
        public string MessageTitle
        {
            get
            {
                return _MessageTitle;
            }
            set
            {
                if (_MessageTitle != value)
                {
                    _MessageTitle = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _MessageText = "Message Text ...";
        public string MessageText
        {
            get
            {
                return _MessageText;
            }
            set
            {
                if (_MessageText != value)
                {
                    _MessageText = value;
                    OnPropertyChanged();
                }
            }
        }

        private Visibility _YesButtonVisibility = Visibility.Collapsed;
        public Visibility YesButtonVisibility
        {
            get
            {
                return _YesButtonVisibility;
            }
            set
            {
                if (_YesButtonVisibility != value)
                {
                    _YesButtonVisibility = value;
                    OnPropertyChanged();
                }
            }
        }
        private Visibility _NoButtonVisibility = Visibility.Collapsed;
        public Visibility NoButtonVisibility
        {
            get
            {
                return _NoButtonVisibility;
            }
            set
            {
                if (_NoButtonVisibility != value)
                {
                    _NoButtonVisibility = value;
                    OnPropertyChanged();
                }
            }
        }
        private Visibility _OKButtonVisibility = Visibility.Visible;
        public Visibility OKButtonVisibility
        {
            get
            {
                return _OKButtonVisibility;
            }
            set
            {
                if (_OKButtonVisibility != value)
                {
                    _OKButtonVisibility = value;
                    OnPropertyChanged();
                }
            }
        }

        private Visibility _CancelVisibility = Visibility.Visible;
        public Visibility CancelVisibility
        {
            get
            {
                return _CancelVisibility;
            }
            set
            {
                if (_CancelVisibility != value)
                {
                    _CancelVisibility = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _IsDialogOpen;
        public bool IsDialogOpen
        {
            get
            {
                return _IsDialogOpen;
            }
            set
            {
                if (_IsDialogOpen != value)
                {
                    _IsDialogOpen = value;
                    OnPropertyChanged();
                }
            }
        }


    }
}
