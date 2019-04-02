using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using UADAPI;

namespace UniversalAnimeDownloader.ViewModels
{
    class NotifycationPanelViewModel : BaseViewModel
    {
        public ICommand InvokeActionButton { get; set; }
        public ICommand RemoveNotificationItem { get; set; }
        public ICommand ClearCommand { get; set; }

        public NotifycationPanelViewModel()
        {
            InvokeActionButton = new RelayCommand<Button>(p => true, async p => await (p.DataContext as NotificationItem).InvokeAsync());
            RemoveNotificationItem = new RelayCommand<Button>(p => true, p => NotificationManager.Remove(p.DataContext as NotificationItem));
            ClearCommand = new RelayCommand<object>(p => true, p => NotificationManager.RemoveAll());
        }
    }
}
