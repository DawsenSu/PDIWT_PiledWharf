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
    using Model;
    using System.Collections.ObjectModel;
    using LiveCharts.Defaults;

    /// <summary>
    /// Interaction logic for PileFrameDivider.xaml
    /// </summary>
    public partial class PileFrameDividerWindow : Window
    {
        public PileFrameDividerWindow(BM.AddIn addIn)
        {
            InitializeComponent();
            ViewModelLocator _locator = new ViewModelLocator();
            DataContext = _locator.PileFrameDivider;
            m_wpfhelper = new BMWPF.WPFInteropHelper(this);
            m_wpfhelper.Attach(addIn, true, "PileFrameDivider");
            Icon = new BitmapImage(new Uri("pack://application:,,,/PDIWT.Resources;component/Images/Icons/PileFrameDivider.ico", UriKind.RelativeOrAbsolute));
            ComboBox_LastSectionHandleMethod.ItemsSource = PDIWT.Resources.PDIWT_Helper.GetEnumDescriptionDictionary<LastSectionHandleMethod>();

            Messenger.Default.Register<List<List<WharfSection>>>(this, DrawWharf);
            Closing += (s, e) => Messenger.Default.Unregister(this);
        }


        readonly BM.MessageCenter _mc = BM.MessageCenter.Instance;
        static PileFrameDividerWindow m_PileFrameDivider;
        BMWPF.WPFInteropHelper m_wpfhelper;

        public static void ShowWindow(BM.AddIn addIn)
        {
            if (m_PileFrameDivider != null)
            {
                m_PileFrameDivider.Focus();
                return;
            };

            m_PileFrameDivider = new PileFrameDividerWindow(addIn);
            m_PileFrameDivider.Show();
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
            m_PileFrameDivider = null;
        }

        private void DrawWharf(List<List<WharfSection>> sections)
        {
            Board.Children.Clear();

            Board.Width = Vm.WharfLength + 20;
            Board.Height = sections.Count * 20;
            double xposition = 0;
            int _yindex = 1;
            foreach (var _section in sections)
            {
                foreach (var _con in _section)
                {
                    DrawWharfSection(xposition, _yindex, _con);
                    xposition += _con.GetSectionLength();
                }

                //TextBlock _text = new TextBlock() { Text = $"#{_yindex}", Height = 5,TextAlignment= TextAlignment.Left, VerticalAlignment = VerticalAlignment.Top};
                //Canvas.SetTop(_text, (_yindex - 1) * 20 +5); Canvas.SetRight(_text, 20);
                //Board.Children.Add(_text);

                xposition = 0;
                _yindex++;
            }
        }

        private void DrawWharfSection(double xpostion, int yindex, WharfSection section)
        {
            Rectangle _leftRect = new Rectangle()
            {
                Height = 10,
                Width = section.LeftLength,
                Fill = new SolidColorBrush(Colors.Blue),
                //Stroke = new SolidColorBrush(Colors.Black),
                //StrokeThickness = 1
            };
            Rectangle _midRect = new Rectangle()
            {
                Height = 10,
                Width = section.Number * section.Span,
                Fill = new SolidColorBrush(Colors.Orange),
                //Stroke = new SolidColorBrush(Colors.Black),
                //StrokeThickness = 1
            };
            Rectangle _rightRect = new Rectangle()
            {
                Height = 10,
                Width = section.RightLength,
                Fill = new SolidColorBrush(Colors.Red),
                //Stroke = new SolidColorBrush(Colors.Black),
                //StrokeThickness = 1
            };
            Canvas.SetTop(_leftRect, 5 + (yindex - 1) * 20); Canvas.SetTop(_midRect, 5 + (yindex - 1) * 20); Canvas.SetTop(_rightRect, 5 + (yindex - 1) * 20);
            Canvas.SetLeft(_leftRect, xpostion); Canvas.SetLeft(_midRect, xpostion + _leftRect.Width); Canvas.SetLeft(_rightRect, xpostion + _leftRect.Width + _midRect.Width);


            Board.Children.Add(_leftRect);
            Board.Children.Add(_midRect);
            Board.Children.Add(_rightRect);
        }

        /// <summary>
        /// Gets the view's ViewModel.
        /// </summary>
        public PileFrameDividerViewModel Vm
        {
            get
            {
                return (PileFrameDividerViewModel)DataContext;
            }
        }

        private void ComboBox_LastSectionHandleMethod_Selected(object sender, RoutedEventArgs e)
        {

        }
    }
}
