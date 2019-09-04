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

using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Ioc;

using BM = Bentley.MstnPlatformNET;
using BMWPF = Bentley.MstnPlatformNET.WPF;

namespace PDIWT_PiledWharf_Core
{
    /// <summary>
    /// Interaction logic for Input_ImportFromFileWindow.xaml
    /// </summary>
    public partial class Input_ImportFromFileWindow : Window, IDisposable
    {
        private Input_ImportFromFileWindow(BM.AddIn addIn)
        {
            InitializeComponent();
            Closing += (s, e) => ViewModelLocator.Cleanup();
            ViewModelLocator _viewModelLocator = new ViewModelLocator();
            DataContext = _viewModelLocator.Input_ImportFromFile;

            m_wpfhelper = new BMWPF.WPFInteropHelper(this);
            m_wpfhelper.Attach(addIn, true, "Input_ImportFromFileWindow");
            Icon = new BitmapImage(new Uri("pack://application:,,,/PDIWT.Resources;component/Images/Icons/ImportFromFiles.ico", UriKind.RelativeOrAbsolute));
            if (!SimpleIoc.Default.IsRegistered<BM.AddIn>())
                SimpleIoc.Default.Register<BM.AddIn>(() => addIn);
            Messenger.Default.Register<Visibility>(this, "ImportWindowVisibility", v => Visibility = v);
        }

        ~Input_ImportFromFileWindow()
        {
            Dispose(false);
        }

        static Input_ImportFromFileWindow m_mainwindowhost;
        BMWPF.WPFInteropHelper m_wpfhelper;

        public static void ShowWindow(BM.AddIn addIn)
        {
            if (m_mainwindowhost != null)
            {
                m_mainwindowhost.Focus();
                return;
            };

            m_mainwindowhost = new Input_ImportFromFileWindow(addIn);
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
            if (disposing)
            {
                //m_mainwindowhost.Dispose();
            }
            m_wpfhelper.Detach();
            m_wpfhelper.Dispose();
            m_mainwindowhost = null;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    this.Close();
        //}
    }
}
