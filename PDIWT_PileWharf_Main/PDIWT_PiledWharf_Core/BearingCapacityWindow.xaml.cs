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

using BM = Bentley.MstnPlatformNET;
using BMWPF = Bentley.MstnPlatformNET.WPF;
using GalaSoft.MvvmLight.Messaging;
namespace PDIWT_PiledWharf_Core
{
    using ViewModel;
    using Model;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Interaction logic for BearingCapacityWindow.xaml
    /// </summary>
    public partial class BearingCapacityWindow : Window
    {
        public BearingCapacityWindow(BM.AddIn addIn)
        {
            InitializeComponent();

            ViewModelLocator _locator = new ViewModelLocator();
            DataContext = _locator.BearingCapacity;
            m_wpfhelper = new BMWPF.WPFInteropHelper(this);
            m_wpfhelper.Attach(addIn, true, "BearingCapacityWidnow");
            Icon = new BitmapImage(new Uri("pack://application:,,,/PDIWT.Resources;component/Images/Icons/BearingCapacity.ico", UriKind.RelativeOrAbsolute));

            _controlListNeedForChange = new List<Control>()
            {
                LabelTextBox_PileName,
                CheckBox_skewness,
                LabelTextBox_PileTopElevation,
                LabelTextBox_PileWidth,
                LabelTextBox_PileInsideDiameter,
                LabelTextBox_PileLength,
                LabelTextBox_PileSkewness,
                LabelTextBox_ConcreteWeight,
                LabelTextBox_ConcreteUnderWaterWeight,
                LabelTextBox_SteelWeight,
                LabelTextBox_SteelUnderWaterWeigth,
                Lable_PileGeoType,
                //Lable_PileWorkmanshipType,
            };

            Messenger.Default.Register<NotificationMessage<bool>>(this, "BearingCapacityForegroundChange", ChangeControlsForeground);
            Messenger.Default.Register<Visibility>(this, "ShowMainWindow", v => Visibility = v);
            Messenger.Default.Register<BearingCapacityWindowType>(this, OpenWindows);

            Closed += (s, e) => Messenger.Default.Unregister(this);
        }

        readonly BM.MessageCenter _mc = BM.MessageCenter.Instance;
        static BearingCapacityWindow m_BearingCapacityWindowHost;
        BMWPF.WPFInteropHelper m_wpfhelper;

        public static void ShowWindow(BM.AddIn addIn)
        {
            if (m_BearingCapacityWindowHost != null)
            {
                m_BearingCapacityWindowHost.Focus();
                return;
            };

            m_BearingCapacityWindowHost = new BearingCapacityWindow(addIn);
            m_BearingCapacityWindowHost.Show();
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(true);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                //m_SettingsWindowhost.Dispose();
            }
            m_wpfhelper.Detach();
            m_wpfhelper.Dispose();
            m_BearingCapacityWindowHost = null;
        }

        /// <summary>
        /// Gets the view's ViewModel.
        /// </summary>
        public BearingCapacityViewModel Vm
        {
            get
            {
                return (BearingCapacityViewModel)DataContext;
            }
        }

        List<Control> _controlListNeedForChange;
        private void ChangeControlsForeground(NotificationMessage<bool> notification)
        {
            bool _isLoadFromEntity = notification.Content;
            foreach (var _control in _controlListNeedForChange)
            {
                if (_isLoadFromEntity)
                    _control.Foreground = FindResource("LoadedParametersForeground") as SolidColorBrush;
                else
                    _control.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void OpenWindows(BearingCapacityWindowType windowType)
        {
            switch (windowType)
            {
                case BearingCapacityWindowType.SoilLayerLibraryWindow:
                    try
                    {
                        BuildUpSoilLayersWindow _window = new BuildUpSoilLayersWindow();

                        BuildUpSoilLayersViewModel _vm = new BuildUpSoilLayersViewModel()
                        {
                            SoilLayerCollection = new SoilLayerCollection(Vm.SoilLayersLibrary)
                        };
                        _window.DataContext = _vm;
                        _window.Owner = this;

                        bool? _dialogResult = _window.ShowDialog();
                        if (_dialogResult == true)
                        {
                            BM.MessageCenter.Instance.ShowInfoMessage("The soil Library is updated", "", false);
                            Vm.SoilLayersLibrary = _vm.SoilLayerCollection;
                        }

                    }
                    catch (Exception e)
                    {
                        BM.MessageCenter.Instance.ShowErrorMessage("The soil Library is Can't be update", e.ToString(), false);
                    }
                    break;
                case BearingCapacityWindowType.SoilLayerPickUpWindow:
                    try
                    {
                        if (Vm.SoilLayersLibrary.Count == 0)
                        {
                            _mc.ShowErrorMessage("Please build up soil layer library first!", "", false);
                            return;
                        }
                        PickUpSoilLayersFromLibWindow _window = new PickUpSoilLayersFromLibWindow();
                        ObservableCollection<PDIWT_BearingCapacity_SoilLayerInfo> _soilLayerInfos = new ObservableCollection<PDIWT_BearingCapacity_SoilLayerInfo>();
                        foreach (var _layer in Vm.SoilLayersLibrary)
                            _soilLayerInfos.Add(new PDIWT_BearingCapacity_SoilLayerInfo() { SoilLayerObject = _layer, PileIntersectionBottomEle = 0, PileIntersectionTopEle = 0 });

                        PickUpSoilLayersFromLibViewModel _vm = new PickUpSoilLayersFromLibViewModel(_soilLayerInfos, Vm.SelectedPile.PileSoilLayersInfo);
                        _window.DataContext = _vm;
                        _window.Owner = this;
                        if (true == _window.ShowDialog())
                        {
                            Vm.SelectedPile.PileSoilLayersInfo = new ObservableCollection<PDIWT_BearingCapacity_SoilLayerInfo>(_vm.SelectedSoilLayerInfos);
                            _mc.ShowInfoMessage("Picked soil layers from library", "", false);
                        }
                    }
                    catch (Exception e)
                    {
                        _mc.ShowErrorMessage("Can't pick soil layer", e.ToString(), false);
                    }
                    break;
                case BearingCapacityWindowType.ReportGenerator:
                    try
                    {
                        ReportGeneratorWindow _reportWindow = new ReportGeneratorWindow();
                        Messenger.Default.Send(new NotificationMessage(Vm, "BearingCapacityViewModelInvoke"), "ViewModelForReport");
                        _reportWindow.Owner = this;
                        _reportWindow.ShowDialog();
                    }
                    catch (Exception e)
                    {
                        _mc.ShowErrorMessage(e.Message, e.ToString(), false);
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
