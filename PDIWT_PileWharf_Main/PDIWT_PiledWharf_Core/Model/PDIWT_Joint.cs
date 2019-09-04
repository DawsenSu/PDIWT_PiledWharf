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

namespace PDIWT_PiledWharf_Core.Model
{
    /// <summary>
    /// Joint class represent joint object
    /// </summary>
    public class Joint : ObservableObject, IEquatable<Joint>, ICloneable
    {
        public Joint() : this(-1, 0, 0, 0)
        { }

        public Joint(double x, double y, double z) : this(-1, x, y, z) { }

        public Joint(BG.DPoint3d dPoint3D) : this(-1, dPoint3D) { }

        public Joint(long num, BG.DPoint3d dPoint3D) : this(num, dPoint3D.X, dPoint3D.Y, dPoint3D.Z) { }

        public Joint(long num, double x, double y, double z)
        {
            _numbering = num;
            _point = new BG.DPoint3d(x, y, z);
        }


        private long _numbering;
        /// <summary>
        /// positive numbering, -1 represent no particular assignment 
        /// </summary>
        public long Numbering
        {
            get { return _numbering; }
            set { Set(ref _numbering, value); }
        }


        private BG.DPoint3d _point;
        /// <summary>
        /// unit: uor
        /// </summary>
        public BG.DPoint3d Point
        {
            get { return _point; }
            set { Set(ref _point, value); }
        }

        /// <summary>
        /// construct Joint object by parsing the string
        /// </summary>
        /// <param name="staadJointString">Joint string example: 1 1 2 3</param>
        /// <returns>new joint Object</returns>
        public static Joint ParseFromString(string staadJointString)
        {
            staadJointString = staadJointString.Trim();
            string[] jointComponent = staadJointString.Split(' ');
            try
            {
                return new Joint(Convert.ToInt32(jointComponent[0]),
                                 Convert.ToDouble(jointComponent[1]),
                                 Convert.ToDouble(jointComponent[2]),
                                 Convert.ToDouble(jointComponent[3]));
            }
            catch (Exception e)
            {
                throw new FormatException($"{staadJointString} is not valid Joint String format",e);
            }

        }

        public override string ToString()
        {
            return string.Format("{0} {1:G2} {2:G2} {3:G2}", _numbering, _point.X, _point.Y, _point.Z);
        }
        /// <summary>
        /// Get the Joint string
        /// </summary>
        /// <param name="codestring">CSV or STD right now. STD stands for staad</param>
        /// <returns></returns>
        public string ToString(string codestring)
        {
            if (codestring.ToUpper() == "CSV")
                return string.Format("{0},{1:G2},{2:G2},{3:G2}", _numbering, _point.X, _point.Y, _point.Z);
            else if (codestring.ToUpper() == "STD")
                return ToString();
            else
                return ToString();
        }

        public bool Equals(Joint other)
        {
            return (Numbering == other.Numbering && Point == other.Point);
        }

        public override int GetHashCode()
        {
            int _hashNumbering = Numbering.GetHashCode();
            int _hashPoint = Point.GetHashCode();
            return _hashNumbering ^ _hashPoint;
        }

        public object Clone()
        {
            return new Joint(_numbering, _point.X, _point.Y, _point.Z);
        }
    }

}
