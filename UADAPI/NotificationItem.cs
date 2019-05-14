using System;
using System.Threading.Tasks;

namespace UADAPI
{
    /// <summary>
    /// Indiviual Notification
    /// </summary>
    public class NotificationItem
    {
        public NotificationItem()
        {
            CreatedTime = DateTime.Now;
        }

        public DateTime CreatedTime { get; set; }
        public string Title { get; set; }
        public bool ShowActionButton { get; set; }
        public string Detail { get; set; }
        /// <summary>
        /// The string will be placed in the button. Can be null or empty
        /// </summary>
        public string ActionButtonContent { get; set; }

        /// <summary>
        /// Will be invoke if user click the button, Method must be public 
        /// <para>Format: {Type in full name} Example: UADEx.HelperClass</para>
        /// <para>You should create a static class for any method link to this property, Nested class is not avaible</para>
        /// </summary>
        public string ButtonActionStringClass { get; set; }
        /// <summary>
        /// Will be invoke if user click the button. Method must be public 
        /// <para>Format: {Method name without bracket} Example: CreateMethod</para>
        /// <para>You should create a static class for any method link to this property, Nested class is not avaible</para>
        /// </summary>
        public string ButtonActionStringMethod { get; set; }

        public async Task InvokeAsync()
        {
            return;
            var type = Type.GetType(ButtonActionStringClass);
            var method = type.GetMethod(ButtonActionStringMethod);
            await Task.Run(() => method.Invoke(null, null));
        }
    }
}
