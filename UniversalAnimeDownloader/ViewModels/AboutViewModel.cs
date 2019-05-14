using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

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
        }
    }
}
