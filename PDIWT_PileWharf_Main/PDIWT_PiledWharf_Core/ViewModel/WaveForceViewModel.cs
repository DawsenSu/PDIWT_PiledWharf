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

namespace PDIWT_PiledWharf_Core.ViewModel
{
    public class WaveForceViewModel : ViewModelBase
    {
        public WaveForceViewModel()
        {

            // Register Broadcast message 
            Messenger.Default.Register<PropertyChangedMessage<ShapeInfo>>(this, SelectedShapeChanged);

            //Init Data
            ShapeCategory = new List<ShapeInfo>
                {
                    new ShapeInfo() { Shape = Resources.SquarePile, Value =2 },
                    new ShapeInfo() { Shape = Resources.TubePile, Value =1 },
                    new ShapeInfo() { Shape = Resources.PHCTubePile, Value =1 },
                    new ShapeInfo() { Shape = Resources.SteelTubePile, Value =1 }
                };
            SelectedShape = ShapeCategory[0];
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
                new PDIWT_WaveForce_DesignInputParamters(){DesignWaterLevel = DesignWaterLevelCondition.HAT, H1=4.2,H13=3.1,T=11.2,WaterDepth =26.02},
                new PDIWT_WaveForce_DesignInputParamters(){DesignWaterLevel = DesignWaterLevelCondition.MHW, H1=4,H13=2.9,T=6.6,WaterDepth =24.82},
                new PDIWT_WaveForce_DesignInputParamters(){DesignWaterLevel = DesignWaterLevelCondition.MLW, H1=3.7,H13=2.8,T=6.3,WaterDepth =20.97},
                new PDIWT_WaveForce_DesignInputParamters(){DesignWaterLevel = DesignWaterLevelCondition.LAT, H1=2.9,H13=2.8,T=11.2,WaterDepth =19.67}
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


        }

        private List<ShapeInfo> _shapeCategory;
        /// <summary>
        /// Property Description
        /// </summary>
        public List<ShapeInfo> ShapeCategory
        {
            get { return _shapeCategory; }
            set { Set(ref _shapeCategory, value); }
        }

        private ShapeInfo _selectedShape;
        /// <summary>
        /// Property Description
        /// </summary>
        public ShapeInfo SelectedShape
        {
            get { return _selectedShape; }
            set { Set(ref _selectedShape, value,true); }
        }
        
        private void SelectedShapeChanged(PropertyChangedMessage<ShapeInfo> shapePropertyChanged)
        {
            if(shapePropertyChanged.PropertyName == "SelectedShape" )
            {
                if (shapePropertyChanged.NewValue.Value == 1)
                {
                    VelocityForceCoeffCD = 1.2;
                    VelocityForceCoeffCM = 2.0;
                }
                else
                {
                    VelocityForceCoeffCD = 2.0;
                    VelocityForceCoeffCM = 2.2;
                }
            }
        }


        private double _pileDiameter;
        /// <summary>
        /// Property Description
        /// </summary>
        public double PileDiameter
        {
            get { return _pileDiameter; }
            set { Set(ref _pileDiameter, value); }
        }

        private double _pileCentraSpan;
        /// <summary>
        /// Property Description
        /// </summary>
        public double PileCentraSpan
        {
            get { return _pileCentraSpan; }
            set { Set(ref _pileCentraSpan, value); }
        }

        private double _topElevation;
        /// <summary>
        /// Property Description
        /// </summary>
        public double TopElevation
        {
            get { return _topElevation; }
            set { Set(ref _topElevation, value); }
        }

        private double _bottomElevation;
        /// <summary>
        /// Property Description
        /// </summary>
        public double BottomElevation
        {
            get { return _bottomElevation; }
            set { Set(ref _bottomElevation, value); }
        }


        private double _hat;
        /// <summary>
        /// Property Description
        /// </summary>
        public double HAT
        {
            get { return _hat; }
            set { Set(ref _hat, value); }
        }

        private double _mhw;
        /// <summary>
        /// Property Description
        /// </summary>
        public double MHW
        {
            get { return _mhw; }
            set { Set(ref _mhw, value); }
        }

        private double _mlw;
        /// <summary>
        /// Property Description
        /// </summary>
        public double MLW
        {
            get { return _mlw; }
            set { Set(ref _mlw, value); }
        }

        private double _lat;
        /// <summary>
        /// Property Description
        /// </summary>
        public double LAT
        {
            get { return _lat; }
            set { Set(ref _lat, value); }
        }

        private double _velocityForceCoeffCD;
        /// <summary>
        /// Property Description
        /// </summary>
        public double VelocityForceCoeffCD
        {
            get { return _velocityForceCoeffCD; }
            set { Set(ref _velocityForceCoeffCD, value); }
        }

        private double _velocityForceCoeffCM;
        /// <summary>
        /// Property Description
        /// </summary>
        public double VelocityForceCoeffCM
        {
            get { return _velocityForceCoeffCM; }
            set { Set(ref _velocityForceCoeffCM, value); }
        }

        private double _waterWeight;
        /// <summary>
        /// Property Description
        /// </summary>
        public double WaterWeight
        {
            get { return _waterWeight; }
            set { Set(ref _waterWeight, value); }
        }

        private double _gravitationalAcceleration;
        /// <summary>
        /// Property Description
        /// </summary>
        public double GravitationalAcceleration
        {
            get { return _gravitationalAcceleration; }
            set { Set(ref _gravitationalAcceleration, value); }
        }

        private ObservableCollection<PDIWT_WaveForce_DesignInputParamters> _designInputParameters;
        /// <summary>
        /// Property Description
        /// </summary>
        public ObservableCollection<PDIWT_WaveForce_DesignInputParamters> DesignInputParameters
        {
            get { return _designInputParameters; }
            set { Set(ref _designInputParameters, value); }
        }

        private ObservableCollection<PDIWT_WaveForce_CalculatedParameters> _calculatedParamters;
        /// <summary>
        /// Property Description
        /// </summary>
        public ObservableCollection<PDIWT_WaveForce_CalculatedParameters> CalculatedParameters
        {
            get { return _calculatedParamters; }
            set { Set(ref _calculatedParamters, value); }
        }

        private ObservableCollection<PDIWT_WaveForce_Results> _results;
        /// <summary>
        /// Property Description
        /// </summary>
        public ObservableCollection<PDIWT_WaveForce_Results> Results
        {
            get { return _results; }
            set { Set(ref _results, value); }
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
                    ?? (_calculate = new RelayCommand(ExecuteCalculate));
            }
        }

        private void ExecuteCalculate()
        {
            foreach (var _input in DesignInputParameters)
            {
                _input.WaveLength = WaveForce.CalculateWaveLength(_input.T, _input.WaterDepth, GravitationalAcceleration);
            }

            foreach (var _parameter in CalculatedParameters)
            {
                var _inputparameters = DesignInputParameters.Where(e => e.DesignWaterLevel == _parameter.DesignWaterLevel).FirstOrDefault();
                _parameter.D_L = PileDiameter / _inputparameters.WaveLength;
                _parameter.H_D = _inputparameters.H1 / _inputparameters.WaterDepth;
                _parameter.DD_L = _inputparameters.WaterDepth / _inputparameters.WaveLength;
                _parameter.YitaMax = WaveForce.CalculateYitaMax(_inputparameters.H1, _inputparameters.WaterDepth);
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
                _parameter.K2 = WaveForce.CalculateK2(0, _inputparameters.WaterDepth + _parameter.YitaMax - _inputparameters.H1/2, _inputparameters.WaveLength, _inputparameters.WaterDepth);
                _parameter.K3 = WaveForce.CalculateK3(0, _inputparameters.WaterDepth + _parameter.YitaMax, _inputparameters.WaveLength, _inputparameters.WaterDepth);
                _parameter.K4 = WaveForce.CalculateK4(0, _inputparameters.WaterDepth + _parameter.YitaMax - _inputparameters.H1 / 2, _inputparameters.WaveLength, _inputparameters.WaterDepth);
            }
            double _crossectionarea = Math.PI * Math.Pow(PileDiameter / 2, 2);
            foreach (var _result in Results)
            {
                var _inputparams = DesignInputParameters.Where(e => e.DesignWaterLevel == _result.DesignWaterLevel).FirstOrDefault();
                var _calculatedparams = CalculatedParameters.Where(e => e.DesignWaterLevel == _result.DesignWaterLevel).FirstOrDefault();
                _result.PMax = _calculatedparams.K * WaveForce.CalculateMaxInertiaComponentOfWaveForce(_calculatedparams.GammaP, VelocityForceCoeffCM, WaterWeight,
                    _crossectionarea, _inputparams.H1,_calculatedparams.K2);
                _result.MMax = _calculatedparams.K * WaveForce.CalculateMaxInertiaComponentOfWaveMonment(_calculatedparams.GammaM, VelocityForceCoeffCM, WaterWeight,
                    _crossectionarea, _inputparams.H1, _inputparams.WaveLength, _calculatedparams.K4);
                _result.Pu = WaveForce.CalculatePu(WaterWeight, _inputparams.H1, PileDiameter, 0, _inputparams.WaveLength, _inputparams.WaterDepth, _calculatedparams.F2, _calculatedparams.F0, _calculatedparams.OmgaT);
                _result.Mu = WaveForce.CalculateMu(WaterWeight, _inputparams.H1, PileDiameter, 0, _inputparams.WaveLength, _inputparams.WaterDepth, _calculatedparams.F3, _calculatedparams.F1, _calculatedparams.OmgaT);
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
                Set(() => H13, ref _h13, value);
            }
        }

        /// <summary>
        /// The <see cref="T" /> property's name.
        /// </summary>
        public const string TPropertyName = "T";

        private double _t;

        /// <summary>
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
