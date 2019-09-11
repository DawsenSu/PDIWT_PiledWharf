using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;

using PDIWT_PiledWharf_Core_Cpp;
using PDIWT_PiledWharf_Core.Model;
using PDIWT.Resources.Localization.MainModule;

using BM = Bentley.MstnPlatformNET;
using BD = Bentley.DgnPlatformNET;
using BDE = Bentley.DgnPlatformNET.Elements;
using BG = Bentley.GeometryNET;
using BDEC = Bentley.DgnPlatformNET.DgnEC;
using BES = Bentley.ECObjects.Schema;
using BEX = Bentley.ECObjects.XML;
using BECN = Bentley.ECN;
using BE = Bentley.ECObjects;
using BEPQ = Bentley.EC.Persistence.Query;
using BCI = Bentley.ECObjects.Instance;

using OfficeOpenXml;

namespace PDIWT_PiledWharf_Core.ViewModel
{
    using Model.Tools;

    public class Input_ImportFromFileViewModel : ViewModelBase
    {
        public Input_ImportFromFileViewModel()
        {
            //_pileGeoTypes = PDIWT.Resources.PDIWT_Helper.GetEnumDescriptionDictionary<PileTypeManaged>();
            _pileName = "Unknown";
            _selectedPileGeoType = PileTypeManaged.SqaurePile;
#if DEBUG
            _pileWidth = 0.6;
            _pileInsideDiameter = 0.5;
#endif
            _pileTipTypes = PDIWT.Resources.PDIWT_Helper.GetEnumDescriptionDictionary<PileTipType>();
            _concreteCoreLength = 0;
            _selecetedPileTipType = PileTipType.TotalSeal; 
            //_pileTipSealTypes = new List<string> { Resources.PileTip_TotalSeal, Resources.PileTip_HalfSeal, Resources.PileTip_SingleBorad, Resources.PileTip_DoubleBoard, Resources.PileTip_QuadBoard };
            //_selectedPileTipType = _pileTipSealTypes[0];
            //_pileDataTable = new DataTable("PileDataTable");
            //_pileDataTable.Columns.Add("PileNumber",typeof(long));
            //_pileDataTable.Columns.Add("TopX", typeof(double));
            //_pileDataTable.Columns.Add("TopY", typeof(double));
            //_pileDataTable.Columns.Add("TopZ", typeof(double));
            //_pileDataTable.Columns.Add("Length", typeof(double));
            //_pileDataTable.Columns.Add("Skewness", typeof(double));
            //_pileDataTable.Columns.Add("AxisRotAngle", typeof(double));

            //_isStaadFileChecked = true;
            //_isExcelFileChecked = false;
            //_filePath = string.Empty;
            //_pileSpatialInforamtion = new ObservableCollection<PileBase>();
        }

        private BM.MessageCenter _mc = BM.MessageCenter.Instance;

        //private Dictionary<PileTypeManaged, string> _pileGeoTypes;
        ///// <summary>
        ///// Pile Geo Type dictionary, to let use to choose.
        ///// </summary>
        //public Dictionary<PileTypeManaged, string> PileGeoTypes
        //{
        //    get { return _pileGeoTypes; }
        //    set { Set(ref _pileGeoTypes, value); }
        //}

        private string _pileName;
        /// <summary>
        /// Property Description
        /// </summary>
        public string PileName
        {
            get { return _pileName; }
            set { Set(ref _pileName, value); }
        }

        private PileTypeManaged _selectedPileGeoType;
        /// <summary>
        /// Property Description
        /// </summary>
        public PileTypeManaged SelectedPileGeoType
        {
            get { return _selectedPileGeoType; }
            set { Set(ref _selectedPileGeoType, value); }
        }

        private double _pileWidth;
        /// <summary>
        /// unit: m
        /// </summary>
        public double PileWidth
        {
            get { return _pileWidth; }
            set
            {
                if (value < 0)
                    Set(ref _pileWidth, 0);
                else
                    Set(ref _pileWidth, value);
            }
        }

        private double _pileInsideDiameter;
        /// <summary>
        /// unit: m
        /// </summary>
        public double PileInsideDiameter
        {
            get { return _pileInsideDiameter; }
            set
            {
                if (value < 0)
                    Set(ref _pileInsideDiameter, 0);
                else
                    Set(ref _pileInsideDiameter, value);
            }
        }

        private double _concreteCoreLength;
        /// <summary>
        /// unit: m
        /// </summary>
        public double ConcreteCoreLength
        {
            get { return _concreteCoreLength; }
            set
            {
                if (value < 0)
                    Set(ref _concreteCoreLength, 0);
                else
                    Set(ref _concreteCoreLength, value);
            }
        }

        private Dictionary<PileTipType,string> _pileTipTypes;
        /// <summary>
        /// The dictionary used to let user select pile Tip types
        /// </summary>
        public Dictionary<PileTipType,string> PileTipTypes
        {
            get { return _pileTipTypes; }
            set { Set(ref _pileTipTypes, value); }
        }

        private PileTipType _selecetedPileTipType;
        /// <summary>
        /// Property Description
        /// </summary>
        public PileTipType SelectedPileTipType
        {
            get { return _selecetedPileTipType; }
            set { Set(ref _selecetedPileTipType, value); }
        }

        public BM.AddIn AddIn { get { return SimpleIoc.Default.GetInstance<BM.AddIn>(); } }

        //private DataTable _pileDataTable;
        ///// <summary>
        ///// Pile Data table Variables
        ///// </summary>
        //public DataTable PileDataTable
        //{
        //    get { return _pileDataTable; }
        //    set { Set(ref _pileDataTable, value); }
        //}

        //private bool _isStaadFileChecked;
        ///// <summary>
        ///// the staad file radio button is checked
        ///// </summary>
        //public bool IsStaadFileChecked
        //{
        //    get { return _isStaadFileChecked; }
        //    set { Set(ref _isStaadFileChecked, value); }
        //}

        //private bool _isExcelFileChecked;
        ///// <summary>
        ///// the excel file radio button is checked
        ///// </summary>
        //public bool IsExcelFileChecked
        //{
        //    get { return _isExcelFileChecked; }
        //    set { Set(ref _isExcelFileChecked, value); }
        //}

        //private string _filePath;
        ///// <summary>
        ///// the excel file radio button is checked
        ///// </summary>
        //public string FilePath
        //{
        //    get { return _filePath; }
        //    set { Set(ref _filePath, value); }
        //}



        //private RelayCommand _browseFile;

        ///// <summary>
        ///// Gets the BrowseFile.
        ///// </summary>
        //public RelayCommand BrowseFile
        //{
        //    get
        //    {
        //        return _browseFile
        //            ?? (_browseFile = new RelayCommand(ExecuteBrowseFile));
        //    }
        //}

        //private void ExecuteBrowseFile()
        //{
        //    try
        //    {
        //        // Clear the data in datagrid before browse new file
        //        //PileSpatialInformation.Clear();

        //        using (OpenFileDialog _openFileDialog = new OpenFileDialog())
        //        {
        //            if (_isStaadFileChecked)
        //                _openFileDialog.Filter = Resources.StaadFileFilter;
        //            if (_isExcelFileChecked)
        //                _openFileDialog.Filter = Resources.ExcelFileFilter;
        //            _openFileDialog.Multiselect = false;
        //            _openFileDialog.InitialDirectory = Path.GetDirectoryName(BM.Session.Instance.GetActiveFileName());

        //            if (_openFileDialog.ShowDialog() == DialogResult.OK)
        //            {
        //                FilePath = _openFileDialog.FileName;
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        MessageBox.Show(e.Message, Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}

        private RelayCommand _draw;

        /// <summary>
        /// Gets the Draw. Create Pile by using the tool.
        /// </summary>
        public RelayCommand Draw
        {
            get
            {
                return _draw
                    ?? (_draw = new RelayCommand(ExecuteDraw));
            }
        }

        private void ExecuteDraw()
        {
            try
            {
                PilePlacementTool _tool = new PilePlacementTool(AddIn);
                _tool.InstallNewInstance(Tuple.Create(PileName,SelectedPileGeoType, PileWidth, PileInsideDiameter, ConcreteCoreLength, SelectedPileTipType));

                //// send the pile infos to tool
                //Messenger.Default.Send(new NotificationMessage<Tuple<PileTypeManaged, double, double, double, PileTipType>>(
                //    , "DrawPile"), "DrawPile");
                // send the tool show or hide the mian window
                Messenger.Default.Send(Visibility.Hidden, "ImportWindowVisibility");
            }
            catch (Exception e)
            {
                _mc.ShowErrorMessage("Can't create the pile", e.ToString(), false);
            }

        }

        private RelayCommand _import;

        /// <summary>
        /// Gets the ImportFromFile.
        /// </summary>
        public RelayCommand Import
        {
            get
            {
                return _import
                    ?? (_import = new RelayCommand(ExecuteImport));
            }
        }

        /// <summary>
        /// Build pile from excel file which contains column:
        /// top x(m), top y(m), z(m), pileskewness(vertical is NaN), length(m), Rotation Angle(°）
        /// </summary>
        private void ExecuteImport()
        {
            try
            {
                // Clear the data in datagrid before browse new file
                //PileSpatialInformation.Clear();
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

                        List<PileBase> _pileList = new List<PileBase>();
                        if (!_excelFile.Exists)
                            throw new ArgumentException($"{_excelFile} doesn't exist");

                        string _pileName;
                        double _topx, _topy, _topz, _skewness, _length, _rotationAngel;
                        using (var _excelPackage = new ExcelPackage(_excelFile))
                        {
                            var _worksheet = _excelPackage.Workbook.Worksheets[1];
                            for (int i = 2; i < _worksheet.Dimension.Rows + 1; i++)
                            {
                                _pileName = _worksheet.Cells[i, 1].Text;
                                _topx = double.Parse(_worksheet.Cells[i, 2].Text) * _uorpermeter;
                                _topy = double.Parse(_worksheet.Cells[i, 3].Text) * _uorpermeter;
                                _topz = double.Parse(_worksheet.Cells[i, 4].Text) * _uorpermeter;
                                _skewness = double.Parse(_worksheet.Cells[i, 5].Text);
                                _length = double.Parse(_worksheet.Cells[i, 6].Text) * _uorpermeter;
                                _rotationAngel = double.Parse(_worksheet.Cells[i, 7].Text);
                                BG.Angle _angle = new BG.Angle(); _angle.Degrees = _rotationAngel;
                                PileBase _pile = PileBase.CreateFromTopPointandLength(new BG.DPoint3d(_topx, _topy, _topz),
                                    _length, _skewness, _angle, SelectedPileGeoType, 
                                    PileWidth * _uorpermeter, 
                                    PileInsideDiameter * _uorpermeter, 
                                    ConcreteCoreLength * _uorpermeter, 
                                    SelectedPileTipType);
                                _pile.Name = _pileName;
                                _pile.DrawInActiveModel();
                                _numberofpiles++;
                            }
                        }                        
                    }
                }
                _mc.ShowInfoMessage($"Create {_numberofpiles} piles successfully.", "", false);
            }
            catch (Exception e)
            {
                _mc.ShowErrorMessage("Can't import piles from files.", e.ToString(), false);
            }
            //try
            //{
            //    PiledWharfStaadGenerator _generator = new PiledWharfStaadGenerator();
            //    List<PileBase> _piles = null;
            //    if (_isStaadFileChecked)
            //    {
            //        _piles = _generator.BuildPileListFromStaadFile(new FileInfo(_filePath));

            //    }
            //    if (_isExcelFileChecked)
            //    {
            //        _piles = _generator.BuildPileListFromExcel(new FileInfo(_filePath));
            //    }

            //    if (_piles != null)
            //    {
            //        PileSpatialInformation.Clear();
            //        foreach (var _pile in _piles)
            //            PileSpatialInformation.Add(_pile);
            //    }
            //}
            //catch (Exception e)
            //{
            //    MessageBox.Show(e.Message,Resources.Error,MessageBoxButton.OK,MessageBoxImage.Error);
            //}

        }

        //private bool CanExecuteImportFromFile()
        //{
        //    return File.Exists(_filePath);
        //}

        //private ObservableCollection<PileBase> _pileSpatialInforamtion;
        ///// <summary>
        ///// Property Description
        ///// </summary>
        //public ObservableCollection<PileBase> PileSpatialInformation
        //{
        //    get { return _pileSpatialInforamtion; }
        //    set { Set(ref _pileSpatialInforamtion, value); }
        //}

        //private RelayCommand<Grid> _createPiles;

        ///// <summary>
        ///// Gets the CreatePiles.
        ///// </summary>
        //public RelayCommand<Grid> CreatePiles
        //{
        //    get
        //    {
        //        return _createPiles ?? (_createPiles = new RelayCommand<Grid>(
        //            ExecuteCreatePiles,
        //            CanExecuteCreatePiles));
        //    }
        //}

        //private void ExecuteCreatePiles(Grid mainGrid)
        //{
        //    try
        //    {
        //        if (PileSpatialInformation == null || PileSpatialInformation.Count == 0)
        //            return;
        //        BD.DgnModel _activeDgnModel = BM.Session.Instance.GetActiveDgnModel();
        //        double uorpermm = BM.Session.Instance.GetActiveDgnModel().GetModelInfo().UorPerMaster;
        //        foreach (var _pileSpInfo in PileSpatialInformation)
        //        {
        //            //BDE.LineElement _line = new BDE.LineElement(_activeDgnModel, null, new Bentley.GeometryNET.DSegment3d(_pileSpInfo.TopJoint.Point, _pileSpInfo.BottomJoint.Point));
        //            //_line.AddToModel();

        //            BG.DPoint3d _topPoint = new BG.DPoint3d(_pileSpInfo.TopJoint.Point.X,
        //                                                    _pileSpInfo.TopJoint.Point.Y,
        //                                                    _pileSpInfo.TopJoint.Point.Z );
        //            BG.DPoint3d _bottomPoint = new BG.DPoint3d(_pileSpInfo.BottomJoint.Point.X,
        //                                                        _pileSpInfo.BottomJoint.Point.Y,
        //                                                        _pileSpInfo.BottomJoint.Point.Z);
        //            _topPoint.ScaleInPlace(uorpermm);
        //            _bottomPoint.ScaleInPlace(uorpermm);
        //            PDIWT_PiledWharf_Core_Cpp.EntityCreation.CreatePile(SelectedPileGeoType,
        //                                                               _pileGeoTypes,
        //                                                               _pileWidth * uorpermm, 
        //                                                               _pileInsideDiameter * uorpermm, 
        //                                                               _concreteCoreLength * uorpermm, 
        //                                                               _topPoint, 
        //                                                               _bottomPoint);
        //        }
        //        BM.MessageCenter.Instance.ShowInfoMessage(Resources.Success, Resources.Success, BM.MessageAlert.Balloon);
        //    }
        //    catch (Exception e)
        //    {
        //        MessageBox.Show(e.Message, Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}

        //private bool CanExecuteCreatePiles(Grid mainGrid)
        //{
        //    return _pileSpatialInforamtion.Count != 0 && !PDIWT.Resources.PDIWT_Helper.EnumTextBoxHasError(mainGrid);
        //}

        //private PDIWT_PiledWharf_Core_Cpp.PileTypeManaged SelectedPileTypeToEnum()
        //{
        //    if (_selectedPileGeoType == Resources.SquarePile)
        //        return PDIWT_PiledWharf_Core_Cpp.PileTypeManaged.SqaurePile;
        //    else if (_selectedPileGeoType == Resources.TubePile)
        //        return PDIWT_PiledWharf_Core_Cpp.PileTypeManaged.TubePile;
        //    else if (_selectedPileGeoType == Resources.PHCTubePile)
        //        return PDIWT_PiledWharf_Core_Cpp.PileTypeManaged.PHCTubePile;
        //    else
        //        return PDIWT_PiledWharf_Core_Cpp.PileTypeManaged.SteelTubePile;
        //}
    }


}
