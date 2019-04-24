using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using UniversalAnimeDownloader.Behaviour;
using UniversalAnimeDownloader.UADSettingsPortal;
using Xceed.Wpf.Toolkit;

namespace UniversalAnimeDownloader.ViewModels
{
    class UADSettingsViewModel : BaseViewModel, IPageContent
    {
        #region Commands
        public ICommand ApplyPrimaryCommand { get; set; }
        public ICommand ApplyAccentCommand { get; set; }
        public ICommand ApplyFramerateCommand { get; set; }
        public ICommand OpenFileDialogCommand { get; set; }
        public ICommand HostLoadedCommand { get; set; }
        public ICommand StretchModeChangedCommand { get; set; }
        public ICommand ColorPickerDialogClosingCommand { get; set; }
        public ICommand ChooseColorCommand { get; set; }
        public ICommand ChooseKeyCommand { get; set; }
        public ICommand ApplyColorCommand { get; set; }
        public ICommand ValidateKeyCommand { get; set; }
        public ICommand BrowseFolderDialogCommand { get; set; }
        public ICommand MainScrollChangedCommand { get; set; }
        public ICommand AnimateSrcoll { get; set; }
        #endregion

        public bool IsHostLoaded { get; set; } = false;
        public string CurrentObjectRequestColorName { get; set; }
        public string CurrentObjectRequestKeyName { get; set; }


        private UADSettingsData _SettingData;
        public UADSettingsData SettingData
        {
            get
            {
                return _SettingData;
            }
            set
            {
                if (_SettingData != value)
                {
                    _SettingData = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _IsColorPickerOpen;
        public bool IsColorPickerOpen
        {
            get
            {
                return _IsColorPickerOpen;
            }
            set
            {
                if (_IsColorPickerOpen != value)
                {
                    _IsColorPickerOpen = value;
                    if (!value)
                    {
                        CurrentObjectRequestColorName = string.Empty;
                    }

                    OnPropertyChanged();
                }
            }
        }

        private bool _IsKeyInputOpen;
        public bool IsKeyInputOpen
        {
            get
            {
                return _IsKeyInputOpen;
            }
            set
            {
                if (_IsKeyInputOpen != value)
                {
                    _IsKeyInputOpen = value;
                    if (!value)
                    {
                        CurrentObjectRequestKeyName = string.Empty;
                    }

                    OnPropertyChanged();
                }
            }
        }

        private int _CurrentSettingIndex = 0;
        public int CurrentSettingIndex
        {
            get
            {
                return _CurrentSettingIndex;
            }
            set
            {
                if (_CurrentSettingIndex != value)
                {
                    _CurrentSettingIndex = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _EndEmptySpaceHeight = 0;
        public double EndEmptySpaceHeight
        {
            get
            {
                return _EndEmptySpaceHeight;
            }
            set
            {
                if (_EndEmptySpaceHeight != value)
                {
                    _EndEmptySpaceHeight = value;
                    OnPropertyChanged();
                }
            }
        }



        public PaletteHelper Helper { get; set; }
        public IList<Swatch> Swatches { get; }

        public UADSettingsViewModel()
        {
            //SettingData = UADSettingsManager.Instance.CurrentSettings;
            Swatches = new SwatchesProvider().Swatches.ToList();
            Helper = new PaletteHelper();

            ApplyAccentCommand = new RelayCommand<Swatch>(p => true, p => UADSettingsManager.Instance.CurrentSettings.AccentColorTheme = p);
            ApplyPrimaryCommand = new RelayCommand<Swatch>(p => true, p => UADSettingsManager.Instance.CurrentSettings.PrimaryColorTheme = p);
            ApplyFramerateCommand = new RelayCommand<double>(p => true, p => SettingData.AnimationFrameRate = (int)p);
            HostLoadedCommand = new RelayCommand<UserControl>(p => true, async p =>
            {
                //Load the setting
                if (UADSettingsManager.Instance.CurrentSettings == null)
                {
                    await UADSettingsManager.Instance.Init();
                    SettingData = UADSettingsManager.Instance.CurrentSettings;
                    OnPropertyChanged("SettingData");
                }


                //To avoid the control binding will erase the setting while the comonent are init. So after the UserContro is loaded we will add binding
                IsHostLoaded = true;
                var cbxStret = p.FindName("stretchMode") as ComboBox;
                cbxStret.SelectedItem = SettingData.BlockerStretchMode.ToString("G");

                Ins = p;

                //Get the mainWindow SizeChanged
                HostWindow = Window.GetWindow(p);
                IndexChangeMark = HostWindow.Height / 2;
                EndEmptySpaceHeight = HostWindow.Height / 4d * 3d - 100;
                HostWindow.SizeChanged += (s, e) =>
                {
                    IndexChangeMark = e.NewSize.Height / 2d;
                    EndEmptySpaceHeight = e.NewSize.Height / 4d * 3d - 100;
                };
            });
            OpenFileDialogCommand = new RelayCommand<string>(p => true, p =>
            {
                Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog();
                fileDialog.Multiselect = false;
                fileDialog.Filter = "All Images|*.BMP;*.DIB;*.RLE;*.JPG;*.JPEG;*.JPE;*.JFIF;*.GIF;*.TIF;*.TIFF;*.PNG|BMP Files: (*.BMP;*.DIB;*.RLE)|*.BMP;*.DIB;*.RLE|JPEG Files: (*.JPG;*.JPEG;*.JPE;*.JFIF)|*.JPG;*.JPEG;*.JPE;*.JFIF|GIF Files: (*.GIF)|*.GIF|TIFF Files: (*.TIF;*.TIFF)|*.TIF;*.TIFF|PNG Files: (*.PNG)|*.PNG|All Files|*.*";
                var res = fileDialog.ShowDialog();
                if (res == true)
                {
                    if (p == "BlockerImage")
                    {
                        SettingData.BlockerImageLocation = fileDialog.FileName;
                    }
                }
            });
            StretchModeChangedCommand = new RelayCommand<ComboBox>(p => IsHostLoaded, p => SettingData.BlockerStretchMode = (Stretch)Enum.Parse(typeof(Stretch), p.SelectedItem.ToString()));
            ColorPickerDialogClosingCommand = new RelayCommand<object>(p => true, p => { });
            ChooseKeyCommand = new RelayCommand<string>(p => true, p => { CurrentObjectRequestKeyName = p; IsKeyInputOpen = true; });
            ValidateKeyCommand = new RelayCommand<TextBox>(p => !string.IsNullOrEmpty(p.Text), p =>
           {
               char current = p.Text.ToUpper()[0];

               switch (CurrentObjectRequestKeyName)
               {
                   case "Blocker":
                       SettingData.BlockerToggleHotKeys = current;
                       break;
                   case "FakeCrash":
                       SettingData.AppCrashToggleHotKeys = current;
                       break;
                   case "Background":
                       SettingData.BgPlayerToggleHotKeys = current;
                       break;
                   default:
                       break;
               }
               p.Clear();
               IsKeyInputOpen = false;
           });
            ChooseColorCommand = new RelayCommand<string>(p => string.IsNullOrEmpty(CurrentObjectRequestColorName), p =>
            {
                CurrentObjectRequestColorName = p;
                IsColorPickerOpen = true;
            });
            ApplyColorCommand = new RelayCommand<ColorCanvas>(p => true, p =>
            {
                switch (CurrentObjectRequestColorName)
                {
                    case "Blocker":
                        SettingData.BlockerColor = (Color)p.SelectedColor;
                        break;
                    case "Primary":
                        SettingData.PrimaryPenColor = (Color)p.SelectedColor;
                        break;
                    case "Secondary":
                        SettingData.SecondaryPenColor = (Color)p.SelectedColor;
                        break;
                    case "Highlight":
                        SettingData.HighlighterPenColor = (Color)p.SelectedColor;
                        break;
                    default:
                        break;
                }
                CurrentObjectRequestColorName = string.Empty;
                IsColorPickerOpen = false;
            });
            BrowseFolderDialogCommand = new RelayCommand<string>(p => true, p =>
            {
                System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
                dialog.ShowNewFolderButton = true;
                var result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    switch (p)
                    {
                        case "AnimeLibrary":
                            SettingData.AnimeLibraryLocation = dialog.SelectedPath;
                            break;
                        case "Screenshots":
                            SettingData.ScreenShotLocation = dialog.SelectedPath;
                            break;
                        default:
                            break;
                    }

                }
            });
            MainScrollChangedCommand = new RelayCommand<ScrollViewer>(p => IsHostLoaded, MainScrollChangedAction);
            AnimateSrcoll = new RelayCommand<string>(p => true, AnimateAction);
        }

        private void AnimateAction(string obj)
        {
            int parsed = int.Parse(obj);
            var scrViewer = Ins.FindName("mainScroll") as ScrollViewer;
            double toValue = scrViewer.VerticalOffset + GetDistFromHostWindow(scrViewer, parsed) - 195;
            DoubleAnimation anim = new DoubleAnimation(scrViewer.VerticalOffset, toValue, TimeSpan.FromSeconds(0.5)) { EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseOut } };
            scrViewer.BeginAnimation(ScrollAnimationBehavior.VerticalOffsetProperty, anim);
        }


        //Test feature
        //This will take care the visual tab index will change if the control hit this height mark
        public Window HostWindow { get; set; }
        public UserControl Ins { get; set; }
        public double IndexChangeMark { get; set; }

        private double GetDistFromHostWindow(ScrollViewer obj, int index)
        {
            var currentTab = (obj.Content as VirtualizingStackPanel).Children[index];
            return currentTab.TranslatePoint(new Point(0, 0), HostWindow).Y;
        }

        private void MainScrollChangedAction(ScrollViewer obj)
        {
            if (GetDistFromHostWindow(obj, CurrentSettingIndex) > IndexChangeMark)
            {
                //(Backward scroll)
                CurrentSettingIndex--;
                //If the scroll change to quickly (jump 1000 - 700) So a recursion is made to prevent wrong tab
                GetPastTab(obj);
            }
            else
            {
                if (CurrentSettingIndex + 1 >= (obj.Content as VirtualizingStackPanel).Children.Count - 1)
                    return;
                if (GetDistFromHostWindow(obj, CurrentSettingIndex + 1) < IndexChangeMark)
                {
                    //(Forward scroll)
                    CurrentSettingIndex++;
                    //If the scroll change to quickly (jump 1000 - 700) So a recursion is made to prevent wrong tab
                    GetNextTab(obj);
                }
            }
        }

        //Recursion to get the right tab
        private void GetNextTab(ScrollViewer obj)
        {
            if (CurrentSettingIndex + 1 >= (obj.Content as VirtualizingStackPanel).Children.Count - 1)
                return;
            if (GetDistFromHostWindow(obj, CurrentSettingIndex + 1) < IndexChangeMark)
            {
                CurrentSettingIndex++;
                GetNextTab(obj);
            }
        }

        //Recursion to get the right tab
        private void GetPastTab(ScrollViewer obj)
        {
            if (CurrentSettingIndex - 1 < 0)
                return;
            if (GetDistFromHostWindow(obj, CurrentSettingIndex) > IndexChangeMark)
            {
                CurrentSettingIndex--;
                GetPastTab(obj);
            }
        }




        public void OnShow() => UADSettingsManager.Instance.CurrentSettings.Save();

        public void OnHide() => UADSettingsManager.Instance.CurrentSettings.Save();
    }
}

