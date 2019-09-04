using System;
using System.Linq;
using System.Collections.Generic;

using GalaSoft.MvvmLight;

using System.ComponentModel;
using System.Collections.ObjectModel;

using BD = Bentley.DgnPlatformNET;
using BG = Bentley.GeometryNET;
using BM = Bentley.MstnPlatformNET;
using BDE = Bentley.DgnPlatformNET.Elements;
using PDIWT.Formulas;

namespace PDIWT_PiledWharf_Core.Model
{
    using PDIWT_PiledWharf_Core.Model.Tools;
    using ViewModel;



    public class PDIWT_BearingCapacity_PileInfo : ObservableObject
    {
        public PDIWT_BearingCapacity_PileInfo()
        {
            _isCalculated = false;
            _bearingCapacityPileCategory = PDIWT.Resources.PDIWT_Helper.GetEnumDescriptionDictionary<BearingCapacityPileTypes>();
            _selectedBearingCapacityPileType = BearingCapacityPileTypes.DrivenPileWithSealedEnd;
            _pileGeoCategory = PDIWT.Resources.PDIWT_Helper.GetEnumDescriptionDictionary<PDIWT_PiledWharf_Core_Cpp.PileTypeManaged>();
            _selectedPileGeoType = PDIWT_PiledWharf_Core_Cpp.PileTypeManaged.SqaurePile;
            _isVerticalPile = true;
            _pileSkewness = double.NaN;
            _concreteCoreLength = 0;
            _partialCoeff = 1.5;
            _blockCoeff = 1;
            _concreteWeight = 25;
            _concreteUnderwaterWeight = 15;
            _steelWeight = 78;
            _steelUnderwaterWeight = 68;
            PileSoilLayersInfo = new ObservableCollection<PDIWT_BearingCapacity_SoilLayerInfo>();
            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName != "IsCalculated")
                    IsCalculated = false;
            };
        }
        //****************** For The Status of Calculation Item **********************//

        private bool _isCalculated;
        /// <summary>
        /// Property Description
        /// </summary>
        public bool IsCalculated
        {
            get { return _isCalculated; }
            set { Set(ref _isCalculated, value); }
        }
        //****************** Input Parameters | Basic **********************//
        private string _pileCode;
        /// <summary>
        /// Pile Name or Code from IFC
        /// </summary>
        public string PileCode
        {
            get { return _pileCode; }
            set { Set(ref _pileCode, value); }
        }

        private Dictionary<BearingCapacityPileTypes, string> _bearingCapacityPileCategory;
        /// <summary>
        /// Property Description
        /// </summary>
        public Dictionary<BearingCapacityPileTypes, string> BearingCapacityPileCategory
        {
            get { return _bearingCapacityPileCategory; }
            set { Set(ref _bearingCapacityPileCategory, value); }
        }

        private BearingCapacityPileTypes _selectedBearingCapacityPileType;
        /// <summary>
        /// S
        /// </summary>
        public BearingCapacityPileTypes SelectedBearingCapacityPileType
        {
            get { return _selectedBearingCapacityPileType; }
            set { Set(ref _selectedBearingCapacityPileType, value); }
        }


        private Dictionary<PDIWT_PiledWharf_Core_Cpp.PileTypeManaged, string> _pileGeoCategory;
        /// <summary>
        /// Property Description
        /// </summary>
        public Dictionary<PDIWT_PiledWharf_Core_Cpp.PileTypeManaged, string> PileGeoCategory
        {
            get { return _pileGeoCategory; }
            set { Set(ref _pileGeoCategory, value); }
        }


        private PDIWT_PiledWharf_Core_Cpp.PileTypeManaged _selectedPileGeoType;
        /// <summary>
        /// Property Description
        /// </summary>
        public PDIWT_PiledWharf_Core_Cpp.PileTypeManaged SelectedPileGeoType
        {
            get { return _selectedPileGeoType; }
            set { Set(ref _selectedPileGeoType, value); }
        }

        private bool _isVerticalPile;
        /// <summary>
        /// Property Description
        /// </summary>
        public bool IsVerticalPile
        {
            get { return _isVerticalPile; }
            set { Set(ref _isVerticalPile, value); }
        }

        //****************** Input parameters | Geometry **********************//
        private double _pileTopElevation;
        /// <summary>
        /// unit: m
        /// </summary>
        public double PileTopElevation
        {
            get { return _pileTopElevation; }
            set { Set(ref _pileTopElevation, value); }
        }

        //private BG.DPoint3d _topPoint;
        ///// <summary>
        ///// unit: mm
        ///// </summary>
        //public BG.DPoint3d TopPoint
        //{
        //    get { return _topPoint; }
        //    set { Set(ref _topPoint, value); }
        //}

        private double _pileLength;
        /// <summary>
        /// unit: m
        /// </summary>
        public double PileLength
        {
            get { return _pileLength; }
            set
            {
                if(value < 0)
                    Set(ref _pileLength, 0);
                else
                    Set(ref _pileLength, value);
            }
        }

        private double _pileSkewness;
        /// <summary>
        /// if it's vertical, NaN; otherwise double type.
        /// </summary>
        public double PileSkewness
        {
            get { return _pileSkewness; }
            set
            {
                if (value < 0)
                    Set(ref _pileSkewness, double.NaN);
                else
                    Set(ref _pileSkewness, value);
            }
        }

        private double _pilediameter;
        /// <summary>
        /// unit: m
        /// </summary>
        public double PileDiameter
        {
            get { return _pilediameter; }
            set
            {
                if (value < 0)
                    Set(ref _pilediameter, 0);
                else
                    Set(ref _pilediameter, value);
            }
        }

        private double _pileInsideDiameter;
        /// <summary>
        /// unit: m
        /// </summary>
        public double PileInsideDiameter
        {
            get { return _pileInsideDiameter; }
            set
            {
                if (value < 0)
                    Set(ref _pileInsideDiameter, 0);
                else
                    Set(ref _pileInsideDiameter, value);
            }
        }

        private double _concreteCoreLength;
        /// <summary>
        /// unit: m
        /// </summary>
        public double ConcreteCoreLength
        {
            get { return _concreteCoreLength; }
            set
            {
                if (value < 0)
                    Set(ref _concreteCoreLength, 0);
                else
                    Set(ref _concreteCoreLength, value);
            }
        }
        //****************** Input parameters | Calculation Coefficient **********************//
        private double _partialCoeff;
        /// <summary>
        /// unit less
        /// </summary>
        public double PartialCoeff
        {
            get { return _partialCoeff; }
            set
            {
                if (value < 1)
                    Set(ref _partialCoeff, 1);
                else
                    Set(ref _partialCoeff, value);
            }
        }

        private double _blockCoeff;
        /// <summary>
        /// unit less
        /// </summary>
        public double BlockCoeff
        {
            get { return _blockCoeff; }
            set
            {
                if (value < 1)
                    Set(ref _blockCoeff, 1);
                else
                    Set(ref _blockCoeff, value);
            }
        }

        private double _calculateWaterlevel;
        /// <summary>
        /// unit: m
        /// </summary>
        public double CalculatedWaterLevel
        {
            get { return _calculateWaterlevel; }
            set { Set(ref _calculateWaterlevel, value); }
        }

        //****************** Input parameters | Material **********************//

        private double _concreteWeight;
        /// <summary>
        /// unit: kN/m3
        /// </summary>
        public double ConcreteWeight
        {
            get { return _concreteWeight; }
            set
            {
                if (value < 0)
                    Set(ref _concreteWeight, 25);
                else
                    Set(ref _concreteWeight, value);
            }
        }

        private double _concreteUnderwaterWeight;
        /// <summary>
        /// unit: kN/m3
        /// </summary>
        public double ConcreteUnderwaterWeight
        {
            get { return _concreteUnderwaterWeight; }
            set
            {
                if (value < 0)
                    Set(ref _concreteUnderwaterWeight, 15);
                else
                    Set(ref _concreteUnderwaterWeight, value);
            }
        }

        private double _steelWeight;
        /// <summary>
        /// unit: kN/m3
        /// </summary>
        public double SteelWeight
        {
            get { return _steelWeight; }
            set
            {
                if (value < 0)
                    Set(ref _steelWeight, 78);
                else
                    Set(ref _steelWeight, value);
            }
        }

        private double _steelUnderwaterWeight;
        /// <summary>
        /// unit: kN/m3
        /// </summary>
        public double SteelUnderwaterWeight
        {
            get { return _steelUnderwaterWeight; }
            set
            {
                if (value < 0)
                    Set(ref _steelUnderwaterWeight, 68);
                else
                    Set(ref _steelUnderwaterWeight, value);
            }
        }

        //****************** Input parameters | Soil Layer Parameters **********************//
        private ObservableCollection<PDIWT_BearingCapacity_SoilLayerInfo> _pileSoilLayersInfo;
        /// <summary>
        /// Property Description
        /// </summary>
        public ObservableCollection<PDIWT_BearingCapacity_SoilLayerInfo> PileSoilLayersInfo
        {
            get { return _pileSoilLayersInfo; }
            set
            {
                Set(ref _pileSoilLayersInfo, value);

                foreach (var _pileSoil in _pileSoilLayersInfo)
                    _pileSoil.PropertyChanged += (s, e) => IsCalculated = false;
            }
        }

        //****************** calculated parameters **********************//
        private double _pilePerimeter;
        /// <summary>
        /// unit: m
        /// </summary>
        public double PilePerimeter
        {
            get { return _pilePerimeter; }
            private set { Set(ref _pilePerimeter, value); }
        }

        private double _pileCrossSectionArea;
        /// <summary>
        /// unit: m2
        /// </summary>
        public double PileCrossSectionArea
        {
            get { return _pileCrossSectionArea; }
            private set { Set(ref _pileCrossSectionArea, value); }
        }

        private double _pileBottomElevation;
        /// <summary>
        /// unit: m
        /// </summary>
        public double PileBottomElevation
        {
            get { return _pileBottomElevation; }
            private set { Set(ref _pileBottomElevation, value); }
        }

        private double _pileSelfWeight;
        /// <summary>
        /// unit: kN
        /// </summary>
        public double PileSelfWeight
        {
            get { return _pileSelfWeight; }
            private set { Set(ref _pileSelfWeight, value); }
        }

        //****************** results parameters **********************//
        private double _designAxialBearingCapacity;
        /// <summary>
        /// unit: kN
        /// </summary>
        public double DesignAxialBearingCapacity
        {
            get { return _designAxialBearingCapacity; }
            private set { Set(ref _designAxialBearingCapacity, value); }
        }

        private double _designAxialUpliftCapacity;
        /// <summary>
        /// unit: kN
        /// </summary>
        public double DesignAxialUpliftCapacity
        {
            get { return _designAxialUpliftCapacity; }
            private set { Set(ref _designAxialUpliftCapacity, value); }
        }

        ////** other parameters **//
        //public BDE.CellHeaderElement PileCell { get; private set; }

        /// <summary>
        /// Calculate Bearing Capacity
        /// </summary>
        public void CalculatedBearingCapacity()
        {
            try
            {
                PilePerimeter = AxialBearingCapacity.CalculatePilePrimeter(SelectedPileGeoType, PileDiameter);
                PileCrossSectionArea = AxialBearingCapacity.CalculatePileEndOutsideArea(SelectedPileGeoType, PileDiameter);
                PileBottomElevation = AxialBearingCapacity.CalculatePileBottomElevation(PileTopElevation, PileLength, PileSkewness);
                // Pile Geo Type has been taken into consideration in following method.

                PileSelfWeight = AxialBearingCapacity.CalculatePileSelfWeight(
                    SelectedPileGeoType, PileDiameter, PileInsideDiameter, PileTopElevation,
                    PileLength, PileSkewness, CalculatedWaterLevel,
                    ConcreteWeight, ConcreteUnderwaterWeight, SteelWeight, SteelUnderwaterWeight, ConcreteCoreLength);
                // Calculate Axial Bearing Capacity
                List<double> _betasi = (from _soilInfo in PileSoilLayersInfo select _soilInfo.Betasi).ToList();
                List<double> _psii = (from _soilInfo in PileSoilLayersInfo select _soilInfo.Psii).ToList();
                List<double> _qfi = (from _soilInfo in PileSoilLayersInfo select _soilInfo.SideFrictionStandardValue).ToList();
                List<double> _li = (from _soilInfo in PileSoilLayersInfo select _soilInfo.SoilLayerThickness).ToList();
                //last layer is the holding layer
                double _betap = (from _soilInfo in PileSoilLayersInfo select _soilInfo.Betap).Last();
                double _psip = (from _soilInfo in PileSoilLayersInfo select _soilInfo.Psip).Last();
                double _qr = (from _soilInfo in PileSoilLayersInfo select _soilInfo.EndResistanceStandardValue).Last();
                double _axialBearingCapacity = 0;
                switch (SelectedBearingCapacityPileType)
                {
                    case BearingCapacityPileTypes.DrivenPileWithSealedEnd:
                        _axialBearingCapacity = AxialBearingCapacity.CalculateDrivenPileBearingCapacity(PartialCoeff, _qfi, _li, PilePerimeter, _qr, PileCrossSectionArea);
                        break;
                    case BearingCapacityPileTypes.TubePileOrSteelPile:
                        _axialBearingCapacity = AxialBearingCapacity.CalculateDrivenPileBearingCapacity(PartialCoeff, _qfi, _li, PilePerimeter, _qr, PileCrossSectionArea, BlockCoeff);
                        break;
                    case BearingCapacityPileTypes.CastInSituPile:
                        _axialBearingCapacity = AxialBearingCapacity.CalculateCasInSituPileBearingCapacity(PartialCoeff, _qfi, _li, _psii, PilePerimeter, _qr, PileCrossSectionArea, _psip);
                        break;
                    case BearingCapacityPileTypes.CastInSituAfterGrountingPile:
                        _axialBearingCapacity = AxialBearingCapacity.CalculateCastInSituAfterGrountingPileBearingCapacity(PartialCoeff, _qfi, _li, _psii, _betasi, PilePerimeter, _qr, PileCrossSectionArea, _psip, _betap);
                        break;
                    default:
                        break;
                }
                DesignAxialBearingCapacity = _axialBearingCapacity;
                // Calculate Axial uplift capacity
                List<double> _discountsi = (from _soilInfo in PileSoilLayersInfo select _soilInfo.DiscountCoeff).ToList();
                DesignAxialUpliftCapacity = AxialBearingCapacity.CalculateDrivenAndCastInSituPileUpliftForce(PartialCoeff, _qfi, _li, _discountsi,
                    PilePerimeter, PileSelfWeight, AxialBearingCapacity.CalculateCosAlpha(PileSkewness));
                IsCalculated = true;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Can't calculate bearing capacity",e);
            }
        }

        //public BD.StatusInt ObtainPileFromCellElementAndActiveModel(BDE.CellHeaderElement pileCell)
        //{
        //    var _activeModel = BM.Session.Instance.GetActiveDgnModel();
        //    double _uorpermeter = _activeModel.GetModelInfo().UorPerMeter;
        //    var _mc = BM.MessageCenter.Instance;

        //    if (pileCell == null)
        //        return BD.StatusInt.Error;

        //    BDE.LineElement _axisLine = pileCell.GetChildren().Where(_line => _line is BDE.LineElement).First() as BDE.LineElement;
        //    BG.DPoint3d _lineStartPoint, _lineEndPoint;
        //    if (!_axisLine.AsCurvePathEdit().GetCurveVector().GetStartEnd(out _lineStartPoint, out _lineEndPoint))
        //        return BD.StatusInt.Error;

        //    //Swap start and end point if necessary, start point => top point, end point => bottom point;
        //    if (_lineEndPoint.Z > _lineStartPoint.Z)
        //    {
        //        BG.DPoint3d _temp = _lineEndPoint;
        //        _lineEndPoint = _lineStartPoint;
        //        _lineStartPoint = _temp;
        //    }
        //    BG.DRay3d _axisRay = new BG.DRay3d(_lineStartPoint, _lineEndPoint);

        //    //Pile Material and Pile Self Related Parameters
        //    string _environmentECSchemaName = "PDIWT";
        //    string _materialECClassName = "PileMaterialSettings";
        //    Dictionary<string, object> _materialProps;
        //    List<string> _requireMaterialPropNameList = new List<string>()
        //    {
        //        "ConcreteUnitWeight",
        //        "ConcreteUnderWaterUnitWeight",
        //        "SteelUnitWeight",
        //        "WaterUnitWeight"
        //    };
        //    if (BD.StatusInt.Error == ECSChemaReader.ReadECInstanceProperties(_environmentECSchemaName, _materialECClassName, _requireMaterialPropNameList, _activeModel, out _materialProps))
        //        return BD.StatusInt.Error;
        //    ConcreteWeight = double.Parse(_materialProps["ConcreteUnitWeight"].ToString());
        //    ConcreteUnderwaterWeight = double.Parse(_materialProps["ConcreteUnderWaterUnitWeight"].ToString());
        //    SteelWeight = double.Parse(_materialProps["SteelUnitWeight"].ToString());
        //    SteelUnderwaterWeight = double.Parse(_materialProps["SteelUnitWeight"].ToString()) - double.Parse(_materialProps["WaterUnitWeight"].ToString());

        //    string _ifcECSchemaName = "IfcPort";
        //    string _ifcPileECClassName = "IfcPile";
        //    Dictionary<string, object> _pileProps;
        //    List<string> _requirePilePropNameList = new List<string>()
        //    {
        //        "Code",
        //        "Type",
        //        "CrossSectionWidth",
        //        "OutsideDiameter",
        //        "InnerDiameter",
        //        "Length"
        //    };
        //    if (BD.StatusInt.Error == ECSChemaReader.ReadECInstanceProperties(_ifcECSchemaName, _ifcPileECClassName, _requirePilePropNameList, pileCell, out _pileProps))
        //        return BD.StatusInt.Error;
        //    SelectedPileGeoType = PDIWT.Resources.PDIWT_Helper.GetEnumDescriptionDictionary<PDIWT_PiledWharf_Core_Cpp.PileTypeManaged>()
        //                                                      .Where(e => e.Value == _pileProps["Type"].ToString())
        //                                                      .First().Key;
        //    switch (SelectedPileGeoType)
        //    {
        //        case PDIWT_PiledWharf_Core_Cpp.PileTypeManaged.SqaurePile:
        //            PileDiameter = double.Parse(_pileProps["CrossSectionWidth"].ToString()) / 1000;
        //            break;
        //        case PDIWT_PiledWharf_Core_Cpp.PileTypeManaged.TubePile:
        //            PileDiameter = double.Parse(_pileProps["OutsideDiameter"].ToString()) / 1000;
        //            break;
        //        case PDIWT_PiledWharf_Core_Cpp.PileTypeManaged.PHCTubePile:
        //            PileDiameter = double.Parse(_pileProps["OutsideDiameter"].ToString()) / 1000;
        //            PileInsideDiameter = double.Parse(_pileProps["InnerDiameter"].ToString()) / 1000;
        //            break;
        //        case PDIWT_PiledWharf_Core_Cpp.PileTypeManaged.SteelTubePile:
        //            PileDiameter = double.Parse(_pileProps["OutsideDiameter"].ToString()) / 1000;
        //            PileInsideDiameter = double.Parse(_pileProps["InnerDiameter"].ToString()) / 1000;
        //            break;
        //        default:
        //            break;
        //    }

        //    PileLength = double.Parse(_pileProps["Length"].ToString()) / 1000;

        //    //PileCode = _pileProps["Code"].ToString();

        //    BG.DVector3d _pileAxisVector = new BG.DVector3d(_lineStartPoint, _lineEndPoint);
        //    if (_pileAxisVector.IsParallelOrOppositeTo(BG.DVector3d.UnitZ))
        //    {
        //        IsVerticalPile = true;
        //        PileSkewness = double.NaN;
        //    }
        //    else
        //    {
        //        IsVerticalPile = false;
        //        PileSkewness = Math.Abs(_pileAxisVector.Z / Math.Sqrt(_pileAxisVector.X * _pileAxisVector.X + _pileAxisVector.Y * _pileAxisVector.Y));
        //    }
        //    PileTopElevation = _lineStartPoint.Z / _uorpermeter;

        //    PileCell = pileCell;
        //    return ObtainSoilLayersInfoFromAtiveModel();
        //}

        /// <summary>
        /// Get Soil layers which insect with this pile
        /// </summary>
        /// <returns>success when has intersection, otherwise error </returns>
        //private BD.StatusInt ObtainSoilLayersInfoFromAtiveModel()
        //{
        //    var _activeModel = BM.Session.Instance.GetActiveDgnModel();

        //    BD.ScanCriteria _sc = new BD.ScanCriteria();
        //    _sc.SetModelRef(_activeModel);
        //    _sc.SetModelSections(BD.DgnModelSections.GraphicElements);
        //    BD.BitMask _meshBitMask = new BD.BitMask(false);
        //    _meshBitMask.Capacity = 400;
        //    _meshBitMask.ClearAll();
        //    _meshBitMask.SetBit(104, true);
        //    _sc.SetElementTypeTest(_meshBitMask);
        //    _sc.Scan((_element, _model) =>
        //    {
        //        var _layerInfo = new PDIWT_BearingCapacity_SoilLayerInfo();
        //        BDE.MeshHeaderElement _mesh = (BDE.MeshHeaderElement)_element;
        //        if(_layerInfo.ObtainInfoFromMesh(_mesh) == BD.StatusInt.Success)
        //        {
        //            PDIWT_BearingCapacity_PileSoilIntersectionInfo _psIntersectionInfo =
        //                new PDIWT_BearingCapacity_PileSoilIntersectionInfo(this, _layerInfo);
        //            InterSectionInfo _isInfo = _psIntersectionInfo.GetInterSectionInfo();
        //            if(true ==_isInfo.IsIntersection )
        //            {
        //                _layerInfo.SoilLayerThickness = _psIntersectionInfo.GetPileLengthInSoilLayer();
        //                PileSoilLayersInfo.Add(_layerInfo);
        //            }
        //        }
        //        return BD.StatusInt.Success;
        //    });
        //    if (PileSoilLayersInfo.Count == 0)
        //        return BD.StatusInt.Error;
        //    else
        //        return BD.StatusInt.Success;
        //}

        /// <summary>
        /// get pile ray from top to bottom, must invoke ObtainInfoFromCellElement first
        /// </summary>
        /// <returns>DRay3d in UOR</returns>
        //public BG.DRay3d GetPileRay3D()
        //{
        //    double _uorpermm = BM.Session.Instance.GetActiveDgnModel().GetModelInfo().UorPerMeter / 1000;        
        //    if(PileCell == null)
        //    {
        //        BG.DVector3d _unitZ = new BG.DVector3d(0, 0, -1);
        //        _unitZ.ScaleInPlace(PileLength * _uorpermm * 1000);
        //        if(!double.IsNaN(PileSkewness))
        //        {
        //            BG.Angle _yAngel = new BG.Angle();
        //            _yAngel.Radians =- Math.Atan(1 / PileSkewness);
        //            BG.DTransform3d _yTrans = BG.DTransform3d.Rotation(1, _yAngel);
        //            _yTrans.MultiplyInPlace(ref _unitZ);
        //        }
        //        return new BG.DRay3d(new BG.DPoint3d(0, 0, PileTopElevation * _uorpermm * 1000), _unitZ);
        //    }
        //    else
        //    {
        //        BDE.LineElement _axisLine = PileCell.GetChildren().Where(_line => _line is BDE.LineElement).First() as BDE.LineElement;
        //        BG.DPoint3d _lineStartPoint, _lineEndPoint;
        //        _axisLine.AsCurvePathEdit().GetCurveVector().GetStartEnd(out _lineStartPoint, out _lineEndPoint);
        //        //Swap start and end point if necessary, start point => top point, end point => bottom point;
        //        if (_lineEndPoint.Z > _lineStartPoint.Z)
        //        {
        //            BG.DPoint3d _temp = _lineEndPoint;
        //            _lineEndPoint = _lineStartPoint;
        //            _lineStartPoint = _temp;
        //        }
        //        return new BG.DRay3d(_lineStartPoint, _lineEndPoint);
        //    }
        //}
    }
}
