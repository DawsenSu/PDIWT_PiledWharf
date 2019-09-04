using System;
using System.Collections;
using System.Windows;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;

using PDIWT_PiledWharf_Core.Model;
using PDIWT.Resources.Localization.MainModule;


using BM = Bentley.MstnPlatformNET;
using BD = Bentley.DgnPlatformNET;
using BDEC = Bentley.DgnPlatformNET.DgnEC;
using BES = Bentley.ECObjects.Schema;
using BEX = Bentley.ECObjects.XML;
using BECN = Bentley.ECN;
using BE = Bentley.ECObjects;
using BEPQ = Bentley.EC.Persistence.Query;
using BCI = Bentley.ECObjects.Instance;
using System.Windows.Media;

namespace PDIWT_PiledWharf_Core.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class SettingsViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public SettingsViewModel()
        {

            _settingsInstance = new Settings()
            {
                StructureImportanceFactor = 1.1,
                AntiEarthquakeIntensity = 1.0,
                FixedDepthFactor = 1.0,
                AntiEarthquakeAdjustmentFactor = 1.0,

                ConcreteModulus = 3.2e7,
                ConcretePoisson = 0.2,
                ConcreteUnitWeight = 25,
                ConcreteUnderWaterUnitWeight = 15,
                SteelModulus = 2e8,
                SteelPoisson = 0.2,
                SteelUnitWeight = 78.5,
                WaterUnitWeight = 10.25,

                HAT = 2,
                MHW = 1,
                MSL = 0,
                MLW = -1,
                LAT = -2,

                WaveHeight_HAT = 4.2,
                WavePeriod_HAT = 11.20,
                WaveHeight_MHW = 4.00,
                WavePeriod_MHW = 6.60,
                WaveHeight_MSL = 3.8,
                WavePeriod_MSL = 6.0,
                WaveHeight_MLW = 3.70,
                WavePeriod_MLW = 6.30,
                WaveHeight_LAT = 2.90,
                WavePeriod_LAT = 11.20,
            };

        }


        private Settings _settingsInstance;
        /// <summary>
        /// Property Description
        /// </summary>
        public Settings SettingsInstance
        {
            get { return _settingsInstance; }
            set { Set(ref _settingsInstance, value); }
        }

        
        private RelayCommand<Grid> _writeEnvParameters;
        /// <summary>
        /// Write the Environment related parameter to dgn model through EC
        /// </summary>
        public RelayCommand<Grid> WriteEnvParameters
        {
            get
            {
                return _writeEnvParameters
                    ?? (_writeEnvParameters = new RelayCommand<Grid>(
                        grid =>
                        {
                            try
                            {
                                SettingsInstance.WriteECInstanceOnActiveModel();
                                BM.MessageCenter.Instance.ShowInfoMessage("Write Settings on AtiveModel","",BM.MessageAlert.None);
                                MessengerInstance.Send(new NotificationMessage("CloseWindow"));
                            }
                            catch (Exception e)
                            {
                                BM.MessageCenter.Instance.ShowErrorMessage("Can't write Settings on active model", e.ToString(), false);
                            }
                        }
                        , grid => !PDIWT.Resources.PDIWT_Helper.EnumTextBoxHasError(grid)));
            }
        }
    }
}