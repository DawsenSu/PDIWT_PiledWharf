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
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;

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

using MessageBox = System.Windows.MessageBox;
using System.Collections.ObjectModel;

namespace PDIWT_PiledWharf_Core.ViewModel
{
    public class Input_ImportFromFileViewModel : ViewModelBase
    {
        public Input_ImportFromFileViewModel()
        {
            _pileTypes = new List<string> { Resources.SquarePile, Resources.TubePile, Resources.PHCTubePile, Resources.SteelTubePile };
            _selectedPileType = _pileTypes[0];
            _pileWidth = 1.0;
            _pileInsideDiameter = 0.9;
            _concreteCoreLength = 1.0;
            _pileTipSealTypes = new List<string> { Resources.PileTip_TotalSeal, Resources.PileTip_HalfSeal, Resources.PileTip_SingleBorad, Resources.PileTip_DoubleBoard, Resources.PileTip_QuadBoard };
            _selectedPileTipType = _pileTipSealTypes[0];
            _isStaadFileChecked = true;
            _isExcelFileChecked = false;
            _filePath = string.Empty;
            _pileSpatialInforamtion = new ObservableCollection<Pile>();
        }


        private List<string> _pileTypes;
        /// <summary>
        /// Property Description
        /// </summary>
        public List<string> PileTypes
        {
            get { return _pileTypes; }
            set { Set(ref _pileTypes, value); }
        }

        private string _selectedPileType;
        /// <summary>
        /// Property Description
        /// </summary>
        public string SelectedPileType
        {
            get { return _selectedPileType; }
            set { Set(ref _selectedPileType, value); }
        }

        private double _pileWidth;
        /// <summary>
        /// Property Description
        /// </summary>
        public double PileWidth
        {
            get { return _pileWidth; }
            set { Set(ref _pileWidth, value); }
        }

        private double _pileInsideDiameter;
        /// <summary>
        /// Property Description
        /// </summary>
        public double PileInsideDiameter
        {
            get { return _pileInsideDiameter; }
            set { Set(ref _pileInsideDiameter, value); }
        }

        private double _concreteCoreLength;
        /// <summary>
        /// Property Description
        /// </summary>
        public double ConcreteCoreLength
        {
            get { return _concreteCoreLength; }
            set { Set(ref _concreteCoreLength, value); }
        }

        private List<string> _pileTipSealTypes;
        /// <summary>
        /// Property Description
        /// </summary>
        public List<string> PileTipSealTypes
        {
            get { return _pileTipSealTypes; }
            set { Set(ref _pileTipSealTypes, value); }
        }


        private string _selectedPileTipType;
        /// <summary>
        /// Property Description
        /// </summary>
        public string SelectedPileTipType
        {
            get { return _selectedPileTipType; }
            set { Set(ref _selectedPileTipType, value); }
        }

        private bool _isStaadFileChecked;
        /// <summary>
        /// the staad file radio button is checked
        /// </summary>
        public bool IsStaadFileChecked
        {
            get { return _isStaadFileChecked; }
            set { Set(ref _isStaadFileChecked, value); }
        }

        private bool _isExcelFileChecked;
        /// <summary>
        /// the excel file radio button is checked
        /// </summary>
        public bool IsExcelFileChecked
        {
            get { return _isExcelFileChecked; }
            set { Set(ref _isExcelFileChecked, value); }
        }

        private string _filePath;
        /// <summary>
        /// the excel file radio button is checked
        /// </summary>
        public string FilePath
        {
            get { return _filePath; }
            set { Set(ref _filePath, value); }
        }


        private RelayCommand _browseFile;

        /// <summary>
        /// Gets the BrowseFile.
        /// </summary>
        public RelayCommand BrowseFile
        {
            get
            {
                return _browseFile
                    ?? (_browseFile = new RelayCommand(ExecuteBrowseFile));
            }
        }

        private void ExecuteBrowseFile()
        {
            try
            {
                using (OpenFileDialog _openFileDialog = new OpenFileDialog())
                {
                    if (_isStaadFileChecked)
                        _openFileDialog.Filter = Resources.StaadFileFilter;
                    if (_isExcelFileChecked)
                        _openFileDialog.Filter = Resources.ExcelFileFilter;
                    _openFileDialog.Multiselect = false;
                    _openFileDialog.InitialDirectory = Path.GetDirectoryName(BM.Session.Instance.GetActiveFileName());

                    if (_openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        FilePath = _openFileDialog.FileName;
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private RelayCommand _importFromFile;

        /// <summary>
        /// Gets the ImportFromFile.
        /// </summary>
        public RelayCommand ImportFromFile
        {
            get
            {
                return _importFromFile
                    ?? (_importFromFile = new RelayCommand(ExecuteImportFromFile, CanExecuteImportFromFile));
            }
        }

        private void ExecuteImportFromFile()
        {
            try
            {
                PileWharfStaadGenerator _generator = new PileWharfStaadGenerator();
                List<Pile> _piles = null;
                if (_isStaadFileChecked)
                {
                    _piles = _generator.BuildPileListFromStaadFile(new FileInfo(_filePath));

                }
                if (_isExcelFileChecked)
                {
                    _piles = _generator.BuildJointListFromExcel(new FileInfo(_filePath));
                }

                if (_piles != null)
                {
                    PileSpatialInformation.Clear();
                    foreach (var _pile in _piles)
                        PileSpatialInformation.Add(_pile);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message,Resources.Error,MessageBoxButton.OK,MessageBoxImage.Error);
            }

        }

        private bool CanExecuteImportFromFile()
        {
            return File.Exists(_filePath);
        }

        private ObservableCollection<Pile> _pileSpatialInforamtion;
        /// <summary>
        /// Property Description
        /// </summary>
        public ObservableCollection<Pile> PileSpatialInformation
        {
            get { return _pileSpatialInforamtion; }
            set { Set(ref _pileSpatialInforamtion, value); }
        }

        private RelayCommand<Grid> _createPiles;

        /// <summary>
        /// Gets the CreatePiles.
        /// </summary>
        public RelayCommand<Grid> CreatePiles
        {
            get
            {
                return _createPiles ?? (_createPiles = new RelayCommand<Grid>(
                    ExecuteCreatePiles,
                    CanExecuteCreatePiles));
            }
        }

        private void ExecuteCreatePiles(Grid mainGrid)
        {
            //TODO: To accomplish Draw pile function 
            try
            {
                if (PileSpatialInformation == null || PileSpatialInformation.Count == 0)
                    return;
                BD.DgnModel _activeDgnModel = BM.Session.Instance.GetActiveDgnModel();
                foreach (var _pileSpInfo in PileSpatialInformation)
                {
                    BDE.LineElement _line = new BDE.LineElement(_activeDgnModel, null, new Bentley.GeometryNET.DSegment3d(_pileSpInfo.TopJoint.Point, _pileSpInfo.BottomJoint.Point));
                    _line.AddToModel();
                }
                BM.MessageCenter.Instance.ShowInfoMessage(Resources.Success, Resources.Success, BM.MessageAlert.Balloon);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanExecuteCreatePiles(Grid mainGrid)
        {
            return _pileSpatialInforamtion.Count != 0 && !PDIWT_Helper.EnumTextBoxHasError(mainGrid);
        }
    }


}
