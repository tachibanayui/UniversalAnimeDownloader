using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UniversalAnimeDownloader
{
    static class CustomCommands
    {
        public static RoutedUICommand ScreenBlockerHotkey = new RoutedUICommand("ScreenBlockerHotKey", "ScreenBlockerHotKey", typeof(CustomCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.B, ModifierKeys.Control)
            });

        public static RoutedUICommand FakeAppCrashHotkey = new RoutedUICommand("ScreenBlockerHotKey", "ScreenBlockerHotKey", typeof(CustomCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.C, ModifierKeys.Control)
            });
        public static RoutedUICommand BackgroundPlayerHotkey = new RoutedUICommand("ScreenBlockerHotKey", "ScreenBlockerHotKey", typeof(CustomCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.N, ModifierKeys.Control)
            });
    }
}
