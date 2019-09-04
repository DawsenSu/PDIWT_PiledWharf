using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GalaSoft.MvvmLight;

using System.ComponentModel;
using System.Collections.ObjectModel;

using BD = Bentley.DgnPlatformNET;
using BG = Bentley.GeometryNET;
using BM = Bentley.MstnPlatformNET;
using BDE = Bentley.DgnPlatformNET.Elements;
using PDIWT.Formulas;
using PDIWT_PiledWharf_Core.Model.Tools;
using PDIWT_PiledWharf_Core_Cpp;

namespace PDIWT_PiledWharf_Core.Model
{
    /// <summary>
    /// Pile class represent pileAxial in piledwharf system. 
    /// </summary>
    public class PileBase : ObservableObject, IPile
    {
        public readonly static string _unknownPileName = "UnKnown";
        public readonly static long _unknowPileNumbering = -1;

        /// <summary>
        /// Construct Pile from given joint list and pile top and bottom numberings in the jont list.
        /// </summary>
        /// <param name="topJointNumber">pile top nubmering in the joints list.</param>
        /// <param name="bottomJointNumber">pile bottom numbering in the joints list.</param>
        public PileBase(long number, long topJointNumber, long bottomJointNumber, List<Joint> joints)
        {
            _numbering = number;
            _name = _unknownPileName;
            _topJoint = joints.Find(joint => joint.Numbering == topJointNumber);
            _bottomJoint = joints.Find(joint => joint.Numbering == bottomJointNumber);
            _geoType = PileTypeManaged.SqaurePile;
            _diameter = 0;
            _innerDiameter = 0;
            _pileTipType = PileTipType.TotalSeal;
        }

        /// <summary>
        /// Construct Pile  from given joint list and top and bottom point objects.
        /// </summary>
        public PileBase(long number, BG.DPoint3d topPoint, BG.DPoint3d bottomPoint, List<Joint> joints)
        {
            _numbering = number;
            _name = _unknownPileName;
            _topJoint = joints.Find(joint => joint.Point == topPoint);
            _bottomJoint = joints.Find(joint => joint.Point == bottomPoint);
            _geoType = PileTypeManaged.SqaurePile;
            _diameter = 0;
            _innerDiameter = 0;
            _pileTipType = PileTipType.TotalSeal;
        }

        //public PileBase(long number, Joint topJoint, Joint bottomJoint, PileTypeManaged pileGeoType, double diameter, double innerDiameter, double concreteCoreLength = 0)
        //{
        //    _numbering = number;
        //    _topJoint = topJoint;
        //    _bottomJoint = bottomJoint;
        //    _geoType = pileGeoType;
        //    _diameter = diameter;
        //    _innerDiameter = innerDiameter;
        //    _concreteCoreLength = concreteCoreLength;
        //}

        public PileBase() : this(_unknowPileNumbering, _unknownPileName, new Joint(), new Joint(), PileTypeManaged.SqaurePile, 0, 0, 0, PileTipType.TotalSeal) { }

        /// <summary>
        /// pile numbering is -1, Both joints have -1 numbering.
        /// </summary>
        public PileBase(long number,
            string name,
            Joint topJoint,
            Joint bottomJoint,
            PileTypeManaged pileGeoType,
            double diameter,
            double innerDiameter,
            double concreteCoreLength,
            PileTipType pileTipType)
        {
            _numbering = number;
            _name = name;
            _topJoint = topJoint;
            _bottomJoint = bottomJoint;
            _geoType = pileGeoType;
            if (diameter < 0)
                _diameter = 0;
            else
                _diameter = diameter;
            if (innerDiameter < 0)
                _innerDiameter = 0;
            else
                _innerDiameter = innerDiameter;
            if (concreteCoreLength < 0)
                _concreteCoreLength = 0;
            else
                _concreteCoreLength = concreteCoreLength;
            _pileTipType = pileTipType;
        }

        ///// <summary>
        ///// pile number is assigned by number. Both joints have -1 numbering.
        ///// </summary>
        //public PileBase(BG.DPoint3d topPoint, BG.DPoint3d bottomPoint, PileTypeManaged pileGeoType, double diameter, double innerDiameter, double concreteCoreLength = 0)
        //    : this(-1,"", new Joint(topPoint), new Joint(bottomPoint), pileGeoType, diameter, innerDiameter, concreteCoreLength,PileTipType.TotalSeal) { }

        ///// <summary>
        ///// Pile numbering is -1, as well as both joints
        ///// </summary>
        //public PileBase(BG.DPoint3d topPoint, BG.DPoint3d bottomPoint, PileTypeManaged pileGeoType, double diameter, double innerDiameter, double concreteCoreLength = 0)
        //    : this(-1, topPoint, bottomPoint, pileGeoType, diameter, innerDiameter, concreteCoreLength) { }

        ///// <summary>
        ///// Pile it's geotype is square, its diameter and innerdiameter are 0;
        ///// </summary>
        //public PileBase(long number, BG.DPoint3d topPoint, BG.DPoint3d bottomPoint)
        //    : this(number, topPoint, bottomPoint, PileTypeManaged.SqaurePile, 0, 0) { }



        //public PileBase(BG.DPoint3d topPoint, BG.DPoint3d bottomPoint, PileTypeManaged pileGeoType, double diameter)
        //    : this(topPoint, bottomPoint, pileGeoType, diameter, 0) { }

        //public PileBase(BG.DPoint3d topPoint, BG.DPoint3d bottomPoint)
        //    : this(topPoint, bottomPoint, PileTypeManaged.SqaurePile, 0) { }

      
        private long _numbering;
        /// <summary>
        /// positive numbering, -1 represent no particular assignment 
        /// </summary>
        public long Numbering
        {
            get { return _numbering; }
            set { Set(ref _numbering, value); }
        }

        private string _name;
        /// <summary>
        /// PileName
        /// </summary>
        public string Name
        {
            get { return _name ?? string.Empty; }
            set { Set(ref _name, value); }
        }

        private Joint _topJoint;
        /// <summary>
        /// unit: uor
        /// </summary>
        public Joint TopJoint
        {
            get { return _topJoint; }
            set { Set(ref _topJoint, value); }
        }

        private Joint _bottomJoint;
        /// <summary>
        /// unit: uor
        /// </summary>
        public Joint BottomJoint
        {
            get { return _bottomJoint; }
            set { Set(ref _bottomJoint, value); }
        }

        private double _diameter;
        /// <summary>
        /// unit: uor. For square pile, it's pile width
        /// </summary>
        public double Diameter
        {
            get { return _diameter; }
            set
            {
                double setValue = value;
                if (setValue < 0)
                    setValue = 0;
                Set(ref _diameter, setValue);
            }
        }

        private double _innerDiameter;
        /// <summary>
        /// unit: uor. Only for tube pile
        /// </summary>
        public double InnerDiameter
        {
            get { return _innerDiameter; }
            set
            {
                double setValue = value;
                if (setValue < 0)
                    setValue = 0;
                Set(ref _innerDiameter, setValue);
            }
        }

        private double _concreteCoreLength;
        /// <summary>
        /// Concrete core length, only for steel pile. Unit: uor
        /// </summary>
        public double ConcreteCoreLength
        {
            get { return _concreteCoreLength; }
            set
            {
                double setValue = value;
                if (setValue < 0)
                    setValue = 0;
                Set(ref _concreteCoreLength, value);
            }
        }

        private PileTypeManaged _geoType;
        /// <summary>
        /// pile geometry type
        /// </summary>
        public PileTypeManaged GeoType
        {
            get { return _geoType; }
            set { Set(ref _geoType, value); }
        }

        private PileTipType _pileTipType;
        /// <summary>
        /// pile tip type
        /// </summary>
        public PileTipType PileTipType
        {
            get { return _pileTipType; }
            set { Set(ref _pileTipType, value); }
        }

        /// <summary>
        /// Get pile is whether vertical or not.
        /// </summary>
        public bool IsVertical
        {
            get
            {
                return double.IsNaN(GetSkewness());
            }
        }

        /// <summary>
        /// Construct Pile from staad string and predefined jointlist
        /// </summary>
        /// <param name="pileStaadString">pilestring example: 1 1 2</param>
        /// <param name="jointList">the list that contains the Joint list</param>
        /// <returns>pile object</returns>
        public static PileBase ParseFromString(string pileStaadString, List<Joint> jointList)
        {
            try
            {
                pileStaadString = pileStaadString.Trim();
                string[] _pileComponent = pileStaadString.Split(' ');
                return new PileBase(Convert.ToInt32(_pileComponent[0]),
                                Convert.ToInt32(_pileComponent[1]),
                                Convert.ToInt32(_pileComponent[2]),
                                jointList);
            }
            catch (Exception e)
            {
                throw new FormatException($"{pileStaadString} is not valid string or its index is not in jointList", e);
            }

        }
        /// <summary>
        /// Construct pilebase class from axis line element. The pileGeoType and diameter and innerdiameter are default.
        /// </summary>
        /// <param name="lineElement"></param>
        /// <returns></returns>
        public static PileBase ObtainFromLineElement(BDE.LineElement lineElement)
        {

            if (lineElement.GetCurveVector().GetPrimitive(0).TryGetLine(out BG.DSegment3d _lineSegment))
            {
                long _id = lineElement.ElementId;
                if (_lineSegment.StartPoint.Z >= _lineSegment.EndPoint.Z)
                    return new PileBase() { Numbering = _id, TopJoint = new Joint(_lineSegment.StartPoint), BottomJoint = new Joint(_lineSegment.EndPoint) };
                else
                    return new PileBase() { Numbering = _id, TopJoint = new Joint(_lineSegment.EndPoint), BottomJoint = new Joint(_lineSegment.StartPoint) };
            }
            else
            {
                throw new InvalidOperationException("LineElement can't be tryGetLine");
            }
        }

        /// <summary>
        /// construct pilebase from pilecell
        /// </summary>
        /// <param name="pileCell">Pile Cell element which is created by tool</param>
        /// <returns></returns>
        public static PileBase ObtainFromPileCell(BDE.CellHeaderElement pileCell)
        {
            double _uorpermm = pileCell.DgnModel.GetModelInfo().UorPerMeter / 1000;

            if (pileCell == null)
                throw new NullReferenceException("Input pile cell is null");

            BDE.LineElement _axisLine = pileCell.GetChildren().Where(_line => _line is BDE.LineElement).First() as BDE.LineElement;

            PileBase _pileBase = ObtainFromLineElement(_axisLine);
            BG.DRay3d _axisRay = _pileBase.GetPileRay3D();

            string _ifcECSchemaName = "IfcPort";
            string _ifcPileECClassName = "IfcPile";
            Dictionary<string, object> _pileProps;
            List<string> _requirePilePropNameList = new List<string>()
            {
                "Type",
                "CrossSectionWidth",
                "OutsideDiameter",
                "InnerDiameter",
            };

            if (BD.StatusInt.Error == ECSChemaReader.ReadECInstanceProperties(_ifcECSchemaName, _ifcPileECClassName, _requirePilePropNameList, pileCell, out _pileProps))
                throw new InvalidOperationException("Can't read properties from pile cell");

            var _pileGeoType = PDIWT.Resources.PDIWT_Helper.GetEnumDescriptionDictionary<PileTypeManaged>()
                                                            .Where(e => e.Value == _pileProps["Type"].ToString())
                                                            .First().Key;
            double _pileDiameter, _pileInnerDiameter;
            _pileDiameter = _pileInnerDiameter = 0;
            switch (_pileGeoType)
            {
                case PileTypeManaged.SqaurePile:
                    _pileDiameter = double.Parse(_pileProps["CrossSectionWidth"].ToString()) * _uorpermm;
                    break;
                case PileTypeManaged.TubePile:
                    _pileDiameter = double.Parse(_pileProps["OutsideDiameter"].ToString()) * _uorpermm;
                    break;
                case PileTypeManaged.PHCTubePile:
                    _pileDiameter = double.Parse(_pileProps["OutsideDiameter"].ToString()) * _uorpermm;
                    _pileInnerDiameter = double.Parse(_pileProps["InnerDiameter"].ToString()) * _uorpermm;
                    break;
                case PileTypeManaged.SteelTubePile:
                    _pileDiameter = double.Parse(_pileProps["OutsideDiameter"].ToString()) * _uorpermm;
                    _pileInnerDiameter = double.Parse(_pileProps["InnerDiameter"].ToString()) * _uorpermm;
                    break;
                default:
                    break;
            }

            return new PileBase(pileCell.ElementId, _unknownPileName, new Joint(_pileBase.TopJoint.Point), new Joint(_pileBase.BottomJoint.Point), _pileGeoType, _pileDiameter, _pileInnerDiameter, 0, PileTipType.TotalSeal);
        }

        /// <summary>
        /// construct pilebase object by topPoint, pileLength, pile skewness and z axis rotation angle
        /// </summary>
        public static PileBase CreateFromTopPointandLength(BG.DPoint3d topPoint, double pileLengthInUor, double pileskewness, BG.Angle zplanRotationAngle, PileTypeManaged pileGeoType, double diameter, double innerDiameter, double concreteCoreLength = 0, PileTipType pileTipType = PileTipType.TotalSeal)
        {
            BG.DVector3d _unitZ = BG.DVector3d.UnitZ;
            _unitZ.NegateInPlace();

            //set rotation matrix
            BG.Angle _yAngle = new BG.Angle();
            if (pileskewness <= 0 || pileLengthInUor <= 0)
                throw new ArgumentException("pileskewness or pileLengthInUor can't be zero or negative value");
            if (double.IsNaN(pileskewness))
                _yAngle.Radians = 0;
            else
                _yAngle.Radians = -Math.Atan(1 / pileskewness);

            BG.DTransform3d _ytrans = BG.DTransform3d.Rotation(1, _yAngle);
            BG.DTransform3d _ztrans = BG.DTransform3d.Rotation(2, zplanRotationAngle);
            var _wholetrans = BG.DTransform3d.Multiply(_ztrans, _ytrans);
            _wholetrans.MultiplyInPlace(ref _unitZ);

            BG.DRay3d _unitAxisRan3D = new BG.DRay3d(topPoint, _unitZ);
            BG.DPoint3d _bottomPoint = _unitAxisRan3D.PointAtFraction(pileLengthInUor);
            return new PileBase(_unknowPileNumbering, _unknownPileName, new Joint(topPoint), new Joint(_bottomPoint), pileGeoType, diameter, innerDiameter, concreteCoreLength, pileTipType);
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", _numbering, _topJoint.Numbering, _bottomJoint.Numbering);
        }

        /// <summary>
        /// Get the Joint string
        /// </summary>
        /// <param name="codestring">CSV or STD right now. STD stands for staad</param>
        /// <returns></returns>
        public string ToString(string codestring)
        {
            if (codestring.ToUpper() == "CSV")
                return string.Format("{0},{1},{2}", _numbering, _topJoint.Numbering, _bottomJoint.Numbering);
            else if (codestring.ToUpper() == "STD")
                return ToString();
            else
                return ToString();
        }

        /// <summary>
        /// Get the pile Skewness
        /// </summary>
        /// <returns>if is vertical, return double.NaN; otherwise skewness</returns>
        public double GetSkewness()
        {
            BG.DRay3d _pileAxialRay = GetPileRay3D();
            BG.DVector3d _pileVector = _pileAxialRay.Direction;
            BG.DVector3d _negativeUnitZ = new BG.DVector3d(0, 0, -1);
            BG.Angle _angel = _pileVector.AngleTo(_negativeUnitZ);
            if (_angel.Radians == 0)
                return double.NaN;
            else
                return 1 / _angel.Tan;
        }

        /// <summary>
        /// Get the pile length. Unit: uor
        /// </summary>
        /// <returns>length: uor</returns>
        public double GetLength()
        {
            return GetPileRay3D().Length;
        }
        /// <summary>
        /// Get Pile Ray3D, Unit: uor
        /// </summary>
        /// <returns>Pile Ray3D</returns>
        public BG.DRay3d GetPileRay3D()
        {
            return new BG.DRay3d(_topJoint.Point, _bottomJoint.Point);
        }
        /// <summary>
        /// Get Pile Self weight in give calculated water level. Unit: SI, length -> m; force -> kN
        /// </summary>
        /// <param name="calculatedWaterLevel"></param>
        /// <param name="concreteWeight"></param>
        /// <param name="concreteUnderwaterWeight"></param>
        /// <param name="steelWeight"></param>
        /// <param name="steelUnderwaterWeight"></param>
        /// <returns>weight kN</returns>
        public double GetPileSelfWeight(
            double calculatedWaterLevel,
            double concreteWeight,
            double concreteUnderwaterWeight,
            double steelWeight,
            double steelUnderwaterWeight)
        {
            double _uorpermeter = BM.Session.Instance.GetActiveDgnModel().GetModelInfo().UorPerMeter;
            return AxialBearingCapacity.CalculatePileSelfWeight(
                GeoType,
                Diameter / _uorpermeter,
                InnerDiameter / _uorpermeter,
                TopJoint.Point.Z / _uorpermeter,
                GetLength() / _uorpermeter,
                GetSkewness(),
                calculatedWaterLevel, concreteWeight, concreteUnderwaterWeight, steelWeight, steelUnderwaterWeight, ConcreteCoreLength / _uorpermeter);
        }

        /// <summary>
        /// Get Pile outer perimeter
        /// </summary>
        /// <returns>perimeter. Unit: uor</returns>
        public double GetPerimeter()
        {
            return AxialBearingCapacity.CalculatePilePrimeter(GeoType, Diameter);
        }
        /// <summary>
        /// Get outer cross section area of pile
        /// </summary>
        /// <returns>Cross section. Unit: uor</returns>
        public double GetOuterArea()
        {
            return AxialBearingCapacity.CalculatePileEndOutsideArea(GeoType, Diameter);
        }

        public void DrawInActiveModel()
        {
            var _activeModel = BM.Session.Instance.GetActiveDgnModel();
            if (false == _activeModel.Is3d)
                throw new InvalidProgramException("The active model is not 3D");

            var _pileTypes = PDIWT.Resources.PDIWT_Helper.GetEnumDescriptionDictionary<PileTypeManaged>();
            EntityCreation.CreatePile(GeoType, _pileTypes, Diameter, InnerDiameter, ConcreteCoreLength, TopJoint.Point, BottomJoint.Point);
        }
    }

}
