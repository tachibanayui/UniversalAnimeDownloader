using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversalAnimeDownloader.ViewModel
{
    public class SettingsAccountViewModel : ViewModelBase
    {
        public ObservableCollection<FriendListViewModel> FriendList { get; set; }


        public SettingsAccountViewModel()
        {
            FriendList = new ObservableCollection<FriendListViewModel>();
            FriendList.Add(new FriendListViewModel() { Username = "Example User 1", UserID = 1001, UserDescription = "Yolo!!!" });
            FriendList.Add(new FriendListViewModel() { Username = "Example User 2", UserID = 1002, UserDescription = "Yolo!!!" });
            FriendList.Add(new FriendListViewModel() { Username = "Example User 3", UserID = 1003, UserDescription = "Yolo!!!" });

        }
    }
}
