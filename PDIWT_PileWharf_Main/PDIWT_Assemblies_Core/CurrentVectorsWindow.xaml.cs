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

namespace PDIWT_Assemblies_Core
{
    using ViewModel;
    using Model;
    using System.Collections.ObjectModel;
    using GalaSoft.MvvmLight.Ioc;

    /// <summary>
    /// Interaction logic for CurrentVectorsWindow.xaml
    /// </summary>
    public partial class CurrentVectorsWindow : Window, IDisposable
    {
        public CurrentVectorsWindow(BM.AddIn addIn)
        {
            InitializeComponent();

            ViewModelLocator _locator = new ViewModelLocator();
            DataContext = _locator.CurrentVectors;
            m_wpfhelper = new BMWPF.WPFInteropHelper(this);
            m_wpfhelper.Attach(addIn, true, "CurrentVectorsWindow");
            Icon = new BitmapImage(new Uri("pack://application:,,,/PDIWT.Resources;component/Images/Icons/CurrentVectors.ico", UriKind.RelativeOrAbsolute));

            if (!SimpleIoc.Default.IsRegistered<BM.AddIn>())
                SimpleIoc.Default.Register(() => addIn);

            comboBox_pickMethod.ItemsSource = PDIWT.Resources.PDIWT_Helper.GetEnumDescriptionDictionary<PickMenthod>();

            Closed += (s, e) => Messenger.Default.Unregister(this);
        }

        readonly BM.MessageCenter _mc = BM.MessageCenter.Instance;
        static CurrentVectorsWindow m_CurrentVectorsWindowHost;
        BMWPF.WPFInteropHelper m_wpfhelper;

        public static void ShowWindow(BM.AddIn addIn)
        {
            if (m_CurrentVectorsWindowHost != null)
            {
                m_CurrentVectorsWindowHost.Focus();
                return;
            };

            m_CurrentVectorsWindowHost = new CurrentVectorsWindow(addIn);
            m_CurrentVectorsWindowHost.Show();
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
            m_CurrentVectorsWindowHost = null;
        }
    }
}
