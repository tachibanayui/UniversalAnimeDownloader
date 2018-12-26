using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversalAnimeDownloader.ViewModel
{
    public class UADPlayerViewModel : ViewModelBase
    {
        private double seekLocation;
        public double SeekLocation
        {
            get { return seekLocation; }
            set
            {
                if(seekLocation != value)
                {
                    seekLocation = value;
                    OnPropertyChanged("SeekLocation");
                }
            }
        }
    }
}
