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
    public class PiledWharfStaadGenerator : ObservableObject
    {
        public PiledWharfStaadGenerator()
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

            List<PileBase> _pileList = new List<PileBase>();
            for (int j = 1; j < 2000; j++)
            {
                _pileList.Add(new PileBase(j, _rand.Next(1, _numberOfJoint + 1), _rand.Next(1, _numberOfJoint + 1), _jointList));
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
                    Joint _joint =Joint.ParseFromString(_jointStr);
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
        public List<PileBase> BuildPileListFromStaadFile(FileInfo _stdFile)
        {
            List<PileBase> _pileList = new List<PileBase>();

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
                    PileBase _joint = PileBase.ParseFromString(_jointStr, _jointList);
                    _pileList.Add(_joint);
                }
            }
            return _pileList;
        }

        /// <summary>
        /// Build pile from excel file which contains column:
        /// pile numbering, top joint numbering, top x(mm), top y(mm), z(mm), bottom joint numbering, bottom x(mm), bottom y(mm), bottom z(mm)
        /// </summary>
        /// <param name="excelFile"></param>
        /// <returns></returns>
        public List<PileBase> BuildPileListFromExcel(FileInfo excelFile)
        {
            List<PileBase> _pileList = new List<PileBase>();

            if (!excelFile.Exists)
                return _pileList;

            using (var _excelPackage = new ExcelPackage(excelFile))
            {
                var _worksheet = _excelPackage.Workbook.Worksheets[1];
                //Read and renumber all Joints if necessary
                List<Joint> _topJoints = new List<Joint>();
                List<Joint> _bottomJoints = new List<Joint>();
                List<long> _pileNumbering = new List<long>();

                for (int i = 2; i < _worksheet.Dimension.Rows+1; i++)
                {
                    //pile numbering
                    object _value = _worksheet.Cells[i, 1].Value;
                    if (_value == null || string.IsNullOrEmpty(_value.ToString()))
                        _pileNumbering.Add(long.MinValue);
                    else
                        _pileNumbering.Add(long.Parse(_value.ToString()));
                    //Top joint
                    object _BColumn = _worksheet.Cells[i, 2].Value;
                    if (_BColumn == null || string.IsNullOrEmpty(_BColumn.ToString()))
                        _topJoints.Add(new Joint(long.MinValue,
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
                        _bottomJoints.Add(new Joint(long.MinValue,
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

                if (_allJoints.Any(p => p.Numbering == long.MinValue))
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
                        _pileList.Add(new PileBase(i, _topJoints[i].Point, _bottomJoints[i].Point, _allJoints));
                    }
                }
                else
                {
                    for (int i = 0; i < _pileNumbering.Count; i++)
                    {
                        _pileList.Add(new PileBase(_pileNumbering[i], PileBase._unknownPileName,_topJoints[i], _bottomJoints[i], PDIWT_PiledWharf_Core_Cpp.PileTypeManaged.SqaurePile,0,0,0,PileTipType.TotalSeal));
                    }
                }
            }
            return _pileList;
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

    class PileJointRelationship
    {
        public int PileNumbering { get; set; }
        public int PileTopPointNumbering { get; set; }
        public int PileBottomPointNumbering { get; set; }
    }
}
