using MaterialDesignThemes.Wpf;
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

namespace UniversalAnimeDownloader.CustomControls
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:UniversalAnimeDownloader.CustomControls"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:UniversalAnimeDownloader.CustomControls;assembly=UniversalAnimeDownloader.CustomControls"
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
    ///     <MyNamespace:SidePanelNavigateButton/>
    ///
    /// </summary>
    public class SidePanelNavigateButton : Control
    {
        public ICommand ButtonCommand
        {
            get { return (ICommand)GetValue(ButtonCommandProperty); }
            set { SetValue(ButtonCommandProperty, value); }
        }
        public static readonly DependencyProperty ButtonCommandProperty =
            DependencyProperty.Register("ButtonCommand", typeof(ICommand), typeof(SidePanelNavigateButton), new PropertyMetadata());

        public object ButtonCommandParameter
        {
            get { return (object)GetValue(ButtonCommandParameterProperty); }
            set { SetValue(ButtonCommandParameterProperty, value); }
        }
        public static readonly DependencyProperty ButtonCommandParameterProperty =
            DependencyProperty.Register("ButtonCommandParameter", typeof(object), typeof(SidePanelNavigateButton), new PropertyMetadata());

        public int ControlTabIndex
        {
            get { return (int)GetValue(ControlTabIndexProperty); }
            set { SetValue(ControlTabIndexProperty, value); }
        }
        public static readonly DependencyProperty ControlTabIndexProperty =
            DependencyProperty.Register("ControlTabIndex", typeof(int), typeof(SidePanelNavigateButton), new PropertyMetadata());

        public int CurrentTabIndex
        {
            get { return (int)GetValue(CurrentTabIndexProperty); }
            set { SetValue(CurrentTabIndexProperty, value); }
        }
        public static readonly DependencyProperty CurrentTabIndexProperty =
            DependencyProperty.Register("CurrentTabIndex", typeof(int), typeof(SidePanelNavigateButton), new PropertyMetadata());

        public PackIconKind SidePanelIconKind
        {
            get { return (PackIconKind)GetValue(SidePanelIconKindProperty); }
            set { SetValue(SidePanelIconKindProperty, value); }
        }
        public static readonly DependencyProperty SidePanelIconKindProperty =
            DependencyProperty.Register("SidePanelIconKind", typeof(PackIconKind), typeof(SidePanelNavigateButton), new PropertyMetadata());

        public string SidePanelText
        {
            get { return (string)GetValue(SidePanelTextProperty); }
            set { SetValue(SidePanelTextProperty, value); }
        }
        public static readonly DependencyProperty SidePanelTextProperty =
            DependencyProperty.Register("SidePanelText", typeof(string), typeof(SidePanelNavigateButton), new PropertyMetadata());

        public bool EnableAnimation
        {
            get { return (bool)GetValue(EnableAnimationProperty); }
            set { SetValue(EnableAnimationProperty, value); }
        }
        public static readonly DependencyProperty EnableAnimationProperty =
            DependencyProperty.Register("EnableAnimation", typeof(bool), typeof(SidePanelNavigateButton), new PropertyMetadata(true));

        static SidePanelNavigateButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SidePanelNavigateButton), new FrameworkPropertyMetadata(typeof(SidePanelNavigateButton)));
        }
    }
}
