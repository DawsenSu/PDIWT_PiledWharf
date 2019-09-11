using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;

using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;

namespace PDIWT_PiledWharf_Core.ViewModel
{
    public class PileLengthCurveViewModel : ViewModelBase
    {
        public PileLengthCurveViewModel()
        {
            //{
            //    new LineSeries
            //    {
            //        Title = "Series 1",
            //        Values = new ChartValues<double> { 4, 6, 5, 2, 4 }
            //    }
            //};
            //Labels = new List<string>();
            //var _mapper = LiveCharts.Configurations.Mappers.Xy<Tuple<double, double>>().X(item => item.Item1).Y(item => item.Item2);
            //LineSeries _lineSeries = new LineSeries()
            //{
            //    Title = "Length",
            //    Values = _dataResources,
            //    LineSmoothness = 0
            //};
            InverseFormatter = value => (-value).ToString();
            LabelFromater = value => string.Format("BearCapacity:{0:F2}\nLength:{1:F2}", value.X, value.Y);
        }

        private ChartValues<ObservablePoint> _dataResources;
        /// <summary>
        /// Property Description
        /// </summary>
        public ChartValues<ObservablePoint> DataResources
        {
            get { return _dataResources; }
            set { Set(ref _dataResources, value); }
        }
        public Func<double,string> InverseFormatter { get; set; }
        //private List<string> _labels;
        ///// <summary>
        ///// Property Description
        ///// </summary>
        //public List<string> Labels
        //{
        //    get { return _labels; }
        //    set { Set(ref _labels, value); }
        //}
        public Func<ChartPoint,string> LabelFromater { get; set; }
    }
}
