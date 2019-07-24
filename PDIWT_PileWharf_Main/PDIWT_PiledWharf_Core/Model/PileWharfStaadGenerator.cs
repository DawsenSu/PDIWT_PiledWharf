using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.IO;

using GalaSoft.MvvmLight;

using EPPlus;
using EPPlus.DataExtractor;
using BD = Bentley.DgnPlatformNET;
using BM = Bentley.MstnPlatformNET;
using BG = Bentley.GeometryNET;
using OfficeOpenXml;

namespace PDIWT_PiledWharf_Core.Model
{
    public class PileWharfStaadGenerator : ObservableObject
    {
        public PileWharfStaadGenerator()
        {
            _outputLineWidth = 79;
        }
        private int _outputLineWidth;

        private string GenerateProjectInformation()
        {
            string input = "*-----------------------------------------------------------" + Environment.NewLine +
                           "*This program is created by Dawsensu @ Jul - 2019" + Environment.NewLine +
                           "*All Rights are Reserved by PDIWT" + Environment.NewLine +
                           "*-----------------------------------------------------------" + Environment.NewLine +
                           "STAAD SPACE" + Environment.NewLine +
                           "START JOB INFORMATION" + Environment.NewLine +
                           $"ENGINEER DATE {DateTime.Now.ToString("dd-MMM-yy")}" + Environment.NewLine +
                           "END JOB INFORMATION" + Environment.NewLine +
                           "INPUT WIDTH 79" + Environment.NewLine + Environment.NewLine +
                           "UNIT METER KN" + Environment.NewLine + Environment.NewLine;
            return input;
        }
        public string GenerateJointCoordinates()
        {
            StringBuilder _corrdinatesBuilder = new StringBuilder("JOINT COORDINATES\n");
            List<Joint> _jointList = new List<Joint>();
            // Generate test data
            Random _rand = new Random();
            for (int i = 1; i < 101; i++)
            {
                _jointList.Add(new Joint(i, _rand.NextDouble(), _rand.NextDouble(), _rand.NextDouble()));
            }
            OutputSeriesList(_corrdinatesBuilder, _jointList);

            return _corrdinatesBuilder.ToString();
        }
        public string GenerateMemberIncidences()
        {
            StringBuilder _memberincidencesBuilder = new StringBuilder("MEMBER INCIDENCES\n");

            List<Joint> _jointList = new List<Joint>();
            // Generate test data
            Random _rand = new Random();
            for (int i = 1; i < 101; i++)
            {
                _jointList.Add(new Joint(i, _rand.NextDouble(), _rand.NextDouble(), _rand.NextDouble()));
            }
            int _numberOfJoint = _jointList.Count;

            List<Pile> _pileList = new List<Pile>();
            for (int j = 1; j < 2000; j++)
            {
                _pileList.Add(new Pile(j, _rand.Next(1, _numberOfJoint + 1), _rand.Next(1, _numberOfJoint + 1), _jointList));
            }
            OutputSeriesList(_memberincidencesBuilder, _pileList);

            return _memberincidencesBuilder.ToString();
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

        /// <summary>
        /// Build Joints list by reading data from *.std file
        /// </summary>
        /// <param name="_stdFile">"*.std" file info</param>
        /// <returns>The joints list</returns>
        public List<Joint> BuildJointListFromStaadFile(FileInfo _stdFile)
        {
            List<Joint> _jointList = new List<Joint>();

            if (!_stdFile.Exists)
                return _jointList;
            StreamReader _streamReader = new StreamReader(_stdFile.FullName);
            string _line;
            do
            {
                _line = _streamReader.ReadLine();
            } while (!_line.Contains("JOINT COORDINATES"));
            while (!string.IsNullOrEmpty(_line = _streamReader.ReadLine()) && _line.Contains(";"))
            {
                _line = _line.Trim();
                string[] _jointStrings = _line.Split(';'); // the Last split string is empty
                foreach (var _jointStr in _jointStrings)
                {
                    if (string.IsNullOrEmpty(_jointStr))
                        continue;
                    Joint.ParseFromString(_jointStr, out Joint _joint);
                    _jointList.Add(_joint);
                }
            }
            return _jointList;
        }


        /// <summary>
        /// Build Piles by reading data from *.std file
        /// </summary>
        /// <param name="_stdFile">*.std file info</param>
        /// <returns>pile list</returns>
        public List<Pile> BuildPileListFromStaadFile(FileInfo _stdFile)
        {
            List<Pile> _pileList = new List<Pile>();

            if (!_stdFile.Exists)
                return _pileList;
            List<Joint> _jointList = BuildJointListFromStaadFile(_stdFile);

            StreamReader _streamReader = new StreamReader(_stdFile.FullName);
            string _line;
            do
            {
                _line = _streamReader.ReadLine();
            } while (!_line.Contains("MEMBER INCIDENCES"));
            while (!string.IsNullOrEmpty(_line = _streamReader.ReadLine()) && _line.Contains(";"))
            {
                _line = _line.Trim();
                string[] _jointStrings = _line.Split(';'); // the Last split string is empty
                foreach (var _jointStr in _jointStrings)
                {
                    if (string.IsNullOrEmpty(_jointStr))
                        continue;
                    Pile.ParseFromString(_jointStr, _jointList, out Pile _joint);
                    _pileList.Add(_joint);
                }
            }
            return _pileList;
        }

        public List<Pile> BuildJointListFromExcel(FileInfo excelFile)
        {
            List<Pile> _pileList = new List<Pile>();

            if (!excelFile.Exists)
                return _pileList;

            using (var _excelPackage = new ExcelPackage(excelFile))
            {
                var _worksheet = _excelPackage.Workbook.Worksheets[1];
                //Read and renumber all Joints if necessary
                List<Joint> _topJoints = new List<Joint>();
                List<Joint> _bottomJoints = new List<Joint>();
                List<int> _pileNumbering = new List<int>();

                for (int i = 2; i < _worksheet.Dimension.Rows+1; i++)
                {
                    //pile numbering
                    object _value = _worksheet.Cells[i, 1].Value;
                    if (_value == null || string.IsNullOrEmpty(_value.ToString()))
                        _pileNumbering.Add(int.MinValue);
                    else
                        _pileNumbering.Add(int.Parse(_value.ToString()));
                    //Top joint
                    object _BColumn = _worksheet.Cells[i, 2].Value;
                    if (_BColumn == null || string.IsNullOrEmpty(_BColumn.ToString()))
                        _topJoints.Add(new Joint(int.MinValue,
                                                 Convert.ToDouble(_worksheet.Cells[i, 3].Value),
                                                 Convert.ToDouble(_worksheet.Cells[i, 4].Value),
                                                 Convert.ToDouble(_worksheet.Cells[i, 5].Value)));
                    else
                        _topJoints.Add(new Joint(Convert.ToInt32(_worksheet.Cells[i,2].Value),
                                                 Convert.ToDouble(_worksheet.Cells[i, 3].Value),
                                                 Convert.ToDouble(_worksheet.Cells[i, 4].Value),
                                                 Convert.ToDouble(_worksheet.Cells[i, 5].Value)));
                    //Bottom Joint
                    object _FColumn = _worksheet.Cells[i, 6].Value;
                    if (_FColumn == null || string.IsNullOrEmpty(_FColumn.ToString()))
                        _bottomJoints.Add(new Joint(int.MinValue,
                                                 Convert.ToDouble(_worksheet.Cells[i, 7].Value),
                                                 Convert.ToDouble(_worksheet.Cells[i, 8].Value),
                                                 Convert.ToDouble(_worksheet.Cells[i, 9].Value)));
                    else
                        _bottomJoints.Add(new Joint(Convert.ToInt32(_worksheet.Cells[i, 6].Value),
                                                 Convert.ToDouble(_worksheet.Cells[i, 7].Value),
                                                 Convert.ToDouble(_worksheet.Cells[i, 8].Value),
                                                 Convert.ToDouble(_worksheet.Cells[i, 9].Value)));

                }

                List<Joint> _allJoints = new List<Joint>();
                _allJoints.AddRange(_topJoints);
                _allJoints.AddRange(_bottomJoints);
                _allJoints = _allJoints.Distinct().ToList();

                if (_allJoints.Any(p => p.Numbering == int.MinValue))
                {
                    _allJoints.Sort((front, back) =>
                    {
                        if (front.Point.X > back.Point.X)
                            return 1;
                        else if (front.Point.X < back.Point.X)
                            return -1;
                        else
                        {
                            if (front.Point.Y > back.Point.Y)
                                return 1;
                            else if (front.Point.Y < back.Point.Y)
                                return -1;
                            else
                            {
                                if (front.Point.Z > back.Point.Z)
                                    return 1;
                                else if (front.Point.Z < back.Point.Z)
                                    return -1;
                                return 0;
                            }
                        }
                    });

                    for (int i = 0; i < _allJoints.Count; i++)
                    {
                        _allJoints[i].Numbering = i;
                    }
                }

                //Decide how to construct pile instances.
                if(_pileNumbering.Any(p=>p == int.MinValue))
                {
                    for (int i = 0; i < _pileNumbering.Count; i++)
                    {
                        _pileList.Add(new Pile(i, _topJoints[i].Point, _bottomJoints[i].Point, _allJoints));
                    }
                }
                else
                {
                    for (int i = 0; i < _pileNumbering.Count; i++)
                    {
                        _pileList.Add(new Pile(_pileNumbering[i], _topJoints[i], _bottomJoints[i]));
                    }
                }
            }
            return _pileList;
        }


        class PileJointRelationship
        {
            public int PileNumbering { get; set; }
            public int PileTopPointNumbering { get; set; }
            public int PileBottomPointNumbering { get; set; }
        }
        /// <summary>
        /// Build the output string with fixed line width and designated separator
        /// </summary>
        /// <param name="stringBuilder">THe StringBuilder which will form output string</param>
        /// <param name="er">IEnumberable collection</param>
        /// <param name="separator"></param>
        /// <param name="isContainBlankAdditionalSeparator">If true, add additional " " between different objects</param>
        private void OutputSeriesList(StringBuilder stringBuilder, IEnumerable er, string separator = ";", bool isContainBlankAdditionalSeparator = true)
        {
            int _numberOfCurrentLine = 0;
            foreach (var _erItem in er)
            {
                string _erItemStr = _erItem.ToString() + separator;
                if (isContainBlankAdditionalSeparator)
                    _erItemStr += " ";
                int _numberOfJointStr = _erItemStr.Length;
                if (_numberOfCurrentLine + _numberOfJointStr < _outputLineWidth)
                {
                    stringBuilder.Append(_erItemStr);
                    _numberOfCurrentLine += _numberOfJointStr;
                }
                else
                {
                    if (isContainBlankAdditionalSeparator)
                        stringBuilder.Remove(stringBuilder.Length - 1, 1);
                    stringBuilder.AppendLine();
                    stringBuilder.Append(_erItemStr);
                    _numberOfCurrentLine = 0;
                }
            }
        }

    }



    /// <summary>
    /// Joint class represent joint object
    /// </summary>
    public class Joint : ObservableObject,IEquatable<Joint>
    {

        public Joint(int num, double x, double y, double z)
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

        // Joint string example: 1 1 2 3
        public static void ParseFromString(string jointString, out Joint joint)
        {
            jointString = jointString.Trim();
            string[] jointComponent = jointString.Split(' ');
            joint = new Joint(Convert.ToInt32(jointComponent[0]),
                              Convert.ToDouble(jointComponent[1]),
                              Convert.ToDouble(jointComponent[2]),
                              Convert.ToDouble(jointComponent[3]));
        }

        public override string ToString()
        {
            return string.Format("{0} {1:G2} {2:G2} {3:G2}", _numbering, _point.X, _point.Y, _point.Z);
        }

        public bool Equals(Joint other)
        {
            if (this.Numbering == other.Numbering && this.Point == other.Point)
                return true;
            return false;
        }
        public override int GetHashCode()
        {
            int _hashNumbering = Numbering.GetHashCode();
            int _hashPoint = Point.GetHashCode();
            return _hashNumbering ^ _hashPoint;
        }
    }

    /// <summary>
    /// Pile class represent pile in piledwharf system
    /// </summary>
    public class Pile : ObservableObject
    {
        public Pile(int number, int topPointNumber, int bottomPointNumber, List<Joint> joints)
        {
            _numbering = number;
            _topJoint = joints.Find(joint => joint.Numbering == topPointNumber);
            _bottomJoint = joints.Find(joint => joint.Numbering == bottomPointNumber);
        }

        public Pile(int number, BG.DPoint3d topPoint, BG.DPoint3d bottomPoint, List<Joint> joints)
        {
            _numbering = number;
            _topJoint = joints.Find(joint => joint.Point == topPoint);
            _bottomJoint = joints.Find(joint => joint.Point == bottomPoint);
        }

        public Pile(int number, Joint topJoint, Joint bottomJoint)
        {
            _numbering = number;
            _topJoint = topJoint;
            _bottomJoint = bottomJoint;
        }
        private int _numbering;

        public int Numbering
        {
            get { return _numbering; }
            set { Set(ref _numbering, value); }
        }

        private Joint _topJoint;

        public Joint TopJoint
        {
            get { return _topJoint; }
            set { Set(ref _topJoint, value); }
        }

        private Joint _bottomJoint;

        public Joint BottomJoint
        {
            get { return _bottomJoint; }
            set { Set(ref _bottomJoint, value); }
        }

        // pilestring example: 1 1 2
        public static void ParseFromString(string pileString, List<Joint> jointList, out Pile pile)
        {
            pileString = pileString.Trim();
            string[] _pileComponent = pileString.Split(' ');
            pile = new Pile(Convert.ToInt32(_pileComponent[0]),
                            Convert.ToInt32(_pileComponent[1]),
                            Convert.ToInt32(_pileComponent[2]),
                            jointList);
        }
        public override string ToString()
        {
            return string.Format("{0} {1} {2}", _numbering, _topJoint.Numbering, _bottomJoint.Numbering);
        }

        public class PileFrame : ObservableObject
        {
            public PileFrame()
            {
                Piles = new List<Pile>();
                FrameNumber = 0;
            }
            public PileFrame(List<Pile> piles, int frameNumber)
            {
                Piles = piles;
                FrameNumber = frameNumber;
            }
            public List<Pile> Piles { get; }

            public int FrameNumber { get; set; }

            public int NumberOfPilesInFrame => Piles.Count;

            public bool Add(Pile pile)
            {
                if (Piles.Contains(pile))
                    return false;
                else
                {
                    Piles.Add(pile);
                    return true;
                }
            }

            public void Clear() => Piles.Clear();
            /// <summary>
            /// Clone the this PileFrame for next PileFrame with given offset
            /// </summary>
            /// <param name="offset">the offset from next pileframe to current one</param>
            /// <returns>The next pileframe</returns>
            public PileFrame CloneNextWithOffset(double offset)
            {
                int _numberofpiles = NumberOfPilesInFrame;
                int _biggestJointNumbering = 0;
                int _biggestPileNumbering = 0;
                foreach (var _pile in Piles)
                {
                    if (_pile.TopJoint.Numbering > _biggestJointNumbering)
                        _biggestJointNumbering = _pile.TopJoint.Numbering;
                    if (_pile.BottomJoint.Numbering > _biggestJointNumbering)
                        _biggestJointNumbering = _pile.BottomJoint.Numbering;

                    if (_pile.Numbering > _biggestPileNumbering)
                        _biggestPileNumbering = _pile.Numbering;
                }

                List<Pile> _nextPilesList = new List<Pile>();
                for (int i = 0; i < _numberofpiles; i++)
                {
                    // top joint goes first, then bottom joint
                    Joint _newTopJoint = new Joint(_biggestJointNumbering + i + 1,
                                                   Piles[i].TopJoint.Point.X + offset,
                                                   Piles[i].TopJoint.Point.Y,
                                                   Piles[i].TopJoint.Point.Z);
                    Joint _newBottomJoint = new Joint(_biggestJointNumbering + i + 2,
                                                      Piles[i].BottomJoint.Point.X + offset,
                                                      Piles[i].BottomJoint.Point.Y,
                                                      Piles[i].BottomJoint.Point.Z);
                    Pile _newPile = new Pile(_biggestPileNumbering + i + 1,
                                             _biggestJointNumbering + i + 1,
                                             _biggestJointNumbering + i + 2,
                                             new List<Joint> { _newBottomJoint, _newTopJoint });
                    _nextPilesList.Add(_newPile);
                }
                return new PileFrame(_nextPilesList, FrameNumber + 1);
            }
            public double GetOffsetFrom(PileFrame pileFrameFrom)
            {
                return pileFrameFrom.Piles.First().TopJoint.Point.Distance(Piles.First().TopJoint.Point);
            }

            public override string ToString()
            {
                StringBuilder _sb = new StringBuilder();
                _sb.Append(FrameNumber.ToString() + "\n");
                foreach (var _pile in Piles)
                {
                    _sb.Append(_pile + "; ");
                }
                return _sb.ToString();
            }

        }

        public class Wharf : ObservableObject
        {
            public Wharf()
            {
                PileFrameList = new List<PileFrame>();
                FrameSpans = new List<double>();
            }
            public Wharf(List<PileFrame> pileFrames)
            {
                PileFrameList = pileFrames;
                FrameSpans = new List<double>();
                for (int i = 0; i < pileFrames.Count - 1; i++)
                {
                    FrameSpans.Add(pileFrames[i].GetOffsetFrom(pileFrames[i + 1]));
                }
            }

            public Wharf(PileFrame firstPileFrame, List<double> spans)
            {
                PileFrameList = new List<PileFrame>() { firstPileFrame };
                List<double> _distanceFromFirstFrame = new List<double>();
                for (int i = 0; i < spans.Count; i++)
                {
                    if (i == 0)
                        _distanceFromFirstFrame.Add(spans[i]);
                    else
                        _distanceFromFirstFrame.Add(_distanceFromFirstFrame[i - 1] + spans[i]);
                }
                foreach (var _distance in _distanceFromFirstFrame)
                {
                    PileFrameList.Add(firstPileFrame.CloneNextWithOffset(_distance));
                }
            }

            List<PileFrame> PileFrameList { get; }
            public List<double> FrameSpans { get; }

            public int NumberOfFrames => PileFrameList.Count;
            public int NumberOfPiles
            {
                get
                {
                    int _numberOfPiles = 0;
                    foreach (var _frame in PileFrameList)
                    {
                        _numberOfPiles += _frame.NumberOfPilesInFrame;
                    }
                    return _numberOfPiles;
                }
            }

            public int NumberOfJoints => 2 * NumberOfPiles;

            public override string ToString()
            {
                StringBuilder _sb = new StringBuilder();
                _sb.AppendFormat("Wharf Information: Contains {0} Frames, {1} Piles, {2} Joints\n", NumberOfFrames, NumberOfPiles, NumberOfJoints);
                foreach (var _frame in PileFrameList)
                {
                    _sb.Append(_frame);
                }
                return _sb.ToString();
            }
        }

    }
}
