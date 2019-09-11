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
using BG = Bentley.GeometryNET;
using System.Windows.Controls;

namespace PDIWT_PiledWharf_Core.ViewModel
{
    using Model;
    /// <summary>
    /// Main View model for bearing capacity view
    /// </summary>
    public class BearingCapacityViewModel : ViewModelBase
    {
        public BearingCapacityViewModel()
        {
            PileInfos = new ObservableCollection<PDIWT_BearingCapacity_PileInfo>()
            {
#if DEBUG
                new PDIWT_BearingCapacity_PileInfo
                {
                    PileName = "Test Pile",
                    CalculatedWaterLevel = 2.4,
                    PileTopElevation = 2.71,
                    PileLength= 40,
                    PileDiameter = 0.65,
                    PileInnerDiameter = 0.33,
                    PileSoilLayersInfo = new ObservableCollection<PDIWT_BearingCapacity_SoilLayerInfo>()
                    {
                        new PDIWT_BearingCapacity_SoilLayerInfo()
                        {
                            SoilLayerObject = new SoilLayer()
                            {
                                Number= "3-1", Name="粉质粘土",
                                DPSE_Qfi = 3.1, DPSE_Qr = 33,
                                TPSP_Qfi = 11,TPSP_Qr =12, TPSP_Yita = 1,
                                CISP_Psisi = 1, CISP_Qfi = 13, CISP_Psip =11, CISP_Qr =13,
                                CISAGP_Betasi =2, CISAGP_Psisi=3, CISAGP_Qfi =4,CISAGP_Betap =5, CISAGP_Psip=6, CISAGP_Qr=7,
                                Xii = 0.7
                            },PileIntersectionTopEle =1,PileIntersectionBottomEle=-1
                        },
                        new PDIWT_BearingCapacity_SoilLayerInfo()
                        {
                            SoilLayerObject = new SoilLayer()
                            {
                                Number= "3-2", Name="粉土",
                                DPSE_Qfi = 64, DPSE_Qr = 0,
                                TPSP_Qfi = 11,TPSP_Qr =12, TPSP_Yita = 1,
                                CISP_Psisi = 1, CISP_Qfi = 13, CISP_Psip =11, CISP_Qr =13,
                                CISAGP_Betasi =2, CISAGP_Psisi=3, CISAGP_Qfi =4,CISAGP_Betap =5, CISAGP_Psip=6, CISAGP_Qr=7,
                                Xii = 0.7
                            },PileIntersectionTopEle =-1,PileIntersectionBottomEle=-2
                        },
                        new PDIWT_BearingCapacity_SoilLayerInfo()
                        {
                            SoilLayerObject = new SoilLayer()
                            {
                                Number= "4", Name="粉质粘土",
                                DPSE_Qfi = 42, DPSE_Qr = 0,
                                TPSP_Qfi = 11,TPSP_Qr =12, TPSP_Yita = 1,
                                CISP_Psisi = 1, CISP_Qfi = 13, CISP_Psip =11, CISP_Qr =13,
                                CISAGP_Betasi =2, CISAGP_Psisi=3, CISAGP_Qfi =4,CISAGP_Betap =5, CISAGP_Psip=6, CISAGP_Qr=7,
                                Xii = 0.7
                            },PileIntersectionTopEle =-2,PileIntersectionBottomEle=-3
                        },
                        new PDIWT_BearingCapacity_SoilLayerInfo()
                        {
                            SoilLayerObject = new SoilLayer()
                            {
                                Number= "5", Name="粉质粘土",
                                DPSE_Qfi = 60, DPSE_Qr = 0,
                                TPSP_Qfi = 11,TPSP_Qr =12, TPSP_Yita = 1,
                                CISP_Psisi = 1, CISP_Qfi = 13, CISP_Psip =11, CISP_Qr =13,
                                CISAGP_Betasi =2, CISAGP_Psisi=3, CISAGP_Qfi =4,CISAGP_Betap =5, CISAGP_Psip=6, CISAGP_Qr=7,
                                Xii = 0.7
                            },PileIntersectionTopEle =-3,PileIntersectionBottomEle=-4
                        },
                        new PDIWT_BearingCapacity_SoilLayerInfo()
                        {
                            SoilLayerObject = new SoilLayer()
                            {
                                Number= "6-1", Name="粉土",
                                DPSE_Qfi = 90, DPSE_Qr = 1800,
                                TPSP_Qfi = 11,TPSP_Qr =12, TPSP_Yita = 1,
                                CISP_Psisi = 1, CISP_Qfi = 13, CISP_Psip =11, CISP_Qr =13,
                                CISAGP_Betasi =2, CISAGP_Psisi=3, CISAGP_Qfi =4,CISAGP_Betap =5, CISAGP_Psip=6, CISAGP_Qr=7,
                                Xii = 0.7
                            },PileIntersectionTopEle =-4,PileIntersectionBottomEle=-5
                        },
                    },

                    //new PDIWT_BearingCapacity_SoilLayerInfo(){SoilLayerNumber = "3-2", SoilLayerName="粉土", SoilLayerThickness = 3.0, SideFrictionStandardValue = 64, EndResistanceStandardValue = 0, DiscountCoeff = 0.7},
                    //new PDIWT_BearingCapacity_SoilLayerInfo(){SoilLayerNumber = "4", SoilLayerName="粉质粘土", SoilLayerThickness = 4.4, SideFrictionStandardValue = 42, EndResistanceStandardValue = 0, DiscountCoeff = 0.7},
                    //new PDIWT_BearingCapacity_SoilLayerInfo(){SoilLayerNumber = "5", SoilLayerName="粘土", SoilLayerThickness = 3.0, SideFrictionStandardValue = 32, EndResistanceStandardValue = 0, DiscountCoeff = 0.7},
                    //new PDIWT_BearingCapacity_SoilLayerInfo(){SoilLayerNumber = "6-2", SoilLayerName="粉质粘土", SoilLayerThickness = 3.0, SideFrictionStandardValue = 60, EndResistanceStandardValue = 1800, DiscountCoeff = 0.7},
                    //new PDIWT_BearingCapacity_SoilLayerInfo(){SoilLayerNumber = "6-1", SoilLayerName="粉土", SoilLayerThickness = 3.0, SideFrictionStandardValue = 90, EndResistanceStandardValue = 3200, DiscountCoeff = 0.7},
                    //new PDIWT_BearingCapacity_SoilLayerInfo(){SoilLayerNumber = "6-3", SoilLayerName="粉细沙", SoilLayerThickness = 2.7, SideFrictionStandardValue = 120, EndResistanceStandardValue = 6100, DiscountCoeff = 0.5}
                
                }
#endif
            };

            _soilLayersLibrary = new SoilLayerCollection()
            {
#if DEBUG
                new SoilLayer()
                {
                    Number = "3-2",
                    Name = "粉土",
                    DPSE_Qfi = 64,
                    DPSE_Qr = 0,
                    TPSP_Qfi = 11,
                    TPSP_Qr = 12,
                    TPSP_Yita = 1,
                    CISP_Psisi = 1,
                    CISP_Qfi = 13,
                    CISP_Psip = 11,
                    CISP_Qr = 13,
                    CISAGP_Betasi = 2,
                    CISAGP_Psisi = 3,
                    CISAGP_Qfi = 4,
                    CISAGP_Betap = 5,
                    CISAGP_Psip = 6,
                    CISAGP_Qr = 7,
                    Xii = 0.7
                },
                new SoilLayer()
                {
                    Number = "4",
                    Name = "粉质粘土",
                    DPSE_Qfi = 42,
                    DPSE_Qr = 0,
                    TPSP_Qfi = 11,
                    TPSP_Qr = 12,
                    TPSP_Yita = 1,
                    CISP_Psisi = 1,
                    CISP_Qfi = 13,
                    CISP_Psip = 11,
                    CISP_Qr = 13,
                    CISAGP_Betasi = 2,
                    CISAGP_Psisi = 3,
                    CISAGP_Qfi = 4,
                    CISAGP_Betap = 5,
                    CISAGP_Psip = 6,
                    CISAGP_Qr = 7,
                    Xii = 0.7
                },
                new SoilLayer()
                {
                    Number = "5",
                    Name = "粉质粘土",
                    DPSE_Qfi = 60,
                    DPSE_Qr = 0,
                    TPSP_Qfi = 11,
                    TPSP_Qr = 12,
                    TPSP_Yita = 1,
                    CISP_Psisi = 1,
                    CISP_Qfi = 13,
                    CISP_Psip = 11,
                    CISP_Qr = 13,
                    CISAGP_Betasi = 2,
                    CISAGP_Psisi = 3,
                    CISAGP_Qfi = 4,
                    CISAGP_Betap = 5,
                    CISAGP_Psip = 6,
                    CISAGP_Qr = 7,
                    Xii = 0.7
                },
                new SoilLayer()
                {
                    Number = "6-1",
                    Name = "粉土",
                    DPSE_Qfi = 90,
                    DPSE_Qr = 1800,
                    TPSP_Qfi = 11,
                    TPSP_Qr = 12,
                    TPSP_Yita = 1,
                    CISP_Psisi = 1,
                    CISP_Qfi = 13,
                    CISP_Psip = 11,
                    CISP_Qr = 13,
                    CISAGP_Betasi = 2,
                    CISAGP_Psisi = 3,
                    CISAGP_Qfi = 4,
                    CISAGP_Betap = 5,
                    CISAGP_Psip = 6,
                    CISAGP_Qr = 7,
                    Xii = 0.7
                }
#endif
            };

            Messenger.Default.Register<NotificationMessage<PDIWT_BearingCapacity_PileInfo>>(this, LoadParametersFromPileEntity);
        }

        readonly private BM.MessageCenter _mc = BM.MessageCenter.Instance;


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

        //private ObservableCollection<PDIWT_BearingCapacity_SoilLayerInfo> _soilLayerInfosLib;
        ///// <summary>
        ///// Property Description
        ///// </summary>
        //public ObservableCollection<PDIWT_BearingCapacity_SoilLayerInfo> SoilLayerInfosLib
        //{
        //    get { return _soilLayerInfosLib; }
        //    set { Set(ref _soilLayerInfosLib, value); }
        //}

        private SoilLayerCollection _soilLayersLibrary;
        /// <summary>
        /// Property Description
        /// </summary>
        public SoilLayerCollection SoilLayersLibrary
        {
            get { return _soilLayersLibrary; }
            set { Set(ref _soilLayersLibrary, value); }
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
            //BearingCapacityGetters.Add(new PileSoilLayersIntersectionGetter(new BearingCapacityPile(), null));
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

        private RelayCommand _loadFromModel;

        /// <summary>
        /// Gets the LoadParameters.
        /// </summary>
        public RelayCommand LoadFromModel
        {
            get
            {
                return _loadFromModel
                    ?? (_loadFromModel = new RelayCommand(ExecuteLoadFromModel));
            }
        }

        private void ExecuteLoadFromModel()
        {
            try
            {
                //Registered by main window class
                Messenger.Default.Send(Visibility.Hidden, "ShowMainWindow");
                LoadPileParametersTool<PDIWT_BearingCapacity_PileInfo>.InstallNewInstance(LoadParametersToolEnablerProvider.AxialBearingCapacityPileInfoEnabler);
            }
            catch (Exception e)
            {
                _mc.ShowErrorMessage("Can't load tool", e.ToString(), false);
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
                    Messenger.Default.Send(new NotificationMessage<Visibility>(Visibility.Visible, "Hidden Main Window"), "BearingCapacityMainWindowVisibility");

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
            try
            {
                BuildUpSoilLayersWindow _window = new BuildUpSoilLayersWindow();

                BuildUpSoilLayersViewModel _vm = new BuildUpSoilLayersViewModel()
                {
                    SoilLayerCollection = new SoilLayerCollection(SoilLayersLibrary)
                };
                _window.DataContext = _vm;
                bool? _dialogResult = _window.ShowDialog();
                if (_dialogResult == true)
                {
                    BM.MessageCenter.Instance.ShowInfoMessage("The soil Library is updated", "", false);
                    SoilLayersLibrary = _vm.SoilLayerCollection;
                }

            }
            catch (Exception e)
            {
                BM.MessageCenter.Instance.ShowErrorMessage("The soil Library is Can't be update", e.ToString(), false);
            }
            finally
            {
            }

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
            try
            {
                if (SoilLayersLibrary.Count == 0)
                {
                    _mc.ShowErrorMessage("Please build up soil layer library first!", "", false);
                    return;
                }
                PickUpSoilLayersFromLibWindow _window = new PickUpSoilLayersFromLibWindow();
                ObservableCollection<PDIWT_BearingCapacity_SoilLayerInfo> _soilLayerInfos = new ObservableCollection<PDIWT_BearingCapacity_SoilLayerInfo>();
                foreach (var _layer in SoilLayersLibrary)
                    _soilLayerInfos.Add(new PDIWT_BearingCapacity_SoilLayerInfo() { SoilLayerObject = _layer, PileIntersectionBottomEle = 0, PileIntersectionTopEle = 0 });

                PickUpSoilLayersFromLibViewModel _vm = new PickUpSoilLayersFromLibViewModel(_soilLayerInfos, SelectedPile.PileSoilLayersInfo);
                _window.DataContext = _vm;
                if (true == _window.ShowDialog())
                {
                    SelectedPile.PileSoilLayersInfo = new ObservableCollection<PDIWT_BearingCapacity_SoilLayerInfo>(_vm.SelectedSoilLayerInfos);
                    _mc.ShowInfoMessage("Picked soil layers from library", "", false);
                }
            }
            catch (Exception e)
            {
                _mc.ShowErrorMessage("Can't pick soil layer", e.ToString(), false);
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
                    ?? (_calculate = new RelayCommand(ExecuteCalculate,
                    () => SelectedPile != null && SelectedPile.PileSoilLayersInfo != null & SelectedPile.PileSoilLayersInfo.Count != 0));
            }
        }

        private void ExecuteCalculate()
        {
            try
            {
                SelectedPile.CalculatedBearingCapacity();
                //Registered by its view behind code
                Messenger.Default.Send(new NotificationMessage<bool>(false, "Changed!"), "BearingCapacityForegroundChange");

            }
            catch (Exception e)
            {
                _mc.ShowErrorMessage("Calculation fail", e.ToString(), false);
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
                        () =>
                        {
                            if (PileInfos == null || PileInfos.Count == 0)
                                return false;
                            else
                            {
                                foreach (var _pile in PileInfos)
                                {
                                    if (_pile.IsCalculated == false)
                                        return false;
                                }
                                return true;
                            }
                        }
                        ));
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

    /// <summary>
    /// The soil layer information class holding all related numbering, name and related calculation coefficients.
    /// </summary>
    public class PDIWT_BearingCapacity_SoilLayerInfo : ObservableObject, IEquatable<PDIWT_BearingCapacity_SoilLayerInfo>
    {

        public PDIWT_BearingCapacity_SoilLayerInfo()
        {
        }

        private SoilLayer _soilLayerObject;
        /// <summary>
        /// Property Description
        /// </summary>
        public SoilLayer SoilLayerObject
        {
            get { return _soilLayerObject; }
            set { Set(ref _soilLayerObject, value); }
        }


        private double? _pileIntersectionTopEle;
        /// <summary>
        /// unit: m
        /// </summary>
        public double? PileIntersectionTopEle
        {
            get { return _pileIntersectionTopEle; }
            set { Set(ref _pileIntersectionTopEle, value); }
        }

        private double? _pileIntersectionBottomEle;
        /// <summary>
        /// unit : m
        /// </summary>
        public double? PileIntersectionBottomEle
        {
            get { return _pileIntersectionBottomEle; }
            set { Set(ref _pileIntersectionBottomEle, value); }
        }

        public bool Equals(PDIWT_BearingCapacity_SoilLayerInfo other)
        {
            if (other.SoilLayerObject == null
                || SoilLayerObject == null
                || other.SoilLayerObject.Number == null
                || SoilLayerObject.Number == null)
                return false;
            return other.SoilLayerObject.Number == SoilLayerObject.Number;
        }
        public override int GetHashCode()
        {
            if (SoilLayerObject == null)
                return 0;
            else
                return SoilLayerObject.Number == null ? 0 : SoilLayerObject.Number.GetHashCode();
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public class PDIWT_BearingCapacity_PileInfo : ObservableObject
    {
        public PDIWT_BearingCapacity_PileInfo()
        {
            _isCalculated = false;
            //_bearingCapacityPileCategory = PDIWT.Resources.PDIWT_Helper.GetEnumDescriptionDictionary<BearingCapacityPileTypes>();
            _selectedBearingCapacityPileType = BearingCapacityPileTypes.DrivenPileWithSealedEnd;
            //_pileGeoCategory = PDIWT.Resources.PDIWT_Helper.GetEnumDescriptionDictionary<PDIWT_PiledWharf_Core_Cpp.PileTypeManaged>();
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
            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName != "IsCalculated")
                    IsCalculated = false;
            };
        }
        //****************** For The Status of Calculation Item **********************//

        private bool _isCalculated;
        /// <summary>
        /// Property Description
        /// </summary>
        public bool IsCalculated
        {
            get { return _isCalculated; }
            set { Set(ref _isCalculated, value); }
        }
        //****************** Input Parameters | Basic **********************//

        private string _pileName;
        /// <summary>
        /// Property Description
        /// </summary>
        public string PileName
        {
            get { return _pileName; }
            set { Set(ref _pileName, value); }
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
        /// unit: m
        /// </summary>
        public double PileTopElevation
        {
            get { return _pileTopElevation; }
            set { Set(ref _pileTopElevation, value); }
        }

        private double _pileLength;
        /// <summary>
        /// unit: m
        /// </summary>
        public double PileLength
        {
            get { return _pileLength; }
            set
            {
                if (value < 0)
                    Set(ref _pileLength, 0);
                else
                    Set(ref _pileLength, value);
            }
        }

        private double _pileSkewness;
        /// <summary>
        /// if it's vertical, NaN; otherwise double type.
        /// </summary>
        public double PileSkewness
        {
            get { return _pileSkewness; }
            set
            {
                if (value < 0)
                    Set(ref _pileSkewness, double.NaN);
                else
                    Set(ref _pileSkewness, value);
            }
        }

        private double _pilediameter;
        /// <summary>
        /// unit: m
        /// </summary>
        public double PileDiameter
        {
            get { return _pilediameter; }
            set
            {
                if (value < 0)
                    Set(ref _pilediameter, 0);
                else
                    Set(ref _pilediameter, value);
            }
        }

        private double _pileInnerDiameter;
        /// <summary>
        /// unit: m
        /// </summary>
        public double PileInnerDiameter
        {
            get { return _pileInnerDiameter; }
            set
            {
                if (value < 0)
                    Set(ref _pileInnerDiameter, 0);
                else
                    Set(ref _pileInnerDiameter, value);
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
        //****************** Input parameters | Calculation Coefficient **********************//
        private double _partialCoeff;
        /// <summary>
        /// unit less
        /// </summary>
        public double PartialCoeff
        {
            get { return _partialCoeff; }
            set
            {
                if (value < 1)
                    Set(ref _partialCoeff, 1);
                else
                    Set(ref _partialCoeff, value);
            }
        }

        private double _blockCoeff;
        /// <summary>
        /// unit less
        /// </summary>
        public double BlockCoeff
        {
            get { return _blockCoeff; }
            set
            {
                if (value < 1)
                    Set(ref _blockCoeff, 1);
                else
                    Set(ref _blockCoeff, value);
            }
        }

        private double _calculateWaterlevel;
        /// <summary>
        /// unit: m
        /// </summary>
        public double CalculatedWaterLevel
        {
            get { return _calculateWaterlevel; }
            set { Set(ref _calculateWaterlevel, value); }
        }

        //****************** Input parameters | Material **********************//

        private double _concreteWeight;
        /// <summary>
        /// unit: kN/m3
        /// </summary>
        public double ConcreteWeight
        {
            get { return _concreteWeight; }
            set
            {
                if (value < 0)
                    Set(ref _concreteWeight, 25);
                else
                    Set(ref _concreteWeight, value);
            }
        }

        private double _concreteUnderwaterWeight;
        /// <summary>
        /// unit: kN/m3
        /// </summary>
        public double ConcreteUnderwaterWeight
        {
            get { return _concreteUnderwaterWeight; }
            set
            {
                if (value < 0)
                    Set(ref _concreteUnderwaterWeight, 15);
                else
                    Set(ref _concreteUnderwaterWeight, value);
            }
        }

        private double _steelWeight;
        /// <summary>
        /// unit: kN/m3
        /// </summary>
        public double SteelWeight
        {
            get { return _steelWeight; }
            set
            {
                if (value < 0)
                    Set(ref _steelWeight, 78);
                else
                    Set(ref _steelWeight, value);
            }
        }

        private double _steelUnderwaterWeight;
        /// <summary>
        /// unit: kN/m3
        /// </summary>
        public double SteelUnderwaterWeight
        {
            get { return _steelUnderwaterWeight; }
            set
            {
                if (value < 0)
                    Set(ref _steelUnderwaterWeight, 68);
                else
                    Set(ref _steelUnderwaterWeight, value);
            }
        }

        //****************** Input parameters | Soil Layer Parameters **********************//
        private ObservableCollection<PDIWT_BearingCapacity_SoilLayerInfo> _pileSoilLayersInfo;
        /// <summary>
        /// Property Description
        /// </summary>
        public ObservableCollection<PDIWT_BearingCapacity_SoilLayerInfo> PileSoilLayersInfo
        {
            get { return _pileSoilLayersInfo; }
            set
            {
                Set(ref _pileSoilLayersInfo, value);

                foreach (var _pileSoil in _pileSoilLayersInfo)
                    _pileSoil.PropertyChanged += (s, e) => IsCalculated = false;
            }
        }

        //****************** calculated parameters **********************//
        private double _pilePerimeter;
        /// <summary>
        /// unit: m
        /// </summary>
        public double PilePerimeter
        {
            get { return _pilePerimeter; }
            private set { Set(ref _pilePerimeter, value); }
        }

        private double _pileCrossSectionArea;
        /// <summary>
        /// unit: m2
        /// </summary>
        public double PileCrossSectionArea
        {
            get { return _pileCrossSectionArea; }
            private set { Set(ref _pileCrossSectionArea, value); }
        }

        private double _pileBottomElevation;
        /// <summary>
        /// unit: m
        /// </summary>
        public double PileBottomElevation
        {
            get { return _pileBottomElevation; }
            private set { Set(ref _pileBottomElevation, value); }
        }

        private double _pileSelfWeight;
        /// <summary>
        /// unit: kN
        /// </summary>
        public double PileSelfWeight
        {
            get { return _pileSelfWeight; }
            private set { Set(ref _pileSelfWeight, value); }
        }

        //****************** results parameters **********************//
        private double _designAxialBearingCapacity;
        /// <summary>
        /// unit: kN
        /// </summary>
        public double DesignAxialBearingCapacity
        {
            get { return _designAxialBearingCapacity; }
            private set { Set(ref _designAxialBearingCapacity, value); }
        }

        private double _designAxialUpliftCapacity;
        /// <summary>
        /// unit: kN
        /// </summary>
        public double DesignAxialUpliftCapacity
        {
            get { return _designAxialUpliftCapacity; }
            private set { Set(ref _designAxialUpliftCapacity, value); }
        }

        ////** other parameters **//
        //public BDE.CellHeaderElement PileCell { get; private set; }

        /// <summary>
        /// Calculate Bearing Capacity
        /// </summary>
        public void CalculatedBearingCapacity()
        {
            try
            {
                double _uorpermeter = BM.Session.Instance.GetActiveDgnModel().GetModelInfo().UorPerMeter;

                PileBase _pileBase = PileBase.CreateFromTopPointandLength(new BG.DPoint3d(0, 0, PileTopElevation * _uorpermeter), PileLength * _uorpermeter, PileSkewness, BG.Angle.Zero, SelectedPileGeoType, PileDiameter * _uorpermeter, PileInnerDiameter * _uorpermeter);
                BearingCapacityPile _pile = new BearingCapacityPile(_pileBase, SelectedBearingCapacityPileType);

                PilePerimeter = _pile.GetPerimeter() / _uorpermeter;
                PileCrossSectionArea = _pile.GetOuterArea() / Math.Pow(_uorpermeter, 2);
                PileBottomElevation = _pile.BottomJoint.Point.Z / _uorpermeter;
                PileSelfWeight = _pile.GetPileSelfWeight(CalculatedWaterLevel, ConcreteWeight, ConcreteUnderwaterWeight, SteelWeight, SteelUnderwaterWeight);

                List<IntersectionInfo> _intersectioInfos = new List<IntersectionInfo>();
                foreach (var _layer in PileSoilLayersInfo)
                {
                    if (_layer.PileIntersectionBottomEle >= _layer.PileIntersectionTopEle)
                        throw new ArgumentException($"The bottom elevation {_layer.PileIntersectionBottomEle} is greater or equal to top elevation {_layer.PileIntersectionTopEle} in Soil Layer {_layer.SoilLayerObject.Number} ");
                    if (_layer.PileIntersectionBottomEle * _uorpermeter < _pile.BottomJoint.Point.Z)
                        throw new ArgumentException($"The bottom elevation {_layer.PileIntersectionBottomEle} is less than pile bottom elevation {_pile.BottomJoint.Point.Z / _uorpermeter} in soil layer {_layer.SoilLayerObject.Number}");
                    double _topFraction = (PileTopElevation - _layer.PileIntersectionTopEle.Value) * _uorpermeter / (_pile.TopJoint.Point.Z - _pile.BottomJoint.Point.Z);
                    double _bottomFraction = (PileBottomElevation - _layer.PileIntersectionTopEle.Value) * _uorpermeter / (_pile.TopJoint.Point.Z - _pile.BottomJoint.Point.Z);
                    IntersectionInfo _itInfo = new IntersectionInfo(_topFraction, _bottomFraction, _pileBase, _layer.SoilLayerObject);
                    _intersectioInfos.Add(_itInfo);
                }

                _pile.GetPileAxialCapacity(PartialCoeff, _intersectioInfos, CalculatedWaterLevel, ConcreteWeight, ConcreteUnderwaterWeight, SteelWeight, SteelUnderwaterWeight, out double _bc, out double _lift);
                DesignAxialBearingCapacity = _bc;
                DesignAxialUpliftCapacity = _lift;

                IsCalculated = true;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Can't calculate bearing capacity", e);
            }
        }

        #region old code
        //public BD.StatusInt ObtainPileFromCellElementAndActiveModel(BDE.CellHeaderElement pileCell)
        //{
        //    var _activeModel = BM.Session.Instance.GetActiveDgnModel();
        //    double _uorpermeter = _activeModel.GetModelInfo().UorPerMeter;
        //    var _mc = BM.MessageCenter.Instance;

        //    if (pileCell == null)
        //        return BD.StatusInt.Error;

        //    BDE.LineElement _axisLine = pileCell.GetChildren().Where(_line => _line is BDE.LineElement).First() as BDE.LineElement;
        //    BG.DPoint3d _lineStartPoint, _lineEndPoint;
        //    if (!_axisLine.AsCurvePathEdit().GetCurveVector().GetStartEnd(out _lineStartPoint, out _lineEndPoint))
        //        return BD.StatusInt.Error;

        //    //Swap start and end point if necessary, start point => top point, end point => bottom point;
        //    if (_lineEndPoint.Z > _lineStartPoint.Z)
        //    {
        //        BG.DPoint3d _temp = _lineEndPoint;
        //        _lineEndPoint = _lineStartPoint;
        //        _lineStartPoint = _temp;
        //    }
        //    BG.DRay3d _axisRay = new BG.DRay3d(_lineStartPoint, _lineEndPoint);

        //    //Pile Material and Pile Self Related Parameters
        //    string _environmentECSchemaName = "PDIWT";
        //    string _materialECClassName = "PileMaterialSettings";
        //    Dictionary<string, object> _materialProps;
        //    List<string> _requireMaterialPropNameList = new List<string>()
        //    {
        //        "ConcreteUnitWeight",
        //        "ConcreteUnderWaterUnitWeight",
        //        "SteelUnitWeight",
        //        "WaterUnitWeight"
        //    };
        //    if (BD.StatusInt.Error == ECSChemaReader.ReadECInstanceProperties(_environmentECSchemaName, _materialECClassName, _requireMaterialPropNameList, _activeModel, out _materialProps))
        //        return BD.StatusInt.Error;
        //    ConcreteWeight = double.Parse(_materialProps["ConcreteUnitWeight"].ToString());
        //    ConcreteUnderwaterWeight = double.Parse(_materialProps["ConcreteUnderWaterUnitWeight"].ToString());
        //    SteelWeight = double.Parse(_materialProps["SteelUnitWeight"].ToString());
        //    SteelUnderwaterWeight = double.Parse(_materialProps["SteelUnitWeight"].ToString()) - double.Parse(_materialProps["WaterUnitWeight"].ToString());

        //    string _ifcECSchemaName = "IfcPort";
        //    string _ifcPileECClassName = "IfcPile";
        //    Dictionary<string, object> _pileProps;
        //    List<string> _requirePilePropNameList = new List<string>()
        //    {
        //        "Code",
        //        "Type",
        //        "CrossSectionWidth",
        //        "OutsideDiameter",
        //        "InnerDiameter",
        //        "Length"
        //    };
        //    if (BD.StatusInt.Error == ECSChemaReader.ReadECInstanceProperties(_ifcECSchemaName, _ifcPileECClassName, _requirePilePropNameList, pileCell, out _pileProps))
        //        return BD.StatusInt.Error;
        //    SelectedPileGeoType = PDIWT.Resources.PDIWT_Helper.GetEnumDescriptionDictionary<PDIWT_PiledWharf_Core_Cpp.PileTypeManaged>()
        //                                                      .Where(e => e.Value == _pileProps["Type"].ToString())
        //                                                      .First().Key;
        //    switch (SelectedPileGeoType)
        //    {
        //        case PDIWT_PiledWharf_Core_Cpp.PileTypeManaged.SqaurePile:
        //            PileDiameter = double.Parse(_pileProps["CrossSectionWidth"].ToString()) / 1000;
        //            break;
        //        case PDIWT_PiledWharf_Core_Cpp.PileTypeManaged.TubePile:
        //            PileDiameter = double.Parse(_pileProps["OutsideDiameter"].ToString()) / 1000;
        //            break;
        //        case PDIWT_PiledWharf_Core_Cpp.PileTypeManaged.PHCTubePile:
        //            PileDiameter = double.Parse(_pileProps["OutsideDiameter"].ToString()) / 1000;
        //            PileInsideDiameter = double.Parse(_pileProps["InnerDiameter"].ToString()) / 1000;
        //            break;
        //        case PDIWT_PiledWharf_Core_Cpp.PileTypeManaged.SteelTubePile:
        //            PileDiameter = double.Parse(_pileProps["OutsideDiameter"].ToString()) / 1000;
        //            PileInsideDiameter = double.Parse(_pileProps["InnerDiameter"].ToString()) / 1000;
        //            break;
        //        default:
        //            break;
        //    }

        //    PileLength = double.Parse(_pileProps["Length"].ToString()) / 1000;

        //    //PileCode = _pileProps["Code"].ToString();

        //    BG.DVector3d _pileAxisVector = new BG.DVector3d(_lineStartPoint, _lineEndPoint);
        //    if (_pileAxisVector.IsParallelOrOppositeTo(BG.DVector3d.UnitZ))
        //    {
        //        IsVerticalPile = true;
        //        PileSkewness = double.NaN;
        //    }
        //    else
        //    {
        //        IsVerticalPile = false;
        //        PileSkewness = Math.Abs(_pileAxisVector.Z / Math.Sqrt(_pileAxisVector.X * _pileAxisVector.X + _pileAxisVector.Y * _pileAxisVector.Y));
        //    }
        //    PileTopElevation = _lineStartPoint.Z / _uorpermeter;

        //    PileCell = pileCell;
        //    return ObtainSoilLayersInfoFromAtiveModel();
        //}

        /// <summary>
        /// Get Soil layers which insect with this pile
        /// </summary>
        /// <returns>success when has intersection, otherwise error </returns>
        //private BD.StatusInt ObtainSoilLayersInfoFromAtiveModel()
        //{
        //    var _activeModel = BM.Session.Instance.GetActiveDgnModel();

        //    BD.ScanCriteria _sc = new BD.ScanCriteria();
        //    _sc.SetModelRef(_activeModel);
        //    _sc.SetModelSections(BD.DgnModelSections.GraphicElements);
        //    BD.BitMask _meshBitMask = new BD.BitMask(false);
        //    _meshBitMask.Capacity = 400;
        //    _meshBitMask.ClearAll();
        //    _meshBitMask.SetBit(104, true);
        //    _sc.SetElementTypeTest(_meshBitMask);
        //    _sc.Scan((_element, _model) =>
        //    {
        //        var _layerInfo = new PDIWT_BearingCapacity_SoilLayerInfo();
        //        BDE.MeshHeaderElement _mesh = (BDE.MeshHeaderElement)_element;
        //        if(_layerInfo.ObtainInfoFromMesh(_mesh) == BD.StatusInt.Success)
        //        {
        //            PDIWT_BearingCapacity_PileSoilIntersectionInfo _psIntersectionInfo =
        //                new PDIWT_BearingCapacity_PileSoilIntersectionInfo(this, _layerInfo);
        //            InterSectionInfo _isInfo = _psIntersectionInfo.GetInterSectionInfo();
        //            if(true ==_isInfo.IsIntersection )
        //            {
        //                _layerInfo.SoilLayerThickness = _psIntersectionInfo.GetPileLengthInSoilLayer();
        //                PileSoilLayersInfo.Add(_layerInfo);
        //            }
        //        }
        //        return BD.StatusInt.Success;
        //    });
        //    if (PileSoilLayersInfo.Count == 0)
        //        return BD.StatusInt.Error;
        //    else
        //        return BD.StatusInt.Success;
        //}

        /// <summary>
        /// get pile ray from top to bottom, must invoke ObtainInfoFromCellElement first
        /// </summary>
        /// <returns>DRay3d in UOR</returns>
        //public BG.DRay3d GetPileRay3D()
        //{
        //    double _uorpermm = BM.Session.Instance.GetActiveDgnModel().GetModelInfo().UorPerMeter / 1000;        
        //    if(PileCell == null)
        //    {
        //        BG.DVector3d _unitZ = new BG.DVector3d(0, 0, -1);
        //        _unitZ.ScaleInPlace(PileLength * _uorpermm * 1000);
        //        if(!double.IsNaN(PileSkewness))
        //        {
        //            BG.Angle _yAngel = new BG.Angle();
        //            _yAngel.Radians =- Math.Atan(1 / PileSkewness);
        //            BG.DTransform3d _yTrans = BG.DTransform3d.Rotation(1, _yAngel);
        //            _yTrans.MultiplyInPlace(ref _unitZ);
        //        }
        //        return new BG.DRay3d(new BG.DPoint3d(0, 0, PileTopElevation * _uorpermm * 1000), _unitZ);
        //    }
        //    else
        //    {
        //        BDE.LineElement _axisLine = PileCell.GetChildren().Where(_line => _line is BDE.LineElement).First() as BDE.LineElement;
        //        BG.DPoint3d _lineStartPoint, _lineEndPoint;
        //        _axisLine.AsCurvePathEdit().GetCurveVector().GetStartEnd(out _lineStartPoint, out _lineEndPoint);
        //        //Swap start and end point if necessary, start point => top point, end point => bottom point;
        //        if (_lineEndPoint.Z > _lineStartPoint.Z)
        //        {
        //            BG.DPoint3d _temp = _lineEndPoint;
        //            _lineEndPoint = _lineStartPoint;
        //            _lineStartPoint = _temp;
        //        }
        //        return new BG.DRay3d(_lineStartPoint, _lineEndPoint);
        //    }
        //}
        #endregion
    }
}
