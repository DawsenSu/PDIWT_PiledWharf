using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;

using System.ComponentModel;
using System.Collections.ObjectModel;
using PDIWT.Formulas;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Controls.Primitives;
using PDIWT_PiledWharf_Core.Model.Tools;
using PDIWT.Resources.Localization.MainModule;
using BD = Bentley.DgnPlatformNET;
using BM = Bentley.MstnPlatformNET;
using System.Windows.Controls;

namespace PDIWT_PiledWharf_Core.ViewModel
{
    
    public class BearingCapacityViewModel : ViewModelBase
    {
        public BearingCapacityViewModel()
        {
            PileInfos = new ObservableCollection<PDIWT_BearingCapacity_PileInfo>()
            {
                new PDIWT_BearingCapacity_PileInfo
                {
                    PileCode = "Test Pile",
                    CalculatedWaterLevel = 2.4,
                    PileTopElevation = 2.71,
                    PileLength= 40,
                    PileDiameter = 0.65,
                    PileInsideDiameter = 0.33,
                    PileSoilLayersInfo = new ObservableCollection<PDIWT_BearingCapacity_SoilLayerInfo>()
                    {
                        new PDIWT_BearingCapacity_SoilLayerInfo(){SoilLayerNumber = "3-1", SoilLayerName="粉质粘土", SoilLayerThickness = 3.1, SideFrictionStandardValue = 33, EndResistanceStandardValue = 0, DiscountCoeff = 0.7},
                        new PDIWT_BearingCapacity_SoilLayerInfo(){SoilLayerNumber = "3-2", SoilLayerName="粉土", SoilLayerThickness = 3.0, SideFrictionStandardValue = 64, EndResistanceStandardValue = 0, DiscountCoeff = 0.7},
                        new PDIWT_BearingCapacity_SoilLayerInfo(){SoilLayerNumber = "4", SoilLayerName="粉质粘土", SoilLayerThickness = 4.4, SideFrictionStandardValue = 42, EndResistanceStandardValue = 0, DiscountCoeff = 0.7},
                        new PDIWT_BearingCapacity_SoilLayerInfo(){SoilLayerNumber = "5", SoilLayerName="粘土", SoilLayerThickness = 3.0, SideFrictionStandardValue = 32, EndResistanceStandardValue = 0, DiscountCoeff = 0.7},
                        new PDIWT_BearingCapacity_SoilLayerInfo(){SoilLayerNumber = "6-2", SoilLayerName="粉质粘土", SoilLayerThickness = 3.0, SideFrictionStandardValue = 60, EndResistanceStandardValue = 1800, DiscountCoeff = 0.7},
                        new PDIWT_BearingCapacity_SoilLayerInfo(){SoilLayerNumber = "6-1", SoilLayerName="粉土", SoilLayerThickness = 3.0, SideFrictionStandardValue = 90, EndResistanceStandardValue = 3200, DiscountCoeff = 0.7},
                        new PDIWT_BearingCapacity_SoilLayerInfo(){SoilLayerNumber = "6-3", SoilLayerName="粉细沙", SoilLayerThickness = 2.7, SideFrictionStandardValue = 120, EndResistanceStandardValue = 6100, DiscountCoeff = 0.5}
                    }
                }
            };

            SoilLayerInfosLib = new ObservableCollection<PDIWT_BearingCapacity_SoilLayerInfo>()
            {
                new PDIWT_BearingCapacity_SoilLayerInfo(){SoilLayerNumber = "1", SoilLayerName="淤泥",SideFrictionStandardValue = 4, EndResistanceStandardValue = 0, DiscountCoeff = 0.7},
                new PDIWT_BearingCapacity_SoilLayerInfo(){SoilLayerNumber = "2-1", SoilLayerName="淤泥质粉质粘土",  SideFrictionStandardValue = 5, EndResistanceStandardValue = 0, DiscountCoeff = 0.7},
                new PDIWT_BearingCapacity_SoilLayerInfo(){SoilLayerNumber = "2-2", SoilLayerName="淤泥质粘土", SideFrictionStandardValue = 8, EndResistanceStandardValue = 0, DiscountCoeff = 0.7},
                new PDIWT_BearingCapacity_SoilLayerInfo(){SoilLayerNumber = "3-1", SoilLayerName="粉质粘土",  SideFrictionStandardValue = 33, EndResistanceStandardValue = 0, DiscountCoeff = 0.7},
                new PDIWT_BearingCapacity_SoilLayerInfo(){SoilLayerNumber = "3-2", SoilLayerName="粉土",  SideFrictionStandardValue = 64, EndResistanceStandardValue = 0, DiscountCoeff = 0.7},
                new PDIWT_BearingCapacity_SoilLayerInfo(){SoilLayerNumber = "3-3", SoilLayerName="粉细砂",  SideFrictionStandardValue = 70, EndResistanceStandardValue = 0, DiscountCoeff = 0.5},
                new PDIWT_BearingCapacity_SoilLayerInfo(){SoilLayerNumber = "4", SoilLayerName="粉质粘土",  SideFrictionStandardValue = 42, EndResistanceStandardValue = 0, DiscountCoeff = 0.7},
                new PDIWT_BearingCapacity_SoilLayerInfo(){SoilLayerNumber = "5", SoilLayerName="粘土",  SideFrictionStandardValue = 32, EndResistanceStandardValue = 0, DiscountCoeff = 0.7},
                new PDIWT_BearingCapacity_SoilLayerInfo(){SoilLayerNumber = "6-1", SoilLayerName="粉土", SideFrictionStandardValue = 90, EndResistanceStandardValue = 3200, DiscountCoeff = 0.7},
                new PDIWT_BearingCapacity_SoilLayerInfo(){SoilLayerNumber = "6-2", SoilLayerName="粉质粘土",  SideFrictionStandardValue = 60, EndResistanceStandardValue = 1800, DiscountCoeff = 0.7},
                new PDIWT_BearingCapacity_SoilLayerInfo(){SoilLayerNumber = "6-3", SoilLayerName="粉细砂",  SideFrictionStandardValue = 120, EndResistanceStandardValue = 6100, DiscountCoeff = 0.7},
                new PDIWT_BearingCapacity_SoilLayerInfo(){SoilLayerNumber = "6-4", SoilLayerName="粉土",  SideFrictionStandardValue = 105, EndResistanceStandardValue = 4000, DiscountCoeff = 0.7},
                new PDIWT_BearingCapacity_SoilLayerInfo(){SoilLayerNumber = "7", SoilLayerName="粉质粘土",  SideFrictionStandardValue = 66, EndResistanceStandardValue = 2300, DiscountCoeff = 0.7},
                new PDIWT_BearingCapacity_SoilLayerInfo(){SoilLayerNumber = "8", SoilLayerName="粉土",  SideFrictionStandardValue = 132, EndResistanceStandardValue = 5000, DiscountCoeff = 0.7},
                new PDIWT_BearingCapacity_SoilLayerInfo(){SoilLayerNumber = "8-1", SoilLayerName="中沙", SideFrictionStandardValue = 150, EndResistanceStandardValue = 6000, DiscountCoeff = 0.5}
            };

            Messenger.Default.Register<NotificationMessage<ObservableCollection<PDIWT_BearingCapacity_SoilLayerInfo>>>(this, "BuildupSoilLayerLib",
                n => SoilLayerInfosLib = n.Content);
            Messenger.Default.Register<NotificationMessage<ObservableCollection<PDIWT_BearingCapacity_SoilLayerInfo>>>(this, "SelectedSoilLayers",
                n => SelectedPile.PileSoilLayersInfo = n.Content);
            Messenger.Default.Register<NotificationMessage<PDIWT_BearingCapacity_PileInfo>>(this, LoadParametersFromPileEntity);
        }

        private ObservableCollection<PDIWT_BearingCapacity_PileInfo> _pileinfos;
        /// <summary>
        /// Property Description
        /// </summary>
        public ObservableCollection<PDIWT_BearingCapacity_PileInfo> PileInfos
        {
            get { return _pileinfos; }
            set { Set(ref _pileinfos, value); }
        }

        private PDIWT_BearingCapacity_PileInfo _selectedPile;
        /// <summary>
        /// Property Description
        /// </summary>
        public PDIWT_BearingCapacity_PileInfo SelectedPile
        {
            get { return _selectedPile; }
            set { Set(ref _selectedPile, value); }
        }


        private ObservableCollection<PDIWT_BearingCapacity_SoilLayerInfo> _soilLayerInfosLib;
        /// <summary>
        /// Property Description
        /// </summary>
        public ObservableCollection<PDIWT_BearingCapacity_SoilLayerInfo> SoilLayerInfosLib
        {
            get { return _soilLayerInfosLib; }
            set { Set(ref _soilLayerInfosLib, value); }
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
                    ?? (_add = new RelayCommand(ExecuteAdd));
            }
        }

        private void ExecuteAdd()
        {
            PileInfos.Add(new PDIWT_BearingCapacity_PileInfo());
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
                    ?? (_delete = new RelayCommand(ExecuteDelete, () => SelectedPile != null));
            }
        }

        private void ExecuteDelete()
        {
            var _currentPile = SelectedPile;
            if (PileInfos.Count == 1)
            {
                SelectedPile = null;
            }
            else
            {
                int _index = PileInfos.IndexOf(_currentPile);

                if (_index == PileInfos.Count - 1)
                {
                    SelectedPile = PileInfos.ElementAt(_index - 1);
                }
                else
                {
                    SelectedPile = PileInfos.ElementAt(_index + 1);
                }
            }
            PileInfos.Remove(_currentPile);

        }

        private RelayCommand _loadParameters;

        /// <summary>
        /// Gets the LoadParameters.
        /// </summary>
        public RelayCommand LoadParameters
        {
            get
            {
                return _loadParameters
                    ?? (_loadParameters = new RelayCommand(ExecuteLoadParameters));
            }
        }

        private void ExecuteLoadParameters()
        {
            try
            {
                LoadPileParametersTool<PDIWT_BearingCapacity_PileInfo>.InstallNewInstance(LoadParametersToolEnablerProvider.AxialBearingCapacityPileInfoEnabler);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void LoadParametersFromPileEntity(NotificationMessage<PDIWT_BearingCapacity_PileInfo> notification)
        {
            try
            {
                if (notification.Notification == Resources.Error)
                    BM.MessageCenter.Instance.ShowErrorMessage("Can't Load Parameter From selected element", "Error", BM.MessageAlert.None);
                else
                {
                    PileInfos.Add(notification.Content);
                    SelectedPile = notification.Content;
                    BM.MessageCenter.Instance.ShowInfoMessage("SUCCESS", "Load Parameter", BM.MessageAlert.None);
                    //Registered by its view behind code, change the foreground color of calculation.
                    Messenger.Default.Send(new NotificationMessage<bool>(true, "Changed!"), "BearingCapacityForegroundChange");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private RelayCommand _buildupSoilLayerLib;

        /// <summary>
        /// Gets the BuildUpSoilLayerLib.
        /// </summary>
        public RelayCommand BuildUpSoilLayerLib
        {
            get
            {
                return _buildupSoilLayerLib
                    ?? (_buildupSoilLayerLib = new RelayCommand(ExecuteBuildUpSoilLayerLib));
            }
        }

        private void ExecuteBuildUpSoilLayerLib()
        {
            BuildUpSoilLayersWindow _window = new BuildUpSoilLayersWindow();
            BuildUpSoilLayersViewModel _vm = new BuildUpSoilLayersViewModel() { SoilLayerInfos = SoilLayerInfosLib };
            _window.DataContext = _vm;
            _window.ShowDialog();
        }

        private RelayCommand _pickupSoilLayersFromLib;

        /// <summary>
        /// Gets the PickupSoilLayersFromLib.
        /// </summary>
        public RelayCommand PickupSoilLayersFromLib
        {
            get
            {
                return _pickupSoilLayersFromLib
                    ?? (_pickupSoilLayersFromLib = new RelayCommand(ExecutePickupSoilLayersFromLib,
                    () => SelectedPile != null));
            }
        }

        private void ExecutePickupSoilLayersFromLib()
        {
            if (SoilLayerInfosLib.Count == 0)
            {
                MessageBox.Show("Please build up soil layer library first!", PDIWT.Resources.Localization.MainModule.Resources.Error, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            PickUpSoilLayersFromLibWindow _window = new PickUpSoilLayersFromLibWindow();
            PickUpSoilLayersFromLibViewModel _vm = new PickUpSoilLayersFromLibViewModel(SoilLayerInfosLib, _selectedPile.PileSoilLayersInfo);
            _window.DataContext = _vm;
            _window.ShowDialog();
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
                    ?? (_calculate = new RelayCommand(ExecuteCalculate, 
                    () => SelectedPile != null && SelectedPile.PileSoilLayersInfo != null & SelectedPile.PileSoilLayersInfo.Count != 0));
            }
        }

        private void ExecuteCalculate()
        {
            try
            {
                SelectedPile.PilePerimeter = AxialBearingCapacity.CalculatePilePrimeter(new ShapeInfo() { Value = 2 }, SelectedPile.PileDiameter);
                SelectedPile.PileCrossSectionArea = AxialBearingCapacity.CalculatePileEndOutsideArea(new ShapeInfo() { Value = 2 }, SelectedPile.PileDiameter);
                SelectedPile.PileBottomElevation = AxialBearingCapacity.CalculatePileBottomElevation(SelectedPile.PileTopElevation, SelectedPile.PileLength, SelectedPile.PileSkewness);
                // Pile Geo Type has been taken into consideration in following method.
                SelectedPile.PileSelfWeight = AxialBearingCapacity.CalculatePileSelfWeight(
                    SelectedPile.SelectedPileGeoType, SelectedPile.PileDiameter, SelectedPile.PileInsideDiameter, SelectedPile.PileTopElevation,
                    SelectedPile.PileLength, SelectedPile.PileSkewness, SelectedPile.CalculatedWaterLevel,
                    SelectedPile.ConcreteWeight, SelectedPile.ConcreteUnderwaterWeight, SelectedPile.SteelWeight, SelectedPile.SteelUnderwaterWeight, SelectedPile.ConcreteCoreLength);
                // Calculate Axial Bearing Capacity
                List<double> _betasi = (from _soilInfo in SelectedPile.PileSoilLayersInfo select _soilInfo.Betasi).ToList();
                List<double> _psii = (from _soilInfo in SelectedPile.PileSoilLayersInfo select _soilInfo.Psii).ToList();
                List<double> _qfi = (from _soilInfo in SelectedPile.PileSoilLayersInfo select _soilInfo.SideFrictionStandardValue).ToList();
                List<double> _li = (from _soilInfo in SelectedPile.PileSoilLayersInfo select _soilInfo.SoilLayerThickness).ToList();
                //last layer is the holding layer
                double _betap = (from _soilInfo in SelectedPile.PileSoilLayersInfo select _soilInfo.Betap).Last();
                double _psip = (from _soilInfo in SelectedPile.PileSoilLayersInfo select _soilInfo.Psip).Last();
                double _qr = (from _soilInfo in SelectedPile.PileSoilLayersInfo select _soilInfo.EndResistanceStandardValue).Last();
                double _axialBearingCapacity = 0;
                switch (SelectedPile.SelectedBearingCapacityPileType)
                {
                    case BearingCapacityPileTypes.DrivenPileWithSealedEnd:
                        _axialBearingCapacity = AxialBearingCapacity.DrivenPileBearingCapacity(SelectedPile.PartialCoeff, _qfi, _li, SelectedPile.PilePerimeter, _qr, SelectedPile.PileCrossSectionArea);
                        break;
                    case BearingCapacityPileTypes.TubePileOrSteelPile:
                        _axialBearingCapacity = AxialBearingCapacity.DrivenPileBearingCapacity(SelectedPile.PartialCoeff, _qfi, _li, SelectedPile.PilePerimeter, _qr, SelectedPile.PileCrossSectionArea, SelectedPile.BlockCoeff);
                        break;
                    case BearingCapacityPileTypes.CastInSituPile:
                        _axialBearingCapacity = AxialBearingCapacity.CalculateCasInSituPileBearingCapacity(SelectedPile.PartialCoeff, _qfi, _li, _psii, SelectedPile.PilePerimeter, _qr, SelectedPile.PileCrossSectionArea, _psip);
                        break;
                    case BearingCapacityPileTypes.CastInSituAfterGrountingPile:
                        _axialBearingCapacity = AxialBearingCapacity.CalculateCastInSituAfterGrountingPileBearingCapacity(SelectedPile.PartialCoeff, _qfi, _li, _psii,_betasi, SelectedPile.PilePerimeter, _qr, SelectedPile.PileCrossSectionArea, _psip,_betap);
                        break;
                    default:
                        break;
                }
                SelectedPile.DesignAxialBearingCapacity = _axialBearingCapacity;
                // Calculate Axial uplift capacity
                List<double> _discountsi = (from _soilInfo in SelectedPile.PileSoilLayersInfo select _soilInfo.DiscountCoeff).ToList();
                SelectedPile.DesignAxialUpliftCapacity = AxialBearingCapacity.CalculateDrivenAndCastInSituPileUpliftForce(SelectedPile.PartialCoeff, _qfi, _li, _discountsi,
                    SelectedPile.PilePerimeter, SelectedPile.PileSelfWeight,AxialBearingCapacity.CalculateCosAlpha(SelectedPile.PileSkewness));
                //Registered by its view behind code
                Messenger.Default.Send(new NotificationMessage<bool>(false, "Changed!"), "BearingCapacityForegroundChange");

            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

        }

        private RelayCommand _generateNote;

        /// <summary>
        /// Gets the GenerateNote.
        /// </summary>
        public RelayCommand GenerateNote
        {
            get
            {
                return _generateNote
                    ?? (_generateNote = new RelayCommand(
                        () =>
                        {
                            ReportGeneratorWindow _reportWindow = new ReportGeneratorWindow();
                            Messenger.Default.Send(new NotificationMessage(this, "BearingCapacityViewModelInvoke"), "ViewModelForReport");
                            _reportWindow.ShowDialog();
                        },
                        () => SelectedPile!=null && ( SelectedPile.DesignAxialBearingCapacity != 0 || SelectedPile.DesignAxialUpliftCapacity != 0)));
            }
        }

        private RelayCommand<SelectionChangedEventArgs> _selectionChanged;

        /// <summary>
        /// Gets the SelectionChanged. Handle pile list selection changed event.
        /// </summary>
        public RelayCommand<SelectionChangedEventArgs> SelectionChanged
        {
            get
            {
                return _selectionChanged
                    ?? (_selectionChanged = new RelayCommand<SelectionChangedEventArgs>(ExecuteSelectionChanged));
            }
        }

        private void ExecuteSelectionChanged(SelectionChangedEventArgs parameter)
        {
            // Register in view, send false to change foreground of controls to black.
            Messenger.Default.Send(new NotificationMessage<bool>(false, "Changed!"), "BearingCapacityForegroundChange");
        }

    }

    public class PDIWT_BearingCapacity_PileInfo : ObservableObject
    {
        public PDIWT_BearingCapacity_PileInfo()
        {
            _bearingCapacityPileCategory = PDIWT.Resources.PDIWT_Helper.GetEnumDescriptionDictionary<BearingCapacityPileTypes>();
            _selectedBearingCapacityPileType = BearingCapacityPileTypes.DrivenPileWithSealedEnd;
            _pileGeoCategory = PDIWT.Resources.PDIWT_Helper.GetEnumDescriptionDictionary<PDIWT_PiledWharf_Core_Cpp.PileTypeManaged>();
            _selectedPileGeoType = PDIWT_PiledWharf_Core_Cpp.PileTypeManaged.SqaurePile;
            _isVerticalPile = true;
            _pileSkewness = double.NaN;
            _concreteCoreLength = 0;
            _partialCoeff = 1.5;
            _blockCoeff = 1;
            _concreteWeight = 25;
            _concreteUnderwaterWeight = 15;
            _steelWeight = 78;
            _steelUnderwaterWeight = 68;
            PileSoilLayersInfo = new ObservableCollection<PDIWT_BearingCapacity_SoilLayerInfo>();
        }

        //****************** Input Parameters | Basic **********************//
        private string _pileCode;
        /// <summary>
        /// Property Description
        /// </summary>
        public string PileCode
        {
            get { return _pileCode; }
            set { Set(ref _pileCode, value); }
        }

        private Dictionary<BearingCapacityPileTypes, string> _bearingCapacityPileCategory;
        /// <summary>
        /// Property Description
        /// </summary>
        public Dictionary<BearingCapacityPileTypes, string> BearingCapacityPileCategory
        {
            get { return _bearingCapacityPileCategory; }
            set { Set(ref _bearingCapacityPileCategory, value); }
        }

        private BearingCapacityPileTypes _selectedBearingCapacityPileType;
        /// <summary>
        /// S
        /// </summary>
        public BearingCapacityPileTypes SelectedBearingCapacityPileType
        {
            get { return _selectedBearingCapacityPileType; }
            set { Set(ref _selectedBearingCapacityPileType, value); }
        }


        private Dictionary<PDIWT_PiledWharf_Core_Cpp.PileTypeManaged, string> _pileGeoCategory;
        /// <summary>
        /// Property Description
        /// </summary>
        public Dictionary<PDIWT_PiledWharf_Core_Cpp.PileTypeManaged, string> PileGeoCategory
        {
            get { return _pileGeoCategory; }
            set { Set(ref _pileGeoCategory, value); }
        }


        private PDIWT_PiledWharf_Core_Cpp.PileTypeManaged _selectedPileGeoType;
        /// <summary>
        /// Property Description
        /// </summary>
        public PDIWT_PiledWharf_Core_Cpp.PileTypeManaged SelectedPileGeoType
        {
            get { return _selectedPileGeoType; }
            set { Set(ref _selectedPileGeoType, value); }
        }

        private bool _isVerticalPile;
        /// <summary>
        /// Property Description
        /// </summary>
        public bool IsVerticalPile
        {
            get { return _isVerticalPile; }
            set { Set(ref _isVerticalPile, value); }
        }

        //****************** Input parameters | Geometry **********************//
        private double _pileTopElevation;
        /// <summary>
        /// Property Description
        /// </summary>
        public double PileTopElevation
        {
            get { return _pileTopElevation; }
            set { Set(ref _pileTopElevation, value); }
        }

        private double _pileLength;
        /// <summary>
        /// Property Description
        /// </summary>
        public double PileLength
        {
            get { return _pileLength; }
            set { Set(ref _pileLength, value); }
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
               
        private double _pilediameter;
        /// <summary>
        /// Property Description
        /// </summary>
        public double PileDiameter
        {
            get { return _pilediameter; }
            set { Set(ref _pilediameter, value); }
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
        /// C
        /// </summary>
        public double ConcreteCoreLength
        {
            get { return _concreteCoreLength; }
            set { Set(ref _concreteCoreLength, value); }
        }
        //****************** Input parameters | Calculation Coefficient **********************//
        private double _partialCoeff;
        /// <summary>
        /// Property Description
        /// </summary>
        public double PartialCoeff
        {
            get { return _partialCoeff; }
            set { Set(ref _partialCoeff, value); }
        }

        private double _blockCoeff;
        /// <summary>
        /// Property Description
        /// </summary>
        public double BlockCoeff
        {
            get { return _blockCoeff; }
            set { Set(ref _blockCoeff, value); }
        }

        private double _calculateWaterlevel;
        /// <summary>
        /// Property Description
        /// </summary>
        public double CalculatedWaterLevel
        {
            get { return _calculateWaterlevel; }
            set { Set(ref _calculateWaterlevel, value); }
        }

        //****************** Input parameters | Material **********************//

        private double _concreteWeight;
        /// <summary>
        /// Property Description
        /// </summary>
        public double ConcreteWeight
        {
            get { return _concreteWeight; }
            set { Set(ref _concreteWeight, value); }
        }

        private double _concreteUnderwaterWeight;
        /// <summary>
        /// Property Description
        /// </summary>
        public double ConcreteUnderwaterWeight
        {
            get { return _concreteUnderwaterWeight; }
            set { Set(ref _concreteUnderwaterWeight, value); }
        }

        private double _steelWeight;
        /// <summary>
        /// Property Description
        /// </summary>
        public double SteelWeight
        {
            get { return _steelWeight; }
            set { Set(ref _steelWeight, value); }
        }

        private double _steelUnderwaterWeight;
        /// <summary>
        /// Property Description
        /// </summary>
        public double SteelUnderwaterWeight
        {
            get { return _steelUnderwaterWeight; }
            set { Set(ref _steelUnderwaterWeight, value); }
        }



        //****************** Input parameters | Soil Layer Parameters **********************//
        private ObservableCollection<PDIWT_BearingCapacity_SoilLayerInfo> _pileSoilLayersInfo;
        /// <summary>
        /// Property Description
        /// </summary>
        public ObservableCollection<PDIWT_BearingCapacity_SoilLayerInfo> PileSoilLayersInfo
        {
            get { return _pileSoilLayersInfo; }
            set { Set(ref _pileSoilLayersInfo, value); }
        }
        
        //****************** calculated parameters **********************//
        private double _pilePerimeter;
        /// <summary>
        /// Property Description
        /// </summary>
        public double PilePerimeter
        {
            get { return _pilePerimeter; }
            set { Set(ref _pilePerimeter, value); }
        }

        private double _pileCrossSectionArea;
        /// <summary>
        /// Property Description
        /// </summary>
        public double PileCrossSectionArea
        {
            get { return _pileCrossSectionArea; }
            set { Set(ref _pileCrossSectionArea, value); }
        }

        private double _pileBottomElevation;
        /// <summary>
        /// Property Description
        /// </summary>
        public double PileBottomElevation
        {
            get { return _pileBottomElevation; }
            set { Set(ref _pileBottomElevation, value); }
        }

        private double _pileSelfWeight;
        /// <summary>
        /// Property Description
        /// </summary>
        public double PileSelfWeight
        {
            get { return _pileSelfWeight; }
            set { Set(ref _pileSelfWeight, value); }
        }

        //****************** results parameters **********************//
        private double _designAxialBearingCapacity;
        /// <summary>
        /// Property Description
        /// </summary>
        public double DesignAxialBearingCapacity
        {
            get { return _designAxialBearingCapacity; }
            set { Set(ref _designAxialBearingCapacity, value); }
        }

        private double _designAxialUpliftCapacity;
        /// <summary>
        /// Property Description
        /// </summary>
        public double DesignAxialUpliftCapacity
        {
            get { return _designAxialUpliftCapacity; }
            set { Set(ref _designAxialUpliftCapacity, value); }
        }
    }

    /// <summary>
    /// The soil layer information class holding all related numbering, name and related calculation coefficients.
    /// </summary>
    public class PDIWT_BearingCapacity_SoilLayerInfo : ObservableObject, IEquatable<PDIWT_BearingCapacity_SoilLayerInfo>
    {

        public PDIWT_BearingCapacity_SoilLayerInfo()
        {
            Betasi = Psii = Betap = Psip = DiscountCoeff = 1;

        }
        private string _soilLayerNumber;
        /// <summary>
        /// Property Description
        /// </summary>
        public string SoilLayerNumber
        {
            get { return _soilLayerNumber; }
            set { Set(ref _soilLayerNumber, value); }
        }

        private string _soilLayerName;
        /// <summary>
        /// S
        /// </summary>
        public string SoilLayerName
        {
            get { return _soilLayerName; }
            set { Set(ref _soilLayerName, value); }
        }

        private double _soilLayerTopElevation;
        /// <summary>
        /// Property Description
        /// </summary>
        public double SoilLayerTopElevation
        {
            get { return _soilLayerTopElevation; }
            set { Set(ref _soilLayerTopElevation, value); }
        }

        private double _soilLayerBottomElevation;
        /// <summary>
        /// Property Description
        /// </summary>
        public double SoilLayerBottomElevation
        {
            get { return _soilLayerBottomElevation; }
            set { Set(ref _soilLayerBottomElevation, value); }
        }

        private double _betasi;
        /// <summary>
        /// Property Description
        /// </summary>
        public double Betasi
        {
            get { return _betasi; }
            set { Set(ref _betasi, value); }
        }

        private double _psii;
        /// <summary>
        /// Property Description
        /// </summary>
        public double Psii
        {
            get { return _psii; }
            set { Set(ref _psii, value); }
        }

        private double _betap;
        /// <summary>
        /// Property Description
        /// </summary>
        public double Betap
        {
            get { return _betap; }
            set { Set(ref _betap, value); }
        }

        private double _psip;
        /// <summary>
        /// Property Description
        /// </summary>
        public double Psip
        {
            get { return _psip; }
            set { Set(ref _psip, value); }
        }
        
        private double _sideFrictionStandardValue;
        /// <summary>
        /// Property Description
        /// </summary>
        public double SideFrictionStandardValue
        {
            get { return _sideFrictionStandardValue; }
            set { Set(ref _sideFrictionStandardValue, value); }
        }

        private double _soilLayerThickness;
        /// <summary>
        /// Property Description
        /// </summary>
        public double SoilLayerThickness
        {
            get { return _soilLayerThickness; }
            set { Set(ref _soilLayerThickness, value); }
        }

        private double _endResistanceStandardValue;
        /// <summary>
        /// Property Description
        /// </summary>
        public double EndResistanceStandardValue
        {
            get { return _endResistanceStandardValue; }
            set { Set(ref _endResistanceStandardValue, value); }
        }

        private double _discountCoeff;
        /// <summary>
        /// Property Description
        /// </summary>
        public double DiscountCoeff
        {
            get { return _discountCoeff; }
            set { Set(ref _discountCoeff, value); }
        }

        public bool Equals(PDIWT_BearingCapacity_SoilLayerInfo other)
        {
            return other.SoilLayerNumber == SoilLayerNumber;
        }
        public override int GetHashCode()
        {
            if (SoilLayerNumber != null)
                return SoilLayerNumber.GetHashCode();
            else
                return string.Empty.GetHashCode();
        }
    }

    /// <summary>
    /// Enum type for bearing calculation type
    /// </summary>
    public enum BearingCapacityPileTypes
    {
        [Description("DrivenPileWithSealedEnd")]
        DrivenPileWithSealedEnd,
        [Description("TubePileOrSteelPile")]
        TubePileOrSteelPile,
        [Description("CastInSituPile")]
        CastInSituPile,
        [Description("CastInSituAfterGrountingPile")]
        CastInSituAfterGrountingPile
    }

}
