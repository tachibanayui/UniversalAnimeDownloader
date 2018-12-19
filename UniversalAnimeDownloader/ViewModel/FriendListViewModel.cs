using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversalAnimeDownloader.ViewModel
{
    public class FriendListViewModel : ViewModelBase
    {
        private string username;
        public string Username
        {
            get { return username; }
            set
            {
                if(username != value)
                {
                    username = value;
                    OnPropertyChanged("Username");
                }
            }
        }

        private int userID;
        public int UserID
        {
            get { return userID; }
            set
            {
                if(userID != value)
                {
                    userID = value;
                    OnPropertyChanged("UserID");
                }
            }

        }

        private string userDescription;
        public string UserDescription
        {
            get { return userDescription; }
            set
            {
                if(userDescription != value)
                {
                    userDescription = value;
                    OnPropertyChanged("UserDescription");
                }
            }
        }

        private string userImageProfileLocation;
        public string UserImageProfileLocation
        {
            get { return userDescription; }
            set
            {
                if(userDescription != value)
                {
                    userDescription = value;
                    OnPropertyChanged("UserImageProfileLocation");
                }
            }
        }
        
    }
}
