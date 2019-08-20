using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDIWT.Resources.Localization.MainModule;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;


namespace PDIWT_PiledWharf_Core.ViewModel
{
    public class AttachBCInstanceViewModel : ViewModelBase
    {
        public AttachBCInstanceViewModel()
        {
            SoilLayerNumber = string.Empty;
            SoilLayerName = string.Empty;
            Betasi = 1;
            Psisi = 1;
            Betap = 1;
            Psip = 1;
        }

        private string _soilLayerNumber;
        /// <summary>
        /// Property Description
        /// </summary>
        public string SoilLayerNumber
        {
            get { return _soilLayerNumber; }
            set { Set(ref _soilLayerNumber, value); }
        }

        private string _soilLayerName;
        /// <summary>
        /// Property Description
        /// </summary>
        public string SoilLayerName
        {
            get { return _soilLayerName; }
            set { Set(ref _soilLayerName, value); }
        }

        private double _betasi;
        /// <summary>
        /// Property Description
        /// </summary>
        public double Betasi
        {
            get { return _betasi; }
            set { Set(ref _betasi, value); }
        }

        private double _psisi;
        /// <summary>
        /// Property Description
        /// </summary>
        public double Psisi
        {
            get { return _psisi; }
            set { Set(ref _psisi, value); }
        }

        private double _qfi;
        /// <summary>
        /// Property Description
        /// </summary>
        public double Qfi
        {
            get { return _qfi; }
            set { Set(ref _qfi, value); }
        }

        private double _betap;
        /// <summary>
        /// Property Description
        /// </summary>
        public double Betap
        {
            get { return _betap; }
            set { Set(ref _betap, value); }
        }

        private double _psip;
        /// <summary>
        /// Property Description
        /// </summary>
        public double Psip
        {
            get { return _psip; }
            set { Set(ref _psip, value); }
        }

        private double _qr;
        /// <summary>
        /// Property Description
        /// </summary>
        public double Qr
        {
            get { return _qr; }
            set { Set(ref _qr, value); }
        }

    }
}
