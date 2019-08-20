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

namespace PDIWT_PiledWharf_Core.ViewModel
{
    public class PickUpSoilLayersFromLibViewModel : ViewModelBase
    {

        public PickUpSoilLayersFromLibViewModel(
            ObservableCollection<PDIWT_BearingCapacity_SoilLayerInfo> soilLayerInfosFromLib,
            ObservableCollection<PDIWT_BearingCapacity_SoilLayerInfo> pileInSoilLayerInfos)
        {
            _soilLayerInfosFromLib = soilLayerInfosFromLib;
            _selectedSoilLayerInfos = pileInSoilLayerInfos;
            _selectedItemsFromLib = new ObservableCollection<PDIWT_BearingCapacity_SoilLayerInfo>(_soilLayerInfosFromLib.Except(_selectedSoilLayerInfos));
        }

        private readonly ObservableCollection<PDIWT_BearingCapacity_SoilLayerInfo> _soilLayerInfosFromLib;


        private ObservableCollection<PDIWT_BearingCapacity_SoilLayerInfo> _selectedItemsFromLib;
        /// <summary>
        /// S
        /// </summary>
        public ObservableCollection<PDIWT_BearingCapacity_SoilLayerInfo> SelectedItemsFromLib
        {
            get { return _selectedItemsFromLib; }
            set
            {
                Set(ref _selectedItemsFromLib, value);
            }
        }


        private ObservableCollection<PDIWT_BearingCapacity_SoilLayerInfo> _selectedSoilLayerInfos;
        /// <summary>
        /// Property Description
        /// </summary>
        public ObservableCollection<PDIWT_BearingCapacity_SoilLayerInfo> SelectedSoilLayerInfos
        {
            get { return _selectedSoilLayerInfos; }
            set { Set(ref _selectedSoilLayerInfos, value); }
        }


        private RelayCommand _confirm;

        /// <summary>
        /// Gets the Confirm.
        /// </summary>
        public RelayCommand Confirm
        {
            get
            {
                return _confirm
                    ?? (_confirm = new RelayCommand(ExecuteConfirm));
            }
        }

        private void ExecuteConfirm()
        {
            // Bearing Capacity ViewModel Register it.
            Messenger.Default.Send(
                new NotificationMessage<ObservableCollection<PDIWT_BearingCapacity_SoilLayerInfo>>(SelectedSoilLayerInfos, "Confirm"), "SelectedSoilLayers");
            //It's own view Register it.
            Messenger.Default.Send(new NotificationMessage("close the Window"), "PickUpSoilLayersFromLibWindow");
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
                        Messenger.Default.Send(new NotificationMessage("close the Window"), "PickUpSoilLayersFromLibWindow");
                    }));
            }
        }

        public void SortLibAndPickedList()
        {
            SelectedItemsFromLib = new ObservableCollection<PDIWT_BearingCapacity_SoilLayerInfo>(SelectedItemsFromLib.OrderBy(e => e.SoilLayerNumber));
            SelectedSoilLayerInfos = new ObservableCollection<PDIWT_BearingCapacity_SoilLayerInfo>(SelectedSoilLayerInfos.OrderBy(e => e.SoilLayerNumber));
        }
    }

}
