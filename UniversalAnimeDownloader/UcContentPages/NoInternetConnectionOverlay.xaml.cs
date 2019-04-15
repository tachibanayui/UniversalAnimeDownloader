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
    /// Interaction logic for NoInternetConnectionOverlay.xaml
    /// </summary>
    public partial class NoInternetConnectionOverlay : UserControl
    {
        public ICommand ReloadButtonCommand
        {
            get { return (ICommand)GetValue(ReloadButtonCommandProperty); }
            set { SetValue(ReloadButtonCommandProperty, value); }
        }
        public static readonly DependencyProperty ReloadButtonCommandProperty =
            DependencyProperty.Register("ReloadButtonCommand", typeof(ICommand), typeof(NoInternetConnectionOverlay), new PropertyMetadata());

        public event EventHandler<RoutedEventArgs> ReloadButtonClicked;
        protected virtual void OnReloadButtonClicked(RoutedEventArgs e) => ReloadButtonClicked?.Invoke(this, e);

        public NoInternetConnectionOverlay()
        {
            InitializeComponent();
            Binding binding = new Binding("ReloadButtonCommand");
            BindingOperations.SetBinding(btnReload, ButtonBase.CommandProperty, binding);
        }

        private void Button_Click(object sender, RoutedEventArgs e) => OnReloadButtonClicked(e);
    }
}
