using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    }

    public class PileFrame : ObservableObject
    {

    }

    public class Joint : ObservableObject
    {
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

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3};", _numbering, _point.X, _point.Y, _point.Z);
        }

    }

    public class Pile : ObservableObject
    {

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

        public override string ToString()
        {
            return string.Format("{0} {1} {2};", _numbering, _topPoint3D.Numbering, _bottomPoint.Numbering);
        }

    }
}
