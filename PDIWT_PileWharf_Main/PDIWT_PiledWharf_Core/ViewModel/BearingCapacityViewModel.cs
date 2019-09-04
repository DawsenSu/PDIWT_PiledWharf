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
    using Model;

    public class BearingCapacityViewModel : ViewModelBase
    {
        public BearingCapacityViewModel()
        {
            PileInfos = new ObservableCollection<PDIWT_BearingCapacity_PileInfo>()
            {
#if DEBUG
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
#endif
            };

            SoilLayerInfosLib = new ObservableCollection<PDIWT_BearingCapacity_SoilLayerInfo>()
            {
#if DEBUG
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
#endif
            };

            //Messenger.Default.Register<NotificationMessage<ObservableCollection<PDIWT_BearingCapacity_SoilLayerInfo>>>(this, "BuildupSoilLayerLib",
            //    n => SoilLayerInfosLib = n.Content);
            //Messenger.Default.Register<NotificationMessage<ObservableCollection<PDIWT_BearingCapacity_SoilLayerInfo>>>(this, "SelectedSoilLayers",
            //    n => SelectedPile.PileSoilLayersInfo = n.Content);
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
                    SoilLayerInfos = new ObservableCollection<PDIWT_BearingCapacity_SoilLayerInfo>(SoilLayerInfosLib)
                };
                _window.DataContext = _vm;
                bool? _dialogResult = _window.ShowDialog();
                if (_dialogResult == true)
                {
                    BM.MessageCenter.Instance.ShowInfoMessage("The soil Library is updated", "", false);
                    SoilLayerInfosLib = _vm.SoilLayerInfos;
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
                if (SoilLayerInfosLib.Count == 0)
                {
                    _mc.ShowErrorMessage("Please build up soil layer library first!", "", false);
                    return;
                }
                PickUpSoilLayersFromLibWindow _window = new PickUpSoilLayersFromLibWindow();
                PickUpSoilLayersFromLibViewModel _vm = new PickUpSoilLayersFromLibViewModel(SoilLayerInfosLib, _selectedPile.PileSoilLayersInfo);
                _window.DataContext = _vm;
                if (true == _window.ShowDialog())
                {
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







}
