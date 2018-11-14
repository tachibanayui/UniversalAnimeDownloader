using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UniversalAnimeDownloader.ViewModel;

namespace UniversalAnimeDownloader.View
{
    /// <summary>
    /// Interaction logic for OnlineAnimeDetail.xaml
    /// </summary>
    public partial class OnlineAnimeDetail : Page
    {
        public OnlineAnimeDetailViewModel VM;

        public OnlineAnimeDetail()
        {
            InitializeComponent();
            VM = new OnlineAnimeDetailViewModel(Dispatcher);
            DataContext = VM;
        }
    }
}
