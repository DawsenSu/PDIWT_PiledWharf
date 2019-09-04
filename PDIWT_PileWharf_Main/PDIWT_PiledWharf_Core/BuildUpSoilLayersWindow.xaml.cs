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
using GalaSoft.MvvmLight.Messaging;


namespace PDIWT_PiledWharf_Core
{
    using Common;
    /// <summary>
    /// Interaction logic for BuildUpSoilLayersWindow.xaml
    /// </summary>
    public partial class BuildUpSoilLayersWindow : Window
    {
        public BuildUpSoilLayersWindow()
        {
            InitializeComponent();
            //var _loactor = new ViewModel.ViewModelLocator();
            //DataContext = _loactor.BuildUpSoilLayers;
            Icon = new BitmapImage(new Uri("pack://application:,,,/PDIWT.Resources;component/Images/Icons/BearingCapacity.ico", UriKind.RelativeOrAbsolute));

            Messenger.Default.Register<NotificationMessage>(this, "BuildUpWindowButtonClicked", e =>
            {

                if (e.Notification == PDIWT.Resources.Localization.MainModule.Resources.OK)
                    DialogResult = true;
                else
                    Close();

            });
            Closed += (s,e) => Messenger.Default.Unregister<NotificationMessage>(this, "BuildUpWindowButtonClicked");
        }
    }
}
