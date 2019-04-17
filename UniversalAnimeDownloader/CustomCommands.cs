using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UniversalAnimeDownloader
{
    public static class CustomCommands
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
        public static RoutedUICommand PlayerPauseHotkey = new RoutedUICommand("PlayerPauseHotkey", "PlayerPauseHotkey", typeof(CustomCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.Space, ModifierKeys.None)
            });
        public static RoutedUICommand Forward30SecHotkey = new RoutedUICommand("Forward30SecHotkey", "Forward30SecHotkey", typeof(CustomCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.Right, ModifierKeys.None)
            });
        public static RoutedUICommand Previous10SecHotkey = new RoutedUICommand("Previous10SecHotkey", "Previous10SecHotkey", typeof(CustomCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.Left, ModifierKeys.None)
            });
        public static RoutedUICommand VolumeUpHotkey = new RoutedUICommand("VolumeUpHotkey", "VolumeUpHotkey", typeof(CustomCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.Up, ModifierKeys.None)
            });
        public static RoutedUICommand VolumeDownHotkey = new RoutedUICommand("VolumeDownHotkey", "VolumeDownHotkey", typeof(CustomCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.Down, ModifierKeys.None)
            });
        public static RoutedUICommand QuitFullScreenHotkey = new RoutedUICommand("QuitFullScreenHotkey", "QuitFullScreenHotkey", typeof(CustomCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.Escape, ModifierKeys.None)
            });
    }
}
