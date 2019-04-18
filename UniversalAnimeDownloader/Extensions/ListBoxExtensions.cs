using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UniversalAnimeDownloader.Extensions
{
    class ListBoxExtensions
    {
        public static readonly DependencyProperty ScrollChangedCommandProperty = DependencyProperty.RegisterAttached(
            "ScrollChangedCommand", typeof(ICommand), typeof(ListBoxExtensions),
            new PropertyMetadata(default(ICommand), OnScrollChangedCommandChanged));







        public static object GetScrollChangedCommandParameter(DependencyObject obj)
        {
            return (object)obj.GetValue(ScrollChangedCommandParameterProperty);
        }

        public static void SetScrollChangedCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(ScrollChangedCommandParameterProperty, value);
        }

        // Using a DependencyProperty as the backing store for ScrollChangedCommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScrollChangedCommandParameterProperty =
            DependencyProperty.RegisterAttached("ScrollChangedCommandParameter", typeof(object), typeof(ListBoxExtensions), new PropertyMetadata(new object()));




        private static void OnScrollChangedCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ListBox dataGrid = d as ListBox;
            if (dataGrid == null)
                return;
            if (e.NewValue != null)
            {
                dataGrid.Loaded += ListBoxOnLoaded;
            }
            else if (e.OldValue != null)
            {
                dataGrid.Loaded -= ListBoxOnLoaded;
            }
        }

        private static void ListBoxOnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            ListBox dataGrid = sender as ListBox;
            if (dataGrid == null)
                return;

            ScrollViewer scrollViewer = MiscClass.FindChildren<ScrollViewer>(dataGrid).FirstOrDefault();
            if (scrollViewer != null)
            {
                scrollViewer.ScrollChanged += ScrollViewerOnScrollChanged;
            }
        }

        private static void ScrollViewerOnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ListBox dataGrid = MiscClass.FindParent<ListBox>(sender as ScrollViewer);
            if (dataGrid != null)
            {
                ICommand command = GetScrollChangedCommand(dataGrid);
                if (command.CanExecute(GetScrollChangedCommandParameter(dataGrid)))
                    command.Execute(GetScrollChangedCommandParameter(dataGrid));
            }
        }

        public static void SetScrollChangedCommand(DependencyObject element, ICommand value)
        {
            element.SetValue(ScrollChangedCommandProperty, value);
        }

        public static ICommand GetScrollChangedCommand(DependencyObject element)
        {
            return (ICommand)element.GetValue(ScrollChangedCommandProperty);
        }

    }

}
