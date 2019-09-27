using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using System.Windows.Forms;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Controls;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;

using PDIWT.Formulas;
using PDIWT_PiledWharf_Core.Model.Tools;
using PDIWT.Resources.Localization.MainModule;
using BD = Bentley.DgnPlatformNET;
using BM = Bentley.MstnPlatformNET;
using BG = Bentley.GeometryNET;
using PDIWT_PiledWharf_Core_Cpp;


namespace PDIWT_PiledWharf_Core.ViewModel
{
    using LiveCharts.Defaults;
    using Model;
    using OfficeOpenXml;

    using Common;
    using LiveCharts;

    public class PileLengthCalculatorViewModel : ViewModelBase
    {
        public PileLengthCalculatorViewModel()
        {
#if DEBUG
            _targetBearingCapacity = 1000;
            _pileLengthModulus = 0.5;
#endif
            _partialCoefficient = 1.5;
            _blockCoefficient = 1;

            PileLengthCalcInfos = new ObservableCollection<PileLengthCalcInfo>();
            //_virtualPileTable = new DataTable("PileSpatialInfomation");
            //_virtualPileTable.Columns.Add(new DataColumn("PileName", typeof(string)) { DefaultValue = PileBase._unknownPileName }); //0
            //_virtualPileTable.Columns.Add(new DataColumn("PileX", typeof(double)) { DefaultValue = 0 });//1 unit: m
            //_virtualPileTable.Columns.Add(new DataColumn("PileY", typeof(double)) { DefaultValue = 0 });//2 unit: m
            //_virtualPileTable.Columns.Add(new DataColumn("PileZ", typeof(double)) { DefaultValue = 0 });//3 unit: m
            //_virtualPileTable.Columns.Add(new DataColumn("BearingCapacityPileType", typeof(BearingCapacityPileTypes)) { DefaultValue = BearingCapacityPileTypes.DrivenPileWithSealedEnd });//4
            //_virtualPileTable.Columns.Add(new DataColumn("PileGeoType", typeof(PileTypeManaged)) { DefaultValue = PileTypeManaged.SqaurePile });//5
            //_virtualPileTable.Columns.Add(new DataColumn("PileSkewness", typeof(double)) { DefaultValue = double.NaN });//6
            //_virtualPileTable.Columns.Add(new DataColumn("PlanRotationAngle", typeof(double)) { DefaultValue = 0 });//7 Degree
            //_virtualPileTable.Columns.Add(new DataColumn("PileDiameter", typeof(double)) { DefaultValue = 0 });//8 unit: m
            //_virtualPileTable.Columns.Add(new DataColumn("PileInnerDiameter", typeof(double)) { DefaultValue = 0 });//9 unit: m
            //_virtualPileTable.Columns.Add(new DataColumn("PileLength", typeof(double)) { DefaultValue = double.NaN });//10 unit: m
            //_virtualPileTable.Columns.Add(new DataColumn("DataResources", typeof(ChartValues<ObservablePoint>)) { DefaultValue = new  ChartValues<ObservablePoint>() }); //11 unit: IS
            //_virtualPileTable.Columns.Add(new DataColumn("IsCalculated", typeof(bool)) { DefaultValue = false });//12 unit: 

#if DEBUG
            PileLengthCalcInfos.Add(new PileLengthCalcInfo()
            {
                PileName = "Test Pile",
                PileX = 480770,
                PileY = 2500575D,
                PileZ = 16,
                BearingCapacityPileType = BearingCapacityPileTypes.TubePileOrSteelPile,
                PileGeoType = PileTypeManaged.TubePile,
                PlanRotationAngle = 0,
                PileDiameter = 1,
                PileInnerDiameter = 0.8
            });
            //DataRow _row = _virtualPileTable.NewRow();
            //_row[0] = "Test Piles";
            //_row[1] = 480770;
            //_row[2] = 2500575D;
            //_row[3] = 16;
            //_row[4] = BearingCapacityPileTypes.TubePileOrSteelPile;
            //_row[5] = PileTypeManaged.TubePile;
            //_row[6] = double.NaN;
            //_row[7] = 0;
            //_row[8] = 1;
            //_row[9] = 0.8;
            //_virtualPileTable.Rows.Add(_row);
#endif
        }

        //private void _virtualPileTable_RowDeleting(object sender, DataRowChangeEventArgs e)
        //{
        //    int index = 1;
        //    for (int i = 0; i < e.Row.Table.Rows.Count; i++)
        //    {
        //        if (e.Row.Table.Rows[i] == e.Row)
        //            continue;
        //        e.Row.Table.Rows[i][9] = index;
        //        index++;
        //    }
        //}

        //private void _virtualPileTable_RowChanged(object sender, DataRowChangeEventArgs e)
        //{
        //    if (e.Action == DataRowAction.Add)
        //    {
        //        for (int i = 0; i < e.Row.Table.Rows.Count; i++)
        //        {
        //            e.Row.Table.Rows[i][9] = i + 1;
        //        }
        //    }
        //}

        private readonly BM.MessageCenter _mc = BM.MessageCenter.Instance;

        private double _targetBearingCapacity;
        /// <summary>
        /// Property Description
        /// </summary>
        public double TargetBearingCapacity
        {
            get { return _targetBearingCapacity; }
            set { Set(ref _targetBearingCapacity, value); }
        }


        private double _pileLengthModulus;
        /// <summary>
        /// meter
        /// </summary>
        public double PileLengthModulus
        {
            get { return _pileLengthModulus; }
            set
            {
                if (value < 0)
                    Set(ref _pileLengthModulus, 0);
                else
                    Set(ref _pileLengthModulus, value);
            }
        }

        private double _partialCoefficient;
        /// <summary>
        /// Property Description
        /// </summary>
        public double PartialCoefficient
        {
            get { return _partialCoefficient; }
            set
            {
                if (value < 0)
                    Set(ref _partialCoefficient, 1);
                else
                    Set(ref _partialCoefficient, value);
            }
        }

        private double _blockCoefficient;
        /// <summary>
        /// Property Description
        /// </summary>
        public double BlockCoefficient
        {
            get { return _blockCoefficient; }
            set
            {
                if (value < 0)
                    Set(ref _blockCoefficient, 1);
                else
                    Set(ref _blockCoefficient, value);
            }
        }


        //private DataTable _virtualPileTable;
        ///// <summary>
        ///// Property Description
        ///// </summary>
        //public DataTable VirtualPileTable
        //{
        //    get { return _virtualPileTable; }
        //    set { Set(ref _virtualPileTable, value); }
        //}


        //private DataRowView _selectedVirtualPile;
        ///// <summary>
        ///// Property Description
        ///// </summary>
        //public DataRowView SelectedVirtualPile
        //{
        //    get { return _selectedVirtualPile; }
        //    set { Set(ref _selectedVirtualPile, value); }
        //}


        public ObservableCollection<PileLengthCalcInfo> PileLengthCalcInfos { get; set; }


        private PileLengthCalcInfo _selectedPileLengthCalcInfo;
        /// <summary>
        /// Property Description
        /// </summary>
        public PileLengthCalcInfo SelectedPileLengthCalcInfo
        {
            get { return _selectedPileLengthCalcInfo; }
            set { Set(ref _selectedPileLengthCalcInfo, value); }
        }


        private RelayCommand _add;

        /// <summary>
        /// Gets the CreatePile.
        /// </summary>
        public RelayCommand Add
        {
            get
            {
                return _add
                    ?? (_add = new RelayCommand(
                        () =>
                        {
                            PileLengthCalcInfos.Add(new PileLengthCalcInfo() { PileName = "New Pile" });
                        }));
            }
        }

        private RelayCommand _delete;

        /// <summary>
        /// Gets the DeletePile.
        /// </summary>
        public RelayCommand Delete
        {
            get
            {
                return _delete
                    ?? (_delete = new RelayCommand(()=> 
                    {
                        var _currentPile = SelectedPileLengthCalcInfo;
                        if (PileLengthCalcInfos.Count == 1)
                        {
                            SelectedPileLengthCalcInfo = null;
                        }
                        else
                        {
                            int _index = PileLengthCalcInfos.IndexOf(_currentPile);

                            if (_index == PileLengthCalcInfos.Count - 1)
                            {
                                SelectedPileLengthCalcInfo = PileLengthCalcInfos[_index - 1];
                            }
                            else
                            {
                                SelectedPileLengthCalcInfo = PileLengthCalcInfos[_index + 1];
                            }
                        }
                        PileLengthCalcInfos.Remove(_currentPile);
                    }));
            }
        }

        private RelayCommand _clearAll;

        /// <summary>
        /// Gets the ClearAll.
        /// </summary>
        public RelayCommand ClearAll
        {
            get
            {
                return _clearAll
                    ?? (_clearAll = new RelayCommand(
                    () =>
                    {
                        PileLengthCalcInfos.Clear();
                    }));
            }
        }

        //private RelayCommand _showCurve;

        ///// <summary>
        ///// Gets the ShowCurve.
        ///// </summary>
        //public RelayCommand ShowCurve
        //{
        //    get
        //    {
        //        return _showCurve
        //            ?? (_showCurve = new RelayCommand(
        //                () => Messenger.Default.Send<PileLengthCalculatorWindowType, PileLengthCalculatorWindow>(PileLengthCalculatorWindowType.CurveWidnow),
        //                () => SelectedVirtualPile != null));
        //    }
        //}

        private RelayCommand _importFromFile;

        /// <summary>
        /// Gets the LoadParameters.
        /// </summary>
        public RelayCommand ImportFromFile
        {
            get
            {
                return _importFromFile
                    ?? (_importFromFile = new RelayCommand(ExecuteImportFromFile));
            }
        }
        /// <summary>
        /// Import pile table from excel file
        ///   top x(m), top y(m), z(m), pileskewness(vertical is NaN), Rotation Angle(°）,Diameter(m), InnerDiameter(m)
        /// </summary>
        private void ExecuteImportFromFile()
        {
            try
            {
                double _uorpermeter = BM.Session.Instance.GetActiveDgnModel().GetModelInfo().UorPerMeter;
                int _numberofpiles = 0;
                using (OpenFileDialog _openFileDialog = new OpenFileDialog())
                {
                    _openFileDialog.Filter = Resources.ExcelFileFilter;
                    _openFileDialog.Multiselect = false;
                    _openFileDialog.InitialDirectory = Path.GetDirectoryName(BM.Session.Instance.GetActiveFileName());

                    if (_openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        FileInfo _excelFile = new FileInfo(_openFileDialog.FileName);

                        if (!_excelFile.Exists)
                            throw new ArgumentException($"{_excelFile} doesn't exist");

                        using (var _excelPackage = new ExcelPackage(_excelFile))
                        {
                            var _worksheet = _excelPackage.Workbook.Worksheets[1];
                            for (int i = 2; i < _worksheet.Dimension.Rows + 1; i++)
                            {
                                var _newPile = new PileLengthCalcInfo()
                                {
                                    PileName = _worksheet.Cells[i, 1].Text,
                                    PileX = double.Parse(_worksheet.Cells[i, 2].Text),
                                    PileY = double.Parse(_worksheet.Cells[i, 3].Text),
                                    PileZ = double.Parse(_worksheet.Cells[i, 4].Text),
                                    BearingCapacityPileType = (BearingCapacityPileTypes)double.Parse(_worksheet.Cells[i, 5].Text),
                                    PileGeoType = (PileTypeManaged)double.Parse(_worksheet.Cells[i, 6].Text),
                                    PileSkewness = double.Parse(_worksheet.Cells[i, 7].Text),
                                    PlanRotationAngle = double.Parse(_worksheet.Cells[i, 8].Text),
                                    PileDiameter = double.Parse(_worksheet.Cells[i, 9].Text),
                                    PileInnerDiameter = double.Parse(_worksheet.Cells[i, 10].Text)
                                };
                                PileLengthCalcInfos.Add(_newPile);
                                _numberofpiles++;
                            }
                        }
                    }
                }
                _mc.ShowInfoMessage($"Import {_numberofpiles} entries successfully", "", false);
            }
            catch (Exception e)
            {
                _mc.ShowErrorMessage("Can't import pile infomation from execl file", e.ToString(), false);
            }
        }

        private RelayCommand _calculate;

        /// <summary>
        /// Gets the Calculate.
        /// </summary>
        public RelayCommand Calculate
        {
            get
            {
                return _calculate
                    ?? (_calculate = new RelayCommand(ExecuteCalculate));
            }
        }

        private void ExecuteCalculate()
        {
            IntPtr _dialog = Marshal.mdlDialog_completionBarOpen("");
            try
            {
                double _uorpermeter = BM.Session.Instance.GetActiveDgnModel().GetModelInfo().UorPerMeter;
                double _successPileNumber = 0;
                double _totalNumber = 0;
                foreach (var _pileInfo in PileLengthCalcInfos)
                {
                    VirtualSteelOrTubePile _pile = new VirtualSteelOrTubePile()
                    {
                        PileName = _pileInfo.PileName,
                        PileX = _pileInfo.PileX * _uorpermeter,
                        PileY = _pileInfo.PileY * _uorpermeter,
                        PileZ = _pileInfo.PileZ * _uorpermeter,
                        BCPileType = _pileInfo.BearingCapacityPileType,
                        GeoPileType = _pileInfo.PileGeoType,
                        PileSkewness = _pileInfo.PileSkewness,
                        PlanRotationAngle = _pileInfo.PlanRotationAngle,
                        PileDiameter = _pileInfo.PileDiameter * _uorpermeter,
                        PileInnerDiameter = _pileInfo.PileInnerDiameter * _uorpermeter,
                    };
                    //_row[8] = double.NaN;
                    Marshal.mdlDialog_completionBarUpdate(_dialog, "Calculating", (int)(++_totalNumber / PileLengthCalcInfos.Count * 100));
                    switch (_pile.GetPileBearingCapacityCurveInfo(PartialCoefficient, BlockCoefficient, out ObservableCollection<Tuple<double, double>> _results))
                    {
                        case GetPileBearingCapacityCurveInfoStatus.InvalidObjectStruct:
                            _mc.ShowMessage(BM.MessageType.Warning, "Invalid input parameter or internal error", _pile.ToString(), BM.MessageAlert.None);
                            continue;
                        case GetPileBearingCapacityCurveInfoStatus.NoIntersection:
                            _mc.ShowMessage(BM.MessageType.Warning, "No intersection between soil layer and pile", _pile.ToString(), BM.MessageAlert.None);
                            continue;
                        case GetPileBearingCapacityCurveInfoStatus.Success:
                            break;
                    }
                    ChartValues<ObservablePoint> _chartPoints = new ChartValues<ObservablePoint>();
                    foreach (var _result in _results)
                    {
                        _chartPoints.Add(new ObservablePoint(_result.Item2, -_result.Item1 / _uorpermeter));
                    }
                    _pileInfo.DataResources = _chartPoints;

                    switch (_pile.CalculatePileLength(TargetBearingCapacity, PileLengthModulus * _uorpermeter, _results))
                    {
                        case CalculatePileLengthStatues.Success:
                            break;
                        case CalculatePileLengthStatues.TargetBearingCapacityIsTooLarge:
                            _mc.ShowMessage(BM.MessageType.Warning,
                                "Target bearing capacity is too large",
                                $"{TargetBearingCapacity}kN is larger than maximum capacity {_results.Last().Item2.ToString("F2")}kN at which pile {_pile.PileName} can get",
                                BM.MessageAlert.None);
                            continue;
                        case CalculatePileLengthStatues.NoLayerInfos:
                            _mc.ShowMessage(BM.MessageType.Warning, "No capacity curve is obtained", _results.ToString(), BM.MessageAlert.None);
                            continue;
                    }

                    _pileInfo.PileLength = _pile.PileLength / _uorpermeter;
                    _pileInfo.IsCalculated = true;
                    Bentley.UI.Threading.DispatcherHelper.DoEvents();
                    _successPileNumber++;
                }
                _mc.ShowInfoMessage("Calculation Complete", $"Calculated: {PileLengthCalcInfos.Count}, succeed: {_successPileNumber}, fail: {PileLengthCalcInfos.Count - _successPileNumber} ", false);
            }
            catch (Exception e)
            {
                if(e is InvalidOperationException)
                    _mc.ShowErrorMessage("There exists unset bearing capacity calculation value for soil layer", e.ToString(), false);
                else
                    _mc.ShowErrorMessage("Can't calculate the pile", e.ToString(), false);
            }
            finally
            {
                Marshal.mdlDialog_completionBarClose(_dialog);
            }
        }
    }

    public class PileLengthCalcInfo : ObservableObject
    {
        public PileLengthCalcInfo()
        {
            _pilename = PileBase._unknownPileName;
            _bearingcapacityPiletType = BearingCapacityPileTypes.DrivenPileWithSealedEnd;
            _pileGeoType = PileTypeManaged.SqaurePile;
            _pileSkewness = double.NaN;
            _pileLength = double.NaN;
            _dataResources = new ChartValues<ObservablePoint>();
            _isCalaculated = false; 
        }

        private string _pilename;
        /// <summary>
        /// Property Description
        /// </summary>
        public string PileName
        {
            get { return _pilename; }
            set { Set(ref _pilename, value); }
        }

        private double _pileX;
        /// <summary>
        /// Unit: m
        /// </summary>
        public double PileX
        {
            get { return _pileX; }
            set { Set(ref _pileX, value); }
        }

        private double _pileY;
        /// <summary>
        /// unit:m
        /// </summary>
        public double PileY
        {
            get { return _pileY; }
            set { Set(ref _pileY, value); }
        }

        private double _pileZ;
        /// <summary>
        /// unit:m
        /// </summary>
        public double PileZ
        {
            get { return _pileZ; }
            set { Set(ref _pileZ, value); }
        }

        private BearingCapacityPileTypes _bearingcapacityPiletType;
        /// <summary>
        /// Property Description
        /// </summary>
        public BearingCapacityPileTypes BearingCapacityPileType
        {
            get { return _bearingcapacityPiletType; }
            set { Set(ref _bearingcapacityPiletType, value); }
        }

        private PileTypeManaged _pileGeoType;
        /// <summary>
        /// Property Description
        /// </summary>
        public PileTypeManaged PileGeoType
        {
            get { return _pileGeoType; }
            set { Set(ref _pileGeoType, value); }
        }

        private double _pileSkewness;
        /// <summary>
        /// Property Description
        /// </summary>
        public double PileSkewness
        {
            get { return _pileSkewness; }
            set { Set(ref _pileSkewness, value); }
        }

        private double _planRotationAngle;
        /// <summary>
        /// Unit: Degree
        /// </summary>
        public double PlanRotationAngle
        {
            get { return _planRotationAngle; }
            set { Set(ref _planRotationAngle, value); }
        }

        private double _pileDiameter;
        /// <summary>
        /// Unit:m
        /// </summary>
        public double PileDiameter
        {
            get { return _pileDiameter; }
            set { Set(ref _pileDiameter, value); }
        }

        private double _pileInnerDiameter;
        /// <summary>
        /// Unit:m
        /// </summary>
        public double PileInnerDiameter
        {
            get { return _pileInnerDiameter; }
            set { Set(ref _pileInnerDiameter, value); }
        }

        private double _pileLength;
        /// <summary>
        /// Unti:m
        /// </summary>
        public double PileLength
        {
            get { return _pileLength; }
            set { Set(ref _pileLength, value); }
        }

        private ChartValues<ObservablePoint> _dataResources;
        /// <summary>
        /// Used by livechart, unit: SI.
        /// </summary>
        public ChartValues<ObservablePoint> DataResources
        {
            get { return _dataResources; }
            set { Set(ref _dataResources, value); }
        }

        private bool _isCalaculated;
        /// <summary>
        /// Property Description
        /// </summary>
        public bool IsCalculated
        {
            get { return _isCalaculated; }
            set { Set(ref _isCalaculated, value); }
        }
    }
}
