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
using System.Windows.Navigation;
using System.Windows.Shapes;
using PDIWT_PiledWharf_Core.Model;

using GalaSoft.MvvmLight;
namespace PDIWT.Resources.CustomUserControl
{
    /// <summary>
    /// Interaction logic for BearingCapacityPileTypeComboBox.xaml
    /// </summary>
    public partial class BearingCapacityPileTypeComboBox : UserControl
    {
        public BearingCapacityPileTypeComboBox()
        {
            InitializeComponent();
            BearingCapacityPileType_ComboBox.ItemsSource = PDIWT_Helper.GetEnumDescriptionDictionary<BearingCapacityPileTypes>();
        }

        public BearingCapacityPileTypes BearingCapacityPileType
        {
            get { return (BearingCapacityPileTypes)GetValue(BearingCapacityPileTypeProperty); }
            set { SetValue(BearingCapacityPileTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BearingCapacityPileType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BearingCapacityPileTypeProperty =
            DependencyProperty.Register("BearingCapacityPileType", typeof(BearingCapacityPileTypes), typeof(BearingCapacityPileTypeComboBox),
                new FrameworkPropertyMetadata(BearingCapacityPileTypes.DrivenPileWithSealedEnd, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        //private static void OnPileTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    var _pileDict = PDIWT_Helper.GetEnumDescriptionDictionary<BearingCapacityPileTypes>();
        //    int _index = (int)e.NewValue;
        //    var _keypair = _pileDict.ElementAt(_index);
        //    BearingCapacityPileTypeComboBox _combox = (BearingCapacityPileTypeComboBox)d;
        //    _combox.BearingCapacityPileType_ComboBox.SelectedItem = _keypair;
        //}
    }
}
