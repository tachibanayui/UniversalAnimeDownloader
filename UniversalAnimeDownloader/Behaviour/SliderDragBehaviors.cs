using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace UniversalAnimeDownloader.Behaviour
{
    public static class SliderDragBehaviors
    {
        public static readonly DependencyProperty DragCompletedCommandProperty =
            DependencyProperty.RegisterAttached("DragCompletedCommand", typeof(ICommand), typeof(SliderDragBehaviors),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(DragCompleted)));

        private static void DragCompleted(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            return;
            var slider = (Slider)d;
            var thumb = GetThumbFromSlider(slider);
            thumb.DragCompleted += thumb_DragCompleted;
        }

        private static void thumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            element.Dispatcher.Invoke(() =>
            {
                var command = GetDragCompletedCommand(element);
                var slider = FindParentControl<Slider>(element) as Slider;
                command.Execute(slider.Value);
            });
        }

        public static void SetDragCompletedCommand(UIElement element, ICommand value)
        {
            element.SetValue(DragCompletedCommandProperty, value);
        }

        public static ICommand GetDragCompletedCommand(FrameworkElement element)
        {
            var slider = FindParentControl<Slider>(element);
            return (ICommand)slider.GetValue(DragCompletedCommandProperty);
        }

        private static Thumb GetThumbFromSlider(Slider slider)
        {
            var track = slider.Template.FindName("PART_Track", slider) as Track;
            return track == null ? null : track.Thumb;
        }

        private static DependencyObject FindParentControl<T>(DependencyObject control)
        {
            var parent = VisualTreeHelper.GetParent(control);
            while (parent != null && !(parent is T))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            return parent;
        }
    }
}
