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
                Lable_PileWorkmanshipType,
            };

            Messenger.Default.Register<NotificationMessage<bool>>(this, "BearingCapacityForegroundChange", ChangeControlsForeground);
        }

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
    }
}
