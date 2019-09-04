using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;

using LiveCharts;
using LiveCharts.Wpf;

namespace PDIWT_PiledWharf_Core.ViewModel
{
    public class PileLengthCurveViewModel : ViewModelBase
    {
        public PileLengthCurveViewModel()
        {
            SeriesCollection = new SeriesCollection();
            //{
            //    new LineSeries
            //    {
            //        Title = "Series 1",
            //        Values = new ChartValues<double> { 4, 6, 5, 2, 4 }
            //    }
            //};
            //Labels = new List<string>();
        }

        private SeriesCollection _seriesCollection;
        /// <summary>
        /// Property Description
        /// </summary>
        public SeriesCollection SeriesCollection
        {
            get { return _seriesCollection; }
            set { Set(ref _seriesCollection, value); }
        }

        //private List<string> _labels;
        ///// <summary>
        ///// Property Description
        ///// </summary>
        //public List<string> Labels
        //{
        //    get { return _labels; }
        //    set { Set(ref _labels, value); }
        //}
    }
}
