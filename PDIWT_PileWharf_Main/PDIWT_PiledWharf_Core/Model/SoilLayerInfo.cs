using System;
using System.Collections;
using System.Collections.Generic;
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
using System.Linq;

namespace PDIWT_PiledWharf_Core.Model
{
    using Tools;
    /// <summary>
    /// The soil layer information class holding all related numbering, name and related calculation coefficients.
    /// </summary>
    public class PDIWT_BearingCapacity_SoilLayerInfo : ObservableObject, IEquatable<PDIWT_BearingCapacity_SoilLayerInfo>
    {

        public PDIWT_BearingCapacity_SoilLayerInfo()
        {
            Betasi = Psii = Betap = Psip = DiscountCoeff = 1;

        }
        private string _soilLayerNumber;
        /// <summary>
        /// Property Description
        /// </summary>
        public string SoilLayerNumber
        {
            get { return _soilLayerNumber; }
            set { Set(ref _soilLayerNumber, value); }
        }

        private string _soilLayerName;
        /// <summary>
        /// S
        /// </summary>
        public string SoilLayerName
        {
            get { return _soilLayerName; }
            set { Set(ref _soilLayerName, value); }
        }

        private double _soilLayerTopElevation;
        /// <summary>
        /// Property Description
        /// </summary>
        public double SoilLayerTopElevation
        {
            get { return _soilLayerTopElevation; }
            set { Set(ref _soilLayerTopElevation, value); }
        }

        private double _soilLayerBottomElevation;
        /// <summary>
        /// Property Description
        /// </summary>
        public double SoilLayerBottomElevation
        {
            get { return _soilLayerBottomElevation; }
            set { Set(ref _soilLayerBottomElevation, value); }
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

        private double _soilLayerThickness;
        /// <summary>
        /// Property Description
        /// </summary>
        public double SoilLayerThickness
        {
            get { return _soilLayerThickness; }
            set { Set(ref _soilLayerThickness, value); }
        }

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

        ///// <summary>
        ///// Valid if this soillayer instance is loaded from meshElement by using ObtainInfoFromMeshElement
        ///// </summary>
        //public BDE.MeshHeaderElement MeshElement { get; private set; }

        public bool Equals(PDIWT_BearingCapacity_SoilLayerInfo other)
        {
            return other.SoilLayerNumber == SoilLayerNumber;
        }
        public override int GetHashCode()
        {
            if (SoilLayerNumber != null)
                return SoilLayerNumber.GetHashCode();
            else
                return string.Empty.GetHashCode();
        }

        ////Obtain Information from given meshElement
        //public BD.StatusInt ObtainInfoFromMesh(BDE.MeshHeaderElement meshEle)
        //{
        //    var _activeModel = BM.Session.Instance.GetActiveDgnModel();
        //    var _activedgnfile = BM.Session.Instance.GetActiveDgnFile();

        //    string _ecSchemaName = "PDIWT";
        //    string _ecClassName = "BearingCapacitySoilLayerInfo";
        //    Dictionary<string, object> _soilLayerProps;
        //    List<string> _requiredSoilLayerProps = new List<string>()
        //    {
        //        "LayerNumber",
        //        "LayerName",
        //        "Betasi",
        //        "Psisi",
        //        "qfi",
        //        "Betap",
        //        "Psip",
        //        "qr"
        //    };

        //    //determine if the active model contains ECSchmea
        //    var _ecmgr = BDEC.DgnECManager.Manager;
        //    if (!_ecmgr.DiscoverSchemas(_activedgnfile, BDEC.ReferencedModelScopeOption.All, false).Contains(_ecSchemaName + ".01.00"))
        //        return BD.StatusInt.Error;
        //    // Read properties from mesh Element
        //    if (BD.StatusInt.Error == ECSChemaReader.ReadECInstanceProperties(_ecSchemaName, _ecClassName, _requiredSoilLayerProps, meshEle, out _soilLayerProps)
        //        ||_soilLayerProps.Values.Contains(null))
        //        return BD.StatusInt.Error;
        //    //Assign the value to this SoilLayerInfo
        //    SoilLayerNumber = _soilLayerProps["LayerNumber"].ToString();
        //    SoilLayerName = _soilLayerProps["LayerName"].ToString();
        //    Betasi = double.Parse(_soilLayerProps["Betasi"].ToString());
        //    Psii = double.Parse(_soilLayerProps["Psisi"].ToString());
        //    SideFrictionStandardValue = double.Parse(_soilLayerProps["qfi"].ToString());
        //    Betap = double.Parse(_soilLayerProps["Betap"].ToString());
        //    Psip = double.Parse(_soilLayerProps["Psip"].ToString());
        //    EndResistanceStandardValue = double.Parse(_soilLayerProps["qr"].ToString());

        //    MeshElement = meshEle;
        //    return BD.StatusInt.Success;
        //}
    }

    [Obsolete]
    public class PDIWT_SoilLayerInfoReader
    {
        /// <summary>
        /// Obtain the Soil information from mesh element in active model. 
        /// The mesh element which is attached BearingCapacitySoilLayerInfo ECSchmea could be considered as valid soil layer element.
        /// </summary>
        /// <param name="tupleInfo">Get the soil layers info overall from active model. The item1 is mesh element itself, item2 is the dictionary which has 
        /// property and property's value pair stored in it.</param>
        /// <returns>if Success, obtain valid information.if error, not.</returns>
        public static BD.StatusInt ObtainSoilLayerInfoFromModel(out List<Tuple<BDE.MeshHeaderElement, Dictionary<string, object>>> tupleInfo)
        {
            var _activeModel = BM.Session.Instance.GetActiveDgnModel();
            var _activedgnfile = BM.Session.Instance.GetActiveDgnFile();
            tupleInfo = null;
            var _tupleInfo = new List<Tuple<BDE.MeshHeaderElement, Dictionary<string, object>>>(); // local variable to hold information in lambda expression.
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
            if (!_ecmgr.DiscoverSchemas(_activedgnfile, BDEC.ReferencedModelScopeOption.All, false).Contains(_ecSchemaName + ".01.00"))
                return BD.StatusInt.Error;

            // Scan the mesh element
            BD.ScanCriteria _sc = new BD.ScanCriteria();
            _sc.SetModelRef(_activeModel);
            _sc.SetModelSections(BD.DgnModelSections.GraphicElements);
            BD.BitMask _meshBitMask = new BD.BitMask(false);
            _meshBitMask.Capacity = 400;
            _meshBitMask.ClearAll();
            _meshBitMask.SetBit(104, true);
            _sc.SetElementTypeTest(_meshBitMask);
            _sc.Scan((_mesh, _model) =>
            {
                if (BD.StatusInt.Error == ECSChemaReader.ReadECInstanceProperties(_ecSchemaName, _ecClassName, _requiredSoilLayerProps, _mesh, out _soilLayerProps) ||
                    _soilLayerProps.Values.Contains(null))
                    return BD.StatusInt.Error;
                _tupleInfo.Add(new Tuple<BDE.MeshHeaderElement, Dictionary<string, object>>((BDE.MeshHeaderElement)_mesh, _soilLayerProps));
                return BD.StatusInt.Success;
            });

            if (_tupleInfo.Count == 0)
                return BD.StatusInt.Error;
            tupleInfo = _tupleInfo;

            return BD.StatusInt.Success;
        }


    }
}
