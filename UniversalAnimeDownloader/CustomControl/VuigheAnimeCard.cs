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
using uadcorelib.Models;
using uadcorelib;

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
    ///     <MyNamespace:VuigheAnimeCard/>
    ///
    /// </summary>
    public class VuigheAnimeCard : Control
    {
        static VuigheAnimeCard()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VuigheAnimeCard), new FrameworkPropertyMetadata(typeof(VuigheAnimeCard)));
        }

        #region Dependency Properties
        public ImageSource AnimeBG
        {
            get { return (ImageSource)GetValue(AnimeBGProperty); }
            set { SetValue(AnimeBGProperty, value); }
        }
        public static readonly DependencyProperty AnimeBGProperty =
            DependencyProperty.Register("AnimeBG", typeof(ImageSource), typeof(VuigheAnimeCard), new PropertyMetadata(new BitmapImage(new Uri("/UniversalAnimeDownloader;component/Resources/SplashScreen.png", UriKind.Relative))));



        public string AnimeName
        {
            get { return (string)GetValue(AnimeNameProperty); }
            set { SetValue(AnimeNameProperty, value); }
        }
        public static readonly DependencyProperty AnimeNameProperty =
            DependencyProperty.Register("AnimeName", typeof(string), typeof(VuigheAnimeCard), new PropertyMetadata("Loading..."));

        public string AnimeTag
        {
            get { return (string)GetValue(AnimeTagProperty); }
            set { SetValue(AnimeTagProperty, value); }
        }
        public static readonly DependencyProperty AnimeTagProperty =
            DependencyProperty.Register("AnimeTag", typeof(string), typeof(VuigheAnimeCard), new PropertyMetadata("Tag: "));

        public double AnimeFontSize
        {
            get { return (double)GetValue(AnimeFontSizeProperty); }
            set { SetValue(AnimeFontSizeProperty, value); }
        }
        public static readonly DependencyProperty AnimeFontSizeProperty =
            DependencyProperty.Register("AnimeFontSize", typeof(double), typeof(VuigheAnimeCard), new PropertyMetadata((double)18));



        public Brush AnimeForeground
        {
            get { return (Brush)GetValue(AnimeForegroundProperty); }
            set { SetValue(AnimeForegroundProperty, value); }
        }
        public static readonly DependencyProperty AnimeForegroundProperty =
            DependencyProperty.Register("AnimeForeground", typeof(Brush), typeof(VuigheAnimeCard), new PropertyMetadata(new SolidColorBrush(Colors.White)));



        public Style PopUpBoxStyle
        {
            get { return (Style)GetValue(PopUpBoxStyleProperty); }
            set { SetValue(PopUpBoxStyleProperty, value); }
        }
        public static readonly DependencyProperty PopUpBoxStyleProperty =
            DependencyProperty.Register("PopUpBoxStyle", typeof(Style), typeof(VuigheAnimeCard), new PropertyMetadata());

        public Style PlayButtonStyle
        {
            get { return (Style)GetValue(PlayButtonStyleProperty); }
            set { SetValue(PlayButtonStyleProperty, value); }
        }
        public static readonly DependencyProperty PlayButtonStyleProperty =
            DependencyProperty.Register("PlayButtonStyle", typeof(Style), typeof(VuigheAnimeCard), new PropertyMetadata());


        
        /// <summary>
        /// For online use
        /// </summary>
        public VuigheAnimeManager Data
        {
            get { return (uadcorelib.VuigheAnimeManager)GetValue(DataProperty); }
            set
            {
                SetValue(DataProperty, value);
                if(isInit)
                    SetDataProperty();
            }
        }
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(uadcorelib.VuigheAnimeManager), typeof(VuigheAnimeCard), new PropertyMetadata());


        /// <summary>
        /// For offline storage
        /// </summary>
        public AnimeInformation OfflineData
        {
            get { return (AnimeInformation)GetValue(OfflineDataProperty); }
            set
            {
                SetValue(OfflineDataProperty, value);
                if (isInit)
                    SetOfflineDataProperty();
            }
        }


        public static readonly DependencyProperty OfflineDataProperty =
            DependencyProperty.Register("OfflineData", typeof(AnimeInformation), typeof(VuigheAnimeCard), new PropertyMetadata());
        #endregion

        Image myImage;
        Button btnWatchAnime;
        bool isInit;

        public event EventHandler<RoutedEventArgs> WatchAnimeButtonClicked;

        public override void OnApplyTemplate()
        {
            myImage = GetTemplateChild("img") as Image;
            btnWatchAnime = GetTemplateChild("watchAnime") as Button;
            btnWatchAnime.Click += (s, e) => WatchAnimeButtonClicked?.Invoke(this, e);

            SetDataProperty();
            SetOfflineDataProperty();
            isInit = true;
            base.OnApplyTemplate();
        }

        T GetTemplatedChild<T>(string childName) where T : DependencyObject
        {
            T child = GetTemplateChild(childName) as T;
            if (child == null)
                throw new NullReferenceException("Cannot find this child");

            return child;
        }

        private void SetDataProperty()
        {
            if (Data == null)
                return;
            AnimeName = Data.CurrentFilm.Name;
            string tags = string.Empty;

            foreach (GenresDataList item in Data.CurrentFilm.Genres.Data)
            {
                tags += item.Name + ", ";
            }

            AnimeTag = tags;

            myImage.SourceUpdated += (s, e) =>
            {
                DoubleAnimation fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(.5), FillBehavior.Stop);
                myImage.BeginAnimation(OpacityProperty, fadeIn);
            };

            if(Data.CurrentFilm.Thumbnail != null)
                AnimeBG = new BitmapImage(new Uri(Data.CurrentFilm.Thumbnail));
        }


        private void SetOfflineDataProperty()
        {
            if (OfflineData == null)
                return;

            AnimeName = OfflineData.AnimeName;
            AnimeTag = OfflineData.AnimeGenres;

            myImage.SourceUpdated += (s, e) =>
            {
                DoubleAnimation fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(.5), FillBehavior.Stop);
                myImage.BeginAnimation(OpacityProperty, fadeIn);
            };

            if (OfflineData.AnimeThumbnail != null)
            {
                
            }
        }
    }
}
