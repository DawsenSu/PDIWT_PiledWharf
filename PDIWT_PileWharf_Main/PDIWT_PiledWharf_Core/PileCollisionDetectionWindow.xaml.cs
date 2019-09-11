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
using PDIWT_PiledWharf_Core.ViewModel;

namespace PDIWT_PiledWharf_Core
{
    /// <summary>
    /// Interaction logic for PileCollisionDetectionWindow.xaml
    /// </summary>
    public partial class PileCollisionDetectionWindow : Window
    {
        public PileCollisionDetectionWindow(BM.AddIn addIn)
        {
            InitializeComponent();

            ViewModelLocator _locator = new ViewModelLocator();
            DataContext = _locator.PileCollisionDetection;
            m_wpfhelper = new BMWPF.WPFInteropHelper(this);
            m_wpfhelper.Attach(addIn, true, "PileCollisionDetectionWindow");
            Icon = new BitmapImage(new Uri("pack://application:,,,/PDIWT.Resources;component/Images/Icons/Collision.ico", UriKind.RelativeOrAbsolute));

        }

        static PileCollisionDetectionWindow m_PileCollisionDetectionWindowHost;
        BMWPF.WPFInteropHelper m_wpfhelper;

        public static void ShowWindow(BM.AddIn addIn)
        {
            if (m_PileCollisionDetectionWindowHost != null)
            {
                m_PileCollisionDetectionWindowHost.Focus();
                return;
            };

            m_PileCollisionDetectionWindowHost = new PileCollisionDetectionWindow(addIn);
            m_PileCollisionDetectionWindowHost.Show();
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
            m_PileCollisionDetectionWindowHost = null;
        }

    }
}
