using System;
using System.Collections;
using System.Windows;
using System.IO;
using System.Collections.Generic;
using System.Linq;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using PDIWT_PiledWharf_Core.Model;

using BM = Bentley.MstnPlatformNET;
using BD = Bentley.DgnPlatformNET;
using BDEC = Bentley.DgnPlatformNET.DgnEC;
using BES = Bentley.ECObjects.Schema;
using BEX = Bentley.ECObjects.XML;
using BECN = Bentley.ECN;
using BE = Bentley.ECObjects;
using BEPQ = Bentley.EC.Persistence.Query;
using BCI = Bentley.ECObjects.Instance;

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
            _concreteModulus = 3.2e7;
            _concretePoisson = 0.2;
            _concreteDensity = 25;
            _concreteUnderWaterDensity = 15;
            _steelModulus = 2e8;
            _steelPoisson = 0.2;
            _steelDensity = 78.5;
            _hat = 2;
            _mhw = 1;
            _msl = 0;
            _mlw = -1;
            _lat = -2;
            _propValueDictionary.Add("ConcretModulus", _concreteModulus);
            _propValueDictionary.Add("ConcretePoisson", _concretePoisson);
            _propValueDictionary.Add("ConcreteDensity", _concreteDensity);
            _propValueDictionary.Add("ConcreteUnderWaterDensity", _concreteUnderWaterDensity);
            _propValueDictionary.Add("SteelModulus", _steelModulus);
            _propValueDictionary.Add("SteelPoisson", _steelPoisson);
            _propValueDictionary.Add("SteelDensity", _steelDensity);
            //_propValueDictionary.Add("HAT", _hat);
            //_propValueDictionary.Add("MHW", _mhw);
            //_propValueDictionary.Add("MSL", _msl);
            //_propValueDictionary.Add("MLW", _mlw);
            //_propValueDictionary.Add("LAT", _lat);

        }

        private Dictionary<string, double> _propValueDictionary = new Dictionary<string, double>();


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

        private RelayCommand _writeEnvParameters;

        /// <summary>
        /// Write the Environment related parameter to dgn model through EC
        /// </summary>
        public RelayCommand WriteEnvParameters
        {
            get
            {
                return _writeEnvParameters
                    ?? (_writeEnvParameters = new RelayCommand(WriteEnvParametersExcuteMethod));
            }
        }

        private void WriteEnvParametersExcuteMethod()
        {
            PDIWT_PiledWharf_Core_Cpp.ECFrameWorkWraper.WriteSettingsOnActiveModel(_propValueDictionary);
            //BM.MessageCenter _messageCenter = BM.MessageCenter.Instance;
            //BDEC.DgnECManager _dgnECManager = BDEC.DgnECManager.Manager;

            //string ecschemaName = "PDIWT.01.00.ecschema.xml";

            //try
            //{
            //    //parse the full schema Name
            //    if (!BE.ECObjects.ParseFullSchemaName(out string pdiwtSchemaName, out int versionMajor, out int versionMinor, ecschemaName))
            //        _messageCenter.ShowErrorMessage($"Can't Parse {ecschemaName}", $"Can't Parse {ecschemaName}, Setting Parameters failed.", BM.MessageAlert.Balloon);
            //    // Obtain the FileInfo of ECSchema
            //    if (GetOrganizationECSchemaFile(out FileInfo pdiwtECSchemaFileInfo, ecschemaName) == BD.StatusInt.Error)
            //        _messageCenter.ShowErrorMessage($"Can't find {ecschemaName}", $"Can't find {ecschemaName}, Setting Parameters failed.", BM.MessageAlert.Balloon);
            //    // If dgnfile doesn't contain the designated schema, Import it.
            //    if (!_dgnECManager.DiscoverSchemas(BM.Session.Instance.GetActiveDgnFile(), BDEC.ReferencedModelScopeOption.All, false).Contains(BE.ECObjects.FormatFullSchemaName(pdiwtSchemaName, versionMajor, versionMinor)))
            //    {
            //        BD.StatusInt _readSchemaStatus = PDIWT_PiledWharf_Core_Cpp.ECFrameWorkWraper.ImportSChemaXMLFileOnActiveModel(pdiwtECSchemaFileInfo.FullName);
            //        if (_readSchemaStatus == BD.StatusInt.Success)
            //            _messageCenter.StatusMessage = string.Format($"Import {ecschemaName}");
            //        else
            //            _messageCenter.StatusMessage = string.Format($"Can't Import {ecschemaName}");
            //    }

            //    // Locate designated schema and write instatnce on it.
            //    // If the active dgn file contain the instance of this schema, update it.
            //    // If not, write the instance on it.
            //    BDEC.FindInstancesScope scope = BDEC.FindInstancesScope.CreateScope(BM.Session.Instance.GetActiveDgnModel(), new BDEC.FindInstancesScopeOption());
            //    BES.IECSchema _ipdiwtschema = _dgnECManager.LocateSchemaInScope(scope, pdiwtSchemaName, versionMajor, versionMinor, BES.SchemaMatchType.Exact);
            //    if (null == _ipdiwtschema)
            //        _messageCenter.ShowErrorMessage($"Can't locate {ecschemaName} In Dgnfile", $"Can't locate {ecschemaName} Dgnfile, Setting Parameters failed.", BM.MessageAlert.Balloon);

            //    string _piledwharfClassName = "PiledWharfSetting";
            //    BDEC.FindInstancesScope _findPDIWTInstanceScope = BDEC.FindInstancesScope.CreateScope(BM.Session.Instance.GetActiveDgnFile(), new BDEC.FindInstancesScopeOption(BDEC.DgnECHostType.Model));
            //    BEPQ.ECQuery _pdiwtQuery = new BEPQ.ECQuery(_ipdiwtschema.GetClass(_piledwharfClassName));
            //    _pdiwtQuery.SelectClause.SelectAllProperties = true;
            //    BDEC.DgnECInstanceCollection _allPDIWTECInstances = _dgnECManager.FindInstances(_findPDIWTInstanceScope, _pdiwtQuery);
            //    if (_allPDIWTECInstances.Count() == 0)
            //    {
            //        _messageCenter.ShowInfoMessage($"{ecschemaName} Exist in Model", $"{ecschemaName} Exist in Model", BM.MessageAlert.None);

            //        BDEC.IDgnECInstance _ecInstance = _allPDIWTECInstances.First();
            //        foreach (var _itemProp in _ecInstance)
            //        {
            //            _itemProp.DoubleValue = _propValueDictionary[_itemProp.Property.Name];
            //        }
            //        _ecInstance.WriteChanges();
            //        _messageCenter.StatusMessage = string.Format($"Write Instance Successfully.");
            //    }
            //    else
            //    {
            //        BES.IECClass _piledwharfECClass = _ipdiwtschema.GetClass(_piledwharfClassName);
            //        if (null == _piledwharfECClass)
            //            _messageCenter.StatusMessage = string.Format($"Can't Find ECClass {_piledwharfClassName}");
            //        BDEC.DgnECInstanceEnabler _piledwharfEnabler = _dgnECManager.ObtainInstanceEnabler(BM.Session.Instance.GetActiveDgnFile(), _piledwharfECClass);
            //        BE.Instance.ECDInstance _piledwharfWipInstance = _piledwharfEnabler.SharedWipInstance;
            //        foreach (var item in _propValueDictionary)
            //        {
            //            _piledwharfWipInstance.SetAsString(item.Key, item.Value.ToString());
            //        }
            //        BDEC.IDgnECInstance _persistentPiledWharfInstance = _piledwharfEnabler.CreateInstanceOnModel(BM.Session.Instance.GetActiveDgnModel(), _piledwharfWipInstance);
            //        _persistentPiledWharfInstance.WriteChanges();
            //        _messageCenter.StatusMessage = string.Format($"Write Instance Successfully.");
            //    }

            //    //}
            //}
            //catch (Exception ex)
            //{
            //    _messageCenter.StatusMessage = ex.Message;
            //    _messageCenter.StatusWarning = ex.Message;
            //}
        }

        /// <summary>
        /// Get the ECSchema file based on Schema Name  
        /// </summary>
        /// <param name="ecSchemaFile">[out]The ECSchema's FileInfo</param>
        /// <param name="ecFullSchemaName"></param>
        /// <param name="schemaExtension">Default ".xml"</param>
        /// <returns>Success/Error</returns>
        private BD.StatusInt GetOrganizationECSchemaFile(out FileInfo ecSchemaFile, string ecFullSchemaName)
        {
            ecSchemaFile = null;
            if (!BD.ConfigurationManager.IsVariableDefined("PDIWT_ORGANIZATION_ECSCHEMAPATH"))
                return BD.StatusInt.Error;

            string[] folderpaths = BD.ConfigurationManager.GetVariable("PDIWT_ORGANIZATION_ECSCHEMAPATH").Split(';');
            foreach (var path in folderpaths)
            {
                if (File.Exists(path + ecFullSchemaName))
                {
                    ecSchemaFile = new FileInfo(path + ecFullSchemaName);
                    return BD.StatusInt.Success;
                }
            }
            return BD.StatusInt.Error;
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}