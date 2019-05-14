using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;

namespace UADAPI
{
    /// <summary>
    /// Use to access UAD Notification system
    /// </summary>
    public class NotificationManager
    {
        public static ObservableCollection<NotificationItem> Notifications { get; private set; } = new ObservableCollection<NotificationItem>();
        public static void Add(NotificationItem item)
        {
            for (int i = Notifications.Count - 1; i >= 0; i--)
            {
                var tmp = Notifications[i];
                if (i + 1 == Notifications.Count)
                {
                    Notifications.Add(tmp);
                }
                else
                {
                    Notifications[i + 1] = tmp;
                }
            }

            if (Notifications.Count == 0)
            {
                Notifications.Add(item);
            }
            else
            {
                Notifications[0] = item;
            }

            OnItemAdded(item);
        }
        public static void Remove(NotificationItem item)
        {
            Notifications.Remove(item);
            OnItemRemoved(item);
        }
        public static void RemoveAt(int index)
        {
            if (index > Notifications.Count - 1)
            {
                throw new IndexOutOfRangeException();
            }

            var item = Notifications[index];
            Notifications.Remove(item);
            OnItemRemoved(item);
        }
        public static void RemoveAll()
        {
            Notifications.Clear();
            OnItemRemoved(null);
        }


        public static string Serialize() => JsonConvert.SerializeObject(Notifications);

        public static void Deserialize(string value)
        {
            if (value != null)
            {
                var tmp = JsonConvert.DeserializeObject<ObservableCollection<NotificationItem>>(value);
                if(tmp != null)
                    Notifications = new ObservableCollection<NotificationItem>(tmp);
            }
            else
            {
                Notifications = new ObservableCollection<NotificationItem>();
            }
        }

        public static event EventHandler<NotificationEventArgs> ItemAdded;
        public static event EventHandler<NotificationEventArgs> ItemRemoved;
        public static void OnItemAdded(NotificationItem item) => ItemAdded?.Invoke(Notifications, new NotificationEventArgs() { AffectededItem = item });
        public static void OnItemRemoved(NotificationItem item) => ItemRemoved?.Invoke(Notifications, new NotificationEventArgs() { AffectededItem = item });
    }
}
