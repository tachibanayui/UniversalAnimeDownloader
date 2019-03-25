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

        public NotifycationPanelViewModel()
        {
            InvokeActionButton = new RelayCommand<Button>(p => true, p => (p.DataContext as NotificationItem).ButtonAction.BeginInvoke(BtnActionCallBack, (p.DataContext as NotificationItem).ButtonAction));
            RemoveNotificationItem = new RelayCommand<Button>(p => true, p => NotificationManager.Remove(p.DataContext as NotificationItem));
        }

        private void BtnActionCallBack(IAsyncResult ar) => (ar.AsyncState as Action).EndInvoke(ar);
    }
}
