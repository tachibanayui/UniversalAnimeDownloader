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
    /// Interaction logic for ShowMoreAnimeCard.xaml
    /// </summary>
    public partial class ShowMoreAnimeCard : UserControl
    {
        public ICommand AnimeCardCommand
        {
            get { return (ICommand)GetValue(AnimeCardCommandProperty); }
            set { SetValue(AnimeCardCommandProperty, value); }
        }
        public static readonly DependencyProperty AnimeCardCommandProperty =
            DependencyProperty.Register("AnimeCardCommand", typeof(ICommand), typeof(ShowMoreAnimeCard), new PropertyMetadata());

        public object AnimeCardCommandParameter
        {
            get { return (object)GetValue(AnimeCardCommandParameterProperty); }
            set { SetValue(AnimeCardCommandParameterProperty, value); }
        }
        public static readonly DependencyProperty AnimeCardCommandParameterProperty =
            DependencyProperty.Register("AnimeCardCommandParameter", typeof(object), typeof(ShowMoreAnimeCard), new PropertyMetadata());

        public ShowMoreAnimeCard()
        {
            InitializeComponent();
        }
    }
}
