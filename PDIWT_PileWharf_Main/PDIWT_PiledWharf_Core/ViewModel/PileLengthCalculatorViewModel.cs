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

namespace PDIWT_PiledWharf_Core.ViewModel
{
    using LiveCharts.Defaults;
    using Model;
    using OfficeOpenXml;

    using Common;

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
            //_virtualPiles = new ObservableCollection<VirtualSteelOrTubePile>();
            //double _uorpermm = BM.Session.Instance.GetActiveDgnModel().GetModelInfo().UorPerMeter / 1000;
            //_virtualPiles.Add(new VirtualSteelOrTubePile()
            //{
            //    PileX = 480770 * 1000,
            //    PileY = 2500575.0 * 1000,
            //    PileZ = 16 * 1000,
            //});
            //_virtualPiles.Add(new VirtualSteelOrTubePile()
            //{
            //    PileX = 480770 * 1000,
            //    PileY = 2500575.0 * 1000,
            //    PileZ = 16 * 1000,
            //    PileSkewness = 7,
            //    PlanRotationAngle = 90
            //});
            _virtualPileTable = new DataTable("PileSpatialInfomation");
            _virtualPileTable.Columns.Add(new DataColumn("PileName", typeof(string)) { DefaultValue = PileBase._unknownPileName }); //0
            _virtualPileTable.Columns.Add(new DataColumn("PileX", typeof(double)) { DefaultValue = 0 });//1 unit: m
            _virtualPileTable.Columns.Add(new DataColumn("PileY", typeof(double)) { DefaultValue = 0 });//2 unit: m
            _virtualPileTable.Columns.Add(new DataColumn("PileZ", typeof(double)) { DefaultValue = 0 });//3 unit: m
            _virtualPileTable.Columns.Add(new DataColumn("PileSkewness", typeof(double)) { DefaultValue = double.NaN });//4
            _virtualPileTable.Columns.Add(new DataColumn("PlanRotationAngle", typeof(double)) { DefaultValue = 0 });//5 Degree
            _virtualPileTable.Columns.Add(new DataColumn("PileDiameter", typeof(double)) { DefaultValue = 0 });//6 unit: m
            _virtualPileTable.Columns.Add(new DataColumn("PileInnerDiameter", typeof(double)) { DefaultValue = 0 });//7 unit: m
            _virtualPileTable.Columns.Add(new DataColumn("PileLength", typeof(double)) { DefaultValue = double.NaN});//8 unit: m

            DataRow _row = _virtualPileTable.NewRow();
            _row[1] = 480770;
            _row[2] = 2500575.0 ;
            _row[3] = 16;
            _row[4] = double.NaN;
            _row[5] = 0;
            _row[6] = 1;
            _row[7] = 0.8;
            _virtualPileTable.Rows.Add(_row);
        }

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
                if(value < 0)
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
        
        //private ObservableCollection<VirtualSteelOrTubePile> _virtualPiles;
        ///// <summary>
        ///// Property Description
        ///// </summary>
        //public ObservableCollection<VirtualSteelOrTubePile> VirtualPiles
        //{
        //    get { return _virtualPiles; }
        //    set { Set(ref _virtualPiles, value); }
        //}

        private DataRowView _selectedVirtualPile;
        /// <summary>
        /// Property Description
        /// </summary>
        public DataRowView SelectedVirtualPile
        {
            get { return _selectedVirtualPile; }
            set { Set(ref _selectedVirtualPile, value); }
        }

        //private VirtualSteelOrTubePile _selectedVirtualPile;
        ///// <summary>
        ///// Property Description
        ///// </summary>
        //public VirtualSteelOrTubePile SelectedVirtualPile
        //{
        //    get { return _selectedVirtualPile; }
        //    set { Set(ref _selectedVirtualPile, value); }
        //}

        private RelayCommand _showCurve;

        /// <summary>
        /// Gets the ShowCurve.
        /// </summary>
        public RelayCommand ShowCurve
        {
            get
            {
                return _showCurve
                    ?? (_showCurve = new RelayCommand(ExecuteShowCurve,
                    ()=> SelectedVirtualPile != null));
            }
        }
        private void ExecuteShowCurve()
        {
            try
            {
                double _uorpermeter = BM.Session.Instance.GetActiveDgnModel().GetModelInfo().UorPerMeter;
                VirtualSteelOrTubePile _pile = new VirtualSteelOrTubePile()
                {
                    PileName = SelectedVirtualPile[0].ToString(),
                    PileX = (double)SelectedVirtualPile[1] * _uorpermeter,
                    PileY = (double)SelectedVirtualPile[2] * _uorpermeter,
                    PileZ = (double)SelectedVirtualPile[3] * _uorpermeter,
                    PileSkewness = (double)SelectedVirtualPile[4],
                    PlanRotationAngle = (double)SelectedVirtualPile[5],
                    PileDiameter = (double)SelectedVirtualPile[6] * _uorpermeter,
                    PileInnerDiameter = (double)SelectedVirtualPile[7] * _uorpermeter,
                };
                switch (_pile.GetPileBearingCapacityCurveInfo(PartialCoefficient, BlockCoefficient, out ObservableCollection<Tuple<double, double>> _results))
                {
                    case GetPileBearingCapacityCurveInfoStatus.InvalidObjectStruct:
                        _mc.ShowMessage(BM.MessageType.Warning, "Invalid input parameter or internal error", _pile.ToString(), BM.MessageAlert.None);
                        return;
                    case GetPileBearingCapacityCurveInfoStatus.NoIntersection:
                        _mc.ShowMessage(BM.MessageType.Warning, "No intersection between soil layer and pile", _pile.ToString(), BM.MessageAlert.None);
                        return;
                    case GetPileBearingCapacityCurveInfoStatus.Success:
                        break;
                }

                LiveCharts.ChartValues<ObservablePoint> _chartPoints = new LiveCharts.ChartValues<ObservablePoint>();
                foreach (var _result in _results)
                {
                    _chartPoints.Add(new ObservablePoint(_result.Item2, _result.Item1 / _uorpermeter));
                }

                PileLengthCurveWindow _curveWindow = new PileLengthCurveWindow();
                PileLengthCurveViewModel _vm = new PileLengthCurveViewModel()
                {
                    SeriesCollection = new LiveCharts.SeriesCollection
                    {
                        new LiveCharts.Wpf.LineSeries{Title="PileLength/m",Values = _chartPoints, LineSmoothness=0}
                    }
                };
                _curveWindow.DataContext = _vm;
                _curveWindow.ShowDialog();
            }
            catch (Exception e)
            {
                _mc.ShowErrorMessage("Unknown reason: can't show BC - length curve", e.ToString(), false);
            }

            //if (BD.StatusInt.Success == PDIWT_SoilLayerInfoReader.ObtainSoilLayerInfoFromModel(out _soilLayerInfoTuples) &&
            //    BD.StatusInt.Success == SelectedVirtualPile.GetPileBearingCapacityCurveInfo(_targetBearingCapacity, _partialCoefficient, _blockCoefficient, _soilLayerInfoTuples, out _results))
            //{
            //    BG.DRay3d _axisRay = SelectedVirtualPile.GetPileRay3D();

            //    LiveCharts.ChartValues<ObservablePoint> _chartPoints = new LiveCharts.ChartValues<ObservablePoint>();
            //    foreach (var _result in _results)
            //    {
            //        _chartPoints.Add(new ObservablePoint(_result.Item2, _result.Item1 / 10000));
            //        _chartPoints.Add(new ObservablePoint(_result.Item4, _result.Item3 / 10000));
            //    }

            //    PileLengthCurveWindow _curveWindow = new PileLengthCurveWindow();
            //    PileLengthCurveViewModel _vm = new PileLengthCurveViewModel()
            //    {
            //        SeriesCollection = new LiveCharts.SeriesCollection
            //        {
            //            new LiveCharts.Wpf.LineSeries{Title="PileLength/m",Values = _chartPoints, LineSmoothness=0}
            //        }
            //    };
            //    _curveWindow.DataContext = _vm;
            //    _curveWindow.ShowDialog();
            //}
            //else
            //{
            //    BM.MessageCenter.Instance.ShowErrorMessage("Can't obtain the pile length curve information", "", false);
            //}
        }
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

                        //List<PileBase> _pileList = new List<PileBase>();
                        if (!_excelFile.Exists)
                            throw new ArgumentException($"{_excelFile} doesn't exist");

                        using (var _excelPackage = new ExcelPackage(_excelFile))
                        {
                            var _worksheet = _excelPackage.Workbook.Worksheets[1];
                            for (int i = 2; i < _worksheet.Dimension.Rows + 1; i++)
                            {
                                DataRow _newRow = VirtualPileTable.NewRow();
                                _newRow[1] = double.Parse(_worksheet.Cells[i, 1].Text);
                                _newRow[2] = double.Parse(_worksheet.Cells[i, 2].Text);
                                _newRow[3] = double.Parse(_worksheet.Cells[i, 3].Text);
                                _newRow[4] = double.Parse(_worksheet.Cells[i, 4].Text);
                                _newRow[5] = double.Parse(_worksheet.Cells[i, 5].Text);
                                _newRow[6] = double.Parse(_worksheet.Cells[i, 6].Text);
                                _newRow[7] = double.Parse(_worksheet.Cells[i, 7].Text);

                                _numberofpiles++;
                                VirtualPileTable.Rows.Add(_newRow);
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
            //var _firstVP = VirtualPiles.First();
            //ObservableCollection<Tuple<double, double, double, double>> _results;
            //List<Tuple<BD.Elements.MeshHeaderElement, Dictionary<string, object>>> _soilLayerInfoTuples;

            //if (BD.StatusInt.Success == PDIWT_SoilLayerInfoReader.ObtainSoilLayerInfoFromModel(out _soilLayerInfoTuples) &&
            //    BD.StatusInt.Success == _firstVP.GetPileBearingCapacityCurveInfo(_targetBearingCapacity, _partialCoefficient, _blockCoefficient, _soilLayerInfoTuples, out _results))
            //{
            //    StringBuilder _sb = new StringBuilder();
            //    foreach (var _soilLayer in _soilLayerInfoTuples)
            //    {
            //        _sb.AppendFormat("{0:F2}, 1:F2}", _soilLayer.Item2["qfi"], _soilLayer.Item2["qr"]);
            //        //_sb.AppendFormat("{0:F2} , {1:F2}, {2:F2}, {3:F2}\n", _tuple.Item1, _tuple.Item2, _tuple.Item3, _tuple.Item4);
            //    }
            //    MessageBox.Show(_sb.ToString());
            //}
            //else
            //{
            //    MessageBox.Show("Fail");
            //}
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
            try
            {
                double _uorpermeter = BM.Session.Instance.GetActiveDgnModel().GetModelInfo().UorPerMeter;
                double _successPileNumber = 0;
                double _totalNumber = 0;
                IntPtr _dialog = Marshal.mdlDialog_completionBarOpen("Start");
                foreach (DataRow _row in VirtualPileTable.Rows)
                {
                    VirtualSteelOrTubePile _pile = new VirtualSteelOrTubePile()
                    {
                        PileName = _row[0].ToString(),
                        PileX = (double)_row[1] * _uorpermeter,
                        PileY = (double)_row[2] * _uorpermeter,
                        PileZ = (double)_row[3] * _uorpermeter,
                        PileSkewness = (double)_row[4],
                        PlanRotationAngle = (double)_row[5],
                        PileDiameter = (double)_row[6] * _uorpermeter,
                        PileInnerDiameter = (double)_row[7] * _uorpermeter,
                    };
                    _row[8] = double.NaN;
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
                    _row[8] = _pile.PileLength / _uorpermeter;
                    Bentley.UI.Threading.DispatcherHelper.DoEvents();
                    _successPileNumber++;
                }
                Marshal.mdlDialog_completionBarClose(_dialog);
                _mc.ShowInfoMessage("Calculation Complete",$"Calculated: {VirtualPileTable.Rows.Count}, succeed: {_successPileNumber}, fail: {VirtualPileTable.Rows.Count - _successPileNumber} ",false);

            }
            catch (Exception e)
            {
                _mc.ShowErrorMessage("Can't calculate the pile", e.ToString(), false);
            }
                //if (SelectedVirtualPile == null)
                //    return;
                //ObservableCollection<Tuple<double, double, double, double>> _results;
                //List<Tuple<BD.Elements.MeshHeaderElement, Dictionary<string, object>>> _soilLayerInfoTuples;

                //if (BD.StatusInt.Success == PDIWT_SoilLayerInfoReader.ObtainSoilLayerInfoFromModel(out _soilLayerInfoTuples) &&
                //    BD.StatusInt.Success == SelectedVirtualPile.GetPileBearingCapacityCurveInfo(_targetBearingCapacity, _partialCoefficient, _blockCoefficient, _soilLayerInfoTuples, out _results))
                //{
                //    if (SelectedVirtualPile.CalculatePileLength(_targetBearingCapacity,_pileLengthModulus, _results) == CalculatePileLengthStatues.Success)
                //    {
                //        SelectedVirtualPile.DrawInActiveModel();
                //    }
                //}
            }
    }
}
