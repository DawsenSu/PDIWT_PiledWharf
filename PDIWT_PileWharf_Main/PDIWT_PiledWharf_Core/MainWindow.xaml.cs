using System;
using System.Windows;
using System.Windows.Controls;
using PDIWT_PiledWharf_Core.ViewModel;

using BM = Bentley.MstnPlatformNET;
using BMWPF = Bentley.MstnPlatformNET.WPF;

namespace PDIWT_PiledWharf_Core
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        private MainWindow(BM.AddIn addIn)
        {
            InitializeComponent();
            Closing += (s, e) => ViewModelLocator.Cleanup();
   
            var locator = new ViewModel.ViewModelLocator();
            this.DataContext = locator.Main;

            m_wpfhelper = new BMWPF.WPFInteropHelper(this);
            m_wpfhelper.Attach(addIn, true, "MainWindow");
        }

        ~MainWindow()
        {
            Dispose(false);
        }

        static MainWindow m_mainwindowhost;
        BMWPF.WPFInteropHelper m_wpfhelper;

        public static void ShowWindow(BM.AddIn addIn)
        {
            if (m_mainwindowhost != null)
            {
                m_mainwindowhost.Focus();
                return;
            };

            m_mainwindowhost = new MainWindow(addIn);
            m_mainwindowhost.Show();
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
            if(disposing)
            {
                //m_mainwindowhost.Dispose();
            }
            m_wpfhelper.Detach();
            m_wpfhelper.Dispose();
            m_mainwindowhost = null;
        }

    }
}