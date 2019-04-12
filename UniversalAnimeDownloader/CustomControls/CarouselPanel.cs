using MahApps.Metro.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
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
using System.Windows.Threading;
using UniversalAnimeDownloader.ViewModels;

namespace UniversalAnimeDownloader.CustomControls
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:TestMaterialDesignXaml.CustomControls"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:TestMaterialDesignXaml.CustomControls;assembly=TestMaterialDesignXaml.CustomControls"
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
    ///     <MyNamespace:CarouselPanel/>
    ///
    /// </summary>
    public class CarouselPanel : Control
    {
        #region Dependency Properties
        public IEnumerable SlideItemsSource
        {
            get { return (IEnumerable)GetValue(SlideItemsSourceProperty); }
            set { SetValue(SlideItemsSourceProperty, value); }
        }
        public static readonly DependencyProperty SlideItemsSourceProperty =
            DependencyProperty.Register("SlideItemsSource", typeof(IEnumerable), typeof(CarouselPanel), new PropertyMetadata(new PropertyChangedCallback(OnItemsSourcePropertyChanged)));
        private static void OnItemsSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var control = sender as CarouselPanel;
            if (control != null)
            {
                control.OnItemsSourceChanged((IEnumerable)e.OldValue, (IEnumerable)e.NewValue);
            }
        }

        private void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            // Remove handler for oldValue.CollectionChanged
            var oldValueINotifyCollectionChanged = oldValue as INotifyCollectionChanged;

            if (null != oldValueINotifyCollectionChanged)
            {
                oldValueINotifyCollectionChanged.CollectionChanged -= new NotifyCollectionChangedEventHandler(newValueINotifyCollectionChanged_CollectionChanged);
            }
            // Add handler for newValue.CollectionChanged (if possible)
            var newValueINotifyCollectionChanged = newValue as INotifyCollectionChanged;
            if (null != newValueINotifyCollectionChanged)
            {
                newValueINotifyCollectionChanged.CollectionChanged += new NotifyCollectionChangedEventHandler(newValueINotifyCollectionChanged_CollectionChanged);
            }

        }

        void newValueINotifyCollectionChanged_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

        }

        public Brush PagerBrush
        {
            get { return (Brush)GetValue(PagerBrushProperty); }
            set { SetValue(PagerBrushProperty, value); }
        }
        public static readonly DependencyProperty PagerBrushProperty =
            DependencyProperty.Register("PagerBrush", typeof(Brush), typeof(CarouselPanel), new PropertyMetadata(new SolidColorBrush(Colors.LightBlue)));

        public DataTemplate SlideItemTemplate
        {
            get { return (DataTemplate)GetValue(SlideItemTemplateProperty); }
            set { SetValue(SlideItemTemplateProperty, value); }
        }
        public static readonly DependencyProperty SlideItemTemplateProperty =
            DependencyProperty.Register("SlideItemTemplate", typeof(DataTemplate), typeof(CarouselPanel), new PropertyMetadata());

        public int SlideIndex
        {
            get { return (int)GetValue(SlideIndexProperty); }
            set { SetValue(SlideIndexProperty, value); }
        }
        public static readonly DependencyProperty SlideIndexProperty =
            DependencyProperty.Register("SlideIndex", typeof(int), typeof(CarouselPanel), new PropertyMetadata(0));

        public Style PagerButtonStyle
        {
            get { return (Style)GetValue(PagerButtonStyleProperty); }
            set { SetValue(PagerButtonStyleProperty, value); }
        }
        public static readonly DependencyProperty PagerButtonStyleProperty =
            DependencyProperty.Register("PagerButtonStyle", typeof(Style), typeof(CarouselPanel), new FrameworkPropertyMetadata());

        public Style BackNavigationStyle
        {
            get { return (Style)GetValue(BackNavigationStyleProperty); }
            set { SetValue(BackNavigationStyleProperty, value); }
        }
        public static readonly DependencyProperty BackNavigationStyleProperty =
            DependencyProperty.Register("BackNavigationStyle", typeof(Style), typeof(CarouselPanel), new PropertyMetadata());

        public Style ForwardNavigationStyte
        {
            get { return (Style)GetValue(ForwardNavigationStyteProperty); }
            set { SetValue(ForwardNavigationStyteProperty, value); }
        }
        public static readonly DependencyProperty ForwardNavigationStyteProperty =
            DependencyProperty.Register("ForwardNavigationStyte", typeof(Style), typeof(CarouselPanel), new PropertyMetadata());

        public TimeSpan SlideInterval
        {
            get { return (TimeSpan)GetValue(SlideIntervalProperty); }
            set
            {
                Timer.Interval = value;
                SetValue(SlideIntervalProperty, value);
            }
        }
        public static readonly DependencyProperty SlideIntervalProperty =
            DependencyProperty.Register("SlideInterval", typeof(TimeSpan), typeof(CarouselPanel), new PropertyMetadata(TimeSpan.FromSeconds(5)));

        public Brush BackNavigationButtonBrush
        {
            get { return (Brush)GetValue(BackNavigationButtonBrushProperty); }
            set { SetValue(BackNavigationButtonBrushProperty, value); }
        }
        public static readonly DependencyProperty BackNavigationButtonBrushProperty =
            DependencyProperty.Register("BackNavigationButtonBrush", typeof(Brush), typeof(CarouselPanel), new PropertyMetadata(new SolidColorBrush(Colors.White)));

        public Brush ForwardNavigationButtonBrush
        {
            get { return (Brush)GetValue(ForwardNavigationButtonBrushProperty); }
            set { SetValue(ForwardNavigationButtonBrushProperty, value); }
        }
        public static readonly DependencyProperty ForwardNavigationButtonBrushProperty =
            DependencyProperty.Register("ForwardNavigationButtonBrush", typeof(Brush), typeof(CarouselPanel), new PropertyMetadata(new SolidColorBrush(Colors.White)));
        #endregion


        #region Properties / Fields
        public DispatcherTimer Timer { get; set; } = new DispatcherTimer();
        private FlipView _slider = null;
        private Button _btnNavBack = null;
        private Button _btnNavForward = null;
        private Style _pagerDefaultStyle = null;
        private Style _navBackBtnStyle = null;
        private Style _navForwardStyle = null;
        #endregion

        #region Events
        public event EventHandler<RoutedEventArgs> NavigationBackButtonClicked;
        public event EventHandler<RoutedEventArgs> NavigationForwardButtonClicked;
        public event EventHandler<IndexedEventArgs> PagerNavigationButtonClicked;

        protected virtual void OnNavigationBackButtonClicked(RoutedEventArgs e) => NavigationBackButtonClicked?.Invoke(this, e);
        protected virtual void OnNavigationForwardButtonClicked(RoutedEventArgs e) => NavigationForwardButtonClicked?.Invoke(this, e);
        protected virtual void OnPagerNavigationButtonClicked(int index) => PagerNavigationButtonClicked?.Invoke(this, new IndexedEventArgs() { Index = index });
        #endregion

        static CarouselPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CarouselPanel), new FrameworkPropertyMetadata(typeof(CarouselPanel)));
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            TestCommand = new RelayCommand<object>(p => true, p => {
                var tmp = new List<object>(SlideItemsSource.OfType<object>());
                var index = tmp.FindIndex(pp => pp == p);
                if (index > -1 && index < tmp.Count)
                {
                    SlideIndex = index;
                    Timer.Stop();
                    Timer.Start();
                }

                OnPagerNavigationButtonClicked(SlideIndex);
            });
            _slider = GetTemplateChild("slider") as FlipView;
            _pagerDefaultStyle = (GetTemplateChild("root") as Grid).FindResource("NavButton") as Style;
            _navBackBtnStyle = (GetTemplateChild("root") as Grid).FindResource("leftNavButton") as Style;
            _navForwardStyle = (GetTemplateChild("root") as Grid).FindResource("rightNavButton") as Style;

            if (PagerButtonStyle == null)
                PagerButtonStyle = _pagerDefaultStyle;
            if (BackNavigationStyle == null)
                BackNavigationStyle = _navBackBtnStyle;
            if (ForwardNavigationStyte == null)
                ForwardNavigationStyte = _navForwardStyle;

            Timer.Interval = TimeSpan.FromSeconds(5);
            Timer.Tick += NextSlide;
            Timer.Start();

            _btnNavBack = GetTemplateChild("btnBack") as Button;
            _btnNavForward = GetTemplateChild("btnForward") as Button;

            _btnNavForward.Click += (s, e) => {
                if (_slider != null)
                {
                    var max = _slider.Items.Count;
                    if (SlideIndex < max - 1)
                        SlideIndex++;
                    else
                        SlideIndex = 0;
                    Timer.Stop();
                    Timer.Start();
                }
                OnNavigationForwardButtonClicked(e);
            };

            _btnNavBack.Click += (s, e) => {
                if (_slider != null)
                {
                    var max = _slider.Items.Count;
                    if (SlideIndex > 0)
                        SlideIndex--;
                    else
                        SlideIndex = _slider.Items.Count - 1;
                    Timer.Stop();
                    Timer.Start();
                }
                OnNavigationBackButtonClicked(e);
            };
        }
        private void NextSlide(object sender, EventArgs e)
        {
            if (_slider != null)
            {
                var max = _slider.Items.Count;
                if (SlideIndex < max - 1)
                    SlideIndex++;
                else
                    SlideIndex = 0;
            }
        }
        public void PauseSlideShow() => Timer.Stop();
        public void ResumeSlideShow() => Timer.Start();
        public ICommand TestCommand { get; set; }
    }

    public class IndexedEventArgs : EventArgs
    {
        public int Index { get; set; }
    }
}
