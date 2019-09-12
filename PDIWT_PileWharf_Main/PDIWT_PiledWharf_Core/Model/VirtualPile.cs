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
using PDIWT_PiledWharf_Core_Cpp;

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


        private BearingCapacityPileTypes _BCPileType;
        /// <summary>
        /// Property Description
        /// </summary>
        public BearingCapacityPileTypes BCPileType
        {
            get { return _BCPileType; }
            set { Set(ref _BCPileType, value); }
        }

        private PileTypeManaged _geoPileType;
        /// <summary>
        /// Property Description
        /// </summary>
        public PileTypeManaged GeoPileType
        {
            get { return _geoPileType; }
            set { Set(ref _geoPileType, value); }
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
                _yAngle.Radians = -Math.Atan(1 / PileSkewness);
            BG.Angle _zAngle = new BG.Angle
            {
                Degrees = PlanRotationAngle
            };
            BG.DTransform3d _ytrans = BG.DTransform3d.Rotation(1, _yAngle);
            BG.DTransform3d _ztrans = BG.DTransform3d.Rotation(2, _zAngle);
            var _wholetrans = BG.DTransform3d.Multiply(_ztrans, _ytrans);
            _wholetrans.MultiplyInPlace(ref _unitZ);

            //double _uorpermm = BM.Session.Instance.GetActiveDgnModel().GetModelInfo().UorPerMeter / 1000;
            return new BG.DRay3d(new BG.DPoint3d(PileX, PileY, PileZ), _unitZ);
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

            double _pilePerimeter = AxialBearingCapacity.CalculatePilePrimeter(GeoPileType, PileDiameter / _uorpermeter);
            double _pileArea = AxialBearingCapacity.CalculatePileEndOutsideArea(GeoPileType, PileDiameter / _uorpermeter);

            List<Tuple<double, double>> _sideAndendBCPart = new List<Tuple<double, double>>();

            double _sideBCPartEachLayer = 0;
            double _endBCPartEacnLayer = 0;
            foreach (var _layer in _insectInfos)
            {
                switch (BCPileType)
                {
                    case BearingCapacityPileTypes.DrivenPileWithSealedEnd:
                        _sideBCPartEachLayer += 1 / partialCoeff * _pilePerimeter * _layer.SoilLayer.DPSE_Qfi.Value * _layer.GetPileLengthInSoilLayer() / _uorpermeter;
                        _endBCPartEacnLayer = 1 / partialCoeff * _layer.SoilLayer.DPSE_Qr.Value * _pileArea;
                        _sideAndendBCPart.Add(Tuple.Create(_sideBCPartEachLayer, _endBCPartEacnLayer));
                        break;
                    case BearingCapacityPileTypes.TubePileOrSteelPile:
                        _sideBCPartEachLayer += 1 / partialCoeff * _pilePerimeter * _layer.SoilLayer.TPSP_Qfi.Value * _layer.GetPileLengthInSoilLayer() / _uorpermeter;
                        _endBCPartEacnLayer = 1 / partialCoeff * blockCoeff * _layer.SoilLayer.TPSP_Qr.Value * _pileArea;
                        _sideAndendBCPart.Add(Tuple.Create(_sideBCPartEachLayer, _endBCPartEacnLayer));
                        break;
                    case BearingCapacityPileTypes.CastInSituPile:
                        _sideBCPartEachLayer += 1 / partialCoeff * _pilePerimeter * _layer.SoilLayer.CISP_Psisi.Value * _layer.SoilLayer.CISP_Qfi.Value * _layer.GetPileLengthInSoilLayer() / _uorpermeter;
                        _endBCPartEacnLayer = 1 / partialCoeff *  _layer.SoilLayer.CISP_Psip.Value * _layer.SoilLayer.CISP_Qr.Value * _pileArea;
                        _sideAndendBCPart.Add(Tuple.Create(_sideBCPartEachLayer, _endBCPartEacnLayer));
                        break;
                    case BearingCapacityPileTypes.CastInSituAfterGrountingPile:
                        _sideBCPartEachLayer += 1 / partialCoeff * _pilePerimeter * _layer.SoilLayer.CISAGP_Betasi.Value * _layer.SoilLayer.CISAGP_Psisi.Value * _layer.SoilLayer.CISAGP_Qfi.Value * _layer.GetPileLengthInSoilLayer() / _uorpermeter;
                        _endBCPartEacnLayer = 1 / partialCoeff * _layer.SoilLayer.CISAGP_Betap.Value * _layer.SoilLayer.CISAGP_Psip.Value * _layer.SoilLayer.CISAGP_Qr.Value * _pileArea;
                        _sideAndendBCPart.Add(Tuple.Create(_sideBCPartEachLayer, _endBCPartEacnLayer));
                        break;
                    default:
                        break;
                }

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
                    if (Math.Abs(results[i].Item1 - results[j].Item1) < _uorpermeter * 0.01 &&
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
            //if (targetBC > fractionandBCInfos.Last().Item2)
            //    return CalculatePileLengthStatues.TargetBearingCapacityIsTooLarge;

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
}
