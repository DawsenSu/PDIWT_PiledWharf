using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;

using PDIWT_PiledWharf_Core.Model.Tools;
using PDIWT.Resources.Localization.MainModule;
using PDIWT.Formulas;

using MN = MathNet.Numerics;
using BM = Bentley.MstnPlatformNET;
using PDIWT_PiledWharf_Core_Cpp;
using PDIWT.Resources;

namespace PDIWT_PiledWharf_Core.ViewModel
{
    public class CurrentForceViewModel : ViewModelBase
    {
        public CurrentForceViewModel()
        {

            PileGeoTypes = PDIWT_Helper.GetEnumDescriptionDictionary<PileTypeManaged>();
            SelectedPileType = PileTypeManaged.SqaurePile;
#if DEBUG
            PileTopElevation = 4;
            HAT = 0;
            SoilElevation = -22.4;
            ProjectedWidth = 1.6;
            SquarePileAngle = 0;
            CurrentVelocity = 0.8;
            PileHorizontalCentraSpan = 6.4;
            PileVerticalCentraSpan = 6.4;
            WaterDensity = 1025;
#endif
            Messenger.Default.Register<NotificationMessage<PDIWT_CurrentForePileInfo>>(this,LoadParameterFromPileEntity);
            //Change The Loaded Controls Forground
            PropertyChanged += (s,e) => Messenger.Default.Send(new NotificationMessage<bool>(false, "ChangeControlForegroud"), "ControlForegroundChange");

        }
        private bool _isCalculated = false;
        private readonly BM.MessageCenter _mc = BM.MessageCenter.Instance;
        //***************************** Input parameters *************************//
        /// <summary>
        /// The <see cref="PileTopElevation" /> property's name.
        /// </summary>
        public const string PileTopElevationPropertyName = "PileTopElevation";

        private double _pileTopElevation = 0.0;

        /// <summary>
        /// Unit: m
        /// Sets and gets the PileTopElevation property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double PileTopElevation
        {
            get
            {
                return _pileTopElevation;
            }
            set
            {
                Set(PileTopElevationPropertyName, ref _pileTopElevation, value);
                _isCalculated = false;
            }
        }

        /// <summary>
        /// The <see cref="HAT" /> property's name.
        /// </summary>
        public const string HATPropertyName = "HAT";

        private double _hat = 0.0;

        /// <summary>
        /// Unit: m
        /// Sets and gets the HAT property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double HAT
        {
            get
            {
                return _hat;
            }
            set
            {
                Set(() => HAT, ref _hat, value);
                _isCalculated = false;
            }
        }

        /// <summary>
        /// The <see cref="SoilElevation" /> property's name.
        /// </summary>
        public const string SoilElevationPropertyName = "SoilElevation";

        private double _soilElevation = 0.0;

        /// <summary>
        /// Unit: m
        /// Sets and gets the SoilElevation property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double SoilElevation
        {
            get
            {
                return _soilElevation;
            }
            set
            {
                Set(() => SoilElevation, ref _soilElevation, value);
                _isCalculated = false;
            }
        }

        /// <summary>
        /// The <see cref="ProjectedWidth" /> property's name.
        /// </summary>
        public const string ProjectedWidthPropertyName = "ProjectedWidth";

        private double _projectedWidth = 0.0;

        /// <summary>
        /// Unit: m
        /// Sets and gets the ProjectedWidth property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double ProjectedWidth
        {
            get
            {
                return _projectedWidth;
            }
            set
            {
                Set(() => ProjectedWidth, ref _projectedWidth, value);
                _isCalculated = false;
            }
        }


        private Dictionary<PileTypeManaged,string> _pileGeoTypes;
        /// <summary>
        /// Pile Types dictionary
        /// </summary>
        public Dictionary<PileTypeManaged,string> PileGeoTypes
        {
            get { return _pileGeoTypes; }
            set { Set(ref _pileGeoTypes, value); }
        }


        private PileTypeManaged _selectedPileType;
        /// <summary>
        /// Selected Pile Type
        /// </summary>
        public PileTypeManaged SelectedPileType
        {
            get { return _selectedPileType; }
            set
            {
                Set(ref _selectedPileType, value);
                _isCalculated = false;
            }
        }

        /// <summary>
        /// The <see cref="SquarePileAngle" /> property's name.
        /// </summary>
        public const string SquarePileAnglePropertyName = "SquarePileAngle";

        private double _squarePileAngle = 1;

        /// <summary>
        /// Sets and gets the SquarePileAngle property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double SquarePileAngle
        {
            get
            {
                return _squarePileAngle;
            }
            set
            {
                Set(() => SquarePileAngle, ref _squarePileAngle, value);
                _isCalculated = false;
            }
        }

        /// <summary>
        /// The <see cref="CurrentVelocity" /> property's name.
        /// </summary>
        public const string CurrentVelocityPropertyName = "CurrentVelocity";

        private double _currentVelocity = 0.0;

        /// <summary>
        /// Unit: m/s
        /// Sets and gets the CurrentVelocity property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double CurrentVelocity
        {
            get
            {
                return _currentVelocity;
            }
            set
            {
                Set(() => CurrentVelocity, ref _currentVelocity, value);
                _isCalculated = false;
            }
        }

        /// <summary>
        /// The <see cref="PileHorizontalCentraSpan" /> property's name.
        /// </summary>
        public const string PileHorizontalCentraSpanPropertyName = "PileHorizontalCentraSpan";

        private double _pileHorizontalCentraSpan = 0.0;

        /// <summary>
        /// Unit: m
        /// Sets and gets the PileHorizontalCentraSpan property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double PileHorizontalCentraSpan
        {
            get
            {
                return _pileHorizontalCentraSpan;
            }
            set
            {
                Set(() => PileHorizontalCentraSpan, ref _pileHorizontalCentraSpan, value);
                _isCalculated = false;
            }
        }

        /// <summary>
        /// The <see cref="PileVerticalCentraSpan" /> property's name.
        /// </summary>
        public const string PileVerticalCentraSpanPropertyName = "PileVerticalCentraSpan";

        private double _pileVerticalCentraSpan = 0.0;

        /// <summary>
        /// Unit: m
        /// Sets and gets the PileVerticalCentraSpan property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double PileVerticalCentraSpan
        {
            get
            {
                return _pileVerticalCentraSpan;
            }
            set
            {
                Set(() => PileVerticalCentraSpan, ref _pileVerticalCentraSpan, value);
                _isCalculated = false;
            }
        }

        /// <summary>
        /// The <see cref="WaterDensity" /> property's name.
        /// </summary>
        public const string WaterDensityPropertyName = "WaterDensity";

        private double _waterDensity = 0;

        /// <summary>
        /// Unit: kg/m3
        /// Sets and gets the WaterDensity property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double WaterDensity
        {
            get
            {
                return _waterDensity;
            }
            set
            {
                Set(() => WaterDensity, ref _waterDensity, value);
                _isCalculated = false;
            }
        }

        //***************************** calculated parameters *************************//
        /// <summary>
        /// The <see cref="PileHeight" /> property's name.
        /// </summary>
        public const string PileHeightPropertyName = "PileHeight";

        private double _pileHeight = 0.0;

        /// <summary>
        /// Unit: m
        /// Sets and gets the PileHeight property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double PileHeight
        {
            get
            {
                return _pileHeight;
            }
            private set
            {
                Set(() => PileHeight, ref _pileHeight, value);
            }
        }
        /// <summary>
        /// The <see cref="ProjectedArea" /> property's name.
        /// </summary>
        public const string ProjectedAreaPropertyName = "ProjectedArea";

        private double _projectedArea = 0.0;

        /// <summary>
        /// Unit: m2
        /// Sets and gets the ProjectedArea property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double ProjectedArea
        {
            get
            {
                return _projectedArea;
            }
            private set
            {
                Set(() => ProjectedArea, ref _projectedArea, value);
            }
        }

        /// <summary>
        /// The <see cref="CurrentResistentCoeff" /> property's name.
        /// </summary>
        public const string CurrentResistentCoeffPropertyName = "CurrentResistentCoeff";

        private double _currentResistentCoeff = 0.0;

        /// <summary>
        /// Sets and gets the CurrentResistentCoeff property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double CurrentResistentCoeff
        {
            get
            {
                return _currentResistentCoeff;
            }
            private set
            {
                Set(() => CurrentResistentCoeff, ref _currentResistentCoeff, value);
            }
        }

        /// <summary>
        /// The <see cref="ShelteringCoeff" /> property's name.
        /// </summary>
        public const string ShelteringCoeffPropertyName = "ShelteringCoeff";

        private double _shelteringCoeff = 0.0;

        /// <summary>
        /// Sets and gets the ShelteringCoeff property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double ShelteringCoeff
        {
            get
            {
                return _shelteringCoeff;
            }
            private set
            {
                Set(() => ShelteringCoeff, ref _shelteringCoeff, value);
            }
        }
        /// <summary>
        /// The <see cref="SubmergedCoeff" /> property's name.
        /// </summary>
        public const string SubmergedCoeffPropertyName = "SubmergedCoeff";

        private double _submergedCoeff = 0.0;

        /// <summary>
        /// Sets and gets the SubmergedCoeff property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double SubmergedCoeff
        {
            get
            {
                return _submergedCoeff;
            }
            private set
            {
                Set(() => SubmergedCoeff, ref _submergedCoeff, value);
            }
        }

        /// <summary>
        /// The <see cref="WaterDepthCoeff" /> property's name.
        /// </summary>
        public const string WaterDepthCoeffPropertyName = "WaterDepthCoeff";

        private double _waterDepthCoeff = 0.0;

        /// <summary>
        /// Sets and gets the WaterDepthCoeff property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double WaterDepthCoeff
        {
            get
            {
                return _waterDepthCoeff;
            }
            private set
            {
                Set(() => WaterDepthCoeff, ref _waterDepthCoeff, value);
            }
        }
        /// <summary>
        /// The <see cref="HorizontalAffectCoeff" /> property's name.
        /// </summary>
        public const string HorizontalAffectCoeffPropertyName = "HorizontalAffectCoeff";

        private double _horizontalAffectCoeff = 0.0;

        /// <summary>
        /// Sets and gets the HorizontalAffectCoeff property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double HorizontalAffectCoeff
        {
            get
            {
                return _horizontalAffectCoeff;
            }
            private set
            {
                Set(() => HorizontalAffectCoeff, ref _horizontalAffectCoeff, value);
            }
        }
        /// <summary>
        /// The <see cref="IncliningAffectCoeff" /> property's name.
        /// </summary>
        public const string IncliningAffectCoeffPropertyName = "IncliningAffectCoeff";

        private double _incliningAffectCoeff = 0.0;

        /// <summary>
        /// Sets and gets the IncliningAffectCoeff property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double IncliningAffectCoeff
        {
            get
            {
                return _incliningAffectCoeff;
            }
            private set
            {
                Set(() => IncliningAffectCoeff, ref _incliningAffectCoeff, value);
            }
        }

        //************************* Result parameters ******************************//
        /// <summary>
        /// The <see cref="CurrentForceForFrontPile" /> property's name.
        /// </summary>
        public const string CurrentForceForFrontPilePropertyName = "CurrentForceForFrontPile";

        private double _currentForceForFrontPile = 0.0;

        /// <summary>
        /// Unit: kN
        /// Sets and gets the CurrentForceForFrontPile property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double CurrentForceForFrontPile
        {
            get
            {
                return _currentForceForFrontPile;
            }
            private set
            {
                Set(() => CurrentForceForFrontPile, ref _currentForceForFrontPile, value);
            }
        }
        /// <summary>
        /// The <see cref="CurrentForceForRearPile" /> property's name.
        /// </summary>
        public const string CurrentForceForRearPilePropertyName = "CurrentForceForRearPile";

        private double _currentForceForRearPile = 0.0;

        /// <summary>
        /// Unit: kN
        /// Sets and gets the CurrentForceForRearPile property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double CurrentForceForRearPile
        {
            get
            {
                return _currentForceForRearPile;
            }
            private set
            {
                Set(() => CurrentForceForRearPile, ref _currentForceForRearPile, value);
            }
        }

        /// <summary>
        /// The <see cref="ActionPointForFrontPile" /> property's name.
        /// </summary>
        public const string ActionPointForFrontPilePropertyName = "ActionPointForFrontPile";

        private double _actionPointForFrontPile = 0.0;

        /// <summary>
        /// Unit: m
        /// Sets and gets the ActionPointForFrontPile property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double ActionPointForFrontPile
        {
            get
            {
                return _actionPointForFrontPile;
            }
            private set
            {
                Set(() => ActionPointForFrontPile, ref _actionPointForFrontPile, value);
            }
        }
        /// <summary>
        /// The <see cref="ActionPointForRearPile" /> property's name.
        /// </summary>
        public const string ActionPointForRearPilePropertyName = "ActionPointForRearPile";

        private double _actionPointForRearPile = 0.0;

        /// <summary>
        /// Unit: m
        /// Sets and gets the ActionPointForRearPile property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double ActionPointForRearPile
        {
            get
            {
                return _actionPointForRearPile;
            }
            private set
            {
                Set(() => ActionPointForRearPile, ref _actionPointForRearPile, value);
            }
        }

        private RelayCommand _loadParameter;

        /// <summary>
        /// Gets the LoadParameter.
        /// </summary>
        public RelayCommand LoadParameter
        {
            get
            {
                return _loadParameter
                    ?? (_loadParameter = new RelayCommand(ExecuteLoadParameter));
            }
        }

        private void ExecuteLoadParameter()
        {
            try
            {
                LoadPileParametersTool<PDIWT_CurrentForePileInfo>.InstallNewInstance(LoadParametersToolEnablerProvider.CurrentForceInfoEnabler);
                Messenger.Default.Send(Visibility.Hidden, "ShowMainWindow");
            }
            catch (Exception e)
            {
                _mc.ShowErrorMessage("Can't load parameters from entity", e.ToString(), false);
            }

        }

        private void LoadParameterFromPileEntity(NotificationMessage<PDIWT_CurrentForePileInfo> notification)
        {
            if (notification.Notification == Resources.Error)
                BM.MessageCenter.Instance.ShowErrorMessage("Can't Load Parameter From selected element","Error",BM.MessageAlert.None);
            else
            {
                var _pileInfo = notification.Content;
                PileTopElevation = _pileInfo.TopElevation;
                SoilElevation = _pileInfo.SoilElevation;
                HAT = _pileInfo.HAT;
                ProjectedWidth = _pileInfo.ProjectedWidth;
                SelectedPileType = _pileInfo.PileType;
                WaterDensity = _pileInfo.WaterDensity;
                Messenger.Default.Send(new NotificationMessage<bool>(true, "ChangeControlForegroud"), "ControlForegroundChange");
                _mc.ShowInfoMessage("Load parameters successfully", "", false);
            }
        }

        private RelayCommand _calculate;

        /// <summary>
        /// Gets the Calculate.
        /// </summary>
        public RelayCommand Calculate
        {
            get
            {
                return _calculate
                    ?? (_calculate = new RelayCommand(ExecuteCalculate, ()=> !_isCalculated));
            }
        }

        private void ExecuteCalculate()
        {
            try
            {
                PileHeight = HAT - SoilElevation;
                ProjectedArea = PileHeight * ProjectedWidth;
                if (SelectedPileType == PileTypeManaged.SqaurePile)
                    CurrentResistentCoeff = 1.5;
                else
                    CurrentResistentCoeff = 0.73;
                ShelteringCoeff = CurrentForce.CalculateShelteringCoeff((PileVerticalCentraSpan - ProjectedWidth) / ProjectedWidth);
                SubmergedCoeff = 1;
                WaterDepthCoeff = CurrentForce.CalculateWaterDepthCoeff(PileHeight - ProjectedWidth);
                HorizontalAffectCoeff = CurrentForce.CalculatedHorizontalAffectCoeff((PileHorizontalCentraSpan - ProjectedWidth) / ProjectedWidth, SelectedPileType);
                IncliningAffectCoeff = CurrentForce.CalculateIncliningAffectCoeff(SquarePileAngle, SelectedPileType);

                //MessageBox.Show($"Calculate {SelectedShape.Shape},{SelectedShape.Value}");
                CurrentForceForFrontPile = CurrentResistentCoeff * WaterDensity / 1000 / 2 * Math.Pow(CurrentVelocity, 2) * ProjectedArea * 1 * SubmergedCoeff * WaterDepthCoeff * HorizontalAffectCoeff * IncliningAffectCoeff;
                CurrentForceForRearPile = CurrentResistentCoeff * WaterDensity / 1000 / 2 * Math.Pow(CurrentVelocity, 2) * ProjectedArea * ShelteringCoeff * SubmergedCoeff * WaterDepthCoeff * HorizontalAffectCoeff * IncliningAffectCoeff;
                ActionPointForFrontPile = PileTopElevation - HAT + PileHeight / 3;
                ActionPointForRearPile = ActionPointForFrontPile;
                _isCalculated = true;
                _mc.ShowInfoMessage("Calculation Successfully", "", false);
            }
            catch (Exception e)
            {
                _mc.ShowErrorMessage("Calculation failed", e.ToString(), false);
            }
        }
        

        private RelayCommand _generateNote;

        /// <summary>
        /// Gets the GenerateNote.
        /// </summary>
        public RelayCommand GenerateNote
        {
            get
            {
                return _generateNote
                    ?? (_generateNote = new RelayCommand(
                    () =>
                    {
                        ReportGeneratorWindow _reportWindow = new ReportGeneratorWindow();
                        Messenger.Default.Send(new NotificationMessage(this, "CurrentForceViewModelInvoke"), "ViewModelForReport");
                        _reportWindow.ShowDialog();
                    },
                    () => _isCalculated));
            }
        }
    }

    public class ShapeInfo
    {
        public string Shape { get; set; }
        public int Value { get; set; }
    }

}
