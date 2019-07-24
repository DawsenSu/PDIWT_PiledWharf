using System;
using System.Windows;
using System.Threading;
using System.Windows.Controls;
using PDIWT_PiledWharf_Core.ViewModel;

using GalaSoft.MvvmLight.Messaging;

using BM = Bentley.MstnPlatformNET;
using BMWPF = Bentley.MstnPlatformNET.WPF;
using System.Windows.Media.Imaging;

namespace PDIWT_PiledWharf_Core
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the SettingsWindow class.
        /// </summary>
        private SettingsWindow(BM.AddIn addIn)
        {
            InitializeComponent();
            Closing += (s, e) => ViewModelLocator.Cleanup();
            ViewModelLocator locator = new ViewModelLocator();
            DataContext = locator.Settings;

            m_wpfhelper = new BMWPF.WPFInteropHelper(this);
            m_wpfhelper.Attach(addIn, true, "SettingsWindow");
            Icon = new BitmapImage(new Uri("pack://application:,,,/PDIWT.Resources;component/Images/Icons/Settings.ico", UriKind.RelativeOrAbsolute));

            Messenger.Default.Register<NotificationMessage>(this,(e)=> { if (e.Notification == "CloseWindow" )this.Close(); });
        }

        ~SettingsWindow()
        {
            Dispose(false);
        }

        static SettingsWindow m_SettingsWindowhost;
        BMWPF.WPFInteropHelper m_wpfhelper;

        public static void ShowWindow(BM.AddIn addIn)
        {
            if (m_SettingsWindowhost != null)
            {
                m_SettingsWindowhost.Focus();
                return;
            };

            m_SettingsWindowhost = new SettingsWindow(addIn);
            m_SettingsWindowhost.Show();
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
                //m_SettingsWindowhost.Dispose();
            }
            m_wpfhelper.Detach();
            m_wpfhelper.Dispose();
            m_SettingsWindowhost = null;
        }

        private void Button_Click_CancelWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}