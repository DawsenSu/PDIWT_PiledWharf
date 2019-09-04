using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

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
using PDIWT_PiledWharf_Core.Model.Tools;
using System.ComponentModel;

namespace PDIWT_PiledWharf_Core.Model
{
    /// <summary>
    /// Settings Class for read and write to model. The unit of this class is SI.(length -> m, force -> kN, time ->s)
    /// </summary>
    public class Settings : ObservableObject
    {
        public Settings()
        {

        }

        /// <summary>
        /// Build property and value list by using reflection.
        /// </summary>
        private List<Tuple<string, Dictionary<string, string>>> BuildPropValueDictionary()
        {
            List<Tuple<string, Dictionary<string, string>>> _propvalueDictionaryList = new List<Tuple<string, Dictionary<string, string>>>();

            PropertyInfo[] _propertyInfos = GetType().GetProperties();

            foreach (var _info in _propertyInfos)
            {
                CategoryAttribute _categoryAttri = _info.GetCustomAttribute<CategoryAttribute>();
                if (!_propvalueDictionaryList.Exists(i => i.Item1 == _categoryAttri.Category))
                    _propvalueDictionaryList.Add(new Tuple<string, Dictionary<string, string>>(_categoryAttri.Category, new Dictionary<string, string>()));
                var _tuple = _propvalueDictionaryList.Find(i => i.Item1 == _categoryAttri.Category);
                _tuple.Item2.Add(_info.Name, _info.GetValue(this).ToString());
            }
            return _propvalueDictionaryList;
        }

        private double _structureImportanceFactor;
        /// <summary>
        /// dimensionless
        /// </summary>
        [Category("GeneralSettings")]
        public double StructureImportanceFactor
        {
            get { return _structureImportanceFactor; }
            set { Set(ref _structureImportanceFactor, value); }
        }

        private double _antiearthquakeIntensity;
        /// <summary>
        /// dimensionless
        /// </summary>
        [Category("GeneralSettings")]
        public double AntiEarthquakeIntensity
        {
            get { return _antiearthquakeIntensity; }
            set { Set(ref _antiearthquakeIntensity, value); }
        }

        private double _fixedDepthFactor;
        /// <summary>
        /// dimensionless
        /// </summary>
        [Category("GeneralSettings")]
        public double FixedDepthFactor
        {
            get { return _fixedDepthFactor; }
            set { Set(ref _fixedDepthFactor, value); }
        }

        private double _antiEarthquakeAdjustmentFactor;
        /// <summary>
        /// dimensionless
        /// </summary>
        [Category("GeneralSettings")]
        public double AntiEarthquakeAdjustmentFactor
        {
            get { return _antiEarthquakeAdjustmentFactor; }
            set { Set(ref _antiEarthquakeAdjustmentFactor, value); }
        }

        private double _concreteModulus;
        /// <summary>
        /// unit: N/m2
        /// </summary>
        [Category("PileMaterialSettings")]
        public double ConcreteModulus
        {
            get { return _concreteModulus; }
            set { Set(ref _concreteModulus, value); }
        }

        private double _concretePoisson;
        /// <summary>
        /// dimensionless
        /// </summary>
        [Category("PileMaterialSettings")]
        public double ConcretePoisson
        {
            get { return _concretePoisson; }
            set { Set(ref _concretePoisson, value); }
        }

        private double _concreteUnitWeight;
        /// <summary>
        /// unit: kN/m3
        /// </summary>
        [Category("PileMaterialSettings")]
        public double ConcreteUnitWeight
        {
            get { return _concreteUnitWeight; }
            set { Set(ref _concreteUnitWeight, value); }
        }

        private double _concreteUnderWaterUnitWeight;
        /// <summary>
        /// Concrete's underwater density. unit: kN/m3
        /// </summary>
        [Category("PileMaterialSettings")]
        public double ConcreteUnderWaterUnitWeight
        {
            get { return _concreteUnderWaterUnitWeight; }
            set { Set(ref _concreteUnderWaterUnitWeight, value); }
        }

        private double _steelModulus;
        /// <summary>
        /// Steel's Modulus. Unit: N/m2
        /// </summary>
        [Category("PileMaterialSettings")]
        public double SteelModulus
        {
            get { return _steelModulus; }
            set { Set(ref _steelModulus, value); }
        }

        private double _steelPoisson;
        /// <summary>
        /// Steel's Poisson. unit: dimensionless
        /// </summary>
        [Category("PileMaterialSettings")]
        public double SteelPoisson
        {
            get { return _steelPoisson; }
            set { Set(ref _steelPoisson, value); }
        }

        private double _steelUnitWeight;
        /// <summary>
        /// steel's density. Unit: kN/m3
        /// </summary>
        [Category("PileMaterialSettings")]
        public double SteelUnitWeight
        {
            get { return _steelUnitWeight; }
            set { Set(ref _steelUnitWeight, value); }
        }

        private double _waterUnitWeight;
        /// <summary>
        /// water density. Unit: kN/m3
        /// </summary>
        [Category("PileMaterialSettings")]
        public double WaterUnitWeight
        {
            get { return _waterUnitWeight; }
            set { Set(ref _waterUnitWeight, value); }
        }

        private double _hat;
        /// <summary>
        /// Highest Astronomical Tide. Unit: m
        /// </summary>
        [Category("WaterLevelSettings")]
        public double HAT
        {
            get { return _hat; }
            set { Set(ref _hat, value); }
        }

        private double _mhw;
        /// <summary>
        /// Mean High Water. Unit: m
        /// </summary>
        [Category("WaterLevelSettings")]
        public double MHW
        {
            get { return _mhw; }
            set { Set(ref _mhw, value); }
        }

        private double _msl;
        /// <summary>
        ///  Mean Sea Level. Unit: m
        /// </summary>
        [Category("WaterLevelSettings")]
        public double MSL
        {
            get { return _msl; }
            set { Set(ref _msl, value); }
        }

        private double _mlw;
        /// <summary>
        /// Mean Low Water. Unit: m
        /// </summary>
        [Category("WaterLevelSettings")]
        public double MLW
        {
            get { return _mlw; }
            set { Set(ref _mlw, value); }
        }

        private double _lat;
        /// <summary>
        /// Lowest Astronomical Tide. Unit: m
        /// </summary>
        [Category("WaterLevelSettings")]
        public double LAT
        {
            get { return _lat; }
            set { Set(ref _lat, value); }
        }

        private double _waveHeight_HAT;
        /// <summary>
        /// Wave Height. Unit: m
        /// </summary>
        [Category("WaveSettings")]
        public double WaveHeight_HAT
        {
            get { return _waveHeight_HAT; }
            set { Set(ref _waveHeight_HAT, value); }
        }

        private double _wavePeriod_HAT;
        /// <summary>
        /// Wave Period. Unit: s
        /// </summary>
        [Category("WaveSettings")]
        public double WavePeriod_HAT
        {
            get { return _wavePeriod_HAT; }
            set { Set(ref _wavePeriod_HAT, value); }
        }

        private double _waveHeight_MHW;
        /// <summary>
        /// Wave Height. Unit: m
        /// </summary>
        [Category("WaveSettings")]
        public double WaveHeight_MHW
        {
            get { return _waveHeight_MHW; }
            set { Set(ref _waveHeight_MHW, value); }
        }

        private double _wavePeriod_MHW;
        /// <summary>
        /// Wave Period. Unit:s
        /// </summary>
        [Category("WaveSettings")]
        public double WavePeriod_MHW
        {
            get { return _wavePeriod_MHW; }
            set { Set(ref _wavePeriod_MHW, value); }
        }

        private double _waveHeight_MSL;
        /// <summary>
        /// Wave Height. Unit: m
        /// </summary>
        [Category("WaveSettings")]
        public double WaveHeight_MSL
        {
            get { return _waveHeight_MSL; }
            set { Set(ref _waveHeight_MSL, value); }
        }

        private double _wavePeriod_MSL;
        /// <summary>
        /// Wave Period. Unit:s
        /// </summary>
        [Category("WaveSettings")]
        public double WavePeriod_MSL
        {
            get { return _wavePeriod_MSL; }
            set { Set(ref _wavePeriod_MSL, value); }
        }
        private double _waveHeight_MLW;
        /// <summary>
        /// Wave Height. Unit: m
        /// </summary>
        [Category("WaveSettings")]
        public double WaveHeight_MLW
        {
            get { return _waveHeight_MLW; }
            set { Set(ref _waveHeight_MLW, value); }
        }

        private double _wavePeriod_MLW;
        /// <summary>
        /// Wave Period. Unit:s
        /// </summary>
        [Category("WaveSettings")]
        public double WavePeriod_MLW
        {
            get { return _wavePeriod_MLW; }
            set { Set(ref _wavePeriod_MLW, value); }
        }

        private double _waveHeight_LAT;
        /// <summary>
        /// Wave Height. Unit: m
        /// </summary>
        [Category("WaveSettings")]
        public double WaveHeight_LAT
        {
            get { return _waveHeight_LAT; }
            set { Set(ref _waveHeight_LAT, value); }
        }

        private double _wavePeriod_LAT;
        /// <summary>
        /// Wave Period. Unit:s
        /// </summary>
        [Category("WaveSettings")]
        public double WavePeriod_LAT
        {
            get { return _wavePeriod_LAT; }
            set { Set(ref _wavePeriod_LAT, value); }
        }

        public BD.StatusInt WriteECInstanceOnActiveModel()
        {
            try
            {
                string _fullECSchemaFileFullName = "PDIWT.01.00.ecschema.xml";
                var _propvalueDictList = BuildPropValueDictionary();
                foreach (var _item in _propvalueDictList)
                {
                    if (PDIWT_PiledWharf_Core_Cpp.ECFrameWorkWraper.WriteSettingsOnActiveModel(_fullECSchemaFileFullName, _item.Item1, _item.Item2)
                        == PDIWT_PiledWharf_Core_Cpp.SettingsWriteStatus.ERROR)
                        return BD.StatusInt.Error;
                }
                return BD.StatusInt.Success;
            }
            catch
            {
                return BD.StatusInt.Error;
            }
        }

        /// <summary>
        /// construct setting object from given model
        /// </summary>
        /// <param name="dgnModel"></param>
        /// <returns></returns>
        public static Settings ObtainFromModel(BD.DgnModel dgnModel)
        {
            try
            {
                Settings _settings = new Settings();
                string _ecSchemaName = "PDIWT";                
                var _propvalueLists = _settings.BuildPropValueDictionary();
                foreach (var _item in _propvalueLists)
                {
                    ECSChemaReader.ReadECInstanceProperties(_ecSchemaName, _item.Item1,_item.Item2.Keys, dgnModel, out Dictionary<string, object> _props);
                    foreach (var _prop in _props)
                    {
                        typeof(Settings).GetProperty(_prop.Key).SetValue(_settings, _prop.Value);
                    }
                }
                return _settings;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Can't get setting object from {dgnModel.ModelName}", e);
            }
        }

    }
}
