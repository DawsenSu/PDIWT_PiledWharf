using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GalaSoft.MvvmLight;

using BD = Bentley.DgnPlatformNET;
using BM = Bentley.MstnPlatformNET;
using BDEC = Bentley.DgnPlatformNET.DgnEC;
using BES = Bentley.ECObjects.Schema;
using BEI = Bentley.ECObjects.Instance;
using BDE = Bentley.DgnPlatformNET.Elements;
using BMW = Bentley.MstnPlatformNET.WPF;
using BG = Bentley.GeometryNET;
using System.Collections.ObjectModel;
using PDIWT_PiledWharf_Core.Model.Tools;
using PDIWT_PiledWharf_Core.Common;
using System.Reflection;

namespace PDIWT_PiledWharf_Core.Model
{
    public class SoilLayer : ObservableObject, IEquatable<SoilLayer>
    {
        public SoilLayer()
        {
            //_number = string.Empty;
            //_name = string.Empty;
            //_tpsp_yita = _cisp_psisi = _cisp_psip = _cisagp_betasi = _cisagp_psisi = _cisagp_betap = _cisagp_psip = _xii = 1;
            //Mesh = null;
        }

        private string _number;
        /// <summary>
        /// Property Description
        /// </summary>
        [EC(SchemaName = "PDIWT", ClassName = "SoilLayerBase", PropertyName = "LayerNumber")]
        public string Number
        {
            get { return _number; }
            set { Set(ref _number, value); }
        }

        private string _name;
        /// <summary>
        /// Property Description
        /// </summary>
        [EC(SchemaName = "PDIWT", ClassName = "SoilLayerBase", PropertyName = "LayerName")]
        public string Name
        {
            get { return _name; }
            set { Set(ref _name, value); }
        }

        //********* Driven pile with sealed end *************//
        private double? _dpse_qfi;
        /// <summary>
        /// Driven pile with sealed end
        /// </summary>
        [EC(SchemaName = "PDIWT", ClassName = "BCDriven_SI", PropertyName = "qfi")]
        public double? DPSE_Qfi
        {
            get { return _dpse_qfi; }
            set
            {
                if (value.HasValue && value < 0)
                    Set(ref _dpse_qfi, 0);
                else
                    Set(ref _dpse_qfi, value);

            }
        }


        private double? _dpse_qr;
        /// <summary>
        /// Driven pile with sealed end
        /// </summary>
        [EC(SchemaName = "PDIWT", ClassName = "BCDriven_SI", PropertyName = "qr")]
        public double? DPSE_Qr
        {
            get { return _dpse_qr; }
            set
            {
                if (value.HasValue && value < 0)
                    Set(ref _dpse_qr, 0);
                else
                    Set(ref _dpse_qr, value);
            }
        }

        //********* Steel or concrete tube pile *************//

        private double? _tpsp_qfi;
        /// <summary>
        /// Steel or concrete tube pile
        /// </summary>
        [EC(SchemaName = "PDIWT", ClassName = "BCTube_SI", PropertyName = "qfi")]
        public double? TPSP_Qfi
        {
            get { return _tpsp_qfi; }
            set
            {
                if (value.HasValue && value < 0)
                    Set(ref _tpsp_qfi, 0);
                else
                    Set(ref _tpsp_qfi, value);
            }
        }

        private double? _tpsp_yita;
        /// <summary>
        /// Steel or concrete tube pile
        /// </summary>
        [EC(SchemaName = "PDIWT", ClassName = "BCTube_SI", PropertyName = "Yita")]
        public double? TPSP_Yita
        {
            get { return _tpsp_yita; }
            set
            {
                if (value.HasValue && value < 0)
                    Set(ref _tpsp_yita, 0);
                else
                    Set(ref _tpsp_yita, value);
            }
        }

        private double? _tpsp_qr;
        /// <summary>
        /// Steel or concrete tube pile
        /// </summary>
        [EC(SchemaName = "PDIWT", ClassName = "BCTube_SI", PropertyName = "qr")]
        public double? TPSP_Qr
        {
            get { return _tpsp_qr; }
            set
            {
                if (value.HasValue && value < 0)
                    Set(ref _tpsp_qr, 0);
                else
                    Set(ref _tpsp_qr, value);
            }
        }

        //********* Cast-In Situ Pile *************//

        private double? _cisp_psisi;
        /// <summary>
        /// Cast-In Situ Pile
        /// </summary>
        [EC(SchemaName = "PDIWT", ClassName = "BCCastIn_SI", PropertyName = "Psisi")]
        public double? CISP_Psisi
        {
            get { return _cisp_psisi; }
            set
            {
                if (value.HasValue && value < 0)
                    Set(ref _cisp_psisi, 0);
                else
                    Set(ref _cisp_psisi, value);
            }
        }

        private double? _cisp_qfi;
        /// <summary>
        /// Cast-In Situ Pile
        /// </summary>
        [EC(SchemaName = "PDIWT", ClassName = "BCCastIn_SI", PropertyName = "qfi")]
        public double? CISP_Qfi
        {
            get { return _cisp_qfi; }
            set
            {

                if (value.HasValue && value < 0)
                    Set(ref _cisp_qfi, 0);
                else
                    Set(ref _cisp_qfi, value);
            }
        }

        private double? _cisp_psip;
        /// <summary>
        /// Cast-In Situ Pile
        /// </summary>
        [EC(SchemaName = "PDIWT", ClassName = "BCCastIn_SI", PropertyName = "Psip")]
        public double? CISP_Psip
        {
            get { return _cisp_psip; }
            set
            {
                if (value.HasValue && value < 0)
                    Set(ref _cisp_psip, 0);
                else
                    Set(ref _cisp_psip, value);
            }
        }

        private double? _cisp_qr;
        /// <summary>
        /// Cast-In Situ Pile
        /// </summary>
        [EC(SchemaName = "PDIWT", ClassName = "BCCastIn_SI", PropertyName = "qr")]
        public double? CISP_Qr
        {
            get { return _cisp_qr; }
            set
            {
                if (value.HasValue && value < 0)
                    Set(ref _cisp_qr, 0);
                else
                    Set(ref _cisp_qr, value);
            }
        }

        //********* Cast-In Situ After Grouting Pile *************//

        private double? _cisagp_betasi;
        /// <summary>
        /// Cast-In Situ After Grouting Pile
        /// </summary>
        [EC(SchemaName = "PDIWT", ClassName = "BCCastInAfterGrouting_SI", PropertyName = "Betasi")]
        public double? CISAGP_Betasi
        {
            get { return _cisagp_betasi; }
            set
            {
                if (value.HasValue && value < 0)
                    Set(ref _cisagp_betasi, 0);
                else
                    Set(ref _cisagp_betasi, value);
            }
        }

        private double? _cisagp_psisi;
        /// <summary>
        /// Cast-In Situ After Grouting Pile
        /// </summary>
        [EC(SchemaName = "PDIWT", ClassName = "BCCastInAfterGrouting_SI", PropertyName = "Psisi")]
        public double? CISAGP_Psisi
        {
            get { return _cisagp_psisi; }
            set
            {
                if (value.HasValue && value < 0)
                    Set(ref _cisagp_psisi, 0);
                else
                    Set(ref _cisagp_psisi, value);
            }
        }

        private double? _cisagp_qfi;
        /// <summary>
        /// Cast-In Situ After Grouting Pile
        /// </summary>
        [EC(SchemaName = "PDIWT", ClassName = "BCCastInAfterGrouting_SI", PropertyName = "qfi")]
        public double? CISAGP_Qfi
        {
            get { return _cisagp_qfi; }
            set
            {
                if (value.HasValue && value < 0)
                    Set(ref _cisagp_qfi, 0);
                else
                    Set(ref _cisagp_qfi, value);
            }
        }

        private double? _cisagp_betap;
        /// <summary>
        /// Cast-In Situ After Grouting Pile
        /// </summary>
        [EC(SchemaName = "PDIWT", ClassName = "BCCastInAfterGrouting_SI", PropertyName = "Betap")]
        public double? CISAGP_Betap
        {
            get { return _cisagp_betap; }
            set
            {
                if (value.HasValue && value < 0)
                    Set(ref _cisagp_betap, 0);
                else
                    Set(ref _cisagp_betap, value);
            }
        }

        private double? _cisagp_psip;
        /// <summary>
        /// Cast-In Situ After Grouting Pile
        /// </summary>
        [EC(SchemaName = "PDIWT", ClassName = "BCCastInAfterGrouting_SI", PropertyName = "Psip")]
        public double? CISAGP_Psip
        {
            get { return _cisagp_psip; }
            set
            {
                if (value.HasValue && value < 0)
                    Set(ref _cisagp_psip, 0);
                else
                    Set(ref _cisagp_psip, value);
            }
        }

        private double? _cisagp_qr;
        /// <summary>
        /// Cast-In Situ After Grouting Pile
        /// </summary>
        [EC(SchemaName = "PDIWT", ClassName = "BCCastInAfterGrouting_SI", PropertyName = "qr")]
        public double? CISAGP_Qr
        {
            get { return _cisagp_qr; }
            set
            {
                if (value.HasValue && value < 0)
                    Set(ref _cisagp_qr, 0);
                else
                    Set(ref _cisagp_qr, value);
            }
        }

        //********* Pile Lifting *************//

        private double? _xii;
        /// <summary>
        /// Pile Lifting
        /// </summary>
        [EC(SchemaName = "PDIWT", ClassName = "BCLF_SI", PropertyName = "xii")]
        public double? Xii
        {
            get { return _xii; }
            set
            {
                if (value.HasValue && value < 0)
                    Set(ref _xii, 0);
                else
                    Set(ref _xii, value);
            }
        }
        #region older version

        //private string _soilLayerNumber;
        ///// <summary>
        ///// Property Description
        ///// </summary>
        //public string SoilLayerNumber
        //{
        //    get { return _soilLayerNumber ?? "Unknown"; }
        //    set { Set(ref _soilLayerNumber, value); }
        //}

        //private string _soilLayerName;
        ///// <summary>
        ///// S
        ///// </summary>
        //public string SoilLayerName
        //{
        //    get { return _soilLayerName ?? "Unknown"; }
        //    set { Set(ref _soilLayerName, value); }
        //}

        //private double _betasi;
        ///// <summary>
        ///// Property Description
        ///// </summary>
        //public double Betasi
        //{
        //    get { return _betasi; }
        //    set { Set(ref _betasi, value); }
        //}

        //private double _psii;
        ///// <summary>
        ///// Property Description
        ///// </summary>
        //public double Psii
        //{
        //    get { return _psii; }
        //    set { Set(ref _psii, value); }
        //}

        //private double _betap;
        ///// <summary>
        ///// Property Description
        ///// </summary>
        //public double Betap
        //{
        //    get { return _betap; }
        //    set { Set(ref _betap, value); }
        //}

        //private double _psip;
        ///// <summary>
        ///// Property Description
        ///// </summary>
        //public double Psip
        //{
        //    get { return _psip; }
        //    set { Set(ref _psip, value); }
        //}

        //private double _sideFrictionStandardValue;
        ///// <summary>
        ///// Property Description
        ///// </summary>
        //public double SideFrictionStandardValue
        //{
        //    get { return _sideFrictionStandardValue; }
        //    set { Set(ref _sideFrictionStandardValue, value); }
        //}

        ////private double _soilLayerThickness;
        /////// <summary>
        /////// Property Description
        /////// </summary>
        ////public double SoilLayerThickness
        ////{
        ////    get { return _soilLayerThickness; }
        ////    set { Set(ref _soilLayerThickness, value); }
        ////}

        //private double _endResistanceStandardValue;
        ///// <summary>
        ///// Property Description
        ///// </summary>
        //public double EndResistanceStandardValue
        //{
        //    get { return _endResistanceStandardValue; }
        //    set { Set(ref _endResistanceStandardValue, value); }
        //}

        //private double _discountCoeff;
        ///// <summary>
        ///// Property Description
        ///// </summary>
        //public double DiscountCoeff
        //{
        //    get { return _discountCoeff; }
        //    set { Set(ref _discountCoeff, value); }
        //}

        #endregion

        /// <summary>
        /// If the soil layer is constructed by reading mesh element, then reserve the reference to this meshelement.
        /// </summary>
        public BDE.MeshHeaderElement Mesh { get; private set; }

        public bool Equals(SoilLayer other)
        {
            return other.Number == Number;
        }

        public override int GetHashCode()
        {
            return Number == null ? 0 : Number.GetHashCode();
        }
        /// <summary>
        /// Create the list of Standalone EC Instances for attach or use 
        /// </summary>
        /// <returns></returns>
        public List<BEI.StandaloneECDInstance> CreateECInstances()
        {
            BDEC.DgnECManager _mgr = BDEC.DgnECManager.Manager;
            List<BEI.StandaloneECDInstance> _instances = new List<BEI.StandaloneECDInstance>();
            var _activedgnfile = BM.Session.Instance.GetActiveDgnFile();

            if (PDIWT_PiledWharf_Core_Cpp.ECFrameWorkWraper.ImportECSchemaInActiveDgn("PDIWT.01.00.ecschema.xml")== BD.StatusInt.Error)
                throw new InvalidOperationException("Can't import PDIWT ESChema");

            List<Tuple<string, string, string, object>> _listInfo = new List<Tuple<string, string, string, object>>();
            PropertyInfo[] _propInfos = GetType().GetProperties();
            foreach (var _prop in _propInfos)
            {
                if (_prop.IsDefined(typeof(ECAttribute)))
                {
                    ECAttribute _ecAtrr = _prop.GetCustomAttribute<ECAttribute>();
                    _listInfo.Add(Tuple.Create(_ecAtrr.SchemaName, _ecAtrr.ClassName, _ecAtrr.PropertyName, _prop.GetValue(this)));
                }
            }
            var _schemaList = (from _info in _listInfo select _info.Item1).Distinct().ToList();
            var _classList = (from _info in _listInfo select _info.Item2).Distinct().ToList();
            foreach (var _schemaName in _schemaList)
            {
                foreach (var _className in _classList)
                {
                    var _propertiesList = from _info in _listInfo where _info.Item1 == _schemaName && _info.Item2 == _className select Tuple.Create(_info.Item3, _info.Item4);

                    BDEC.FindInstancesScope _scope = BDEC.FindInstancesScope.CreateScope(_activedgnfile, new BDEC.FindInstancesScopeOption(BDEC.DgnECHostType.All, false));
                    BES.IECSchema _ecSchema = _mgr.LocateSchemaInScope(_scope, _schemaName, 1, 0, BES.SchemaMatchType.Exact);
                    if (_ecSchema == null)
                        throw new InvalidOperationException($"{_schemaName} doesn't attach to active dgn file");
                    BES.IECClass _ecClass = _ecSchema.GetClass(_className);
                    if (_ecClass == null)
                        throw new InvalidOperationException($"{_schemaName} doesn't have {_className} class");

                    BDEC.DgnECInstanceEnabler _enabler = _mgr.ObtainInstanceEnabler(BM.Session.Instance.GetActiveDgnFile(), _ecClass);
                    BEI.StandaloneECDInstance _instance = _enabler.SharedWipInstance;

                    foreach (var _tuple in _propertiesList)
                    {
                        if (_tuple.Item2 == null)
                            _instance.MemoryBuffer.SetPropertyToNull(_tuple.Item1, -1);
                        else
                        {
                            if (_tuple.Item2 is string)
                                _instance.MemoryBuffer.SetStringValue(_tuple.Item1, -1, (string)_tuple.Item2);
                            else if (_tuple.Item2 is double?)
                                _instance.MemoryBuffer.SetDoubleValue(_tuple.Item1, -1, ((double?)_tuple.Item2).Value);
                            else if (_tuple.Item2 is int?)
                                _instance.MemoryBuffer.SetIntegerValue(_tuple.Item1, -1, ((int?)_tuple.Item2).Value);
                            else if (_tuple.Item2 is bool?)
                                _instance.MemoryBuffer.SetBooleanValue(_tuple.Item1, -1, ((bool?)_tuple.Item2).Value);
                            else
                                throw new NotImplementedException($"{_tuple.Item2.GetType()} is not support now");
                        }
                    }
                    _instances.Add(_instance);
                }
            }

            return _instances;
        }

        //Obtain Information from given meshElement
        public static SoilLayer ObtainInfoFromMesh(BDE.MeshHeaderElement meshEle)
        {
            var _activeModel = BM.Session.Instance.GetActiveDgnModel();
            var _activedgnfile = BM.Session.Instance.GetActiveDgnFile();
            var _ecmgr = BDEC.DgnECManager.Manager;
            if (!ECSChemaReader.IsElementAttachedECInstance(meshEle, "PDIWT", "SoilLayerBase"))
                return null;

            SoilLayer _soilLayer = new SoilLayer();
            List<Tuple<string, string, string>> _listInfo = new List<Tuple<string, string, string>>();
            PropertyInfo[] _propInfos = typeof(SoilLayer).GetProperties();
            foreach (var _prop in _propInfos)
            {
                if (_prop.IsDefined(typeof(ECAttribute)))
                {
                    ECAttribute _ecAtrr = _prop.GetCustomAttribute<ECAttribute>();
                    _listInfo.Add(Tuple.Create(_ecAtrr.SchemaName, _ecAtrr.ClassName, _ecAtrr.PropertyName));
                }
            }

            var _schemaList = (from _info in _listInfo select _info.Item1).Distinct().ToList();
            var _classList = (from _info in _listInfo select _info.Item2).Distinct().ToList();

            foreach (var _schemaName in _schemaList)
            {
                foreach (var _className in _classList)
                {
                    var _propertiesList = from _info in _listInfo where _info.Item1 == _schemaName && _info.Item2 == _className select _info.Item3;
                    BES.IECSchema _eCSchema = _ecmgr.LocateSchemaInScope(BDEC.FindInstancesScope.CreateScope(BM.Session.Instance.GetActiveDgnFile(), new BDEC.FindInstancesScopeOption(BDEC.DgnECHostType.File, false)),
                                                                         _schemaName, 1, 0, BES.SchemaMatchType.Latest);
                    if (_eCSchema == null)
                        throw new InvalidOperationException("Can't find " + _schemaName);
                    if (BD.StatusInt.Error == ECSChemaReader.ReadECInstanceProperties(_schemaName, _className, _propertiesList, meshEle, out Dictionary<string, object> _soilLayerProps))
                        throw new InvalidOperationException("Can't get information from mesh element");
                    foreach (var _pair in _soilLayerProps)
                    {
                        PropertyInfo _currentPropertyInfo = _propInfos.First(_info =>
                        {
                            if (_info.IsDefined(typeof(ECAttribute)))
                            {
                                ECAttribute _attri = _info.GetCustomAttribute<ECAttribute>();
                                return _attri.SchemaName == _schemaName && _attri.ClassName == _className && _attri.PropertyName == _pair.Key;
                            }
                            else
                                return false;
                        });
                        _currentPropertyInfo.SetValue(_soilLayer, _pair.Value);
                    }

                }
            }
            _soilLayer.Mesh = meshEle;
            return _soilLayer;
        }
    }
}
