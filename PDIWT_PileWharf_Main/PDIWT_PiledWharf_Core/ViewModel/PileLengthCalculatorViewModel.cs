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

            _virtualPileTable = new DataTable("PileSpatialInfomation");
            _virtualPileTable.Columns.Add(new DataColumn("PileName", typeof(string)) { DefaultValue = PileBase._unknownPileName }); //0
            _virtualPileTable.Columns.Add(new DataColumn("PileX", typeof(double)) { DefaultValue = 0 });//1 unit: m
            _virtualPileTable.Columns.Add(new DataColumn("PileY", typeof(double)) { DefaultValue = 0 });//2 unit: m
            _virtualPileTable.Columns.Add(new DataColumn("PileZ", typeof(double)) { DefaultValue = 0 });//3 unit: m
            _virtualPileTable.Columns.Add(new DataColumn("BearingCapacityPileType", typeof(BearingCapacityPileTypes)) { DefaultValue = BearingCapacityPileTypes.DrivenPileWithSealedEnd });//4
            _virtualPileTable.Columns.Add(new DataColumn("PileGeoType", typeof(PileTypeManaged)) { DefaultValue = PileTypeManaged.SqaurePile });//5
            _virtualPileTable.Columns.Add(new DataColumn("PileSkewness", typeof(double)) { DefaultValue = double.NaN });//6
            _virtualPileTable.Columns.Add(new DataColumn("PlanRotationAngle", typeof(double)) { DefaultValue = 0 });//7 Degree
            _virtualPileTable.Columns.Add(new DataColumn("PileDiameter", typeof(double)) { DefaultValue = 0 });//8 unit: m
            _virtualPileTable.Columns.Add(new DataColumn("PileInnerDiameter", typeof(double)) { DefaultValue = 0 });//9 unit: m
            _virtualPileTable.Columns.Add(new DataColumn("PileLength", typeof(double)) { DefaultValue = double.NaN });//10 unit: m
            _virtualPileTable.Columns.Add(new DataColumn("DataResources", typeof(ChartValues<ObservablePoint>)) { DefaultValue = new  ChartValues<ObservablePoint>() }); //11 unit: IS
            _virtualPileTable.Columns.Add(new DataColumn("IsCalculated", typeof(bool)) { DefaultValue = false });//12 unit: 

#if DEBUG
            DataRow _row = _virtualPileTable.NewRow();
            _row[0] = "Test Piles";
            _row[1] = 480770;
            _row[2] = 2500575D;
            _row[3] = 16;
            _row[4] = BearingCapacityPileTypes.TubePileOrSteelPile;
            _row[5] = PileTypeManaged.TubePile;
            _row[6] = double.NaN;
            _row[7] = 0;
            _row[8] = 1;
            _row[9] = 0.8;
            _virtualPileTable.Rows.Add(_row);
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


        private DataTable _virtualPileTable;
        /// <summary>
        /// Property Description
        /// </summary>
        public DataTable VirtualPileTable
        {
            get { return _virtualPileTable; }
            set { Set(ref _virtualPileTable, value); }
        }


        private DataRowView _selectedVirtualPile;
        /// <summary>
        /// Property Description
        /// </summary>
        public DataRowView SelectedVirtualPile
        {
            get { return _selectedVirtualPile; }
            set { Set(ref _selectedVirtualPile, value); }
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
                            DataRow _row = _virtualPileTable.NewRow();
                            _row[0] = "New Pile";
                            VirtualPileTable.Rows.Add(_row);
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
                        var _currentPile = SelectedVirtualPile.Row;
                        if (VirtualPileTable.Rows.Count == 1)
                        {
                            SelectedVirtualPile = null;
                        }
                        else
                        {
                            int _index = VirtualPileTable.Rows.IndexOf(_currentPile);

                            if (_index == VirtualPileTable.Rows.Count - 1)
                            {
                                SelectedVirtualPile = VirtualPileTable.DefaultView[_index - 1];
                            }
                            else
                            {
                                SelectedVirtualPile = VirtualPileTable.DefaultView[_index + 1];
                            }
                        }
                        _currentPile.Delete() ;
                    }, () => SelectedVirtualPile != null));
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
                                DataRow _newRow = VirtualPileTable.NewRow();
                                _newRow[0] = _worksheet.Cells[i, 1].Text;
                                _newRow[1] = double.Parse(_worksheet.Cells[i, 2].Text);
                                _newRow[2] = double.Parse(_worksheet.Cells[i, 3].Text);
                                _newRow[3] = double.Parse(_worksheet.Cells[i, 4].Text);
                                _newRow[4] = (BearingCapacityPileTypes)double.Parse(_worksheet.Cells[i, 5].Text);
                                _newRow[5] = (PileTypeManaged)double.Parse(_worksheet.Cells[i, 6].Text);
                                _newRow[6] = double.Parse(_worksheet.Cells[i, 7].Text);
                                _newRow[7] = double.Parse(_worksheet.Cells[i, 8].Text);
                                _newRow[8] = double.Parse(_worksheet.Cells[i, 9].Text);
                                _newRow[9] = double.Parse(_worksheet.Cells[i, 10].Text);
                                VirtualPileTable.Rows.Add(_newRow);
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
                foreach (DataRow _row in VirtualPileTable.Rows)
                {
                    VirtualSteelOrTubePile _pile = new VirtualSteelOrTubePile()
                    {
                        PileName = _row[0].ToString(),
                        PileX = (double)_row[1] * _uorpermeter,
                        PileY = (double)_row[2] * _uorpermeter,
                        PileZ = (double)_row[3] * _uorpermeter,
                        BCPileType = (BearingCapacityPileTypes)_row[4],
                        GeoPileType = (PileTypeManaged)_row[5],
                        PileSkewness = (double)_row[6],
                        PlanRotationAngle = (double)_row[7],
                        PileDiameter = (double)_row[8] * _uorpermeter,
                        PileInnerDiameter = (double)_row[9] * _uorpermeter,
                    };
                    //_row[8] = double.NaN;
                    Marshal.mdlDialog_completionBarUpdate(_dialog, "Calculating", (int)(++_totalNumber / VirtualPileTable.Rows.Count * 100));
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
                    _row[11] = _chartPoints;

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

                    _row[10] = _pile.PileLength / _uorpermeter;
                    _row[12] = true;
                    Bentley.UI.Threading.DispatcherHelper.DoEvents();
                    _successPileNumber++;
                }
                _mc.ShowInfoMessage("Calculation Complete", $"Calculated: {VirtualPileTable.Rows.Count}, succeed: {_successPileNumber}, fail: {VirtualPileTable.Rows.Count - _successPileNumber} ", false);
            }
            catch (Exception e)
            {
                _mc.ShowErrorMessage("Can't calculate the pile", e.ToString(), false);
            }
            finally
            {
                Marshal.mdlDialog_completionBarClose(_dialog);

            }
        }
    }
}
