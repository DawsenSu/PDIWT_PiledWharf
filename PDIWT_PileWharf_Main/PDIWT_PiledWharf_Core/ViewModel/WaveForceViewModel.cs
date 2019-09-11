using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System.ComponentModel;

using PDIWT.Formulas;
using PDIWT.Resources.Localization.MainModule;
using System.Collections.ObjectModel;

using BD = Bentley.DgnPlatformNET;
using BM = Bentley.MstnPlatformNET;
using System.Windows;
using PDIWT_PiledWharf_Core_Cpp;

namespace PDIWT_PiledWharf_Core.ViewModel
{
    using Model.Tools;

    public class WaveForceViewModel : ViewModelBase
    {
        public WaveForceViewModel()
        {

            // Register Broadcast message 
            Messenger.Default.Register<NotificationMessage<PDIWT_WaveForcePileInfo>>(this, LoadParametersFromPileEntity);
            //Init Data
            //ShapeCategory = new List<ShapeInfo>
            //    {
            //        new ShapeInfo() { Shape = Resources.SquarePile, Value =2 },
            //        new ShapeInfo() { Shape = Resources.TubePile, Value =1 },
            //        new ShapeInfo() { Shape = Resources.PHCTubePile, Value =1 },
            //        new ShapeInfo() { Shape = Resources.SteelTubePile, Value =1 }
            //    };
            //SelectedShape = ShapeCategory[0];
            //PileGeoTypes = PDIWT.Resources.PDIWT_Helper.GetEnumDescriptionDictionary<PileTypeManaged>();
            SelectedPileType = PileTypeManaged.SqaurePile;

            PileDiameter = 11.5;
            PileCentraSpan = 26;
            TopElevation = 10;
            BottomElevation = -20.5;
            HAT = 5.52;
            MHW = 4.32;
            MLW = 0.47;
            LAT = -0.83;
            WaterWeight = 10.25;
            GravitationalAcceleration = 9.8;

            DesignInputParameters = new ObservableCollection<PDIWT_WaveForce_DesignInputParamters>
            {
                new PDIWT_WaveForce_DesignInputParamters(){DesignWaterLevel = DesignWaterLevelCondition.HAT, H1=4.2,H13=3.1,T=11.2},
                new PDIWT_WaveForce_DesignInputParamters(){DesignWaterLevel = DesignWaterLevelCondition.MHW, H1=4,H13=2.9,T=6.6},
                new PDIWT_WaveForce_DesignInputParamters(){DesignWaterLevel = DesignWaterLevelCondition.MLW, H1=3.7,H13=2.8,T=6.3},
                new PDIWT_WaveForce_DesignInputParamters(){DesignWaterLevel = DesignWaterLevelCondition.LAT, H1=2.9,H13=2.8,T=11.2}
            };
            CalculatedParameters = new ObservableCollection<PDIWT_WaveForce_CalculatedParameters>
            {
                new PDIWT_WaveForce_CalculatedParameters(){DesignWaterLevel = DesignWaterLevelCondition.HAT},
                new PDIWT_WaveForce_CalculatedParameters(){DesignWaterLevel = DesignWaterLevelCondition.MHW},
                new PDIWT_WaveForce_CalculatedParameters(){DesignWaterLevel = DesignWaterLevelCondition.MLW},
                new PDIWT_WaveForce_CalculatedParameters(){DesignWaterLevel = DesignWaterLevelCondition.LAT}
            };
            Results = new ObservableCollection<PDIWT_WaveForce_Results>()
            {
                new PDIWT_WaveForce_Results(){DesignWaterLevel = DesignWaterLevelCondition.HAT},
                new PDIWT_WaveForce_Results(){DesignWaterLevel = DesignWaterLevelCondition.MHW},
                new PDIWT_WaveForce_Results(){DesignWaterLevel = DesignWaterLevelCondition.MLW},
                new PDIWT_WaveForce_Results(){DesignWaterLevel = DesignWaterLevelCondition.LAT},
            };

            // change the controls foreground color
            PropertyChanged += (s, e) =>
            {
                Messenger.Default.Send(new NotificationMessage<bool>(false, "Changed!"), "WaveForceForegroundChange");
                _isCalculated = false;
            };
        }

        private bool _isCalculated = false;
        private readonly BM.MessageCenter _mc = BM.MessageCenter.Instance;

        //********************* Input parameters *****************//
        //private Dictionary<PileTypeManaged, string> _pileGeoTypes;
        ///// <summary>
        ///// Geo Type
        ///// </summary>
        //public Dictionary<PileTypeManaged, string> PileGeoTypes
        //{
        //    get { return _pileGeoTypes; }
        //    set { Set(ref _pileGeoTypes, value); }
        //}

        private PileTypeManaged _selectedPileType;
        /// <summary>
        /// selected pile Type
        /// </summary>
        public PileTypeManaged SelectedPileType
        {
            get { return _selectedPileType; }
            set
            {
                Set(ref _selectedPileType, value);
            }
        }

        //private List<ShapeInfo> _shapeCategory;
        ///// <summary>
        ///// Property Description
        ///// </summary>
        //public List<ShapeInfo> ShapeCategory
        //{
        //    get { return _shapeCategory; }
        //    set { Set(ref _shapeCategory, value); }
        //}

        //private ShapeInfo _selectedShape;
        ///// <summary>
        ///// Property Description
        ///// </summary>
        //public ShapeInfo SelectedShape
        //{
        //    get { return _selectedShape; }
        //    set { Set(ref _selectedShape, value,true); }
        //}



        private double _pileDiameter;
        /// <summary>
        /// unit: m
        /// Property Description
        /// </summary>
        public double PileDiameter
        {
            get { return _pileDiameter; }
            set
            {
                if (value < 0)
                    Set(ref _pileDiameter, 0);
                else
                    Set(ref _pileDiameter, value);
            }
        }

        private double _pileCentraSpan;
        /// <summary>
        /// unit: m
        /// </summary>
        public double PileCentraSpan
        {
            get { return _pileCentraSpan; }
            set
            {
                if (value < 0)
                    Set(ref _pileCentraSpan, 0);
                else
                    Set(ref _pileCentraSpan, value);
            }
        }

        private double _topElevation;
        /// <summary>
        /// unit:m 
        /// </summary>
        public double TopElevation
        {
            get { return _topElevation; }
            set
            {
                if (value < 0)
                    Set(ref _topElevation, 0);
                else
                    Set(ref _topElevation, value);
            }
        }

        private double _bottomElevation;
        /// <summary>
        /// unit: m
        /// </summary>
        public double BottomElevation
        {
            get { return _bottomElevation; }
            set
            {
                if (value < 0)
                    Set(ref _bottomElevation, value);
                else
                    Set(ref _bottomElevation, value);
            }
        }


        private double _hat;
        /// <summary>
        /// unit: m
        /// </summary>
        public double HAT
        {
            get { return _hat; }
            set
            {
                Set(ref _hat, value);
            }
        }

        private double _mhw;
        /// <summary>
        /// unit: m
        /// </summary>
        public double MHW
        {
            get { return _mhw; }
            set
            {
                Set(ref _mhw, value);
            }
        }

        private double _mlw;
        /// <summary>
        /// unit: m
        /// </summary>
        public double MLW
        {
            get { return _mlw; }
            set
            {
                Set(ref _mlw, value);
            }
        }

        private double _lat;
        /// <summary>
        /// unit: m
        /// </summary>
        public double LAT
        {
            get { return _lat; }
            set
            {
                Set(ref _lat, value);
            }
        }

        private double _velocityForceCoeffCD;
        /// <summary>
        /// Property Description
        /// </summary>
        public double VelocityForceCoeffCD
        {
            get { return _velocityForceCoeffCD; }
            private set { Set(ref _velocityForceCoeffCD, value); }
        }

        private double _velocityForceCoeffCM;
        /// <summary>
        /// Property Description
        /// </summary>
        public double VelocityForceCoeffCM
        {
            get { return _velocityForceCoeffCM; }
            private set { Set(ref _velocityForceCoeffCM, value); }
        }

        private double _waterWeight;
        /// <summary>
        /// unit : kN/m3
        /// </summary>
        public double WaterWeight
        {
            get { return _waterWeight; }
            set
            {
                if (value < 0)
                    Set(ref _waterWeight, 0);
                else
                    Set(ref _waterWeight, value);
            }
        }

        private double _gravitationalAcceleration;
        /// <summary>
        /// Property Description
        /// </summary>
        public double GravitationalAcceleration
        {
            get { return _gravitationalAcceleration; }
            set
            {
                if (value < 0)
                    Set(ref _gravitationalAcceleration, 0);
                else
                    Set(ref _gravitationalAcceleration, value);
            }
        }

        private ObservableCollection<PDIWT_WaveForce_DesignInputParamters> _designInputParameters;
        /// <summary>
        /// Property Description
        /// </summary>
        public ObservableCollection<PDIWT_WaveForce_DesignInputParamters> DesignInputParameters
        {
            get { return _designInputParameters; }
            set
            {
                Set(ref _designInputParameters, value);
                foreach (var _input in _designInputParameters)
                {
                    _input.PropertyChanged += (s, e) =>
                    {
                        Messenger.Default.Send(new NotificationMessage<bool>(false, "Changed!"), "WaveForceForegroundChange");
                        _isCalculated = false;
                    };
                }
            }
        }

        private ObservableCollection<PDIWT_WaveForce_CalculatedParameters> _calculatedParamters;
        /// <summary>
        /// Property Description
        /// </summary>
        public ObservableCollection<PDIWT_WaveForce_CalculatedParameters> CalculatedParameters
        {
            get { return _calculatedParamters; }
            private set
            {
                Set(ref _calculatedParamters, value);
                foreach (var _calculate in _calculatedParamters)
                {
                    _calculate.PropertyChanged += (s, e) =>
                    {
                        Messenger.Default.Send(new NotificationMessage<bool>(false, "Changed!"), "WaveForceForegroundChange");
                        _isCalculated = false;
                    };
                }
            }
        }

        private ObservableCollection<PDIWT_WaveForce_Results> _results;
        /// <summary>
        /// Property Description
        /// </summary>
        public ObservableCollection<PDIWT_WaveForce_Results> Results
        {
            get { return _results; }
            private set
            {
                Set(ref _results, value);
                foreach (var _result in _results)
                {
                    _result.PropertyChanged += (s, e) =>
                    {
                        Messenger.Default.Send(new NotificationMessage<bool>(false, "Changed!"), "WaveForceForegroundChange");
                        _isCalculated = false;
                    };
                }
            }
        }

        private RelayCommand _loadParameters;

        /// <summary>
        /// Gets the LoadParameters.
        /// </summary>
        public RelayCommand LoadParameters
        {
            get
            {
                return _loadParameters
                    ?? (_loadParameters = new RelayCommand(ExecuteLoadParameters));
            }
        }

        private void ExecuteLoadParameters()
        {
            try
            {
                LoadPileParametersTool<PDIWT_WaveForcePileInfo>.InstallNewInstance(LoadParametersToolEnablerProvider.WaveForceInfoEnabler);
                Messenger.Default.Send(Visibility.Hidden, "ShowMainWindow");
            }
            catch (Exception e)
            {
                _mc.ShowErrorMessage("Load parameters failed!", e.ToString(), false);
            }
        }

        private void LoadParametersFromPileEntity(NotificationMessage<PDIWT_WaveForcePileInfo> notification)
        {
            if (notification.Notification == Resources.Error)
                BM.MessageCenter.Instance.ShowErrorMessage("Can't Load Parameter From selected element", "Error", BM.MessageAlert.None);
            else
            {
                var _pileInfo = notification.Content;

                PileDiameter = _pileInfo.PileDiameter;
                HAT = _pileInfo.HAT;
                MHW = _pileInfo.MHW;
                MLW = _pileInfo.MLW;
                LAT = _pileInfo.LAT;

                SelectedPileType = _pileInfo.PileType;
                WaterWeight = _pileInfo.WaterWeight;

                for (int i = 0; i < 4; i++)
                {
                    DesignInputParameters[i].H1 = _pileInfo.WaveHeight[i];
                    DesignInputParameters[i].T = _pileInfo.WavePeriod[i];
                }
                Messenger.Default.Send(new NotificationMessage<bool>(true, "Changed!"), "WaveForceForegroundChange");
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
                if (SelectedPileType == PileTypeManaged.SqaurePile)
                {
                    VelocityForceCoeffCD = 2.0;
                    VelocityForceCoeffCM = 2.2;
                }
                else
                {
                    VelocityForceCoeffCD = 1.2;
                    VelocityForceCoeffCM = 2.0;
                }

                foreach (var _input in DesignInputParameters)
                {
                    switch (_input.DesignWaterLevel)
                    {
                        case DesignWaterLevelCondition.HAT:
                            _input.WaterDepth = HAT - BottomElevation;
                            break;
                        case DesignWaterLevelCondition.MHW:
                            _input.WaterDepth = MHW - BottomElevation;
                            break;
                        case DesignWaterLevelCondition.MLW:
                            _input.WaterDepth = MLW - BottomElevation;
                            break;
                        case DesignWaterLevelCondition.LAT:
                            _input.WaterDepth = LAT - BottomElevation;
                            break;
                    }
                    _input.WaveLength = WaveForce.CalculateWaveLength(_input.T, _input.WaterDepth, GravitationalAcceleration);
                }

                foreach (var _parameter in CalculatedParameters)
                {
                    var _inputparameters = DesignInputParameters.Where(e => e.DesignWaterLevel == _parameter.DesignWaterLevel).FirstOrDefault();
                    _parameter.D_L = PileDiameter / _inputparameters.WaveLength;
                    _parameter.H_D = _inputparameters.H1 / _inputparameters.WaterDepth;
                    _parameter.DD_L = _inputparameters.WaterDepth / _inputparameters.WaveLength;
                    double _relativeperiod = WaveForce.CalculateReltiavePeriod(_inputparameters.T, _inputparameters.WaterDepth);
                    _parameter.YitaMax = WaveForce.CalculateYitaMax(_inputparameters.H1, _inputparameters.WaterDepth, _inputparameters.WaveLength, PileDiameter, _relativeperiod);
                    _parameter.Alpha = WaveForce.CalculateAlpha(_inputparameters.H1, _inputparameters.WaterDepth, _inputparameters.WaveLength);
                    _parameter.Beta = WaveForce.CalculateBeta(_inputparameters.H1, _inputparameters.WaterDepth, _inputparameters.WaveLength);
                    _parameter.GammaP = WaveForce.CalculateGammaP(_inputparameters.WaterDepth, _inputparameters.WaveLength);
                    _parameter.GammaM = WaveForce.CalculateGammaM(_inputparameters.WaterDepth, _inputparameters.WaveLength);
                    _parameter.K = WaveForce.CalculateK(PileCentraSpan, PileDiameter);
                    _parameter.F0 = WaveForce.Calculatef0(PileDiameter, _inputparameters.WaveLength);
                    _parameter.F1 = WaveForce.Calculatef1(PileDiameter, _inputparameters.WaveLength);
                    _parameter.F2 = WaveForce.Calculatef2(PileDiameter, _inputparameters.WaveLength);
                    _parameter.F3 = WaveForce.Calculatef3(PileDiameter, _inputparameters.WaveLength);
                    _parameter.OmgaT = WaveForce.CalculateOmgaT(_inputparameters.H1, _inputparameters.WaterDepth, _inputparameters.WaveLength);
                    _parameter.K1 = WaveForce.CalculateK1(0, _inputparameters.WaterDepth + _parameter.YitaMax, _inputparameters.WaveLength, _inputparameters.WaterDepth);
                    _parameter.K2 = WaveForce.CalculateK2(0, _inputparameters.WaterDepth + _parameter.YitaMax - _inputparameters.H1 / 2, _inputparameters.WaveLength, _inputparameters.WaterDepth);
                    _parameter.K3 = WaveForce.CalculateK3(0, _inputparameters.WaterDepth + _parameter.YitaMax, _inputparameters.WaveLength, _inputparameters.WaterDepth);
                    _parameter.K4 = WaveForce.CalculateK4(0, _inputparameters.WaterDepth + _parameter.YitaMax - _inputparameters.H1 / 2, _inputparameters.WaveLength, _inputparameters.WaterDepth);
                }
                //Calculate the pile cross section area
                double _crossectionarea = 0;
                if (SelectedPileType == PileTypeManaged.SqaurePile)
                    _crossectionarea = PileDiameter * PileDiameter;
                else
                    _crossectionarea = Math.PI * Math.Pow(PileDiameter / 2, 2);

                foreach (var _result in Results)
                {
                    var _inputparams = DesignInputParameters.Where(e => e.DesignWaterLevel == _result.DesignWaterLevel).FirstOrDefault();
                    var _calculatedparams = CalculatedParameters.Where(e => e.DesignWaterLevel == _result.DesignWaterLevel).FirstOrDefault();
                    // Calculate Final PDMax, PIMax, MDMax, MIMax
                    double _cd = WaveForce.CalculateCD(SelectedPileType);
                    double _cm = WaveForce.CalculateCM(PileDiameter, _inputparams.WaveLength, SelectedPileType);
                    _result.PDMax = WaveForce.CalculatePDMax(_cd, WaterWeight, PileDiameter, _inputparams.H1, _calculatedparams.K1);
                    _result.PIMax = WaveForce.CalculatePIMax(_cm, WaterWeight, _crossectionarea, _inputparams.H1, _calculatedparams.K2);
                    _result.MDMax = WaveForce.CalculateMDMax(_cd, WaterWeight, PileDiameter, _inputparams.H1, _inputparams.WaveLength, _calculatedparams.K3);
                    _result.MIMax = WaveForce.CalculateMIMax(_cm, WaterWeight, _crossectionarea, _inputparams.H1, _inputparams.WaveLength, _calculatedparams.K4);

                    double _PDMax_Final, _PIMax_Final, _MDMax_Final, _MIMax_Final;

                    WaveForce.CalculateFinalCompOfWaveForce(
                        PileDiameter, _inputparams.WaveLength, _inputparams.H1, _inputparams.WaterDepth, _calculatedparams.Alpha, _calculatedparams.Beta, _calculatedparams.GammaP,
                        _calculatedparams.GammaM, _result.PDMax, _result.MDMax, _result.PIMax, _result.MIMax, out _PDMax_Final, out _MDMax_Final, out _PIMax_Final, out _MIMax_Final);

                    _result.PDMax_Final = _PDMax_Final;
                    _result.PIMax_Final = _PIMax_Final;
                    _result.MDMax_Final = _MDMax_Final;
                    _result.MIMax_Final = _MIMax_Final;

                    double _PMax, _MMax, _ogmaT;
                    WaveForce.CalculateMaxWaveAndMoment(PileDiameter, _inputparams.WaveLength, _inputparams.H1, _inputparams.WaterDepth, PileCentraSpan,
                        _PDMax_Final, _MDMax_Final, _PIMax_Final, _MIMax_Final, out _PMax, out _MMax, out _ogmaT);

                    _result.PMax = _PMax;
                    _result.MMax = _MMax;
                    _result.OmgaT = _ogmaT;
                    _result.Pu = WaveForce.CalculatePu(WaterWeight, _inputparams.H1, PileDiameter, 0, _inputparams.WaveLength, _inputparams.WaterDepth, _calculatedparams.F2, _calculatedparams.F0, _calculatedparams.OmgaT);
                    _result.Mu = WaveForce.CalculateMu(WaterWeight, _inputparams.H1, PileDiameter, 0, _inputparams.WaveLength, _inputparams.WaterDepth, _calculatedparams.F3, _calculatedparams.F1, _calculatedparams.OmgaT);
                }
                _isCalculated = true;
                _mc.ShowInfoMessage("Calculation complete","", false);
            }
            catch (Exception e)
            {
                _mc.ShowErrorMessage("Calculation failed!", e.ToString(), false);
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
                    ?? (_generateNote = new RelayCommand(ExecuteGenerateNote,
                    () => _isCalculated));
            }
        }

        private void ExecuteGenerateNote()
        {
            try
            {
                ReportGeneratorWindow _reportWindow = new ReportGeneratorWindow();
                Messenger.Default.Send(new NotificationMessage(this, "WaveForceViewModelInvoke"), "ViewModelForReport");
                _reportWindow.ShowDialog();

            }
            catch (Exception e)
            {
                _mc.ShowErrorMessage("Calculation Note Generation failed!", e.ToString(), false);
            }
        }
    }

    public class PDIWT_WaveForce_DesignInputParamters : ObservableObject
    {
        public PDIWT_WaveForce_DesignInputParamters()
        {
            //Messenger.Default.Register<string>(this, "Input",e =>
            // {
            //     WaveLength = WaveForce.CalculateWaveLength(T, WaterDepth, g);
            // });
        }
        /// <summary>
        /// The <see cref="DesignWaterLevel" /> property's name.
        /// </summary>
        public const string DesignWaterLevelPropertyName = "DesignWaterLevel";

        private DesignWaterLevelCondition _designWaterLevel;

        /// <summary>
        /// Sets and gets the DesignWaterLevel property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public DesignWaterLevelCondition DesignWaterLevel
        {
            get
            {
                return _designWaterLevel;
            }
            set
            {
                Set(() => DesignWaterLevel, ref _designWaterLevel, value);

            }
        }

        /// <summary>
        /// The <see cref="H1" /> property's name.
        /// </summary>
        public const string H1PropertyName = "H1";

        private double _h1;

        /// <summary>
        /// Unit: m
        /// Sets and gets the H1 property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double H1
        {
            get
            {
                return _h1;
            }
            set
            {
                if (value < 0)
                    Set(() => H1, ref _h1, 0);
                else
                    Set(() => H1, ref _h1, value);
                //Messenger.Default.Send("InputParameterChange","Input");
            }
        }
        /// <summary>
        /// The <see cref="H13" /> property's name.
        /// </summary>
        public const string H13PropertyName = "H13";

        private double _h13;

        /// <summary>
        /// Unit: m
        /// Sets and gets the H13 property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double H13
        {
            get
            {
                return _h13;
            }
            set
            {
                if(value < 0)
                    Set(() => H13, ref _h13, 0);
                else
                    Set(() => H13, ref _h13, value);
            }
        }

        /// <summary>
        /// The <see cref="T" /> property's name.
        /// </summary>
        public const string TPropertyName = "T";

        private double _t;

        /// <summary>
        /// Unit: s
        /// Sets and gets the T property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double T
        {
            get
            {
                return _t;
            }
            set
            {
                if(value < 0)
                Set(() => T, ref _t, 0);
                else
                    Set(() => T, ref _t, value);
                //Messenger.Default.Send("InputParameterChange","Input");
            }
        }

        /// <summary>
        /// The <see cref="WaterDepth" /> property's name.
        /// </summary>
        public const string WaterDepthPropertyName = "WaterDepth";

        private double _waterDepth;

        /// <summary>
        /// Unit: m
        /// Sets and gets the WaterDepth property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double WaterDepth
        {
            get
            {
                return _waterDepth;
            }
            set
            {
                if (value < 0)
                    Set(() => WaterDepth, ref _waterDepth, 0);
                else
                    Set(() => WaterDepth, ref _waterDepth, value);
                //Messenger.Default.Send("InputParameterChange","Input");
            }
        }

        /// <summary>
        /// The <see cref="WaveLength" /> property's name.
        /// </summary>
        public const string WaveLengthPropertyName = "WaveLength";

        private double _waveLength;

        /// <summary>
        /// Unit: 0
        /// Sets and gets the WaveLength property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double WaveLength
        {
            get
            {
                return _waveLength;
            }
            set
            {
                if (value < 0)
                    Set(() => WaveLength, ref _waveLength, double.NaN);
                else
                    Set(() => WaveLength, ref _waveLength, value);
            }
        }

    }

    public class PDIWT_WaveForce_CalculatedParameters : ObservableObject
    {
        /// <summary>
        /// The <see cref="DesignWaterLevel" /> property's name.
        /// </summary>
        public const string DesignWaterLevelPropertyName = "DesignWaterLevel";

        private DesignWaterLevelCondition _designWaterLevel;

        /// <summary>
        /// Sets and gets the DesignWaterLevel property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public DesignWaterLevelCondition DesignWaterLevel
        {
            get
            {
                return _designWaterLevel;
            }
            set
            {
                Set(() => DesignWaterLevel, ref _designWaterLevel, value);
            }
        }

        /// <summary>
        /// The <see cref="D_L" /> property's name.
        /// </summary>
        public const string D_LPropertyName = "D_L";

        private double _d_L;

        /// <summary>
        /// Sets and gets the D_L property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double D_L
        {
            get
            {
                return _d_L;
            }
            set
            {
                Set(() => D_L, ref _d_L, value);
            }
        }

        /// <summary>
        /// The <see cref="H_D" /> property's name.
        /// </summary>
        public const string H_DPropertyName = "H_D";

        private double _h_D;

        /// <summary>
        /// Sets and gets the H_D property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double H_D
        {
            get
            {
                return _h_D;
            }
            set
            {
                Set(() => H_D, ref _h_D, value);
            }
        }

        /// <summary>
        /// The <see cref="DD_L" /> property's name.
        /// </summary>
        public const string DD_LPropertyName = "DD_L";

        private double _dDL;

        /// <summary>
        /// Sets and gets the DD_L property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double DD_L
        {
            get
            {
                return _dDL;
            }
            set
            {
                Set(() => DD_L, ref _dDL, value);
            }
        }
        /// <summary>
        /// The <see cref="YitaMax" /> property's name.
        /// </summary>
        public const string YitaMaxPropertyName = "YitaMax";

        private double _yitaMax;

        /// <summary>
        /// Sets and gets the YitaMax property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double YitaMax
        {
            get
            {
                return _yitaMax;
            }
            set
            {
                Set(() => YitaMax, ref _yitaMax, value);
            }
        }

        /// <summary>
        /// The <see cref="Alpha" /> property's name.
        /// </summary>
        public const string AlphaPropertyName = "Alpha";

        private double _alpha;

        /// <summary>
        /// Sets and gets the Alpha property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double Alpha
        {
            get
            {
                return _alpha;
            }
            set
            {
                Set(() => Alpha, ref _alpha, value);
            }
        }

        /// <summary>
        /// The <see cref="Beta" /> property's name.
        /// </summary>
        public const string BetaPropertyName = "Beta";

        private double _beta;

        /// <summary>
        /// Sets and gets the Beta property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double Beta
        {
            get
            {
                return _beta;
            }
            set
            {
                Set(() => Beta, ref _beta, value);
            }
        }

        /// <summary>
        /// The <see cref="GammaP" /> property's name.
        /// </summary>
        public const string GammaPPropertyName = "GammaP";

        private double _gammaP;

        /// <summary>
        /// Sets and gets the GammaP property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double GammaP
        {
            get
            {
                return _gammaP;
            }
            set
            {
                Set(() => GammaP, ref _gammaP, value);
            }
        }

        /// <summary>
        /// The <see cref="GammaM" /> property's name.
        /// </summary>
        public const string GammaMPropertyName = "GammaM";

        private double _gammaM;

        /// <summary>
        /// Sets and gets the GammaM property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double GammaM
        {
            get
            {
                return _gammaM;
            }
            set
            {
                Set(() => GammaM, ref _gammaM, value);
            }
        }

        /// <summary>
        /// The <see cref="K" /> property's name.
        /// </summary>
        public const string KPropertyName = "K";

        private double _k;

        /// <summary>
        /// Sets and gets the K property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double K
        {
            get
            {
                return _k;
            }
            set
            {
                Set(() => K, ref _k, value);
            }
        }

        /// <summary>
        /// The <see cref="F0" /> property's name.
        /// </summary>
        public const string F0PropertyName = "F0";

        private double _f0;

        /// <summary>
        /// Sets and gets the F0 property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double F0
        {
            get
            {
                return _f0;
            }
            set
            {
                Set(() => F0, ref _f0, value);
            }
        }

        /// <summary>
        /// The <see cref="F1" /> property's name.
        /// </summary>
        public const string F1PropertyName = "F1";

        private double _f1;

        /// <summary>
        /// Sets and gets the F1 property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double F1
        {
            get
            {
                return _f1;
            }
            set
            {
                Set(() => F1, ref _f1, value);
            }
        }

        /// <summary>
        /// The <see cref="F2" /> property's name.
        /// </summary>
        public const string F2PropertyName = "F2";

        private double _f2;

        /// <summary>
        /// Sets and gets the F2 property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double F2
        {
            get
            {
                return _f2;
            }
            set
            {
                Set(() => F2, ref _f2, value);
            }
        }

        /// <summary>
        /// The <see cref="F3" /> property's name.
        /// </summary>
        public const string F3PropertyName = "F3";

        private double _f3;

        /// <summary>
        /// Sets and gets the F3 property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double F3
        {
            get
            {
                return _f3;
            }
            set
            {
                Set(() => F3, ref _f3, value);
            }
        }

        /// <summary>
        /// The <see cref="OmgaT" /> property's name.
        /// </summary>
        public const string OmgaTPropertyName = "OmgaT";

        private double _omgaT;

        /// <summary>
        /// Sets and gets the OmgaT property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double OmgaT
        {
            get
            {
                return _omgaT;
            }
            set
            {
                Set(() => OmgaT, ref _omgaT, value);
            }
        }

        /// <summary>
        /// The <see cref="K1" /> property's name.
        /// </summary>
        public const string K1PropertyName = "K1";

        private double _k1;

        /// <summary>
        /// Sets and gets the K1 property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double K1
        {
            get
            {
                return _k1;
            }
            set
            {
                Set(() => K1, ref _k1, value);
            }
        }

        /// <summary>
        /// The <see cref="K2" /> property's name.
        /// </summary>
        public const string K2PropertyName = "K2";

        private double _k2;

        /// <summary>
        /// Sets and gets the K2 property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double K2
        {
            get
            {
                return _k2;
            }
            set
            {
                Set(() => K2, ref _k2, value);
            }
        }

        /// <summary>
        /// The <see cref="K3" /> property's name.
        /// </summary>
        public const string K3PropertyName = "K3";

        private double _k3;

        /// <summary>
        /// Sets and gets the K3 property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double K3
        {
            get
            {
                return _k3;
            }
            set
            {
                Set(() => K3, ref _k3, value);
            }
        }
        /// <summary>
        /// The <see cref="K4" /> property's name.
        /// </summary>
        public const string K4PropertyName = "K4";

        private double _k4;

        /// <summary>
        /// Sets and gets the K4 property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double K4
        {
            get
            {
                return _k4;
            }
            set
            {
                Set(() => K4, ref _k4, value);
            }
        }

    }

    public class PDIWT_WaveForce_Results : ObservableObject
    {

        private double _pDMax;
        /// <summary>
        /// Property Description
        /// </summary>
        public double PDMax
        {
            get { return _pDMax; }
            set { Set(ref _pDMax, value); }
        }

        private double _mDMax;
        /// <summary>
        /// Property Description
        /// </summary>
        public double MDMax
        {
            get { return _mDMax; }
            set { Set(ref _mDMax, value); }
        }

        private double _pIMax;
        /// <summary>
        /// Property Description
        /// </summary>
        public double PIMax
        {
            get { return _pIMax; }
            set { Set(ref _pIMax, value); }
        }

        private double _mIMax;
        /// <summary>
        /// Property Description
        /// </summary>
        public double MIMax
        {
            get { return _mIMax; }
            set { Set(ref _mIMax, value); }
        }

        private double _pDMax_Final;
        /// <summary>
        /// Property Description
        /// </summary>
        public double PDMax_Final
        {
            get { return _pDMax_Final; }
            set { Set(ref _pDMax_Final, value); }
        }

        private double _mDMax_Final;
        /// <summary>
        /// Property Description
        /// </summary>
        public double MDMax_Final
        {
            get { return _mDMax_Final; }
            set { Set(ref _mDMax_Final, value); }
        }

        private double _pIMax_Final;
        /// <summary>
        /// Property Description
        /// </summary>
        public double PIMax_Final
        {
            get { return _pIMax_Final; }
            set { Set(ref _pIMax_Final, value); }
        }

        private double _mIMax_Final;
        /// <summary>
        /// Property Description
        /// </summary>
        public double MIMax_Final
        {
            get { return _mIMax_Final; }
            set { Set(ref _mIMax_Final, value); }
        }


        private double _omgaT;
        /// <summary>
        /// 
        /// </summary>
        public double OmgaT
        {
            get { return _omgaT; }
            set { Set(ref _omgaT, value); }
        }

        /// <summary>
        /// The <see cref="DesignWaterLevel" /> property's name.
        /// </summary>
        public const string DesignWaterLevelPropertyName = "DesignWaterLevel";

        private DesignWaterLevelCondition _designWaterLevel;

        /// <summary>
        /// Sets and gets the DesignWaterLevel property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public DesignWaterLevelCondition DesignWaterLevel
        {
            get
            {
                return _designWaterLevel;
            }
            set
            {
                Set(() => DesignWaterLevel, ref _designWaterLevel, value);
            }
        }

        /// <summary>
        /// The <see cref="PMax" /> property's name.
        /// </summary>
        public const string PMaxPropertyName = "PMax";

        private double _pMax;

        /// <summary>
        /// Sets and gets the PMax property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double PMax
        {
            get
            {
                return _pMax;
            }
            set
            {
                Set(() => PMax, ref _pMax, value);
            }
        }

        /// <summary>
        /// The <see cref="MMax" /> property's name.
        /// </summary>
        public const string MMaxPropertyName = "MMax";

        private double _mMax;

        /// <summary>
        /// Sets and gets the MMax property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double MMax
        {
            get
            {
                return _mMax;
            }
            set
            {
                Set(() => MMax, ref _mMax, value);
            }
        }

        /// <summary>
        /// The <see cref="Pu" /> property's name.
        /// </summary>
        public const string PuPropertyName = "Pu";

        private double _pu;

        /// <summary>
        /// Sets and gets the Pu property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double Pu
        {
            get
            {
                return _pu;
            }
            set
            {
                Set(() => Pu, ref _pu, value);
            }
        }

        /// <summary>
        /// The <see cref="Mu" /> property's name.
        /// </summary>
        public const string MuPropertyName = "Mu";

        private double _mu;

        /// <summary>
        /// Sets and gets the Mu property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double Mu
        {
            get
            {
                return _mu;
            }
            set
            {
                Set(() => Mu, ref _mu, value);
            }
        }
    }
    public enum DesignWaterLevelCondition
    {
        [Description("HAT")]
        HAT,
        [Description("MHW")]
        MHW,
        [Description("MLW")]
        MLW,
        [Description("LAT")]
        LAT
    }
}
