using System;

namespace UniversalAnimeDownloader.ViewModels
{
    public class ButtonActionEventArgs : EventArgs
    {
        public MessageDialogResult Result { get; set; }
    }
}