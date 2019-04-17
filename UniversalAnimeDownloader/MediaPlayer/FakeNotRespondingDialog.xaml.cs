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

namespace UniversalAnimeDownloader.MediaPlayer
{
    /// <summary>
    /// Interaction logic for FakeNotRespondingDialog.xaml
    /// </summary>
    public partial class FakeNotRespondingDialog : Window
    {
        public FakeWindowProblemReporting Fake { get; set; }
        public bool CloseLock = true;

        public FakeNotRespondingDialog()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Fake = new FakeWindowProblemReporting();
            Fake.ShowDialog();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) => Hide();

        private void CancelClose(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Hide();
            e.Cancel = CloseLock;
        }
    }
}
