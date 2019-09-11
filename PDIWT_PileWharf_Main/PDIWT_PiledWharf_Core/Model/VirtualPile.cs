using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDIWT.Resources.Localization.MainModule;

using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight;

using PDIWT_PiledWharf_Core.ViewModel;

using BD = Bentley.DgnPlatformNET;
using BM = Bentley.MstnPlatformNET;
using BDEC = Bentley.DgnPlatformNET.DgnEC;
using BES = Bentley.ECObjects.Schema;
using BEI = Bentley.ECObjects.Instance;
using BDE = Bentley.DgnPlatformNET.Elements;
using BMW = Bentley.MstnPlatformNET.WPF;
using BG = Bentley.GeometryNET;
using System.Collections.ObjectModel;
using PDIWT.Formulas;
using MathNet.Numerics;

namespace PDIWT_PiledWharf_Core.Model
{
    public class VirtualSteelOrTubePile : ObservableObject, IPile
    {
        public readonly static string _unknownPileName = "UnKnown";

        public VirtualSteelOrTubePile()
        {
            _pileName = _unknownPileName;
            _pileX = _pileY = _pileZ = 0;
            _pileSkewness = double.NaN;
            _planRotationAngle = 0;
            _pileDiameter = 1;
            _pileInnerDiameter = 0.8;

        }

        /********** Input Parameters **********/
        private string _pileName;
        /// <summary>
        /// Property Description
        /// </summary>
        public string PileName
        {
            get { return _pileName; }
            set { Set(ref _pileName, value); }
        }

        private double _pileX;
        /// <summary>
        /// Unit: uor
        /// </summary>
        public double PileX
        {
            get { return _pileX; }
            set { Set(ref _pileX, value); }
        }

        private double _pileY;
        /// <summary>
        /// Unit: uor
        /// </summary>
        public double PileY
        {
            get { return _pileY; }
            set { Set(ref _pileY, value); }
        }

        private double _pileZ;
        /// <summary>
        /// Unit: uor
        /// </summary>
        public double PileZ
        {
            get { return _pileZ; }
            set { Set(ref _pileZ, value); }
        }

        private double _pileSkewness;
        /// <summary>
        /// pile skewness
        /// </summary>
        public double PileSkewness
        {
            get { return _pileSkewness; }
            set { Set(ref _pileSkewness, value); }
        }

        private double _planRotationAngle;
        /// <summary>
        /// Degree
        /// </summary>
        public double PlanRotationAngle
        {
            get { return _planRotationAngle; }
            set { Set(ref _planRotationAngle, value); }
        }


        private double _pileDiameter;
        /// <summary>
        /// Unit: uor
        /// </summary>
        public double PileDiameter
        {
            get { return _pileDiameter; }
            set
            {
                if (value < 0)
                    Set(ref _pileDiameter, 0);
                else
                    Set(ref _pileDiameter, value);
            }
        }

        private double _pileInnerDiameter;
        /// <summary>
        /// Unit: uor
        /// </summary>
        public double PileInnerDiameter
        {
            get { return _pileInnerDiameter; }
            set
            {
                if (value < 0)
                    Set(ref _pileInnerDiameter, 0);
                else
                    Set(ref _pileInnerDiameter, value);
            }
        }

        // ******************** results ************************//
        private double _pileLength;
        /// <summary>
        /// Unit: uor
        /// </summary>
        public double PileLength
        {
            get { return _pileLength; }
            private set { Set(ref _pileLength, value); }
        }

        public BG.DRay3d GetPileRay3D()
        {
            //set vector(0,0,1)
            BG.DVector3d _unitZ = BG.DVector3d.UnitZ;
            _unitZ.NegateInPlace();

            //set rotation matrix
            BG.Angle _yAngle = new BG.Angle();
            if (double.IsNaN(PileSkewness))
                _yAngle.Radians = 0;
            else
                _yAngle.Radians = - Math.Atan(1 / PileSkewness);
            BG.Angle _zAngle = new BG.Angle
            {
                Degrees = PlanRotationAngle
            };
            BG.DTransform3d _ytrans = BG.DTransform3d.Rotation(1, _yAngle);
            BG.DTransform3d _ztrans = BG.DTransform3d.Rotation(2, _zAngle);
            var _wholetrans = BG.DTransform3d.Multiply(_ztrans,_ytrans);
            _wholetrans.MultiplyInPlace(ref _unitZ);

            //double _uorpermm = BM.Session.Instance.GetActiveDgnModel().GetModelInfo().UorPerMeter / 1000;
            return new BG.DRay3d(new BG.DPoint3d(PileX , PileY , PileZ ), _unitZ);
        }

        /// <summary>
        /// Get the curve of pile length against bearing capacity
        /// </summary>
        /// <param name="partialCoeff">partial item coefficient</param>
        /// <param name="blockCoeff">block coefficient</param>
        /// <param name="results">The tuple (double, double, double, double) represents (fractionTop, bcTop, fractionBottom, bcBottom)</param>
        /// <returns>Status</returns>
        public GetPileBearingCapacityCurveInfoStatus GetPileBearingCapacityCurveInfo(
            double partialCoeff,
            double blockCoeff,
            out ObservableCollection<Tuple<double, double>> results // Tuple(double, double) represents (fraction, bc) 
            )
        {
            //if (_meshInfoTuples == null || _meshInfoTuples.Count == 0)
            //    return BD.StatusInt.Error;

            //BG.DRay3d _axisRay = GetPileRay3D();
            //List<Tuple<double, double, PDIWT_BearingCapacity_SoilLayerInfo>> _layerTuples = new List<Tuple<double, double, PDIWT_BearingCapacity_SoilLayerInfo>>();
            //foreach (var _meshInfo in _meshInfoTuples)
            //{
            //    BG.DPoint3d _facetPoint;
            //    double _fractionInAxis;
            //    BG.PolyfaceVisitor _polyfaceVisitor = BG.PolyfaceVisitor.Attach(_meshInfo.Item1.AsMeshEdit().GetMeshData(), true);
            //    List<double> _insertFractions = new List<double>();
            //    //todo Sometimes it will count more than two points, ask the problem on the forum
            //    while (_polyfaceVisitor.AdvanceToFacetBySearchRay(_axisRay, 0, out _facetPoint, out _fractionInAxis))
            //    {
            //        _insertFractions.Add(_fractionInAxis);
            //    }
            //    _insertFractions = _insertFractions.Distinct().ToList();

            //    BG.DPoint3d _topPoint, _bottomPoint;
            //    double _topFraction, _bottomFraction;
            //    if (_insertFractions.Count == 0 || _insertFractions.Count == 1)
            //        continue;
            //    //else if (_insertFractions.Count == 2)
            //    //{                   

            //    //    if (_insertFractions[0] >= _insertFractions[1])
            //    //    {
            //    //        _topFraction = _insertFractions[1];
            //    //        _bottomFraction = _insertFractions[0];
            //    //    }
            //    //    else
            //    //    {
            //    //        _topFraction = _insertFractions[0];
            //    //        _bottomFraction = _insertFractions[1];
            //    //    }

            //    //}
            //    else
            //    {
            //        if (_insertFractions.Count > 2)
            //            _insertFractions = _insertFractions.GetRange(0, 2);
            //        _topFraction = _insertFractions.Min();
            //        _bottomFraction = _insertFractions.Max();
            //    }

            //    _topPoint = _axisRay.PointAtFraction(_topFraction);
            //    _bottomPoint = _axisRay.PointAtFraction(_bottomFraction);

            //    var _soilLayerInfo = new PDIWT_BearingCapacity_SoilLayerInfo();
            //    _soilLayerInfo.SoilLayerTopElevation = _topPoint.Z;
            //    _soilLayerInfo.SoilLayerBottomElevation = _bottomPoint.Z;
            //    _soilLayerInfo.SoilLayerNumber = _meshInfo.Item2["LayerNumber"].ToString();
            //    _soilLayerInfo.SoilLayerName = _meshInfo.Item2["LayerName"].ToString();
            //    _soilLayerInfo.Betasi = double.Parse(_meshInfo.Item2["Betasi"].ToString());
            //    _soilLayerInfo.Psii = double.Parse(_meshInfo.Item2["Psisi"].ToString());
            //    _soilLayerInfo.SideFrictionStandardValue = double.Parse(_meshInfo.Item2["qfi"].ToString());
            //    _soilLayerInfo.Betap = double.Parse(_meshInfo.Item2["Betap"].ToString());
            //    _soilLayerInfo.Psip = double.Parse(_meshInfo.Item2["Psip"].ToString());
            //    _soilLayerInfo.EndResistanceStandardValue = double.Parse(_meshInfo.Item2["qr"].ToString());
            //    _soilLayerInfo.SoilLayerThickness = Math.Abs(_bottomFraction-_topFraction) / _uorpermeter;
            //    _layerTuples.Add(Tuple.Create(_topFraction, _bottomFraction, _soilLayerInfo));
            //}

            //if (_layerTuples.Count == 0)
            //    return BD.StatusInt.Error;

            results = new ObservableCollection<Tuple<double, double>>();
            // Tuple(double, double, double, double) represents (fractionTop, bcTop, fractionBottom, bcBottom) 
            ObservableCollection<Tuple<double, double, double, double>> _tuples = new ObservableCollection<Tuple<double, double, double, double>>();


            double _uorpermeter = BM.Session.Instance.GetActiveDgnModel().GetModelInfo().UorPerMeter;

            SoilLayerCollection _soilLayers = SoilLayerCollection.ObtainFromModel(BM.Session.Instance.GetActiveDgnModel());
            PileSoilLayersIntersectionGetter _getter = new PileSoilLayersIntersectionGetter(this, _soilLayers);

            PileSoilLayersInsectionStatus _insectionStatus = _getter.GetInterSectionInfo(out ObservableCollection<IntersectionInfo> _insectInfos, false);
            switch (_insectionStatus)
            {
                case PileSoilLayersInsectionStatus.NoSoilLayer:                   
                case PileSoilLayersInsectionStatus.NotAllSoilLayersContainMeshElement:
                    return GetPileBearingCapacityCurveInfoStatus.InvalidObjectStruct;
                case PileSoilLayersInsectionStatus.NoIntersection:
                    return GetPileBearingCapacityCurveInfoStatus.NoIntersection;
                case PileSoilLayersInsectionStatus.Success:
                    break;
            }
            //if (_getter.GetInterSectionInfo(out ObservableCollection<IntersectionInfo> _insectInfos, false) != PileSoilLayersInsectionStatus.Success)
            //    return BD.StatusInt.Error;

            double _pilePerimeter = AxialBearingCapacity.CalculatePilePrimeter(PDIWT_PiledWharf_Core_Cpp.PileTypeManaged.TubePile, PileDiameter / _uorpermeter);
            double _pileArea = AxialBearingCapacity.CalculatePileEndOutsideArea(PDIWT_PiledWharf_Core_Cpp.PileTypeManaged.TubePile, PileDiameter / _uorpermeter);

            List<Tuple<double, double>> _sideAndendBCPart = new List<Tuple<double, double>>();
            double _sideBCPartEachLayer = 0;
            foreach (var _layer in _insectInfos)
            {
                _sideBCPartEachLayer += 1 / partialCoeff * _pilePerimeter * _layer.SoilLayer.TPSP_Qfi.GetValueOrDefault() * _layer.GetPileLengthInSoilLayer() / _uorpermeter;
                double _endBCPartEacnLayer = 1 / partialCoeff * blockCoeff * _layer.SoilLayer.TPSP_Qr.GetValueOrDefault() * _pileArea;
                _sideAndendBCPart.Add(Tuple.Create(_sideBCPartEachLayer, _endBCPartEacnLayer));
            }

            for (int i = 0; i < _sideAndendBCPart.Count; i++)
            {
                if (i == 0)
                {
                    results.Add(Tuple.Create(_insectInfos[i].TopFraction, _sideAndendBCPart[i].Item2));
                    results.Add(Tuple.Create(_insectInfos[i].BottomFraction, _sideAndendBCPart[i].Item1 + _sideAndendBCPart[i].Item2));
                }
                else
                {
                    results.Add(Tuple.Create(_insectInfos[i].TopFraction, _sideAndendBCPart[i - 1].Item1 + _sideAndendBCPart[i].Item2));
                    results.Add(Tuple.Create(_insectInfos[i].BottomFraction, _sideAndendBCPart[i].Item1 + _sideAndendBCPart[i].Item2));
                }
            }

            //results = new ObservableCollection<Tuple<double, double>>(results.Distinct(new BCTupleComparer()));
            for (int i = 0; i < results.Count; i++)
            {
                for (int j = results.Count - 1; j > i; j--)
                {
                    if(Math.Abs(results[i].Item1 - results[j].Item1) < _uorpermeter * 0.01 &&
                        Math.Abs(results[i].Item2 - results[j].Item2) < 0.01)
                    {
                        results.RemoveAt(j);
                    }
                }
            }
            return GetPileBearingCapacityCurveInfoStatus.Success;
        }

        /// <summary>
        /// Calculate the pile Length Base on target BC and length modulus. Unit: uor.
        /// </summary>
        /// <param name="targetBC">Target bearing capacity</param>
        /// <param name="pileLengthModulus">Unit: uor. Length modulus, if less than 0, i will not resize the pile length. </param>
        /// <param name="fractionandBCInfos">The tuple (double, double) represents (fraction, bc) in ascending order.</param>
        /// <returns>status</returns>
        public CalculatePileLengthStatues CalculatePileLength(
            double targetBC,
            double pileLengthModulus,
            ObservableCollection<Tuple<double, double>> fractionandBCInfos)
        {
            
            if (fractionandBCInfos == null || fractionandBCInfos.Count == 0)
                return CalculatePileLengthStatues.NoLayerInfos;
            if (targetBC > fractionandBCInfos.Last().Item2)
                return CalculatePileLengthStatues.TargetBearingCapacityIsTooLarge;

            //BG.DRay3d _axisRay = GetPileRay3D();
            ////!Have problems
            ////todo to fix gap between layers

            //var _bclist = from _bc in fractionandBCInfos select _bc.Item2;

            //foreach (var _layer in fractionandBCInfos)
            //{
            //    if((_layer.Item2- targetBC) * (_layer.Item4 - targetBC) <=0)
            //    {

            //    }
            //}

            var _interpolate = Interpolate.Linear(fractionandBCInfos.Select(_tuple => _tuple.Item2), fractionandBCInfos.Select(_tuple => _tuple.Item1));
            double _franction = _interpolate.Interpolate(targetBC);

            PileLength = _franction;
            if (pileLengthModulus > 0)
            {
                double _times = Math.Truncate(PileLength / pileLengthModulus);
                PileLength = pileLengthModulus * (_times + 1);
            }
            return CalculatePileLengthStatues.Success;
        }
        

        public void DrawInActiveModel()
        {
            var _activeModel = BM.Session.Instance.GetActiveDgnModel();
            if (false == _activeModel.Is3d)
                throw new InvalidProgramException("The active model is not 3D");

            BG.DRay3d _pileRay = GetPileRay3D();
            BG.DPoint3d _topPoint = _pileRay.Origin;
            BG.DPoint3d _bottomPoint = _pileRay.PointAtFraction(_pileLength * 10000);
            BDE.LineElement _axis = new BDE.LineElement(_activeModel, null, new BG.DSegment3d(_topPoint, _bottomPoint));
            _axis.AddToModel();
        }
    }

    //class BCTupleComparer : IEqualityComparer<Tuple<double, double>>
    //{
    //    readonly double _uorpermeter = BM.Session.Instance.GetActiveDgnModel().GetModelInfo().UorPerMeter;
    //    public bool Equals(Tuple<double, double> x, Tuple<double, double> y)
    //    {
    //        if (ReferenceEquals(x, y)) return true;
    //        if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
    //            return false;
    //        return (Math.Abs(x.Item1 - y.Item1) < 0.01) && (Math.Abs(x.Item2 - y.Item2) < 0.01);
    //    }

    //    public int GetHashCode(Tuple<double, double> obj)
    //    {
    //        if (ReferenceEquals(obj, null)) return 0;

    //        return obj.GetHashCode();
    //    }
    //}

}
