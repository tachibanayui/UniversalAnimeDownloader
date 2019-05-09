using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
    class CustomImage : Image
    {
        public Stream StreamSource
        {
            get { return (Stream)GetValue(StreamSourceProperty); }
            set { SetValue(StreamSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StreamSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StreamSourceProperty =
            DependencyProperty.Register("StreamSource", typeof(Stream), typeof(CustomImage), new PropertyMetadata(StreamSourceChanged));

        private static async void StreamSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ins = (d as Image);
            var newStream = e.NewValue as Stream;
            if (newStream == null)
                newStream = e.OldValue as Stream;
            if (newStream != null)
            {
                ins.Source = new BitmapImage();

                newStream.Position = 0;
                BitmapImage imageSrc = null;

                await Task.Run(() =>
                {
                    imageSrc = new BitmapImage();
                    imageSrc.BeginInit();
                    imageSrc.StreamSource = newStream;
                    imageSrc.EndInit();
                    imageSrc.Freeze();
                });

                var newImageSrc = imageSrc.Clone();
                ins.Source = imageSrc;
            }
            else
            {
                ins.Source = new BitmapImage();
            }
        }
    }
}
