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

namespace UniversalAnimeDownloader.UcContentPages
{
    /// <summary>
    /// Interaction logic for CustomAnimeSeriesEditor.xaml
    /// </summary>
    public partial class CustomAnimeSeriesEditor : UserControl
    {
        public CustomAnimeSeriesEditor()
        {
            InitializeComponent();
        }



        public ICommand CancelButtonCommand
        {
            get { return (ICommand)GetValue(CancelButtonCommandProperty); }
            set { SetValue(CancelButtonCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CancelButtonCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CancelButtonCommandProperty =
            DependencyProperty.Register("CancelButtonCommand", typeof(ICommand), typeof(CustomAnimeSeriesEditor), new PropertyMetadata());




        public ICommand SaveButtonCommand
        {
            get { return (ICommand)GetValue(SaveButtonCommandProperty); }
            set { SetValue(SaveButtonCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SaveButtonCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SaveButtonCommandProperty =
            DependencyProperty.Register("SaveButtonCommand", typeof(ICommand), typeof(CustomAnimeSeriesEditor), new PropertyMetadata());


    }
}
