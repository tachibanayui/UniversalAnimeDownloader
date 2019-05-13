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
using UniversalAnimeDownloader.ViewModels;

namespace UniversalAnimeDownloader.UcContentPages
{
    /// <summary>
    /// Interaction logic for ErrorOccuredOverlay.xaml
    /// </summary>
    public partial class ErrorOccuredOverlay : UserControl
    {
        public Exception ErrorInfo
        {
            get { return (Exception)GetValue(ErrorInfoProperty); }
            set { SetValue(ErrorInfoProperty, value); }
        }
        public static readonly DependencyProperty ErrorInfoProperty =
            DependencyProperty.Register("ErrorInfo", typeof(Exception), typeof(ErrorOccuredOverlay), new PropertyMetadata());

        public ICommand ReportErrorCommand { get; set; }
        public ErrorOccuredOverlay()
        {
            InitializeComponent();
            ReportErrorCommand = new RelayCommand<object>(null, p => 
            {
                ReportErrorHelper.ReportError(ErrorInfo);
            });
        }
    }
}
