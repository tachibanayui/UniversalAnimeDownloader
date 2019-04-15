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
    /// Interaction logic for AnimeItemCard.xaml
    /// </summary>
    public partial class AnimeItemCard : UserControl
    {
        public ICommand AnimeCardCommand
        {
            get { return (ICommand)GetValue(AnimeCardCommandProperty); }
            set { SetValue(AnimeCardCommandProperty, value); }
        }
        public static readonly DependencyProperty AnimeCardCommandProperty =
            DependencyProperty.Register("AnimeCardCommand", typeof(ICommand), typeof(AnimeItemCard), new PropertyMetadata());

        public ICommand SecondAnimeCardCommand
        {
            get { return (ICommand)GetValue(SecondAnimeCardCommandProperty); }
            set { SetValue(SecondAnimeCardCommandProperty, value); }
        }
        public static readonly DependencyProperty SecondAnimeCardCommandProperty =
            DependencyProperty.Register("SecondAnimeCardCommand", typeof(ICommand), typeof(AnimeItemCard), new PropertyMetadata());

       

        public AnimeItemCard()
        {
            InitializeComponent();
        }
    }
}
