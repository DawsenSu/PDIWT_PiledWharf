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
            //_pileTypes = new List<PileType>()
            //{
            //    new PileType { NamePath = Resources.Round, Value = "Round" },
            //    new PileType { NamePath = Resources.Square, Value = "Square" }
            //};

            _structureImportanceFactor = 1.1;
            _antiearthquakeIntensity = 1.0;
            _fixedDepthFactor = 1.0;
            _antiEarthquakeAdjustmentFactor = 1.0;

            //_selectedPileType = _pileTypes[0].Value;
            //_pileSideLengthDiameter = 1000;

            _concreteModulus = 3.2e7;
            _concretePoisson = 0.2;
            _concreteUnitWeight = 25;
            _concreteUnderWaterUnitWeight = 15;
            _steelModulus = 2e8;
            _steelPoisson = 0.2;
            _steelUnitWeight = 78.5;
            _waterUnitWeight = 10.25;

            _hat = 2;
            _mhw = 1;
            _msl = 0;
            _mlw = -1;
            _lat = -2;

            WaveHeight_HAT = 4.2; WavePeriod_HAT = 11.20;
            WaveHeight_MHW = 4.00; WavePeriod_MHW = 6.60;
            WaveHeight_MSL = 3.8; WavePeriod_MSL = 6.0;
            WaveHeight_MLW = 3.70; WavePeriod_MLW = 6.30;
            WaveHeight_LAT = 2.90; WavePeriod_LAT = 11.20;
        }

        private Dictionary<string, string> _generalPropValueDicitionary = new Dictionary<string, string>();
        //private Dictionary<string, string> _geometryPropValueDictionary = new Dictionary<string, string>();
        private Dictionary<string, string> _materialPropValueDictionary = new Dictionary<string, string>();
        private Dictionary<string, string> _waterLevelPropValueDictionary = new Dictionary<string, string>();
        private Dictionary<string, string> _wavePropValueDictionary = new Dictionary<string, string>();

        /// <summary>
        /// Clear and Build property and value list.
        /// </summary>
        private void BuildPropValueDictionary()
        {
            _generalPropValueDicitionary.Clear();
            _generalPropValueDicitionary.Add("StructureImportanceFactor", _structureImportanceFactor.ToString());
            _generalPropValueDicitionary.Add("AntiEarthquakeIntensity", _antiearthquakeIntensity.ToString());
            _generalPropValueDicitionary.Add("FixedDepthFactor", _fixedDepthFactor.ToString());
            _generalPropValueDicitionary.Add("AntiEarthquakeAdjustmentFactor", _antiEarthquakeAdjustmentFactor.ToString());

            //_geometryPropValueDictionary.Clear();
            //_geometryPropValueDictionary.Add("PileType", _selectedPileType);
            //_geometryPropValueDictionary.Add("PileSideLengthDiameter", _pileSideLengthDiameter.ToString());

            _materialPropValueDictionary.Clear();
            _materialPropValueDictionary.Add("ConcreteModulus", _concreteModulus.ToString());
            _materialPropValueDictionary.Add("ConcretePoisson", _concretePoisson.ToString());
            _materialPropValueDictionary.Add("ConcreteUnitWeight", _concreteUnitWeight.ToString());
            _materialPropValueDictionary.Add("ConcreteUnderWaterUnitWeight", _concreteUnderWaterUnitWeight.ToString());
            _materialPropValueDictionary.Add("SteelModulus", _steelModulus.ToString());
            _materialPropValueDictionary.Add("SteelPoisson", _steelPoisson.ToString());
            _materialPropValueDictionary.Add("SteelUnitWeight", _steelUnitWeight.ToString());
            _materialPropValueDictionary.Add("WaterUnitWeight", _waterUnitWeight.ToString());

            _waterLevelPropValueDictionary.Clear();
            _waterLevelPropValueDictionary.Add("HAT", _hat.ToString());
            _waterLevelPropValueDictionary.Add("MHW", _mhw.ToString());
            _waterLevelPropValueDictionary.Add("MSL", _msl.ToString());
            _waterLevelPropValueDictionary.Add("MLW", _mlw.ToString());
            _waterLevelPropValueDictionary.Add("LAT", _lat.ToString());

            _wavePropValueDictionary.Clear();
            _wavePropValueDictionary.Add("WaveHeight_HAT", WaveHeight_HAT.ToString());
            _wavePropValueDictionary.Add("WavePeriod_HAT", WavePeriod_HAT.ToString());
            _wavePropValueDictionary.Add("WaveHeight_MHW", WaveHeight_MHW.ToString());
            _wavePropValueDictionary.Add("WavePeriod_MHW", WavePeriod_MHW.ToString());
            _wavePropValueDictionary.Add("WaveHeight_MSL", WaveHeight_MSL.ToString());
            _wavePropValueDictionary.Add("WavePeriod_MSL", WavePeriod_MSL.ToString());
            _wavePropValueDictionary.Add("WaveHeight_MLW", WaveHeight_MLW.ToString());
            _wavePropValueDictionary.Add("WavePeriod_MLW", WavePeriod_MLW.ToString());
            _wavePropValueDictionary.Add("WaveHeight_LAT", WaveHeight_LAT.ToString());
            _wavePropValueDictionary.Add("WavePeriod_LAT", WavePeriod_LAT.ToString());
        }

        //private List<PileType> _pileTypes;
        ///// <summary>
        ///// PileType List
        ///// </summary>
        //public List<PileType> PileTypes
        //{
        //    get { return _pileTypes; }
        //    set { Set(ref _pileTypes, value); }
        //}


        //private string _selectedPileType;
        ///// <summary>
        ///// the pile Type that client selected
        ///// </summary>
        //public string SelectedPileType
        //{
        //    get { return _selectedPileType; }
        //    set { Set(ref _selectedPileType, value); }
        //}


        private double _structureImportanceFactor;
        /// <summary>
        /// Property Description
        /// </summary>
        public double StructureImportanceFactor
        {
            get { return _structureImportanceFactor; }
            set { Set(ref _structureImportanceFactor, value); }
        }


        private double _antiearthquakeIntensity;
        /// <summary>
        /// Property Description
        /// </summary>
        public double AntiEarthquakeIntensity
        {
            get { return _antiearthquakeIntensity; }
            set { Set(ref _antiearthquakeIntensity, value); }
        }


        private double _fixedDepthFactor;
        /// <summary>
        /// Property Description
        /// </summary>
        public double FixedDepthFactor
        {
            get { return _fixedDepthFactor; }
            set { Set(ref _fixedDepthFactor, value); }
        }


        private double _antiEarthquakeAdjustmentFactor;
        /// <summary>
        /// Property Description
        /// </summary>
        public double AntiEarthquakeAdjustmentFactor
        {
            get { return _antiEarthquakeAdjustmentFactor; }
            set { Set(ref _antiEarthquakeAdjustmentFactor, value); }
        }

        //private double _pileSideLengthDiameter;
        ///// <summary>
        ///// the length of side for square pile and diameter for round pile
        ///// </summary>
        //public double PileSideLengthDiameter
        //{
        //    get { return _pileSideLengthDiameter; }
        //    set { Set(ref _pileSideLengthDiameter, value); }
        //}

        private double _concreteModulus;
        /// <summary>
        /// Concrete's Modulus
        /// </summary>
        public double ConcreteModulus
        {
            get { return _concreteModulus; }
            set { Set(ref _concreteModulus, value); }
        }

        private double _concretePoisson;
        /// <summary>
        /// Concrete's Poisson Ration
        /// </summary>
        public double ConcretePoisson
        {
            get { return _concretePoisson; }
            set { Set(ref _concretePoisson, value); }
        }


        private double _concreteUnitWeight;
        /// <summary>
        /// Concrete's Density
        /// </summary>
        public double ConcreteUnitWeight
        {
            get { return _concreteUnitWeight; }
            set { Set(ref _concreteUnitWeight, value); }
        }


        private double _concreteUnderWaterUnitWeight;
        /// <summary>
        /// Concrete's underwater density
        /// </summary>
        public double ConcreteUnderWaterUnitWeight
        {
            get { return _concreteUnderWaterUnitWeight; }
            set { Set(ref _concreteUnderWaterUnitWeight, value); }
        }

        private double _steelModulus;
        /// <summary>
        /// Steel's Modulus
        /// </summary>
        public double SteelModulus
        {
            get { return _steelModulus; }
            set { Set(ref _steelModulus, value); }
        }


        private double _steelPoisson;
        /// <summary>
        /// Steel's Poisson
        /// </summary>
        public double SteelPoisson
        {
            get { return _steelPoisson; }
            set { Set(ref _steelPoisson, value); }
        }


        private double _steelUnitWeight;
        /// <summary>
        /// steel's density
        /// </summary>
        public double SteelUnitWeight
        {
            get { return _steelUnitWeight; }
            set { Set(ref _steelUnitWeight, value); }
        }


        private double _waterUnitWeight;
        /// <summary>
        /// water density
        /// </summary>
        public double WaterUnitWeight
        {
            get { return _waterUnitWeight; }
            set { Set(ref _waterUnitWeight, value); }
        }

        private double _hat;
        /// <summary>
        /// Highest Astronomical Tide
        /// </summary>
        public double HAT
        {
            get { return _hat; }
            set { Set(ref _hat, value); }
        }


        private double _mhw;
        /// <summary>
        /// Mean High Water
        /// </summary>
        public double MHW
        {
            get { return _mhw; }
            set { Set(ref _mhw, value); }
        }


        private double _msl;
        /// <summary>
        ///  Mean Sea Level
        /// </summary>
        public double MSL
        {
            get { return _msl; }
            set { Set(ref _msl, value); }
        }


        private double _mlw;
        /// <summary>
        /// Mean Low Water
        /// </summary>
        public double MLW
        {
            get { return _mlw; }
            set { Set(ref _mlw, value); }
        }


        private double _lat;
        /// <summary>
        /// Lowest Astronomical Tide
        /// </summary>
        public double LAT
        {
            get { return _lat; }
            set { Set(ref _lat, value); }
        }


        private double _waveHeight_HAT;
        /// <summary>
        /// Wave Height
        /// </summary>
        public double WaveHeight_HAT
        {
            get { return _waveHeight_HAT; }
            set { Set(ref _waveHeight_HAT, value); }
        }

        private double _wavePeriod_HAT;
        /// <summary>
        /// Wave Period
        /// </summary>
        public double WavePeriod_HAT
        {
            get { return _wavePeriod_HAT; }
            set { Set(ref _wavePeriod_HAT, value); }
        }

        private double _waveHeight_MHW;
        /// <summary>
        /// Wave Height
        /// </summary>
        public double WaveHeight_MHW
        {
            get { return _waveHeight_MHW; }
            set { Set(ref _waveHeight_MHW, value); }
        }

        private double _wavePeriod_MHW;
        /// <summary>
        /// Wave Period
        /// </summary>
        public double WavePeriod_MHW
        {
            get { return _wavePeriod_MHW; }
            set { Set(ref _wavePeriod_MHW, value); }
        }

        private double _waveHeight_MSL;
        /// <summary>
        /// Wave Height
        /// </summary>
        public double WaveHeight_MSL
        {
            get { return _waveHeight_MSL; }
            set { Set(ref _waveHeight_MSL, value); }
        }

        private double _wavePeriod_MSL;
        /// <summary>
        /// Wave Period
        /// </summary>
        public double WavePeriod_MSL
        {
            get { return _wavePeriod_MSL; }
            set { Set(ref _wavePeriod_MSL, value); }
        }
        private double _waveHeight_MLW;
        /// <summary>
        /// Wave Height
        /// </summary>
        public double WaveHeight_MLW
        {
            get { return _waveHeight_MLW; }
            set { Set(ref _waveHeight_MLW, value); }
        }

        private double _wavePeriod_MLW;
        /// <summary>
        /// Wave Period
        /// </summary>
        public double WavePeriod_MLW
        {
            get { return _wavePeriod_MLW; }
            set { Set(ref _wavePeriod_MLW, value); }
        }

        private double _waveHeight_LAT;
        /// <summary>
        /// Wave Height
        /// </summary>
        public double WaveHeight_LAT
        {
            get { return _waveHeight_LAT; }
            set { Set(ref _waveHeight_LAT, value); }
        }

        private double _wavePeriod_LAT;
        /// <summary>
        /// Wave Period
        /// </summary>
        public double WavePeriod_LAT
        {
            get { return _wavePeriod_LAT; }
            set { Set(ref _wavePeriod_LAT, value); }
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
                    ?? (_writeEnvParameters = new RelayCommand<Grid>(WriteEnvParametersExcuteMethod, grid => !PDIWT.Resources.PDIWT_Helper.EnumTextBoxHasError(grid)));
            }
        }

        private void WriteEnvParametersExcuteMethod(Grid grid)
        {
            BuildPropValueDictionary();
            if (PDIWT_PiledWharf_Core_Cpp.ECFrameWorkWraper.WriteSettingsOnActiveModel("PDIWT.01.00.ecschema.xml", "GeneralSettings", _generalPropValueDicitionary)
                == PDIWT_PiledWharf_Core_Cpp.SettingsWriteStatus.ERROR
                || PDIWT_PiledWharf_Core_Cpp.ECFrameWorkWraper.WriteSettingsOnActiveModel("PDIWT.01.00.ecschema.xml", "PileMaterialSettings", _materialPropValueDictionary)
                == PDIWT_PiledWharf_Core_Cpp.SettingsWriteStatus.ERROR
                || PDIWT_PiledWharf_Core_Cpp.ECFrameWorkWraper.WriteSettingsOnActiveModel("PDIWT.01.00.ecschema.xml", "WaterLevelSettings", _waterLevelPropValueDictionary)
                == PDIWT_PiledWharf_Core_Cpp.SettingsWriteStatus.ERROR
                || PDIWT_PiledWharf_Core_Cpp.ECFrameWorkWraper.WriteSettingsOnActiveModel("PDIWT.01.00.ecschema.xml", "WaveSettings", _wavePropValueDictionary)
                == PDIWT_PiledWharf_Core_Cpp.SettingsWriteStatus.ERROR)
                BM.MessageCenter.Instance.ShowErrorMessage("Can't Write SettingsOnAtiveModel", "Can't Write SettingsOnAtiveModel", BM.MessageAlert.Balloon);
            BM.MessageCenter.Instance.ShowInfoMessage("Success", "Write Settings Onto AtiveModel", BM.MessageAlert.None);

            MessengerInstance.Send(new NotificationMessage("CloseWindow"));
        }
        
        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}