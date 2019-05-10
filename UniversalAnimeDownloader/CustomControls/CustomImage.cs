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
using UADAPI;

namespace UniversalAnimeDownloader.CustomControls
{
    class CustomImage : Image
    {
        //public Stream StreamSource
        //{
        //    get { return (Stream)GetValue(StreamSourceProperty); }
        //    set { SetValue(StreamSourceProperty, value); }
        //}
        //public static readonly DependencyProperty StreamSourceProperty =
        //    DependencyProperty.Register("StreamSource", typeof(Stream), typeof(CustomImage), new PropertyMetadata(StreamSourceChanged));


        public MediaSourceInfo ImageInfo
        {
            get { return (MediaSourceInfo)GetValue(ImageInfoProperty); }
            set { SetValue(ImageInfoProperty, value); }
        }
        public static readonly DependencyProperty ImageInfoProperty =
            DependencyProperty.Register("ImageInfo", typeof(MediaSourceInfo), typeof(CustomImage), new PropertyMetadata(ImageInfoChanged));

        private static async void ImageInfoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ins = d as CustomImage;
            var info = e.NewValue as MediaSourceInfo;
            MemoryStream memStream = null;
            BitmapImage imgSrc = null;

            try
            {
                await Task.Run(async () =>
                {
                    if (info != null)
                    {
                        if (!string.IsNullOrEmpty(info.LocalFile))
                        {
                            if (File.Exists(info.LocalFile))
                            {
                                var fs = File.OpenRead(info.LocalFile);
                                memStream = new MemoryStream();
                                fs.CopyTo(memStream);
                                fs.Close();
                            }
                            else if (!string.IsNullOrEmpty(info.Url))
                            {
                                memStream = (await AnimeInformationRequester.GetStreamAsync(info.Url, info.Headers)) as MemoryStream;
                            }
                        }
                        else if (!string.IsNullOrEmpty(info.Url))
                        {
                            memStream = (await AnimeInformationRequester.GetStreamAsync(info.Url, info.Headers)) as MemoryStream;
                        }

                        if (memStream != null)
                        {
                            memStream.Position = 0;
                            imgSrc = new BitmapImage();
                            imgSrc.BeginInit();
                            imgSrc.StreamSource = memStream;
                            imgSrc.EndInit();
                            imgSrc.Freeze();
                        }
                    }
                });

                if (imgSrc != null)
                {
                    imgSrc = imgSrc.Clone();
                    ins.Source = imgSrc;
                }
                else
                {
                    ins.Source = new BitmapImage();
                }
            }
            catch (Exception err)
            {
                ins.Source = new BitmapImage();
            }
        }

        //private static async void StreamSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    var ins = (d as Image);
        //    var newStream = e.NewValue as Stream;
        //    if (newStream == null)
        //        newStream = e.OldValue as Stream;
        //    if (newStream != null)
        //    {
        //        ins.Source = new BitmapImage();

        //        newStream.Position = 0;
        //        BitmapImage imageSrc = null;

        //        await Task.Run(() =>
        //        {
        //            imageSrc = new BitmapImage();
        //            imageSrc.BeginInit();
        //            imageSrc.StreamSource = newStream;
        //            imageSrc.EndInit();
        //            imageSrc.Freeze();
        //        });

        //        var newImageSrc = imageSrc.Clone();
        //        ins.Source = imageSrc;
        //    }
        //    else
        //    {
        //        ins.Source = new BitmapImage();
        //    }
        //}
    }
}
