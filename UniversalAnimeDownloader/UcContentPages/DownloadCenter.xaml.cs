﻿using System;
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

using UADAPI;

namespace UniversalAnimeDownloader.UcContentPages
{
    /// <summary>
    /// Interaction logic for DownloadCenter.xaml
    /// </summary>
    public partial class DownloadCenter : UserControl
    {
        public DownloadCenter()
        {
            InitializeComponent();
            DownloadManager.Instances.CollectionChanged += (s, e) =>
            {
                ListView lst = s as ListView;

            };
        }
    }
}
