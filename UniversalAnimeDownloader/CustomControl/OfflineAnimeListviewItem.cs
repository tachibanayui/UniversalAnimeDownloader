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

namespace UniversalAnimeDownloader.CustomControl
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:UniversalAnimeDownloader.CustomControl"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:UniversalAnimeDownloader.CustomControl;assembly=UniversalAnimeDownloader.CustomControl"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:OfflineAnimeListviewItem/>
    ///
    /// </summary>
    public class OfflineAnimeListviewItem : Control
    {
        static OfflineAnimeListviewItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(OfflineAnimeListviewItem), new FrameworkPropertyMetadata(typeof(OfflineAnimeListviewItem)));
        }



        public string EpisodeName
        {
            get { return (string)GetValue(EpisodeNameProperty); }
            set { SetValue(EpisodeNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EpisodeName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EpisodeNameProperty =
            DependencyProperty.Register("EpisodeName", typeof(string), typeof(OfflineAnimeListviewItem), new PropertyMetadata("Episode Name"));



        public string MediaLocation
        {
            get { return (string)GetValue(MediaLocationProperty); }
            set { SetValue(MediaLocationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MediaLocation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MediaLocationProperty =
            DependencyProperty.Register("MediaLocation", typeof(string), typeof(OfflineAnimeListviewItem), new PropertyMetadata("Location"));



        public double EpisodeNameFontSize
        {
            get { return (double)GetValue(EpisodeNameFontSizeProperty); }
            set { SetValue(EpisodeNameFontSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EpisodeNameFontSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EpisodeNameFontSizeProperty =
            DependencyProperty.Register("EpisodeNameFontSize", typeof(double), typeof(OfflineAnimeListviewItem), new PropertyMetadata(18d));




        public Style PlayButtonStyle
        {
            get { return (Style)GetValue(PlayButtonStyleProperty); }
            set { SetValue(PlayButtonStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PlayButtonStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlayButtonStyleProperty =
            DependencyProperty.Register("PlayButtonStyle", typeof(Style), typeof(OfflineAnimeListviewItem), new PropertyMetadata());




        public Style DeleteButtonStyle
        {
            get { return (Style)GetValue(DeleteButtonStyleProperty); }
            set { SetValue(DeleteButtonStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DeleteButtonStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DeleteButtonStyleProperty =
            DependencyProperty.Register("DeleteButtonStyle", typeof(Style), typeof(OfflineAnimeListviewItem), new PropertyMetadata());



        public ImageSource Thumbnail
        {
            get { return (ImageSource)GetValue(ThumbnailProperty); }
            set { SetValue(ThumbnailProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Thumbnail.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ThumbnailProperty =
            DependencyProperty.Register("Thumbnail", typeof(ImageSource), typeof(OfflineAnimeListviewItem), new PropertyMetadata());




        private Button PlayButton;

        public override void OnApplyTemplate()
        {
            PlayButton = GetTemplateChild("PlayButton") as Button;
            PlayButton.Click += (s, e) => PlayButton_Clicked?.Invoke(this, e);
            base.OnApplyTemplate();
        }

        public event EventHandler<RoutedEventArgs> PlayButton_Clicked;

        
    }
}
