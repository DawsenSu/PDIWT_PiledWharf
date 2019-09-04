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
        }

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

        private void DataGird_PileTable_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }
    }
}
