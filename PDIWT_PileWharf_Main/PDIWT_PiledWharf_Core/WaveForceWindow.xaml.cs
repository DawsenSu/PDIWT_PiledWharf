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
    /// Interaction logic for WaveForceWindow.xaml
    /// </summary>
    public partial class WaveForceWindow : Window
    {
        public WaveForceWindow(BM.AddIn addIn)
        {
            InitializeComponent();

            ViewModelLocator _locator = new ViewModelLocator();
            DataContext = _locator.WaveForce;
            m_wpfhelper = new BMWPF.WPFInteropHelper(this);
            m_wpfhelper.Attach(addIn, true, "WaveForceWindow");
            Icon = new BitmapImage(new Uri("pack://application:,,,/PDIWT.Resources;component/Images/Icons/WaveForce.ico", UriKind.RelativeOrAbsolute));

            _controlListNeedForChange = new List<Control>()
            {
                Label_Shape,
                LabelTextBox_PileDiameter,
                LabelTextBox_HAT,
                LabelTextBox_MHW,
                LabelTextBox_MLW,
                LabelTextBox_LAT,
                LabelTextBox_WaterDensity,
                Label_WaveHeight,
                Label_Period
            };

            Messenger.Default.Register<NotificationMessage<bool>>(this, "WaveForceForegroundChange", ChangeControlsForeground);
            Messenger.Default.Register<Visibility>(this, "ShowMainWindow", v => Visibility = v);

            Closed += (s, e) => Messenger.Default.Unregister(this);
        }

        static WaveForceWindow m_WaveForceWindowHost;
        BMWPF.WPFInteropHelper m_wpfhelper;

        public static void ShowWindow(BM.AddIn addIn)
        {
            if (m_WaveForceWindowHost != null)
            {
                m_WaveForceWindowHost.Focus();
                return;
            };

            m_WaveForceWindowHost = new WaveForceWindow(addIn);
            m_WaveForceWindowHost.Show();
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
            m_WaveForceWindowHost = null;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
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
