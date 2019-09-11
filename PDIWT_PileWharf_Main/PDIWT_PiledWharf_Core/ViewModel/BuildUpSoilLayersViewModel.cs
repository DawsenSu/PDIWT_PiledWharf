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
using System.Windows.Controls;
using System.Collections.Specialized;

using BDE = Bentley.DgnPlatformNET.Elements;
using BD = Bentley.DgnPlatformNET;

namespace PDIWT_PiledWharf_Core.ViewModel
{
    using Model;
    public class BuildUpSoilLayersViewModel : ViewModelBase
    {
        public BuildUpSoilLayersViewModel()
        {
            //SoilLayerInfos = new ObservableCollection<PDIWT_BearingCapacity_SoilLayerInfo>();
            _soilLayerCollection = new SoilLayerCollection();
        }

        /// <summary>
        /// Property Description
        /// </summary>
        //public ObservableCollection<PDIWT_BearingCapacity_SoilLayerInfo> SoilLayerInfos { get; set; }


        //private PDIWT_BearingCapacity_SoilLayerInfo _selectedSoilLayer;
        ///// <summary>
        ///// Property Description
        ///// </summary>
        //public PDIWT_BearingCapacity_SoilLayerInfo SelectedSoilLayer
        //{
        //    get { return _selectedSoilLayer; }
        //    set { Set(ref _selectedSoilLayer, value); }
        //}


        private SoilLayerCollection _soilLayerCollection;
        /// <summary>
        /// Property Description
        /// </summary>
        public SoilLayerCollection SoilLayerCollection
        {
            get { return _soilLayerCollection; }
            set { Set(ref _soilLayerCollection, value); }
        }


        private SoilLayer _selectedSoilLayer;
        /// <summary>
        /// Property Description
        /// </summary>
        public SoilLayer SelectedSoilLayer
        {
            get { return _selectedSoilLayer; }
            set { Set(ref _selectedSoilLayer, value); }
        }

        private RelayCommand<DataGridRowEditEndingEventArgs> _addNewSoilLayer;

        /// <summary>
        /// Gets the AddNewSoilLayer.
        /// </summary>
        public RelayCommand<DataGridRowEditEndingEventArgs> AddNewSoilLayer
        {
            get
            {
                return _addNewSoilLayer
                    ?? (_addNewSoilLayer = new RelayCommand<DataGridRowEditEndingEventArgs>(
                    p =>
                    {
                        var _item = p.Row.Item;
                        if (_item is SoilLayer)
                        {
                            var _newSoilLayer = _item as SoilLayer;

                            if (p.EditAction == DataGridEditAction.Commit)
                            {
                                _isReadyForAction = true;

                                if (string.IsNullOrEmpty(_newSoilLayer.Number))
                                {
                                    MessageBox.Show($"Newly adding soil number {_newSoilLayer.Number} doesn't allow to be empty.", "Invalid Soil Layer Information", MessageBoxButton.OK, MessageBoxImage.Information);
                                    p.Cancel = true;
                                    p.Row.Focus();
                                    _isReadyForAction = false;
                                }
                                var _exceptCollection = SoilLayerCollection.Except(new List<SoilLayer> { _newSoilLayer });
                                if (_exceptCollection.Contains(_newSoilLayer))
                                {
                                    MessageBox.Show($"Newly adding soil number {_newSoilLayer.Number} has already existed in current list.", "Duplicated Soil Layer Information", MessageBoxButton.OK, MessageBoxImage.Information);
                                    p.Cancel = true;
                                    p.Row.Focus();
                                    _isReadyForAction = false;
                                }
                            }
                        }

                    }));
            }
        }

        bool _isReadyForAction = true;

        private RelayCommand _confirm;

        /// <summary>
        /// Gets the Confirm.
        /// </summary>
        public RelayCommand Confirm
        {
            get
            {
                return _confirm
                    ?? (_confirm = new RelayCommand(ExecuteConfirm, () => _isReadyForAction));
            }
        }

        private void ExecuteConfirm()
        {
            SoilLayerCollection = new SoilLayerCollection(SoilLayerCollection.OrderBy(e => e.Number));
            // Bearing Capacity ViewModel Register it.
            //Messenger.Default.Send(
            //    new NotificationMessage<ObservableCollection<PDIWT_BearingCapacity_SoilLayerInfo>>(SoilLayerInfos, "Confirm"), "BuildupSoilLayerLib");
            //It's own view Register it.
            Messenger.Default.Send(new NotificationMessage(PDIWT.Resources.Localization.MainModule.Resources.OK), "BuildUpWindowButtonClicked");
        }

        private RelayCommand _cancel;

        /// <summary>
        /// Gets the Cancel.
        /// </summary>
        public RelayCommand Cancel
        {
            get
            {
                return _cancel
                    ?? (_cancel = new RelayCommand(
                    () =>
                    {
                        Messenger.Default.Send(new NotificationMessage(PDIWT.Resources.Localization.MainModule.Resources.Cancel), "BuildUpWindowButtonClicked");
                    },
                    () => _isReadyForAction));
            }
        }

        private RelayCommand<CancelEventArgs> _mainWindowClosing;

        /// <summary>
        /// Gets the MainWindowClosing.
        /// </summary>
        public RelayCommand<CancelEventArgs> MainWindowClosing
        {
            get
            {
                return _mainWindowClosing
                    ?? (_mainWindowClosing = new RelayCommand<CancelEventArgs>(
                    p =>
                    {
                        foreach (var _layerInfo in SoilLayerCollection)
                        {
                            if (_layerInfo.Number == null)
                                _isReadyForAction = false;  
                        }
                        if (_isReadyForAction == false)
                        {
                            MessageBox.Show(PDIWT.Resources.Localization.MainModule.Resources.Note_EEIL);
                            p.Cancel = true;
                        }
                    }));
            }
        }

        private RelayCommand _add;

        /// <summary>
        /// Gets the Add.
        /// </summary>
        public RelayCommand Add
        {
            get
            {
                return _add
                    ?? (_add = new RelayCommand(
                    () =>
                    {
                        var _newlayer = new SoilLayer();
                        SoilLayerCollection.Add(_newlayer);
                        SelectedSoilLayer = _newlayer;
                    }));
            }
        }

        private RelayCommand _delete;

        /// <summary>
        /// Gets the Delete.
        /// </summary>
        public RelayCommand Delete
        {
            get
            {
                return _delete
                    ?? (_delete = new RelayCommand(
                    () =>
                    {
                        var _temp = SelectedSoilLayer;

                        if (SoilLayerCollection.Count == 1)
                        {
                            SelectedSoilLayer = null;
                        }
                        else
                        {
                            int _indexOfSelectedRow = SoilLayerCollection.IndexOf(SelectedSoilLayer);

                            if (_indexOfSelectedRow == SoilLayerCollection.Count - 1)
                                SelectedSoilLayer = SoilLayerCollection.ElementAt(_indexOfSelectedRow - 1);
                            else
                                SelectedSoilLayer = SoilLayerCollection.ElementAt(_indexOfSelectedRow + 1);
                        }
                        SoilLayerCollection.Remove(_temp);
                    },
                    () => SelectedSoilLayer != null));
            }
        }

        private RelayCommand _loadFromModel;

        /// <summary>
        /// Gets the LoadFromModel.
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
            var _mc = Bentley.MstnPlatformNET.MessageCenter.Instance;
            try
            {
                SoilLayerCollection = SoilLayerCollection.ObtainFromModel(Bentley.MstnPlatformNET.Session.Instance.GetActiveDgnModel());

                //SoilLayerCollection.Clear();
                //foreach (var _layer in _soilLayer)
                //{
                //    var _soilLayerInfo = new PDIWT_BearingCapacity_SoilLayerInfo()
                //    {
                //        SoilLayerNumber = _layer.SoilLayerNumber,
                //        SoilLayerName = _layer.SoilLayerName,
                //        Betasi = _layer.Betasi,
                //        Psii = _layer.Psii,
                //        SideFrictionStandardValue = _layer.SideFrictionStandardValue,
                //        Betap = _layer.Betap,
                //        Psip = _layer.Psip,
                //        EndResistanceStandardValue = _layer.EndResistanceStandardValue
                //    };
                //    SoilLayerInfos.Add(_soilLayerInfo);
                //}
                _mc.ShowMessage(Bentley.MstnPlatformNET.MessageType.Info, "Load successfully from model", "", Bentley.MstnPlatformNET.MessageAlert.None);
            }
            catch (Exception e)
            {
                _mc.ShowErrorMessage("Can't load soil layers information from model", e.ToString(), false);
            }

            //List<Tuple<BDE.MeshHeaderElement, Dictionary<string, object>>> _tuples = new List<Tuple<BDE.MeshHeaderElement, Dictionary<string, object>>>();
            //if (BD.StatusInt.Success == PDIWT_SoilLayerInfoReader.ObtainSoilLayerInfoFromModel(out _tuples))
            //{
            //    SoilLayerInfos.Clear();
            //    foreach (var _item in _tuples)
            //    {
            //        var _soilLayerInfo = new PDIWT_BearingCapacity_SoilLayerInfo();
            //        _soilLayerInfo.SoilLayerNumber = _item.Item2["LayerNumber"].ToString();
            //        _soilLayerInfo.SoilLayerName = _item.Item2["LayerName"].ToString();
            //        _soilLayerInfo.Betasi = double.Parse(_item.Item2["Betasi"].ToString());
            //        _soilLayerInfo.Psii = double.Parse(_item.Item2["Psisi"].ToString());
            //        _soilLayerInfo.SideFrictionStandardValue = double.Parse(_item.Item2["qfi"].ToString());
            //        _soilLayerInfo.Betap = double.Parse(_item.Item2["Betap"].ToString());
            //        _soilLayerInfo.Psip = double.Parse(_item.Item2["Psip"].ToString());
            //        _soilLayerInfo.EndResistanceStandardValue = double.Parse(_item.Item2["qr"].ToString());
            //        SoilLayerInfos.Add(_soilLayerInfo);
            //    }
            //    //SoilLayerInfos = new ObservableCollection<PDIWT_BearingCapacity_SoilLayerInfo>(SoilLayerInfos.OrderByDescending(layer => layer.SoilLayerNumber));
            //    _mc.ShowMessage(Bentley.MstnPlatformNET.MessageType.Info, "Load successfully from model", "", Bentley.MstnPlatformNET.MessageAlert.None);
            //}
            //else
            //{
                
            //}
        }
    }
}
