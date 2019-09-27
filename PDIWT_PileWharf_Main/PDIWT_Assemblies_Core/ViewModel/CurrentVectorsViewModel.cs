using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using BD = Bentley.DgnPlatformNET;
using BM = Bentley.MstnPlatformNET;
using BG = Bentley.GeometryNET;
using System.Windows.Controls;
using System.Windows.Forms;
using OfficeOpenXml;
using OfficeOpenXml.Packaging;

namespace PDIWT_Assemblies_Core.ViewModel
{
    using GalaSoft.MvvmLight.Ioc;
    using Model;
    using Model.Tools;
    using System.Collections.ObjectModel;

    public class CurrentVectorsViewModel : ViewModelBase
    {
        readonly BM.MessageCenter _mc = BM.MessageCenter.Instance;

        private bool _isImportFile = false;
        /// <summary>
        /// Property Description
        /// </summary>
        public bool IsImportFile
        {
            get { return _isImportFile; }
            set { Set(ref _isImportFile, value); }
        }

        private ObservableCollection<string> _monitorStations = new ObservableCollection<string>();
        /// <summary>
        /// Property Description
        /// </summary>
        public ObservableCollection<string> MonitorStations
        {
            get { return _monitorStations; }
            set { Set(ref _monitorStations, value); }
        }

        private string _selectedStation;
        /// <summary>
        /// Property Description
        /// </summary>
        public string SelectedStation
        {
            get { return _selectedStation; }
            set { Set(ref _selectedStation, value); }
        }

        private ObservableCollection<string> _drawDataSource = new ObservableCollection<string>();
        /// <summary>
        /// Property Description
        /// </summary>
        public ObservableCollection<string> DrawDataSource
        {
            get { return _drawDataSource; }
            set { Set(ref _drawDataSource, value); }
        }

        private string _selectedDataSource;
        /// <summary>
        /// Property Description
        /// </summary>
        public string SelectedDataSource
        {
            get { return _selectedDataSource; }
            set { Set(ref _selectedDataSource, value); }
        }

        private PickMenthod? _selectedPickMethod = null;
        /// <summary>
        /// Property Description
        /// </summary>
        public PickMenthod? SelectedPickMethod
        {
            get { return _selectedPickMethod; }
            set { Set(ref _selectedPickMethod, value); }
        }

        private RelayCommand _browse;

        /// <summary>
        /// Gets the Browse.
        /// </summary>
        public RelayCommand Browse
        {
            get
            {
                return _browse
                    ?? (_browse = new RelayCommand(ExecuteBrowse));
            }
        }

        private List<CurrentMonitorSheet> _monitorsheets = new List<CurrentMonitorSheet>();
        private void ExecuteBrowse()
        {
            try
            {
                DrawDataSource.Clear();
                OpenFileDialog _ofd = new OpenFileDialog
                {
                    Filter = PDIWT.Resources.Localization.MainModule.Resources.ExcelFileFilter,
                    Multiselect = false
                    //InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                };
                if (_ofd.ShowDialog() == DialogResult.OK)
                {
                    using (ExcelPackage _package = new ExcelPackage(_ofd.OpenFile()))
                    {
                        foreach (var _sheet in _package.Workbook.Worksheets)
                        {
                            _monitorsheets.Add(new CurrentMonitorSheet(_sheet));
                        }
                    }
                }
                if (_monitorsheets.Count == 0)
                    return;

                var _firstMS = _monitorsheets.First();
                var _firstEtInfos = _firstMS.Entries.First().CurrentInfos.ToList();
                foreach (var _info in _firstEtInfos)
                {
                    DrawDataSource.Add(_info.Layer);
                }
                if (_firstEtInfos.Exists(c => c.Layer == "0.6H"))
                    DrawDataSource.Add("1层");
                if (_firstEtInfos.Exists(c => c.Layer == "0.2H") &&
                    _firstEtInfos.Exists(c => c.Layer == "0.8H"))
                    DrawDataSource.Add("2层");
                if (_firstEtInfos.Exists(c => c.Layer == "表层") &&
                    _firstEtInfos.Exists(c => c.Layer == "0.6H") &&
                    _firstEtInfos.Exists(c => c.Layer == "底层"))
                    DrawDataSource.Add("3层");
                if (_firstEtInfos.Exists(c => c.Layer == "表层") &&
                    _firstEtInfos.Exists(c => c.Layer == "0.2H") &&
                    _firstEtInfos.Exists(c => c.Layer == "0.6H") &&
                    _firstEtInfos.Exists(c => c.Layer == "0.8H") &&
                    _firstEtInfos.Exists(c => c.Layer == "底层"))
                    DrawDataSource.Add("5层");
                if (_firstEtInfos.Exists(c => c.Layer == "表层") &&
                    _firstEtInfos.Exists(c => c.Layer == "0.2H") &&
                    _firstEtInfos.Exists(c => c.Layer == "0.4H") &&
                    _firstEtInfos.Exists(c => c.Layer == "0.6H") &&
                    _firstEtInfos.Exists(c => c.Layer == "0.8H") &&
                    _firstEtInfos.Exists(c => c.Layer == "底层"))
                    DrawDataSource.Add("6层");
                MonitorStations = new ObservableCollection<string>(_monitorsheets.Select(s => s.StationNumber));
                SelectedStation = MonitorStations.First();
                SelectedDataSource = DrawDataSource.First();
                SelectedPickMethod = PickMenthod.All;
                IsImportFile = true;
            }
            catch (Exception e)
            {
                _mc.ShowErrorMessage(e.Message, e.ToString(), false);
            }
        }

        private RelayCommand draw;

        /// <summary>
        /// Gets the Draw.
        /// </summary>
        public RelayCommand Draw
        {
            get
            {
                return draw
                    ?? (draw = new RelayCommand(ExecuteDraw));
            }
        }

        private void ExecuteDraw()
        {
            try
            {
                List<List<Tuple<DateTime, double, double>>> _dataToDraw = new List<List<Tuple<DateTime, double, double>>>();
                if (SelectedDataSource == "1层")
                {
                    foreach (var _sheet in _monitorsheets)
                        _dataToDraw.Add(_sheet.GetCalculateInfos(AverageMethod.oneLayer).ToList());
                }
                else if (SelectedDataSource == "2层")
                {
                    foreach (var _sheet in _monitorsheets)
                        _dataToDraw.Add(_sheet.GetCalculateInfos(AverageMethod.TwoLayers).ToList());
                }
                else if (SelectedDataSource == "3层")
                {
                    foreach (var _sheet in _monitorsheets)
                        _dataToDraw.Add(_sheet.GetCalculateInfos(AverageMethod.ThreeLayers).ToList());
                }
                else if (SelectedDataSource == "5层")
                {
                    foreach (var _sheet in _monitorsheets)
                        _dataToDraw.Add(_sheet.GetCalculateInfos(AverageMethod.FiveLayer).ToList());
                }
                else if (SelectedDataSource == "6层")
                {
                    foreach (var _sheet in _monitorsheets)
                        _dataToDraw.Add(_sheet.GetCalculateInfos(AverageMethod.SixLayer).ToList());
                }
                else
                {
                    foreach (var _sheet in _monitorsheets)
                    {
                        var _currentInfos = from _entry in _sheet.Entries
                                            from _info in _entry.CurrentInfos
                                            where _info.Layer == SelectedDataSource
                                            select Tuple.Create(_entry.RecordTime, _info.Velocity, _info.Direction);
                        _dataToDraw.Add(_currentInfos.ToList());
                    }
                }

                List<List<Tuple<DateTime, double, double>>> _finalData = new List<List<Tuple<DateTime, double, double>>>();
                switch (SelectedPickMethod)
                {
                    case PickMenthod.All:
                        _finalData = _dataToDraw;
                        break;
                    case PickMenthod.WholeHour:
                        foreach (var _dataSheet in _dataToDraw)
                        {
                            var _data = from _entry in _dataSheet
                                        where _entry.Item1.Minute == 0 && _entry.Item1.Second == 0
                                        select _entry;
                            _finalData.Add(_data.ToList());
                        }
                        break;
                    default:
                        break;
                }
                CurrentVectorTool _tool = new CurrentVectorTool(AddIn);
                _tool.InstallNewInstance(MonitorStations.ToList() ,SelectedStation, _finalData);
            }
            catch (Exception e)
            {
                _mc.ShowErrorMessage(e.Message, e.ToString(), false);
            }
        }
        public BM.AddIn AddIn { get { return SimpleIoc.Default.GetInstance<BM.AddIn>(); } }
    }
}
