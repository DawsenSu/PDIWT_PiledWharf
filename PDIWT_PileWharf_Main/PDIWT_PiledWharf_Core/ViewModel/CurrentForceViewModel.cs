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

using PDIWT_PiledWharf_Core.Model;
using MN = MathNet.Numerics;

namespace PDIWT_PiledWharf_Core.ViewModel
{
    public class CurrentForceViewModel : ViewModelBase
    {
        public CurrentForceViewModel()
        {
            PileTopElevation = 4;
            HAT = 0;
            SoilElevation = -22.4;
            ProjectedWidth = 1.6;

            ResourceDictionary _resDic = new ResourceDictionary();
            _resDic.Source = new Uri("PDIWT.Resources;component/Localization/OtherModule/en-US.xaml", UriKind.Relative);
            ShapeCategory = new List<ShapeInfo>
                {
                    new ShapeInfo() { Shape = (string)_resDic["Round"], Value =1 },
                    new ShapeInfo() { Shape = (string)_resDic["Square"], Value =2 },
                };
            SelectedShape = ShapeCategory[0];
            SquarePileAngle = 0;
            CurrentVelocity = 0.8;
            PileHorizontalCentraSpan = 6.4;
            PileVerticalCentraSpan = 6.4;
            WaterDensity = 1025;

            UpdatedRelatedProperties(new PropertyChangedMessage<double>(0, 0, "Initial"));
            MessengerInstance.Register<PropertyChangedMessage<double>>(this, UpdatedRelatedProperties);
            MessengerInstance.Register<PropertyChangedMessage<ShapeInfo>>(this, UpdatedRelatedProperties);
        }


        /// <summary>
        /// The <see cref="PileTopElevation" /> property's name.
        /// </summary>
        public const string PileTopElevationPropertyName = "PileTopElevation";

        private double _pileTopElevation = 0.0;

        /// <summary>
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
                Set(PileTopElevationPropertyName, ref _pileTopElevation, value, true);
            }
        }

        /// <summary>
        /// The <see cref="HAT" /> property's name.
        /// </summary>
        public const string HATPropertyName = "HAT";

        private double _hat = 0.0;

        /// <summary>
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
                Set(() => HAT, ref _hat, value, true);
            }
        }

        /// <summary>
        /// The <see cref="SoilElevation" /> property's name.
        /// </summary>
        public const string SoilElevationPropertyName = "SoilElevation";

        private double _soilElevation = 0.0;

        /// <summary>
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
                Set(() => SoilElevation, ref _soilElevation, value, true);
            }
        }

        /// <summary>
        /// The <see cref="ProjectedWidth" /> property's name.
        /// </summary>
        public const string ProjectedWidthPropertyName = "ProjectedWidth";

        private double _projectedWidth = 0.0;

        /// <summary>
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
                Set(() => ProjectedWidth, ref _projectedWidth, value, true);
            }
        }

        /// <summary>
        /// The <see cref="ShapeCategory" /> property's name.
        /// </summary>
        public const string ShapeCategoryPropertyName = "ShapeCategory";

        private List<ShapeInfo> _shapeInfos = new List<ShapeInfo>();

        /// <summary>
        /// Sets and gets the ShapeCategory property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public List<ShapeInfo> ShapeCategory
        {
            get
            {
                return _shapeInfos;
            }
            set
            {
                Set(() => ShapeCategory, ref _shapeInfos, value);
            }
        }

        /// <summary>
        /// The <see cref="SelectedShape" /> property's name.
        /// </summary>
        public const string SelectedShapePropertyName = "SelectedShape";

        private ShapeInfo _selectedShape;

        /// <summary>
        /// Sets and gets the SelectedShape property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ShapeInfo SelectedShape
        {
            get
            {
                return _selectedShape;
            }
            set
            {
                Set(() => SelectedShape, ref _selectedShape, value, true);
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
                Set(() => SquarePileAngle, ref _squarePileAngle, value,true);
            }
        }

        /// <summary>
        /// The <see cref="CurrentVelocity" /> property's name.
        /// </summary>
        public const string CurrentVelocityPropertyName = "CurrentVelocity";

        private double _currentVelocity = 0.0;

        /// <summary>
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
                Set(() => CurrentVelocity, ref _currentVelocity, value, true);
            }
        }

        /// <summary>
        /// The <see cref="PileHorizontalCentraSpan" /> property's name.
        /// </summary>
        public const string PileHorizontalCentraSpanPropertyName = "PileHorizontalCentraSpan";

        private double _pileHorizontalCentraSpan = 0.0;

        /// <summary>
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
                Set(() => PileHorizontalCentraSpan, ref _pileHorizontalCentraSpan, value, true);
            }
        }

        /// <summary>
        /// The <see cref="PileVerticalCentraSpan" /> property's name.
        /// </summary>
        public const string PileVerticalCentraSpanPropertyName = "PileVerticalCentraSpan";

        private double _pileVerticalCentraSpan = 0.0;

        /// <summary>
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
                Set(() => PileVerticalCentraSpan, ref _pileVerticalCentraSpan, value, true);
            }
        }

        /// <summary>
        /// The <see cref="WaterDensity" /> property's name.
        /// </summary>
        public const string WaterDensityPropertyName = "WaterDensity";

        private double _waterDensity = 0;

        /// <summary>
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
                Set(() => WaterDensity, ref _waterDensity, value, true);
            }
        }

        /// <summary>
        /// The <see cref="PileHeight" /> property's name.
        /// </summary>
        public const string PileHeightPropertyName = "PileHeight";

        private double _pileHeight = 0.0;

        /// <summary>
        /// Sets and gets the PileHeight property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double PileHeight
        {
            get
            {
                return _pileHeight;
            }
            set
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
        /// Sets and gets the ProjectedArea property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double ProjectedArea
        {
            get
            {
                return _projectedArea;
            }
            set
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
            set
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
            set
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
            set
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
            set
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
            set
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
            set
            {
                Set(() => IncliningAffectCoeff, ref _incliningAffectCoeff, value);
            }
        }

        /// <summary>
        /// The <see cref="CurrentForceForFrontPile" /> property's name.
        /// </summary>
        public const string CurrentForceForFrontPilePropertyName = "CurrentForceForFrontPile";

        private double _currentForceForFrontPile = 0.0;

        /// <summary>
        /// Sets and gets the CurrentForceForFrontPile property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double CurrentForceForFrontPile
        {
            get
            {
                return _currentForceForFrontPile;
            }
            set
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
        /// Sets and gets the CurrentForceForRearPile property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double CurrentForceForRearPile
        {
            get
            {
                return _currentForceForRearPile;
            }
            set
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
        /// Sets and gets the ActionPointForFrontPile property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double ActionPointForFrontPile
        {
            get
            {
                return _actionPointForFrontPile;
            }
            set
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
        /// Sets and gets the ActionPointForRearPile property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double ActionPointForRearPile
        {
            get
            {
                return _actionPointForRearPile;
            }
            set
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
            MessageBox.Show("Load Parameter");
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
            //MessageBox.Show($"Calculate {SelectedShape.Shape},{SelectedShape.Value}");
            CurrentForceForFrontPile = CurrentResistentCoeff * WaterDensity / 1000 / 2 * Math.Pow(CurrentVelocity, 2) * ProjectedArea * 1 * SubmergedCoeff * WaterDepthCoeff * HorizontalAffectCoeff * IncliningAffectCoeff;
            CurrentForceForRearPile = CurrentResistentCoeff * WaterDensity / 1000 / 2 * Math.Pow(CurrentVelocity, 2) * ProjectedArea * ShelteringCoeff * SubmergedCoeff * WaterDepthCoeff * HorizontalAffectCoeff * IncliningAffectCoeff;
            ActionPointForFrontPile = PileTopElevation - HAT + PileHeight / 3;
            ActionPointForRearPile = ActionPointForFrontPile;
        }

        private void UpdatedRelatedProperties(PropertyChangedMessage<ShapeInfo> propertyChangedMessage)
        {
            UpdatedRelatedProperties(new PropertyChangedMessage<double>(0,0,"Changed"));
        }
        private void UpdatedRelatedProperties(PropertyChangedMessage<double> propertyChangedMessage)
        {
            PileHeight = HAT - SoilElevation;
            ProjectedArea = PileHeight * ProjectedWidth;
            if (SelectedShape.Value == 1)
                CurrentResistentCoeff = 0.73;
            else
                CurrentResistentCoeff = 1.5;
            ShelteringCoeff = CalculateShelteringCoeff((PileVerticalCentraSpan - ProjectedWidth) / ProjectedWidth);
            SubmergedCoeff = 1;
            WaterDepthCoeff = CalculateWaterDepthCoeff(PileHeight - ProjectedWidth);
            HorizontalAffectCoeff = CalculatedHorizontalAffectCoeff((PileHorizontalCentraSpan - ProjectedWidth) / ProjectedWidth, SelectedShape);
            IncliningAffectCoeff = CalculateIncliningAffectCoeff(SquarePileAngle, SelectedShape);

            CurrentForceForFrontPile = 0.0;
            CurrentForceForRearPile = 0.0;
            ActionPointForFrontPile = 0.0;
            ActionPointForRearPile = 0.0;
        }

        private double CalculateShelteringCoeff(double input, bool isFrontPile = false)
        {

            double _result = 1.0;
            if (isFrontPile)
                return _result;
            else
            {
                double[] _LD = { 1, 2, 3, 4, 6, 8, 12, 16, 18, 20 };
                double[] _m1Rear = { -0.38, 0.25, 0.54, 0.66, 0.78, 0.82, 0.86, 0.88, 0.9, 1 };
                MN.Interpolation.LinearSpline _linearSolver = MN.Interpolation.LinearSpline.InterpolateSorted(_LD, _m1Rear);
                _result = _linearSolver.Interpolate(input);
                if (_result > 1)
                    _result = 1;
                return _result;
            }
        }

        private double CalculateWaterDepthCoeff(double H_D)
        {
            double _result = 0.0;
            double[] _h_d = { 1, 2, 4, 6, 8, 10, 12, 14 };
            double[] _n2 = { 0.76, 0.78, 0.82, 0.85, 0.89, 0.93, 0.97, 1 };
            var _linearSolver = MN.Interpolate.Linear(_h_d, _n2);
            _result = _linearSolver.Interpolate(H_D);
            if (_result > 1)
                _result = 1;
            return _result;
        }

        private double CalculatedHorizontalAffectCoeff(double B_D, ShapeInfo shape)
        {
            double _result = 0;
            if (shape.Value == 1)
            {
                double[] _b_d = { 3, 7, 10, 15 };
                double[] _m2 = { 1.83, 1.25, 1.15, 1 };
                var _linearSolver = MN.Interpolation.LinearSpline.InterpolateSorted(_b_d, _m2);
                _result = _linearSolver.Interpolate(B_D);
            }
            else
            {
                double[] _b_d = { 4, 6, 8, 10, 12 };
                double[] _m2 = { 1.21, 1.08, 1.06, 1.03, 1 };
                var _linearSolver = MN.Interpolation.LinearSpline.InterpolateSorted(_b_d, _m2);
                _result = _linearSolver.Interpolate(B_D);
            }

            if (_result < 1)
                _result = 1;
            return _result;
        }

        private double CalculateIncliningAffectCoeff(double alpha, ShapeInfo shape)
        {
            double _result = 1;
            if(shape.Value == 2)
            {
                double[] _alpha = { 0, 10, 20, 30, 45 };
                double[] _m3 = { 1, 0.67, 0.67, 0.71, 0.75 };
                var _linerSolver = MN.Interpolate.Linear(_alpha, _m3);
                _result = _linerSolver.Interpolate(alpha);
            }
            return _result;
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
                        Messenger.Default.Send(new NotificationMessage<CurrentForceViewModel>(this, "Report"), "ViewModelForReport");
                        _reportWindow.ShowDialog();
                    },
                    () => CurrentForceForFrontPile != 0));
            }
        }
    }

    public class ShapeInfo
    {
        public string Shape { get; set; }
        public int Value { get; set; }
    }

}
