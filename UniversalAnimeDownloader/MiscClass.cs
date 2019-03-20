using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using UADAPI;

namespace UniversalAnimeDownloader
{
    class MiscClass
    {
        public static event EventHandler<SearchEventArgs> UserSearched;
        public static void OnUserSearched(object sender, string searchKeyword) => UserSearched?.Invoke(sender, new SearchEventArgs(searchKeyword));
        public static childItem FindVisualChild<childItem>(DependencyObject obj) where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);

                if (child != null && child is childItem)
                {
                    return (childItem)child;
                }
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);

                    if (childOfChild != null)
                    {
                        return childOfChild;
                    }
                }
            }

            return null;
        }
        public static IList<T> FindChildren<T>(DependencyObject element) where T : FrameworkElement
        {
            List<T> retval = new List<T>();
            for (int counter = 0; counter < VisualTreeHelper.GetChildrenCount(element); counter++)
            {
                FrameworkElement toadd = VisualTreeHelper.GetChild(element, counter) as FrameworkElement;
                if (toadd != null)
                {
                    T correctlyTyped = toadd as T;
                    if (correctlyTyped != null)
                    {
                        retval.Add(correctlyTyped);
                    }
                    else
                    {
                        retval.AddRange(FindChildren<T>(toadd));
                    }
                }
            }
            return retval;
        }
        public static T FindParent<T>(DependencyObject element) where T : FrameworkElement
        {
            FrameworkElement parent = VisualTreeHelper.GetParent(element) as FrameworkElement;
            while (parent != null)
            {
                T correctlyTyped = parent as T;
                if (correctlyTyped != null)
                {
                    return correctlyTyped;
                }
                return FindParent<T>(parent);
            }
            return null;
        }
        public static string AddHtmlColorBody(string content, Color color)
        {
            string rgbValue = $"rgb({color.R}, {color.G}, {color.B})";
            return $"<body style=\"color: {rgbValue}\">" + content + " </body>";
        }
    }


    public class SearchEventArgs : EventArgs
    {
        public SearchEventArgs()
        {

        }

        public SearchEventArgs(string keyword)
        {
            Keyword = keyword;
        }
        public string Keyword { get; set; }
    }
    public class SelectableEpisodeInfo : INotifyPropertyChanged
    {
        private EpisodeInfo _Data;
        public EpisodeInfo Data
        {
            get
            {
                return _Data;
            }
            set
            {
                if (_Data != value)
                {
                    _Data = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _IsSelected;
        public bool IsSelected
        {
            get
            {
                return _IsSelected;
            }
            set
            {
                if (_IsSelected != value)
                {
                    _IsSelected = value;
                    OnSelectedChanged();
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Just for multibinding
        /// </summary>
        public double Offset { get; set; } = 100;

        public SelectableEpisodeInfo()
        {

        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected virtual void OnSelectedChanged() => SelectedIndexChanged?.Invoke(this, EventArgs.Empty);

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<EventArgs> SelectedIndexChanged;
    }
}
