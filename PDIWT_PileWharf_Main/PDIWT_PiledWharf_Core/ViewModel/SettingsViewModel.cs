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
            _pileTypes = new List<PileType>()
            {
                new PileType { NamePath = Resources.Round, Value = "Round" },
                new PileType { NamePath = Resources.Square, Value = "Square" }
            };

            _structureImportanceFactor = 1.1;
            _antiearthquakeIntensity = 1.0;
            _fixedDepthFactor = 1.0;
            _antiEarthquakeAdjustmentFactor = 1.0;

            _selectedPileType = _pileTypes[0].Value;
            _pileSideLengthDiameter = 1000;

            _concreteModulus = 3.2e7;
            _concretePoisson = 0.2;
            _concreteDensity = 25;
            _concreteUnderWaterDensity = 15;
            _steelModulus = 2e8;
            _steelPoisson = 0.2;
            _steelDensity = 78.5;
            _waterDensity = 10;

            _hat = 2;
            _mhw = 1;
            _msl = 0;
            _mlw = -1;
            _lat = -2;

            _waveHeight = 1.0;
            _waveLength = 10;
            _wavePeriod = 20;
        }

        private Dictionary<string, string> _generalPropValueDicitionary = new Dictionary<string, string>();
        private Dictionary<string, string> _geometryPropValueDictionary = new Dictionary<string, string>();
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

            _geometryPropValueDictionary.Clear();
            _geometryPropValueDictionary.Add("PileType", _selectedPileType);
            _geometryPropValueDictionary.Add("PileSideLengthDiameter", _pileSideLengthDiameter.ToString());

            _materialPropValueDictionary.Clear();
            _materialPropValueDictionary.Add("ConcreteModulus", _concreteModulus.ToString());
            _materialPropValueDictionary.Add("ConcretePoisson", _concretePoisson.ToString());
            _materialPropValueDictionary.Add("ConcreteDensity", _concreteDensity.ToString());
            _materialPropValueDictionary.Add("ConcreteUnderWaterDensity", _concreteUnderWaterDensity.ToString());
            _materialPropValueDictionary.Add("SteelModulus", _steelModulus.ToString());
            _materialPropValueDictionary.Add("SteelPoisson", _steelPoisson.ToString());
            _materialPropValueDictionary.Add("SteelDensity", _steelDensity.ToString());
            _materialPropValueDictionary.Add("WaterDensity", _waterDensity.ToString());

            _waterLevelPropValueDictionary.Clear();
            _waterLevelPropValueDictionary.Add("HAT", _hat.ToString());
            _waterLevelPropValueDictionary.Add("MHW", _mhw.ToString());
            _waterLevelPropValueDictionary.Add("MSL", _msl.ToString());
            _waterLevelPropValueDictionary.Add("MLW", _mlw.ToString());
            _waterLevelPropValueDictionary.Add("LAT", _lat.ToString());

            _wavePropValueDictionary.Clear();
            _wavePropValueDictionary.Add("WaveHeight", _waveHeight.ToString());
            _wavePropValueDictionary.Add("WaveLength", _waveLength.ToString());
            _wavePropValueDictionary.Add("WavePeriod", _wavePeriod.ToString());
        }

        private List<PileType> _pileTypes;
        /// <summary>
        /// PileType List
        /// </summary>
        public List<PileType> PileTypes
        {
            get { return _pileTypes; }
            set { Set(ref _pileTypes, value); }
        }


        private string _selectedPileType;
        /// <summary>
        /// the pile Type that client selected
        /// </summary>
        public string SelectedPileType
        {
            get { return _selectedPileType; }
            set { Set(ref _selectedPileType, value); }
        }


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

        private double _pileSideLengthDiameter;
        /// <summary>
        /// the length of side for square pile and diameter for round pile
        /// </summary>
        public double PileSideLengthDiameter
        {
            get { return _pileSideLengthDiameter; }
            set { Set(ref _pileSideLengthDiameter, value); }
        }

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


        private double _concreteDensity;
        /// <summary>
        /// Concrete's Density
        /// </summary>
        public double ConcreteDensity
        {
            get { return _concreteDensity; }
            set { Set(ref _concreteDensity, value); }
        }


        private double _concreteUnderWaterDensity;
        /// <summary>
        /// Concrete's underwater density
        /// </summary>
        public double ConcreteUnderWaterDensity
        {
            get { return _concreteUnderWaterDensity; }
            set { Set(ref _concreteUnderWaterDensity, value); }
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


        private double _steelDensity;
        /// <summary>
        /// steel's density
        /// </summary>
        public double SteelDensity
        {
            get { return _steelDensity; }
            set { Set(ref _steelDensity, value); }
        }


        private double _waterDensity;
        /// <summary>
        /// water density
        /// </summary>
        public double WaterDensity
        {
            get { return _waterDensity; }
            set { Set(ref _waterDensity, value); }
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


        private double _waveHeight;
        /// <summary>
        /// Wave Height
        /// </summary>
        public double WaveHeight
        {
            get { return _waveHeight; }
            set { Set(ref _waveHeight, value); }
        }


        private double _waveLength;
        /// <summary>
        /// Wave Length
        /// </summary>
        public double WaveLength
        {
            get { return _waveLength; }
            set { Set(ref _waveLength, value); }
        }


        private double _wavePeriod;
        /// <summary>
        /// Wave Period
        /// </summary>
        public double WavePeriod
        {
            get { return _wavePeriod; }
            set { Set(ref _wavePeriod, value); }
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