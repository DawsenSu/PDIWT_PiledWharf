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

using PDIWT_PiledWharf_Core.ViewModel;

using BM = Bentley.MstnPlatformNET;
using BMWPF = Bentley.MstnPlatformNET.WPF;
using GalaSoft.MvvmLight.Messaging;

namespace PDIWT_PiledWharf_Core
{
    /// <summary>
    /// Interaction logic for CurrentForceWindow.xaml
    /// </summary>
    public partial class CurrentForceWindow : Window
    {
        public CurrentForceWindow(BM.AddIn addIn)
        {
            InitializeComponent();

            Closing += (s, e) => ViewModelLocator.Cleanup();
            ViewModelLocator _locator = new ViewModelLocator();
            DataContext = _locator.CurrentForce;
            m_wpfhelper = new BMWPF.WPFInteropHelper(this);
            m_wpfhelper.Attach(addIn, true, "CurrentForceWindow");
            Icon = new BitmapImage(new Uri("pack://application:,,,/PDIWT.Resources;component/Images/Icons/WaterForce.ico", UriKind.RelativeOrAbsolute));

            _controlListNeedForChange = new List<Control> {
                LabelTextBox_PileTopElevation,
                LabelTextBox_PileHAT,
                LabelTextBox_SoilElevation,
                LabelTextBox_ProjectedWidth,
                Label_Shape,
                //LabelTextBox_SquarePileAngle,
                //LabelTextBox_CurrentVelocity,
                //LabelTextBox_PileHorizontalCentraSpan,
                //LabelTextBox_PileVerticalCentraSpan,
                LabelTextBox_WaterDensity};

            Messenger.Default.Register<NotificationMessage<bool>>(this, "ControlForegroundChange", ChangeControlsForeground);
            Messenger.Default.Register<Visibility>(this, "ShowMainWindow", v => Visibility = v);

            Closed += (s, e) => Messenger.Default.Unregister(this);

        }

        static CurrentForceWindow m_CurrentForceWindowhost;
        BMWPF.WPFInteropHelper m_wpfhelper;

        public static void ShowWindow(BM.AddIn addIn)
        {
            if (m_CurrentForceWindowhost != null)
            {
                m_CurrentForceWindowhost.Focus();
                return;
            };

            m_CurrentForceWindowhost = new CurrentForceWindow(addIn);
            m_CurrentForceWindowhost.Show();
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
            m_CurrentForceWindowhost = null;
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
