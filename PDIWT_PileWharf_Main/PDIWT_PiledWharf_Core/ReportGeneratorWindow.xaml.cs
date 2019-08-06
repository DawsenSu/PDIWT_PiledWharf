﻿using System;
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

namespace PDIWT_PiledWharf_Core
{
    /// <summary>
    /// Interaction logic for ReportGeneratorWindow.xaml
    /// </summary>
    public partial class ReportGeneratorWindow : Window
    {
        public ReportGeneratorWindow()
        {
            InitializeComponent();
            var _locator = new ViewModel.ViewModelLocator();
            DataContext = _locator.ReportGenerator;
            Icon = new BitmapImage(new Uri("pack://application:,,,/PDIWT.Resources;component/Images/Icons/CalculationNote.ico", UriKind.RelativeOrAbsolute));

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}