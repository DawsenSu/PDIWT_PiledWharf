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


namespace PDIWT_Assemblies_Core
{
    using ViewModel;
    /// <summary>
    /// Interaction logic for StairCreationUserControl.xaml
    /// </summary>
    public partial class StairCreationUserControl : UserControl
    {
        public StairCreationUserControl()
        {
            InitializeComponent();
            ViewModelLocator _locator = new ViewModelLocator();
            DataContext = _locator.Stair;
        }
    }
}
