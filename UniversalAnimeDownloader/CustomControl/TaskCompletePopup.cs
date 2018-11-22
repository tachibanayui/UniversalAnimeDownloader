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
using MaterialDesignThemes.Wpf;

namespace UniversalAnimeDownloader.CustomControl
{


    
    /// <summary>
    /// This control is used to notify completed tasks, either failed or completed
    /// </summary>
    public class TaskCompletePopup : Control
    {
        static TaskCompletePopup()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TaskCompletePopup), new FrameworkPropertyMetadata(typeof(TaskCompletePopup)));
        }

        public double DialogWidth
        {
            get { return (double)GetValue(DialogWidthProperty); }
            set { SetValue(DialogWidthProperty, value); }
        }
        public static readonly DependencyProperty DialogWidthProperty =
            DependencyProperty.Register("DialogWidth", typeof(double), typeof(TaskCompletePopup), new PropertyMetadata(400d));

        public double DialogHeight
        {
            get { return (double)GetValue(DialogHeightProperty); }
            set { SetValue(DialogHeightProperty, value); }
        }
        public static readonly DependencyProperty DialogHeightProperty =
            DependencyProperty.Register("DialogHeight", typeof(double), typeof(TaskCompletePopup), new PropertyMetadata(300d));

        public double DialogUniformRadius
        {
            get { return (double)GetValue(DialogUniformRadiusProperty); }
            set { SetValue(DialogUniformRadiusProperty, value); }
        }
        public static readonly DependencyProperty DialogUniformRadiusProperty =
            DependencyProperty.Register("DialogUniformRadius", typeof(double), typeof(TaskCompletePopup), new PropertyMetadata(15d));

        public double IconSize
        {
            get { return (double)GetValue(IconSizeProperty); }
            set { SetValue(IconSizeProperty, value); }
        }
        public static readonly DependencyProperty IconSizeProperty =
            DependencyProperty.Register("IconSize", typeof(double), typeof(TaskCompletePopup), new PropertyMetadata(25d));

        public double DialogTitleFontSize
        {
            get { return (double)GetValue(DialogTitleFontSizeProperty); }
            set { SetValue(DialogTitleFontSizeProperty, value); }
        }
        public static readonly DependencyProperty DialogTitleFontSizeProperty =
            DependencyProperty.Register("DialogTitleFontSize", typeof(double), typeof(TaskCompletePopup), new PropertyMetadata(24d));

        public bool DialogOpen
        {
            get { return (bool)GetValue(DialogOpenProperty); }
            set { SetValue(DialogOpenProperty, value); }
        }
        public static readonly DependencyProperty DialogOpenProperty =
            DependencyProperty.Register("DialogOpen", typeof(bool), typeof(TaskCompletePopup), new PropertyMetadata(false));

        public string PopupTitle
        {
            get { return (string)GetValue(PopupTitleProperty); }
            set { SetValue(PopupTitleProperty, value); }
        }
        public static readonly DependencyProperty PopupTitleProperty =
            DependencyProperty.Register("PopupTitle", typeof(string), typeof(TaskCompletePopup), new PropertyMetadata("Popup Title"));

        public string PopupText
        {
            get { return (string)GetValue(PopupTextProperty); }
            set { SetValue(PopupTextProperty, value); }
        }
        public static readonly DependencyProperty PopupTextProperty =
            DependencyProperty.Register("PopupText", typeof(string), typeof(TaskCompletePopup), new PropertyMetadata("Content"));

        public Brush DialogBackground
        {
            get { return (Brush)GetValue(DialogBackgroundProperty); }
            set { SetValue(DialogBackgroundProperty, value); }
        }
        public static readonly DependencyProperty DialogBackgroundProperty =
            DependencyProperty.Register("DialogBackground", typeof(Brush), typeof(TaskCompletePopup), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        public Brush DialogTitleForeground
        {
            get { return (Brush)GetValue(DialogTitleForegroundProperty); }
            set { SetValue(DialogTitleForegroundProperty, value); }
        }
        public static readonly DependencyProperty DialogTitleForegroundProperty =
            DependencyProperty.Register("DialogTitleForeground", typeof(Brush), typeof(TaskCompletePopup), new PropertyMetadata(new SolidColorBrush(Colors.White)));

        public Brush CardBackGround
        {
            get { return (Brush)GetValue(CardBackGroundProperty); }
            set { SetValue(CardBackGroundProperty, value); }
        }
        public static readonly DependencyProperty CardBackGroundProperty =
            DependencyProperty.Register("CardBackGround", typeof(Brush), typeof(TaskCompletePopup), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(66,66,66))));

        public Brush ButtonRippleColor
        {
            get { return (Brush)GetValue(ButtonRippleColorProperty); }
            set { SetValue(ButtonRippleColorProperty, value); }
        }
        public static readonly DependencyProperty ButtonRippleColorProperty =
            DependencyProperty.Register("ButtonRippleColor", typeof(Brush), typeof(TaskCompletePopup), new PropertyMetadata());

        public PackIconKind IconKind
        {
            get { return (PackIconKind)GetValue(IconKindProperty); }
            set { SetValue(IconKindProperty, value); }
        }
        public static readonly DependencyProperty IconKindProperty =
            DependencyProperty.Register("IconKind", typeof(PackIconKind), typeof(TaskCompletePopup), new PropertyMetadata(PackIconKind.CheckboxMarkedCircleOutline));

        public Style ActionButtonStyle
        {
            get { return (Style)GetValue(ActionButtonStyleProperty); }
            set { SetValue(ActionButtonStyleProperty, value); }
        }
        public static readonly DependencyProperty ActionButtonStyleProperty =
            DependencyProperty.Register("ActionButtonStyle", typeof(Style), typeof(TaskCompletePopup), new PropertyMetadata());


        private Button btnOK;

        public override void OnApplyTemplate()
        {
            btnOK = GetTemplateChild("btnOk") as Button;

            btnOK.Click += (s, e) =>
            {
                DialogOpen = false;
                Visibility = Visibility.Collapsed;
            };
        }
    }
}
