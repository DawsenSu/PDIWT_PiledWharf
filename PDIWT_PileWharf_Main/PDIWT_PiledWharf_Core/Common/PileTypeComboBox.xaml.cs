using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace PDIWT.Resources.CustomUserControl
{
    /// <summary>
    /// Interaction logic for PileTypeComboBox.xaml
    /// </summary>
    public partial class PileTypeComboBox : UserControl
    {
        public PileTypeComboBox()
        {
            InitializeComponent();
            PileType_ComboBox.ItemsSource = PDIWT_Helper.GetEnumDescriptionDictionary<PDIWT_PiledWharf_Core_Cpp.PileTypeManaged>();
            //PileType_ComboBox.SelectedValuePath = "Key";
            ////PileType_ComboBox.DisplayMemberPath = "Value";
            //PileType_ComboBox.SelectedValue = new Binding("SelectedPileType") { RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(PileTypeComboBox), 1) };
        }



        public PDIWT_PiledWharf_Core_Cpp.PileTypeManaged SelectedPileType
        {
            get { return (PDIWT_PiledWharf_Core_Cpp.PileTypeManaged)GetValue(SelectedPileTypeProperty); }
            set { SetValue(SelectedPileTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedPileType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedPileTypeProperty =
            DependencyProperty.Register("SelectedPileType", typeof(PDIWT_PiledWharf_Core_Cpp.PileTypeManaged), typeof(PileTypeComboBox),
                new FrameworkPropertyMetadata(PDIWT_PiledWharf_Core_Cpp.PileTypeManaged.SqaurePile, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
    }

    public class PileTypeToShape : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double _diameter = 18;
            double _innerdiamter = _diameter - 2;
            PDIWT_PiledWharf_Core_Cpp.PileTypeManaged _pileType = (PDIWT_PiledWharf_Core_Cpp.PileTypeManaged)value;
            switch (_pileType)
            {
                case PDIWT_PiledWharf_Core_Cpp.PileTypeManaged.SqaurePile:
                    return new Rectangle() { Width = _diameter, Height = _diameter, Fill = new SolidColorBrush(Colors.Gray), Stroke = new SolidColorBrush(Colors.Black), StrokeThickness = 2 };
                case PDIWT_PiledWharf_Core_Cpp.PileTypeManaged.TubePile:
                    return new Ellipse() { Width = _diameter, Height = _diameter, Fill = new SolidColorBrush(Colors.Gray), Stroke = new SolidColorBrush(Colors.Black), StrokeThickness = 2 };
                case PDIWT_PiledWharf_Core_Cpp.PileTypeManaged.PHCTubePile:
                    EllipseGeometry _outerEllipse = new EllipseGeometry(new Point((_diameter - 2) / 2, (_diameter - 2) / 2), _diameter / 2, _diameter / 2);
                    EllipseGeometry _innerEllipse = new EllipseGeometry(new Point((_diameter - 2) / 2, (_diameter - 2) / 2), _innerdiamter / 2, _innerdiamter / 2);
                    CombinedGeometry _combinedGeometry = new CombinedGeometry(GeometryCombineMode.Exclude, _outerEllipse, _innerEllipse);
                    return new Path() { Width = _diameter, Height = _diameter, Fill = new SolidColorBrush(Colors.Gray), Stroke = new SolidColorBrush(Colors.Black), Data = _combinedGeometry };
                case PDIWT_PiledWharf_Core_Cpp.PileTypeManaged.SteelTubePile:
                    EllipseGeometry _steelouterEllipse = new EllipseGeometry(new Point((_diameter - 2) / 2, (_diameter - 2) / 2), _diameter / 2, _diameter / 2);
                    EllipseGeometry _steelinnerEllipse = new EllipseGeometry(new Point((_diameter - 2) / 2, (_diameter - 2) / 2), _innerdiamter / 2, _innerdiamter / 2);
                    CombinedGeometry _steelcombinedGeometry = new CombinedGeometry(GeometryCombineMode.Exclude, _steelouterEllipse, _steelinnerEllipse);
                    return new Path() { Width = _diameter, Height = _diameter, Fill = new SolidColorBrush(Colors.Gray), Stroke = new SolidColorBrush(Colors.Coral), Data = _steelcombinedGeometry };
                default:
                    return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
