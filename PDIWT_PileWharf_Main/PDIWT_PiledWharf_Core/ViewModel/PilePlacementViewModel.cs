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
    public class PilePlacementViewModel : ViewModelBase
    {
        public PilePlacementViewModel()
        {
            _pileTypes = PDIWT.Resources.PDIWT_Helper.GetEnumDescriptionDictionary<PDIWT_PiledWharf_Core_Cpp.PileTypeManaged>();
            _selectedPileType = PDIWT_PiledWharf_Core_Cpp.PileTypeManaged.SqaurePile;
            _pileWidth = 600;
            _pileInsideDiameter = 500;
            _concreteCoreLength = 1000;
            _pileTipSealTypes = new List<string> { Resources.PileTip_TotalSeal, Resources.PileTip_HalfSeal, Resources.PileTip_SingleBorad, Resources.PileTip_DoubleBoard, Resources.PileTip_QuadBoard };
            _selectedPileTipType = _pileTipSealTypes[0];
        }

        private Dictionary<PDIWT_PiledWharf_Core_Cpp.PileTypeManaged, string> _pileTypes;
        /// <summary>
        /// Property Description
        /// </summary>
        public Dictionary<PDIWT_PiledWharf_Core_Cpp.PileTypeManaged, string> PileTypes
        {
            get { return _pileTypes; }
            set { Set(ref _pileTypes, value); }
        }


        private PDIWT_PiledWharf_Core_Cpp.PileTypeManaged _selectedPileType;
        /// <summary>
        /// Property Description
        /// </summary>
        public PDIWT_PiledWharf_Core_Cpp.PileTypeManaged SelectedPileType
        {
            get { return _selectedPileType; }
            set { Set(ref _selectedPileType, value); }
        }

        private double _pileWidth;
        /// <summary>
        /// Property Description
        /// </summary>
        public double PileWidth
        {
            get { return _pileWidth; }
            set { Set(ref _pileWidth, value); }
        }

        private double _pileInsideDiameter;
        /// <summary>
        /// Property Description
        /// </summary>
        public double PileInsideDiameter
        {
            get { return _pileInsideDiameter; }
            set { Set(ref _pileInsideDiameter, value); }
        }

        private double _concreteCoreLength;
        /// <summary>
        /// Property Description
        /// </summary>
        public double ConcreteCoreLength
        {
            get { return _concreteCoreLength; }
            set { Set(ref _concreteCoreLength, value); }
        }

        private List<string> _pileTipSealTypes;
        /// <summary>
        /// Property Description
        /// </summary>
        public List<string> PileTipSealTypes
        {
            get { return _pileTipSealTypes; }
            set { Set(ref _pileTipSealTypes, value); }
        }


        private string _selectedPileTipType;
        /// <summary>
        /// Property Description
        /// </summary>
        public string SelectedPileTipType
        {
            get { return _selectedPileTipType; }
            set { Set(ref _selectedPileTipType, value); }
        }
    }
}
