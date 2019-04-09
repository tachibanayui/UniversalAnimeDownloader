using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using UniversalAnimeDownloader.UADSettingsPortal;

namespace UniversalAnimeDownloader.ViewModels
{
    class UADSettingsViewModel : BaseViewModel, IPageContent
    {
        #region Commands
        public ICommand ApplyPrimaryCommand { get; set; }
        public ICommand ApplyAccentCommand { get; set; }
        public ICommand ApplyFramerateCommand { get; set; }
        #endregion


        private UADSettingsData _SettingData;
        public UADSettingsData SettingData
        {
            get
            {
                if(_SettingData == null)
                {
                    UADSettingsManager.Instance.Init();
                    _SettingData = UADSettingsManager.Instance.CurrentSettings;
                }

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
        }

        public void OnShow() => UADSettingsManager.Instance.CurrentSettings.Save();

        public void OnHide() => UADSettingsManager.Instance.CurrentSettings.Save();
    }
}
