using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using UADAPI;

namespace UniversalAnimeDownloader.ViewModels
{
    class AboutViewModel : BaseViewModel
    {
        public ICommand CheckForUpdateCommand { get; set; }
        public ICommand ShowCreditCommand { get; set; }
        public ICommand UserFeedbackCommand { get; set; }
        public ICommand OpenUADProjectCommand { get; set; }

        public AboutViewModel()
        {
            ShowCreditCommand = new RelayCommand<object>(null, p => Process.Start("https://github.com/quangaming2929/UniversalAnimeDownloader/graphs/contributors"));
            OpenUADProjectCommand = new RelayCommand<object>(null, p => Process.Start("https://github.com/quangaming2929/UniversalAnimeDownloader"));
            UserFeedbackCommand = new RelayCommand<object>(null, p => 
            {
                ReportErrorHelper.FeedBack();
            });
            CheckForUpdateCommand = new RelayCommand<object>(null, p => 
            {
                if (ApiHelpper.CheckForUpdates())
                {
                    (Application.Current.FindResource("MainWindowViewModel") as MainWindowViewModel).IsNewUpdatesDialogOpen = true;
                    (Application.Current.FindResource("MainWindowViewModel") as MainWindowViewModel).UpdateDescription = ApiHelpper.NewestVersionInfo?.body;
                }
            });
        }
    }
}
