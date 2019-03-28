using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using UniversalAnimeDownloader.ViewModels;

namespace UniversalAnimeDownloader
{
    class MessageDialog
    {
        private static MessageDialogViewModel _VM;
        public static bool IsCompleted { get; private set; } = true;
        private static MessageDialogResult res;

        static MessageDialog()
        {
            
        }

        public static async Task<MessageDialogResult> ShowAsync(string title, string content, MessageDialogButton button)
        {
            await Task.Run(async () =>
            {
                while (!IsCompleted)
                    await Task.Delay(100);
            });

            if (_VM == null)
            {
                _VM = (Application.Current.FindResource("MainWindowViewModel") as MainWindowViewModel).MessageDialogViewModel;
                _VM.ActionButtonClicked += (s, e) =>
                {
                    IsCompleted = true;
                    res = e.Result;
                };
            }

            _VM.MessageTitle = title;
            _VM.MessageText = content;
            switch (button)
            {
                case MessageDialogButton.YesNoButton:
                    _VM.YesButtonVisibility = Visibility.Visible;
                    _VM.NoButtonVisibility = Visibility.Visible;
                    _VM.OKButtonVisibility = Visibility.Collapsed;
                    _VM.CancelVisibility = Visibility.Collapsed;
                    break;
                case MessageDialogButton.OKCancelButton:
                    _VM.YesButtonVisibility = Visibility.Collapsed;
                    _VM.NoButtonVisibility = Visibility.Collapsed;
                    _VM.OKButtonVisibility = Visibility.Visible;
                    _VM.CancelVisibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
            _VM.IsDialogOpen = true;
            IsCompleted = false;
            await Task.Run(async() =>
            {
                while (!IsCompleted)
                    await Task.Delay(100);
            });

            return res;
        }
    }

   

    public enum MessageDialogButton
    {
        YesNoButton, OKCancelButton
    }

    public enum MessageDialogResult
    {
        Yes,No,OK,Cancel
    }
}
