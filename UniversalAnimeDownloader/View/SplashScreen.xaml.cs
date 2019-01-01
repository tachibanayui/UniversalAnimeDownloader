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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Net;

namespace UniversalAnimeDownloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Background="{DynamicResource MaterialDesignPaper}"
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var fade = FadeInEffect(this, .5);
            fade.Completed += (s, ee) => 
            {
                var progressInit = AnimateChangeValue(progressAction, 50, .175);
                progressInit.Completed += (ss, eee) => 
                {
                    text.Text = "Connecting...";
                    Common.InternetAvaible = Common.CheckForInternetConnection();
                    var progressConnect = AnimateChangeValue(progressAction, 100, 2);
                    progressConnect.Completed += FinishInit;
                    progressAction.BeginAnimation(RangeBase.ValueProperty, progressConnect);
                };
                progressAction.BeginAnimation(RangeBase.ValueProperty, progressInit);
            };


            BeginAnimation(OpacityProperty, fade);
        }

        private void FinishInit(object sender, EventArgs e)
        {
            Common.MainWin = new View.MainWindow();
            Common.MainWin.Show();
            Close();
        }

        private DoubleAnimation FadeInEffect(Control target, double interval)
        {
            DoubleAnimation FadeInAnim = new DoubleAnimation(1, TimeSpan.FromSeconds(interval));
            return FadeInAnim;
        }

        private DoubleAnimation AnimateChangeValue(Control target, double toValue, double interval)
        {
            DoubleAnimation animation = new DoubleAnimation(toValue, TimeSpan.FromSeconds(interval));
            target.BeginAnimation(RangeBase.ValueProperty, animation);
            return animation;
        }
    }
}
