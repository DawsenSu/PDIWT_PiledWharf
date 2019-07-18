using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using GalaSoft.MvvmLight;
using BD = Bentley.DgnPlatformNET;
using BM = Bentley.MstnPlatformNET;
using BG = Bentley.GeometryNET;

namespace PDIWT_PiledWharf_Core.Model
{
    public class PileWharfStaadGenerator : ObservableObject
    {
        private string GenerateProjectInformation()
        {
            string input = "*-----------------------------------------------------------" + Environment.NewLine +
                           "*This program is created by Dawsensu @ Jul - 2019" + Environment.NewLine +
                           "* All Rights are Reserved by PDIWT" + Environment.NewLine +
                           "* -----------------------------------------------------------" + Environment.NewLine +
                           "STAAD SPACE" + Environment.NewLine +
                           "START JOB INFORMATION" + Environment.NewLine +
                           $"ENGINEER DATE {DateTime.Now.ToString("dd-MMM-yy")}" + Environment.NewLine +
                           "END JOB INFORMATION" + Environment.NewLine +
                           "INPUT WIDTH 79" + Environment.NewLine + Environment.NewLine +
                           "UNIT METER KN" + Environment.NewLine + Environment.NewLine;
            return input;
        }
        private string GenerateJointCoordinates()
        {
            StringBuilder corrdinatesBuilder = new StringBuilder("JOINT COORDINATES\n");

            return corrdinatesBuilder.ToString();
        }
        private string GenerateMemberIncidences()
        {
            StringBuilder memberincidencesBuilder = new StringBuilder("MEMBER INCIDENCES\n");

            return memberincidencesBuilder.ToString();
        }
        private string GenerateMemberProperty()
        {
            StringBuilder memberPropertyBuilder = new StringBuilder("MEMBER PROPERTY AMERICAN\n");

            return memberPropertyBuilder.ToString();
        }
        private string GenerateMaterials()
        {
            StringBuilder materialsBuilder = new StringBuilder("DEFINE MATERIAL START\n");

            materialsBuilder.Append("END DEFINE MATERIAL\n");
            return materialsBuilder.ToString();
        }

        private string GenerateConstants()
        {
            StringBuilder contantsBuilder = new StringBuilder("CONSTANTS\n");
            return contantsBuilder.ToString();
        }

        private string GenerateSupports()
        {
            StringBuilder supportsBuilder = new StringBuilder("SUPPORTS\n");

            return supportsBuilder.ToString();
        }

        private string GeneratePerformAndFinish()
        {
            return "PERFORM ANALYSIS\n" + "FINISH\n" + "FINISH\n";
        }

        public List<Joint> BuildJointList(FileInfo _stdFile)
        {
            List<Joint> _jointList = new List<Joint>();

            if (!_stdFile.Exists)
                return _jointList;
            StreamReader _streamReader = new StreamReader(_stdFile.FullName);
            string _line;
            do
            {
                _line = _streamReader.ReadLine();
            } while (_line.Contains("JOINT COORDINATES"));
            do
            {
                _line = _streamReader.ReadLine();
                string[] _jointStrings = _line.Split(';');
                foreach (var _jointStr in _jointStrings)
                {
                    Joint.ParseFromString(_jointStr, out Joint _joint);
                    _jointList.Add(_joint);
                }
            } while (!_line.Contains(";"));
            return _jointList;
        }

        public List<Pile> BuildPileList(FileInfo _stdFile)
        {
            List<Pile> _pileList = new List<Pile>();

            if (!_stdFile.Exists)
                return _pileList;
            List<Joint> _jointList = BuildJointList(_stdFile);

            StreamReader _streamReader = new StreamReader(_stdFile.FullName);
            string _line;
            do
            {
                _line = _streamReader.ReadLine();
            } while (_line.Contains("MEMBER INCIDENCES"));
            do
            {
                _line = _streamReader.ReadLine();
                string[] _jointStrings = _line.Split(';');
                foreach (var _jointStr in _jointStrings)
                {
                    Pile.ParseFromString(_jointStr, _jointList ,out Pile _joint);
                    _pileList.Add(_joint);
                }
            } while (!_line.Contains(";"));
            return _pileList;            
        }
    }

    public class PileFrame : ObservableObject
    {

    }

    public class Joint : ObservableObject
    {
        Joint(int num, double x, double y, double z)
        {
            _numbering = num;
            _point = new BG.DPoint3d(x, y, z);
        }
        private int _numbering;

        public int Numbering
        {
            get { return _numbering; }
            set { _numbering = value; }
        }

        private BG.DPoint3d _point;

        public BG.DPoint3d Point
        {
            get { return _point; }
            set { _point = value; }
        }

        public static void ParseFromString(string jointString, out Joint joint)
        {
            jointString = jointString.TrimEnd(':');
            string[] jointComponent = jointString.Split(' ');
            joint = new Joint(Convert.ToInt32(jointComponent[0]),
                              Convert.ToDouble(jointComponent[1]),
                              Convert.ToDouble(jointComponent[2]),
                              Convert.ToDouble(jointComponent[3]));
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3};", _numbering, _point.X, _point.Y, _point.Z);
        }

    }

    public class Pile : ObservableObject
    {
        Pile(int number, int topPointNumber, int bottomPointNumber, List<Joint> joints)
        {
            _numbering = number;
            _topPoint3D = joints.Find(joint => joint.Numbering == topPointNumber);
            _bottomPoint = joints.Find(joint => joint.Numbering == bottomPointNumber);
        }
        private int _numbering;

        public int Numbering
        {
            get { return _numbering; }
            set { _numbering = value; }
        }

        private Joint _topPoint3D;

        public Joint TopPoint
        {
            get { return _topPoint3D; }
            set { _topPoint3D = value; }
        }

        private Joint _bottomPoint;

        public Joint BottomPoint
        {
            get { return _bottomPoint; }
            set { _bottomPoint = value; }
        }

        public static void ParseFromString(string pileString, List<Joint> jointList, out Pile pile)
        {
            pileString = pileString.TrimEnd(';');
            string[] _pileComponent = pileString.Split(' ');
            pile = new Pile(Convert.ToInt32(_pileComponent[0]),
                            Convert.ToInt32(_pileComponent[1]),
                            Convert.ToInt32(_pileComponent[2]),
                            jointList);
        }
        public override string ToString()
        {
            return string.Format("{0} {1} {2};", _numbering, _topPoint3D.Numbering, _bottomPoint.Numbering);
        }

    }
}
