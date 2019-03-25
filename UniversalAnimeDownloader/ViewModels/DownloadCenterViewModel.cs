using MaterialDesignThemes.Wpf;
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
            PauseResumeDownloadCommand = new RelayCommand<Button>(CanPauseCancelButtonExcute, p =>
            {
                var data = p.DataContext as DownloadInstance;
                var grd = p.Content as Grid;
                if (data.State == UADDownloaderState.Working)
                {
                    data.Pause();
                    (grd.Children[0] as PackIcon).Kind = PackIconKind.Play;
                    (grd.Children[1] as TextBlock).Text = "Resume Download";
                }
                else
                {
                    data.Resume();
                    (grd.Children[0] as PackIcon).Kind = PackIconKind.Pause;
                    (grd.Children[1] as TextBlock).Text = "Pause Download";
                }
            });
            CancelDownloadCommand = new RelayCommand<Button>(CanPauseCancelButtonExcute, p =>
            {
                var data = p.DataContext as DownloadInstance;
                data.Cancel();
            });
            RemoveFromListCommand = new RelayCommand<Button>(p => true, p =>
            {
                var data = p.DataContext as DownloadInstance;
                if (data.State == UADDownloaderState.Working)
                    data.Cancel();

                DownloadManager.Instances.Remove(data);
            });
            CancelAllCommand = new RelayCommand<Button>(p => true, p =>
            {
                foreach (var item in DownloadManager.Instances)
                    if (item.State == UADDownloaderState.Working)
                        item.Cancel();
            });
            PauseAllCommand = new RelayCommand<Button>(p => true, p =>
            {
                foreach (var item in DownloadManager.Instances)
                    if (item.State == UADDownloaderState.Working)
                        item.Pause();
            });
            ResumeAllCommand = new RelayCommand<Button>(p => true, p =>
            {
                foreach (var item in DownloadManager.Instances)
                    if (item.State == UADDownloaderState.Paused)
                        item.Resume();
            });
            RemoveAllCommand = new RelayCommand<Button>(p => true, p =>
            {
                foreach (var item in DownloadManager.Instances)
                    if (item.State == UADDownloaderState.Working)
                        item.Cancel();

                DownloadManager.Instances.Clear();
            });
        }

        private bool CanPauseCancelButtonExcute(Button p)
        {
            var data = p.DataContext as DownloadInstance;
            return data.State == UADDownloaderState.Working || data.State == UADDownloaderState.Paused;
        }
    }
}
