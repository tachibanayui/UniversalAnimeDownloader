using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using UADAPI;

namespace UniversalAnimeDownloader.ViewModels
{
    class DownloadCenterViewModel : BaseViewModel
    {
        #region Commands
        public ICommand RemoveAllCommand { get; set; }
        public ICommand PauseAllCommand { get; set; }
        public ICommand ResumeAllCommand { get; set; }
        public ICommand CancelAllCommand { get; set; }
        public ICommand PauseResumeDownloadCommand { get; set; }
        public ICommand CancelDownloadCommand { get; set; }
        public ICommand RemoveFromListCommand { get; set; }
        #endregion

        public DownloadCenterViewModel()
        {
            
        }
    }
}
