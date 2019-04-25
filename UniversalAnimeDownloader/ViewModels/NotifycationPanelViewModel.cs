using Hardcodet.Wpf.TaskbarNotification;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using UADAPI;

namespace UniversalAnimeDownloader.ViewModels
{
    class NotifycationPanelViewModel : BaseViewModel
    {
        public ICommand InvokeActionButton { get; set; }
        public ICommand RemoveNotificationItem { get; set; }
        public ICommand ClearCommand { get; set; }
        public ICommand HostLoaded { get; set; }

        public Card BallonHost { get; set; }
        public TaskbarIcon MainTaskBarIcon { get; set; }

        public NotifycationPanelViewModel()
        {
            InvokeActionButton = new RelayCommand<Button>(p => true, async p => await (p.DataContext as NotificationItem).InvokeAsync());
            RemoveNotificationItem = new RelayCommand<Button>(p => true, p => NotificationManager.Remove(p.DataContext as NotificationItem));
            ClearCommand = new RelayCommand<object>(p => true, p => NotificationManager.RemoveAll());
            
            BallonHost = Application.Current.FindResource("ToolbarTrayBallon") as Card;
            MainTaskBarIcon = Application.Current.FindResource("mainToolbarTray") as TaskbarIcon;
            NotificationManager.ItemAdded += ShowToolbarTrayBaloon;
        }

        private void ShowToolbarTrayBaloon(object sender, NotificationEventArgs e)
        {
            BallonHost.DataContext = e.AffectededItem;

            MainTaskBarIcon.ShowCustomBalloon(BallonHost, PopupAnimation.Slide, 10000);
        } 
    }
}
