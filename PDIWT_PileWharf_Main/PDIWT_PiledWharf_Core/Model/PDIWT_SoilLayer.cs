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

namespace PDIWT_PiledWharf_Core.Model
{
    public class SoilLayer : ObservableObject, IEquatable<SoilLayer>
    {
        public SoilLayer()
        {
            Betasi = Psii = Betap = Psip = DiscountCoeff = 1;
            Mesh = null;
        }
        private string _soilLayerNumber;
        /// <summary>
        /// Property Description
        /// </summary>
        public string SoilLayerNumber
        {
            get { return _soilLayerNumber ?? "Unknown"; }
            set { Set(ref _soilLayerNumber, value); }
        }

        private string _soilLayerName;
        /// <summary>
        /// S
        /// </summary>
        public string SoilLayerName
        {
            get { return _soilLayerName ?? "Unknown"; }
            set { Set(ref _soilLayerName, value); }
        }

        private double _betasi;
        /// <summary>
        /// Property Description
        /// </summary>
        public double Betasi
        {
            get { return _betasi; }
            set { Set(ref _betasi, value); }
        }

        private double _psii;
        /// <summary>
        /// Property Description
        /// </summary>
        public double Psii
        {
            get { return _psii; }
            set { Set(ref _psii, value); }
        }

        private double _betap;
        /// <summary>
        /// Property Description
        /// </summary>
        public double Betap
        {
            get { return _betap; }
            set { Set(ref _betap, value); }
        }

        private double _psip;
        /// <summary>
        /// Property Description
        /// </summary>
        public double Psip
        {
            get { return _psip; }
            set { Set(ref _psip, value); }
        }

        private double _sideFrictionStandardValue;
        /// <summary>
        /// Property Description
        /// </summary>
        public double SideFrictionStandardValue
        {
            get { return _sideFrictionStandardValue; }
            set { Set(ref _sideFrictionStandardValue, value); }
        }

        //private double _soilLayerThickness;
        ///// <summary>
        ///// Property Description
        ///// </summary>
        //public double SoilLayerThickness
        //{
        //    get { return _soilLayerThickness; }
        //    set { Set(ref _soilLayerThickness, value); }
        //}

        private double _endResistanceStandardValue;
        /// <summary>
        /// Property Description
        /// </summary>
        public double EndResistanceStandardValue
        {
            get { return _endResistanceStandardValue; }
            set { Set(ref _endResistanceStandardValue, value); }
        }

        private double _discountCoeff;
        /// <summary>
        /// Property Description
        /// </summary>
        public double DiscountCoeff
        {
            get { return _discountCoeff; }
            set { Set(ref _discountCoeff, value); }
        }
        /// <summary>
        /// If the soil layer is constructed by reading mesh element, then reserve the reference to this meshelement.
        /// </summary>
        public BDE.MeshHeaderElement Mesh { get; private set; }

        public bool Equals(SoilLayer other)
        {
            return other.SoilLayerNumber == SoilLayerNumber;
        }

        public override int GetHashCode()
        {
            return SoilLayerNumber.GetHashCode();
        }

        //Obtain Information from given meshElement
        public static SoilLayer ObtainInfoFromMesh(BDE.MeshHeaderElement meshEle)
        {
            var _activeModel = BM.Session.Instance.GetActiveDgnModel();
            var _activedgnfile = BM.Session.Instance.GetActiveDgnFile();

            string _ecSchemaName = "PDIWT";
            string _ecClassName = "BearingCapacitySoilLayerInfo";
            Dictionary<string, object> _soilLayerProps;
            List<string> _requiredSoilLayerProps = new List<string>()
            {
                "LayerNumber",
                "LayerName",
                "Betasi",
                "Psisi",
                "qfi",
                "Betap",
                "Psip",
                "qr"
            };

            //determine if the active model contains ECSchmea
            var _ecmgr = BDEC.DgnECManager.Manager;
            BES.IECSchema _eCSchema = _ecmgr.LocateSchemaInScope(BDEC.FindInstancesScope.CreateScope(BM.Session.Instance.GetActiveDgnFile(), new BDEC.FindInstancesScopeOption(BDEC.DgnECHostType.File, false)),
                _ecSchemaName, 1, 0, BES.SchemaMatchType.Latest);
            if (_eCSchema == null)
                throw new InvalidOperationException("Can't find " + _ecSchemaName);
            //if (!_ecmgr.DiscoverSchemas(_activedgnfile, BDEC.ReferencedModelScopeOption.All, false).Contains(_ecSchemaName + ".01.00"))
            // Read properties from mesh Element
            if (BD.StatusInt.Error == ECSChemaReader.ReadECInstanceProperties(_ecSchemaName, _ecClassName, _requiredSoilLayerProps, meshEle, out _soilLayerProps)
                || _soilLayerProps.Values.Contains(null))
                throw new InvalidOperationException("Can't get information from mesh element");

            //Assign the value to this SoilLayerInfo
            return new SoilLayer()
            {
                SoilLayerNumber = _soilLayerProps["LayerNumber"].ToString(),
                SoilLayerName = _soilLayerProps["LayerName"].ToString(),
                Betasi = double.Parse(_soilLayerProps["Betasi"].ToString()),
                Psii = double.Parse(_soilLayerProps["Psisi"].ToString()),
                SideFrictionStandardValue = double.Parse(_soilLayerProps["qfi"].ToString()),
                Betap = double.Parse(_soilLayerProps["Betap"].ToString()),
                Psip = double.Parse(_soilLayerProps["Psip"].ToString()),
                EndResistanceStandardValue = double.Parse(_soilLayerProps["qr"].ToString()),
                Mesh = meshEle
            };
        }
    }
}
