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
    using LiveCharts.Defaults;

    /// <summary>
    /// Interaction logic for PileLengthCalculatorWindow.xaml
    /// </summary>
    public partial class PileLengthCalculatorWindow : Window
    {
        public PileLengthCalculatorWindow(BM.AddIn addIn)
        {
            InitializeComponent();

            ViewModelLocator _locator = new ViewModelLocator();
            DataContext = _locator.PileLengthCalculator;
            m_wpfhelper = new BMWPF.WPFInteropHelper(this);
            m_wpfhelper.Attach(addIn, true, "PileLengthCalculatorWindow");
            Icon = new BitmapImage(new Uri("pack://application:,,,/PDIWT.Resources;component/Images/Icons/PileLength.ico", UriKind.RelativeOrAbsolute));

            xAxis.LabelFormatter = value => (-value).ToString();
            LineSeries.LabelPoint = value => string.Format("BearCapacity:{0:F2}\nLength:{1:F2}", value.X, -value.Y);

            //Messenger.Default.Register<PileLengthCalculatorWindowType>(this, ShowWindow);
            //Closing += (s, e) => Messenger.Default.Unregister(this);
        }

        readonly BM.MessageCenter _mc = BM.MessageCenter.Instance;
        static PileLengthCalculatorWindow m_PileLengthCalculatorWindowHost;
        BMWPF.WPFInteropHelper m_wpfhelper;

        public static void ShowWindow(BM.AddIn addIn)
        {
            if (m_PileLengthCalculatorWindowHost != null)
            {
                m_PileLengthCalculatorWindowHost.Focus();
                return;
            };

            m_PileLengthCalculatorWindowHost = new PileLengthCalculatorWindow(addIn);
            m_PileLengthCalculatorWindowHost.Show();
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
            m_PileLengthCalculatorWindowHost = null;
        }

        /// <summary>
            /// Gets the view's ViewModel.
            /// </summary>
        public PileLengthCalculatorViewModel Vm
        {
            get
            {
                return (PileLengthCalculatorViewModel)DataContext;
            }
        }

        //private void ShowWindow(PileLengthCalculatorWindowType windowType)
        //{
        //    switch (windowType)
        //    {
        //        case PileLengthCalculatorWindowType.CurveWidnow:
        //            try
        //            {
        //                double _uorpermeter = BM.Session.Instance.GetActiveDgnModel().GetModelInfo().UorPerMeter;
        //                VirtualSteelOrTubePile _pile = new VirtualSteelOrTubePile()
        //                {
        //                    PileName = Vm.SelectedVirtualPile[0].ToString(),
        //                    PileX = (double)Vm.SelectedVirtualPile[1] * _uorpermeter,
        //                    PileY = (double)Vm.SelectedVirtualPile[2] * _uorpermeter,
        //                    PileZ = (double)Vm.SelectedVirtualPile[3] * _uorpermeter,
        //                    PileSkewness = (double)Vm.SelectedVirtualPile[4],
        //                    PlanRotationAngle = (double)Vm.SelectedVirtualPile[5],
        //                    PileDiameter = (double)Vm.SelectedVirtualPile[6] * _uorpermeter,
        //                    PileInnerDiameter = (double)Vm.SelectedVirtualPile[7] * _uorpermeter,
        //                };
        //                switch (_pile.GetPileBearingCapacityCurveInfo(Vm.PartialCoefficient, Vm.BlockCoefficient, out ObservableCollection<Tuple<double, double>> _results))
        //                {
        //                    case GetPileBearingCapacityCurveInfoStatus.InvalidObjectStruct:
        //                        _mc.ShowMessage(BM.MessageType.Warning, "Invalid input parameter or internal error", _pile.ToString(), BM.MessageAlert.None);
        //                        return;
        //                    case GetPileBearingCapacityCurveInfoStatus.NoIntersection:
        //                        _mc.ShowMessage(BM.MessageType.Warning, "No intersection between soil layer and pile", _pile.ToString(), BM.MessageAlert.None);
        //                        return;
        //                    case GetPileBearingCapacityCurveInfoStatus.Success:
        //                        break;
        //                }

        //                LiveCharts.ChartValues<ObservablePoint> _chartPoints = new LiveCharts.ChartValues<ObservablePoint>();
        //                foreach (var _result in _results)
        //                {
        //                    _chartPoints.Add(new ObservablePoint(_result.Item2, -_result.Item1 / _uorpermeter));
        //                }
        //                //LiveCharts.ChartValues<Tuple<double, double>> _tuples = new LiveCharts.ChartValues<Tuple<double, double>>();
        //                //foreach (var _result in _results)
        //                //{
        //                //    _tuples.Add(Tuple.Create(_result.Item2, _result.Item1 / _uorpermeter));
        //                //}

        //                PileLengthCurveWindow _curveWindow = new PileLengthCurveWindow();
        //                PileLengthCurveViewModel _vm = new PileLengthCurveViewModel()
        //                {
        //                    //SeriesCollection = new LiveCharts.SeriesCollection
        //                    //{
        //                    //    new LiveCharts.Wpf.LineSeries{Title="PileLength/m",Values = _chartPoints, LineSmoothness=0}
        //                    //}
        //                    ////DataResources = _tuples
        //                    DataResources = _chartPoints
        //                };
        //                _curveWindow.DataContext = _vm;
        //                _curveWindow.Owner = this;
        //                _curveWindow.ShowDialog();
        //            }
        //            catch (Exception e)
        //            {
        //                _mc.ShowErrorMessage("Unknown reason: can't show BC - length curve", e.ToString(), false);
        //            }
        //            break;
        //        default:
        //            break;
        //    }
        //}

        //private void DataGird_PileTable_LoadingAndUnLoadingRow(object sender, DataGridRowEventArgs e)
        //{
        //    e.Row.Header = e.Row.GetIndex() + 1;
        //}

    }
}
