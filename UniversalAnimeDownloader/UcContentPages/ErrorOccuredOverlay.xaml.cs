using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Interaction logic for ErrorOccuredOverlay.xaml
    /// </summary>
    public partial class ErrorOccuredOverlay : UserControl
    {
        public ICommand DetailButtonCommand
        {
            get { return (ICommand)GetValue(DetailButtonCommandProperty); }
            set { SetValue(DetailButtonCommandProperty, value); }
        }
        public static readonly DependencyProperty DetailButtonCommandProperty =
            DependencyProperty.Register("DetailButtonCommand", typeof(ICommand), typeof(ErrorOccuredOverlay), new PropertyMetadata());

        public ErrorOccuredOverlay()
        {
            InitializeComponent();
            Binding binding = new Binding("ReloadButtonCommand");
            BindingOperations.SetBinding(btnDetail, ButtonBase.CommandProperty, binding);
        }
    }
}
