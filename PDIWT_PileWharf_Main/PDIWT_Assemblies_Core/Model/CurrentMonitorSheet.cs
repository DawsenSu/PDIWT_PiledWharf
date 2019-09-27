using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;

using OfficeOpenXml;
using OfficeOpenXml.Packaging;

namespace PDIWT_Assemblies_Core.Model
{
    public class CurrentMonitorSheet : ObservableObject
    {
        public CurrentMonitorSheet(ExcelWorksheet sheet)
        {
            Region = sheet.Cells[2, 1].GetValue<string>();
            StationNumber = sheet.Name;
            //! need to test
            StationCoordinates = sheet.Cells[2,sheet.Dimension.Columns-3].GetValue<string>();
            int columnNumber = sheet.Dimension.Columns;
            if (columnNumber < 7)
                throw new FormatException("Invalid input file format, column number shouldn't be less than 7");
            if (columnNumber > 17)
                throw new FormatException("Invalid input file format, column number shouldn't be great than 17");
            for (int i = 6; i < sheet.Dimension.Rows + 1; i++)
            {
                MoniterEntry _entry = new MoniterEntry
                {
                    RecordTime = new DateTime(sheet.GetValue<int>(i, 3), sheet.GetValue<int>(i, 1), sheet.GetValue<int>(i, 2), sheet.GetValue<int>(i, 4), sheet.GetValue<int>(i, 5), 0)
                };
                for (int j = 6; j < sheet.Dimension.Columns + 1; j = j + 2)
                {
                    CurrentInfo _info = new CurrentInfo
                    {
                        Layer = sheet.Cells[3, j].GetValue<string>(),
                        Velocity = sheet.GetValue<double>(i, j),
                        Direction = sheet.GetValue<double>(i, j + 1)
                    };
                    _entry.CurrentInfos.Add(_info);
                }
                Entries.Add(_entry);
            }
        }

        private string _region;
        /// <summary>
        /// To describe the region where the survey carries out.
        /// </summary>
        public string Region
        {
            get { return _region; }
            set { Set(ref _region, value); }
        }

        private string _stationNumber;
        /// <summary>
        /// the station code number
        /// </summary>
        public string StationNumber
        {
            get { return _stationNumber; }
            set { Set(ref _stationNumber, value); }
        }

        private string _stationCoordinates;
        /// <summary>
        /// Property Description
        /// </summary>
        public string StationCoordinates
        {
            get { return _stationCoordinates; }
            set { Set(ref _stationCoordinates, value); }
        }

        private ObservableCollection<MoniterEntry> _entries = new ObservableCollection<MoniterEntry>();
        /// <summary>
        /// Property Description
        /// </summary>
        public ObservableCollection<MoniterEntry> Entries
        {
            get { return _entries; }
            set { Set(ref _entries, value); }
        }

        public IEnumerable<Tuple<DateTime, double, double>> GetCalculateInfos(AverageMethod method)
        {
            List<Tuple<DateTime, double, double>> _tuples = new List<Tuple<DateTime, double, double>>();

            switch (method)
            {
                case AverageMethod.oneLayer:
                    var _firestItem = Entries.First().CurrentInfos.Where(info => info.Layer == "0.6H");
                    if (_firestItem == null | _firestItem.Count() == 0)
                        throw new InvalidOperationException("There is no 0.6H in input file");
                    foreach (var _e in Entries)
                    {
                        var _0_6H = _e.CurrentInfos.Where(info => info.Layer == "0.6H").First();
                        _tuples.Add(Tuple.Create(_e.RecordTime, _0_6H.Velocity, _0_6H.Direction));
                    }
                    break;
                case AverageMethod.TwoLayers:
                    _firestItem = Entries.First().CurrentInfos.Where(info => info.Layer == "0.2H");
                    if (_firestItem == null | _firestItem.Count() == 0)
                        throw new InvalidOperationException("There is no 0.2H in input file");
                    _firestItem = Entries.First().CurrentInfos.Where(info => info.Layer == "0.8H");
                    if (_firestItem == null | _firestItem.Count() == 0)
                        throw new InvalidOperationException("There is no 0.8H in input file");
                    foreach (var _e in Entries)
                    {
                        var _0_2H = _e.CurrentInfos.Where(info => info.Layer == "0.2H").First();
                        var _0_8H = _e.CurrentInfos.Where(info => info.Layer == "0.8H").First();
                        _tuples.Add(Tuple.Create(_e.RecordTime, (_0_2H.Velocity + _0_8H.Velocity) / 2, (_0_2H.Direction + _0_8H.Velocity) / 2));
                    }
                    break;
                case AverageMethod.ThreeLayers:
                    _firestItem = Entries.First().CurrentInfos.Where(info => info.Layer == "表层");
                    if (_firestItem == null | _firestItem.Count() == 0)
                        throw new InvalidOperationException("There is no 表层 in input file");
                    _firestItem = Entries.First().CurrentInfos.Where(info => info.Layer == "0.6H");
                    if (_firestItem == null | _firestItem.Count() == 0)
                        throw new InvalidOperationException("There is no 0.6H in input file");
                    _firestItem = Entries.First().CurrentInfos.Where(info => info.Layer == "底层");
                    if (_firestItem == null | _firestItem.Count() == 0)
                        throw new InvalidOperationException("There is no 底层 in input file");
                    foreach (var _e in Entries)
                    {
                        var _0_H = _e.CurrentInfos.Where(info => info.Layer == "表层").First();
                        var _0_6H = _e.CurrentInfos.Where(info => info.Layer == "0.6H").First();
                        var _0_1H = _e.CurrentInfos.Where(info => info.Layer == "底层").First();
                        _tuples.Add(Tuple.Create(_e.RecordTime, (4 * _0_H.Velocity + 4 * _0_6H.Velocity + 2 * _0_1H.Velocity) / 10,
                                                                (4 * _0_H.Direction + 4 * _0_6H.Direction + 2 * _0_1H.Direction) / 10));
                    }
                    break;
                case AverageMethod.FiveLayer:
                    _firestItem = Entries.First().CurrentInfos.Where(info => info.Layer == "表层");
                    if (_firestItem == null | _firestItem.Count() == 0)
                        throw new InvalidOperationException("There is no 表层 in input file");
                    _firestItem = Entries.First().CurrentInfos.Where(info => info.Layer == "0.2H");
                    if (_firestItem == null | _firestItem.Count() == 0)
                        throw new InvalidOperationException("There is no 0.2H in input file");
                    _firestItem = Entries.First().CurrentInfos.Where(info => info.Layer == "0.6H");
                    if (_firestItem == null | _firestItem.Count() == 0)
                        throw new InvalidOperationException("There is no 0.6H in input file");
                    _firestItem = Entries.First().CurrentInfos.Where(info => info.Layer == "0.8H");
                    if (_firestItem == null | _firestItem.Count() == 0)
                        throw new InvalidOperationException("There is no 0.8H in input file");
                    _firestItem = Entries.First().CurrentInfos.Where(info => info.Layer == "底层");
                    if (_firestItem == null | _firestItem.Count() == 0)
                        throw new InvalidOperationException("There is no 底层 in input file");
                    foreach (var _e in Entries)
                    {
                        var _0_H = _e.CurrentInfos.Where(info => info.Layer == "表层").First();
                        var _0_2H = _e.CurrentInfos.Where(info => info.Layer == "0.2H").First();
                        var _0_6H = _e.CurrentInfos.Where(info => info.Layer == "0.6H").First();
                        var _0_8H = _e.CurrentInfos.Where(info => info.Layer == "0.8H").First();
                        var _0_1H = _e.CurrentInfos.Where(info => info.Layer == "底层").First();
                        _tuples.Add(Tuple.Create(_e.RecordTime, (_0_H.Velocity + 3 * _0_2H.Velocity + 3 * _0_6H.Velocity + 2 * _0_8H.Velocity + _0_1H.Velocity) / 10,
                                                                (_0_H.Direction + 3 * _0_2H.Direction + 3 * _0_6H.Direction + 2 * _0_8H.Direction + _0_1H.Direction) / 10));
                    }
                    break;
                case AverageMethod.SixLayer:
                    _firestItem = Entries.First().CurrentInfos.Where(info => info.Layer == "表层");
                    if (_firestItem == null | _firestItem.Count() == 0)
                        throw new InvalidOperationException("There is no 表层 in input file");
                    _firestItem = Entries.First().CurrentInfos.Where(info => info.Layer == "0.2H");
                    if (_firestItem == null | _firestItem.Count() == 0)
                        throw new InvalidOperationException("There is no 0.2H in input file");
                    _firestItem = Entries.First().CurrentInfos.Where(info => info.Layer == "0.4H");
                    if (_firestItem == null | _firestItem.Count() == 0)
                        throw new InvalidOperationException("There is no 0.4H in input file");
                    _firestItem = Entries.First().CurrentInfos.Where(info => info.Layer == "0.6H");
                    if (_firestItem == null | _firestItem.Count() == 0)
                        throw new InvalidOperationException("There is no 0.6H in input file");
                    _firestItem = Entries.First().CurrentInfos.Where(info => info.Layer == "0.8H");
                    if (_firestItem == null | _firestItem.Count() == 0)
                        throw new InvalidOperationException("There is no 0.8H in input file");
                    _firestItem = Entries.First().CurrentInfos.Where(info => info.Layer == "底层");
                    if (_firestItem == null | _firestItem.Count() == 0)
                        throw new InvalidOperationException("There is no 底层 in input file");
                    foreach (var _e in Entries)
                    {
                        var _0_H = _e.CurrentInfos.Where(info => info.Layer == "表层").First();
                        var _0_2H = _e.CurrentInfos.Where(info => info.Layer == "0.2H").First();
                        var _0_4H = _e.CurrentInfos.Where(info => info.Layer == "0.4H").First();
                        var _0_6H = _e.CurrentInfos.Where(info => info.Layer == "0.6H").First();
                        var _0_8H = _e.CurrentInfos.Where(info => info.Layer == "0.8H").First();
                        var _0_1H = _e.CurrentInfos.Where(info => info.Layer == "底层").First();
                        _tuples.Add(Tuple.Create(_e.RecordTime, (_0_H.Velocity + 2 * _0_2H.Velocity + 2 * _0_4H.Velocity + 2 * _0_6H.Velocity + 2 * _0_8H.Velocity + _0_1H.Velocity) / 10,
                                                                (_0_H.Direction + 2 * _0_2H.Direction + 2 * _0_4H.Direction + 2 * _0_6H.Direction + 2 * _0_8H.Direction + _0_1H.Direction) / 10));
                    }
                    break;

            }
            return _tuples;
        }
    }

    public class MoniterEntry : ObservableObject
    {

        private DateTime _recordTime;
        /// <summary>
        /// Property Description
        /// </summary>
        public DateTime RecordTime
        {
            get { return _recordTime; }
            set { Set(ref _recordTime, value); }
        }

        private ObservableCollection<CurrentInfo> _currentInfos = new ObservableCollection<CurrentInfo>();
        /// <summary>
        /// Property Description
        /// </summary>
        public ObservableCollection<CurrentInfo> CurrentInfos
        {
            get { return _currentInfos; }
            set { Set(ref _currentInfos, value); }
        }

    }

    public class CurrentInfo : ObservableObject
    {
        private string _layer;
        /// <summary>
        /// Current Layer: 1-> Bottom; 0-> top; 0.6 -> 0.6H
        /// </summary>
        public string Layer
        {
            get { return _layer; }
            set { Set(ref _layer, value); }
        }

        private double _velocity;
        /// <summary>
        /// Unit: cm/s
        /// </summary>
        public double Velocity
        {
            get { return _velocity; }
            set { Set(ref _velocity, value); }
        }

        private double _direction;
        /// <summary>
        /// Unit: degree
        /// </summary>
        public double Direction
        {
            get { return _direction; }
            set { Set(ref _direction, value); }
        }
    }

    public enum AverageMethod
    {
        oneLayer,
        TwoLayers,
        ThreeLayers,
        FiveLayer,
        SixLayer
    }

    public enum PickMenthod
    {
        [Description("PM_All")]
        All,
        [Description("PM_WholeHour")]
        WholeHour
    }
}
