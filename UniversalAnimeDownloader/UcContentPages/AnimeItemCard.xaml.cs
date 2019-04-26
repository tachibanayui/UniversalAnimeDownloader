using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UniversalAnimeDownloader.UADSettingsPortal;

namespace UniversalAnimeDownloader.UcContentPages
{
    /// <summary>
    /// Interaction logic for AnimeItemCard.xaml
    /// </summary>
    public partial class AnimeItemCard : UserControl , INotifyPropertyChanged
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


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName]string name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public AnimeItemCard()
        {
            InitializeComponent();
        }
    }
}
