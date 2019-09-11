using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using PDIWT.Resources;

namespace PDIWT_PiledWharf_Core
{
    using ViewModel;
    using Model;
    /// <summary>
    /// Interaction logic for PickUpSoilLayersFromLibWindow.xaml
    /// </summary>
    public partial class PickUpSoilLayersFromLibWindow : Window
    {
        public PickUpSoilLayersFromLibWindow()
        {
            InitializeComponent();
            Icon = new BitmapImage(new Uri("pack://application:,,,/PDIWT.Resources;component/Images/Icons/BearingCapacity.ico", UriKind.RelativeOrAbsolute));

            Messenger.Default.Register<NotificationMessage>(this, "PickUpSoilLayersFromLibButtonClick", 
                notification =>
                {
                    if (notification.Notification == PDIWT.Resources.Localization.MainModule.Resources.OK)
                        DialogResult = true;
                    else
                        Close();
                });
            Closed += (s, e) => Messenger.Default.Unregister<NotificationMessage>(this, "PickUpSoilLayersFromLibButtonClick");
        }

        private void Button_Click_ToRight(object sender, RoutedEventArgs e)
        {
            var _libItems = ListBox_Library.SelectedItems;

            if (_libItems == null || _libItems.Count == 0)
                return;

            List<PDIWT_BearingCapacity_SoilLayerInfo> _libItemSoilLayers = new List<PDIWT_BearingCapacity_SoilLayerInfo>(_libItems.Cast<PDIWT_BearingCapacity_SoilLayerInfo>());

            //foreach (var _item in _libItems)
            //{
            //    if (_item is PDIWT_BearingCapacity_SoilLayerInfo _itemSoilLayerInfo)
            //    {
            //        _libItemSoilLayers.Add(PDIWT_Helper.DeepCopyByXml(_itemSoilLayerInfo));
            //    }
            //}
            foreach (var _info in _libItemSoilLayers)
            {
                Vm.SelectedSoilLayerInfos.Add(_info);
                Vm.SelectedItemsFromLib.Remove(_info);
            }
            //Vm.SortLibAndPickedList();
        }

        private void Button_Click_ToLeft(object sender, RoutedEventArgs e)
        {
            var _selectedItems = ListBox_Picked.SelectedItems;

            if (_selectedItems == null || _selectedItems.Count == 0)
                return;

            //List<PDIWT_BearingCapacity_SoilLayerInfo> _libItemSoilLayers = new List<PDIWT_BearingCapacity_SoilLayerInfo>();

            //foreach (var _item in _libItems)
            //{
            //    if (_item is PDIWT_BearingCapacity_SoilLayerInfo _itemSoilLayerInfo)
            //    {
            //        _libItemSoilLayers.Add(PDIWT_Helper.DeepCopyByXml(_itemSoilLayerInfo));
            //    }
            //}
            List<PDIWT_BearingCapacity_SoilLayerInfo> _selectedItemSoilLayers = new List<PDIWT_BearingCapacity_SoilLayerInfo>(_selectedItems.Cast<PDIWT_BearingCapacity_SoilLayerInfo>());

            foreach (var _info in _selectedItemSoilLayers)
            {
                Vm.SelectedSoilLayerInfos.Remove(_info);
                Vm.SelectedItemsFromLib.Add(_info);
            }
            //Vm.SortLibAndPickedList();
        }

        /// <summary>
        /// Gets the view's ViewModel.
        /// </summary>
        public PickUpSoilLayersFromLibViewModel Vm
        {
            get
            {
                return (PickUpSoilLayersFromLibViewModel)DataContext;
            }
        }

        private void Buttom_ToUp_Click(object sender, RoutedEventArgs e)
        {
            var _soilLayer = (PDIWT_BearingCapacity_SoilLayerInfo)ListBox_Picked.SelectedItem;
            int _index = Vm.SelectedSoilLayerInfos.IndexOf(_soilLayer);
            if (_index != 0)
                Vm.SelectedSoilLayerInfos.Move(_index, _index - 1);
        }

        private void Buttom_ToDown_Click(object sender, RoutedEventArgs e)
        {
            var _soilLayer = (PDIWT_BearingCapacity_SoilLayerInfo)ListBox_Picked.SelectedItem;
            int _index = Vm.SelectedSoilLayerInfos.IndexOf(_soilLayer);
            if (_index != Vm.SelectedSoilLayerInfos.Count - 1)
                Vm.SelectedSoilLayerInfos.Move(_index, _index + 1);
        }

        private void ListBox_Picked_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListBox_Picked.SelectedItems.Count == 1)
            {
                Buttom_ToUp.IsEnabled = true;
                Buttom_ToDown.IsEnabled = true;
            }
            else
            {
                Buttom_ToUp.IsEnabled = false;
                Buttom_ToDown.IsEnabled = false;
            }
        }
    }
}
