using UniversalAnimeDownloader.UADSettingsPortal;

using UADAPI;
using System.Windows.Input;
using System.Collections.Generic;
using MaterialDesignColors;
using System.Linq;
using MaterialDesignThemes.Wpf;

namespace UniversalAnimeDownloader.ViewModels
{
    class UADSettingsViewModel : BaseViewModel, IPageContent
    {
        #region Commands
        public ICommand ApplyPrimaryCommand { get; set; }
        public ICommand ApplyAccentCommand { get; set; }
        public ICommand ApplyFramerateCommand { get; set; }
        #endregion


        public UADSettingsData SettingData { get; set; }
        public PaletteHelper Helper { get; set; }
        public IList<Swatch> Swatches { get; }

        public UADSettingsViewModel()
        {
            SettingData = UADSettingsManager.Instance.CurrentSettings;
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
