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
using System.Windows.Shapes;

namespace UniversalAnimeDownloader.UserControls
{
    /// <summary>
    /// Interaction logic for FakeWindowProblemReporting.xaml
    /// </summary>
    public partial class FakeWindowProblemReporting : Window
    {
        public FakeWindowProblemReporting()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            btn.IsEnabled = false;
        }
    }
}
