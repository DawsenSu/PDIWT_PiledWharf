using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MathNet.Numerics;
using MathNet.Numerics.Interpolation;

using System.Data.SQLite;
using PDIWT_PiledWharf_Core.ViewModel;

using BD = Bentley.DgnPlatformNET;

using PDIWT_PiledWharf_Core_Cpp;
namespace PDIWT.Formulas
{
    /// <summary>
    /// Formulas for calculating Axial Bearing Capacity
    /// </summary>
    public static class AxialBearingCapacity
    {
        public static double CalculatePilePrimeter(PileTypeManaged pileType, double daimeter)
        {
            if (PileTypeManaged.SqaurePile == pileType)
                return 4 * daimeter;
            else
                return Math.PI * daimeter;
        }

        public static double CalculatePileEndOutsideArea(PileTypeManaged pileType, double outerdiameter)
        {
            if (PileTypeManaged.SqaurePile == pileType)
                return Math.Pow(outerdiameter, 2);
            else
                return Math.PI * Math.Pow(outerdiameter, 2) / 4;
        }

        public static double CalculatePileSelfWeight(
            PDIWT_PiledWharf_Core_Cpp.PileTypeManaged pileGeoType,
            double pileOuterDiameter,
            double pileInnerDiameter,
            double pileTopElevation,
            double pileLength,
            double pileskewness,
            double calculatedWaterLevel,
            double concreteWeight,
            double concreteUnderwaterWeight,
            double steelWeight,
            double steelUnderwaterWeight,
            double concretecorelength
            )
        {
            double _pilelength_aboveWaterLevel = 0;
            double _pileLength_underWaterLevel = 0;

            double _pileBottomElevation = CalculatePileBottomElevation(pileTopElevation, pileLength, pileskewness);
            double _cosAlpha = CalculateCosAlpha(pileskewness);

            if(calculatedWaterLevel>=pileTopElevation)
            {
                _pileLength_underWaterLevel = pileLength;
            }
            else if(calculatedWaterLevel <= _pileBottomElevation)
            {
                _pilelength_aboveWaterLevel = pileLength;
            }
            else
            {
                double _heightDiffBetweenPileTopandWaterLevel = pileTopElevation - calculatedWaterLevel;
                _pilelength_aboveWaterLevel = _heightDiffBetweenPileTopandWaterLevel / _cosAlpha;
                _pileLength_underWaterLevel = pileLength - _pilelength_aboveWaterLevel;
            }

            double _crossSectionArea = 0;

            switch (pileGeoType)
            {
                case PDIWT_PiledWharf_Core_Cpp.PileTypeManaged.SqaurePile:
                    _crossSectionArea = pileOuterDiameter * pileOuterDiameter;
                    break;
                case PDIWT_PiledWharf_Core_Cpp.PileTypeManaged.TubePile:
                    _crossSectionArea = Math.PI * pileOuterDiameter * pileOuterDiameter / 4;
                    break;
                case PDIWT_PiledWharf_Core_Cpp.PileTypeManaged.PHCTubePile:
                    _crossSectionArea = Math.PI * (pileOuterDiameter * pileOuterDiameter - pileInnerDiameter * pileInnerDiameter) / 4;
                    break;
                case PDIWT_PiledWharf_Core_Cpp.PileTypeManaged.SteelTubePile:
                    _crossSectionArea = Math.PI * (pileOuterDiameter * pileOuterDiameter - pileInnerDiameter * pileInnerDiameter) / 4;
                    break;
                default:
                    break;
            }
            double _pileWeight =  _crossSectionArea * (_pilelength_aboveWaterLevel * concreteWeight + _pileLength_underWaterLevel * concreteUnderwaterWeight);
            if (pileGeoType == PDIWT_PiledWharf_Core_Cpp.PileTypeManaged.SteelTubePile)
            {
                double _outterWeight = _crossSectionArea * (_pilelength_aboveWaterLevel * steelWeight + _pileLength_underWaterLevel * steelUnderwaterWeight);
                double _concretecorelength_abovewater = 0;
                double _concretecorelength_underwater = 0;
                double _coreTopElevation = _pileBottomElevation + concretecorelength * _cosAlpha;
                if (calculatedWaterLevel >= _coreTopElevation)
                    _concretecorelength_underwater = concretecorelength;
                else if (calculatedWaterLevel <= _pileBottomElevation)
                    _concretecorelength_abovewater = concretecorelength;
                else
                {
                    double _heightDiffBetweenCoreTopandWaterLevel = _coreTopElevation - calculatedWaterLevel;
                    _concretecorelength_abovewater = _heightDiffBetweenCoreTopandWaterLevel / _cosAlpha;
                    _concretecorelength_underwater = concretecorelength - _concretecorelength_abovewater;
                }
                double _coreWeight = _crossSectionArea * (_concretecorelength_abovewater * concreteWeight + _concretecorelength_underwater * concreteUnderwaterWeight);
                _pileWeight = _outterWeight + _coreWeight;
            }
            return _pileWeight;
        }

        /// <summary>
        /// Calculate Angle based on skewness
        /// </summary>
        /// <param name="skewness">if double.NaN, it's vertical pile</param>
        /// <returns></returns>
        public static double CalculateCosAlpha(double skewness)
        {
            if (skewness == 0)
                throw new InvalidParameterException(0, new Exception("skewness can't be 0!"));
            if (double.IsNaN(skewness))
                return 1;
            else
                return skewness / Math.Sqrt(1 + skewness * skewness);
        }

        public static double CalculatePileBottomElevation(double pileTopElevation, double pileLength, double skewness)
        {
            return pileTopElevation - pileLength * CalculateCosAlpha(skewness);
        }

        


        public static double CalculateCastInSituAfterGrountingPileBearingCapacity(
            double gammar,
            List<double> qfi,
            List<double> li,
            List<double> psii,
            List<double> betasi,
            double U,
            double qr,
            double A,
            double psip,
            double betap)
        {
            int _count = qfi.Count();
            if(li.Count() != _count || psii.Count() != _count || betasi.Count() != _count)
            {
                throw new InvalidParameterException(1,new Exception("The parameters [qfi, li, psii, betasi] is not same length"));
            }
            double _sidefriction = 0;
            for (int i = 0; i < _count; i++)
            {
                _sidefriction += betasi[i] * psii[i] * qfi[i] * li[i];
            }
            _sidefriction *= U;
            return (_sidefriction + betap * psip * qr * A) / gammar;
        }

        public static double CalculateCasInSituPileBearingCapacity(
            double gammar,
            List<double> qfi,
            List<double> li,
            List<double> psii,
            double U,
            double qr,
            double A,
            double psip)
        {
            int _count = qfi.Count();
            if (li.Count() != _count || psii.Count() != _count )
            {
                throw new InvalidParameterException(1, new Exception("The parameters [qfi, li, psii] is not same length"));
            }
            List<double> _betasiWithOnes = new List<double>();
            foreach (var item in qfi)
            {
                _betasiWithOnes.Add(1);
            }
            return CalculateCastInSituAfterGrountingPileBearingCapacity(gammar, qfi, li, psii, _betasiWithOnes, U, qr, A, psip, 1);
        }


        /// <summary>
        /// Calculate both for clause 4.2.8.1 and 4.2.8.2 
        /// </summary>
        /// <param name="gammar"></param>
        /// <param name="qfi"></param>
        /// <param name="li"></param>
        /// <param name="U"></param>
        /// <param name="qr"></param>
        /// <param name="A"></param>
        /// <param name="yita">When it comes to clause 4.2.8.1 yita set to 1 as default</param>
        /// <returns></returns>
        public static double CalculateDrivenPileBearingCapacity(
            double gammar,
            List<double> qfi,
            List<double> li,
            double U,
            double qr,
            double A,
            double yita=1)
        {
            if(qfi.Count() != li.Count())
            {
                throw new InvalidParameterException(1, new Exception("The parameters [qfi, li] is not same length"));
            }
            List<double> _psiiWithOnes = new List<double>();
            foreach (var item in qfi)
            {
                _psiiWithOnes.Add(1);
            }
            return CalculateCasInSituPileBearingCapacity(gammar, qfi, li, _psiiWithOnes, U, qr, A, yita);
        }

        public static double CalculateDrivenAndCastInSituPileUpliftForce(
            double gammar,
            List<double> qfi,
            List<double> li,
            List<double> psii,
            double U,
            double G,
            double Cosalpha)
        {
            double _count = qfi.Count();
            if (li.Count() != _count || psii.Count() !=  _count)
            {
                throw new InvalidParameterException(1, new Exception("The parameters [qfi, li] is not same length"));
            }
            double _sidefriction = 0;
            for (int i = 0; i < _count; i++)
            {
                _sidefriction += psii[i] * qfi[i] * li[i];
            }
            _sidefriction *= U;
            return (_sidefriction + G * Cosalpha) / gammar;
        }

    }

    /// <summary>
    /// Formulas for calculating Current force imposing pile
    /// </summary>
    public static class CurrentForce
    {
        public static double CalculateShelteringCoeff(double input, bool isFrontPile = false)
        {

            double _result = 1.0;
            if (isFrontPile)
                return _result;
            else
            {
                double[] _LD = { 1, 2, 3, 4, 6, 8, 12, 16, 18, 20 };
                double[] _m1Rear = { -0.38, 0.25, 0.54, 0.66, 0.78, 0.82, 0.86, 0.88, 0.9, 1 };
                LinearSpline _linearSolver = LinearSpline.InterpolateSorted(_LD, _m1Rear);
                _result = _linearSolver.Interpolate(input);
                if (_result > 1)
                    _result = 1;
                return _result;
            }
        }

        public static double CalculateWaterDepthCoeff(double H_D)
        {
            double _result = 0.0;
            double[] _h_d = { 1, 2, 4, 6, 8, 10, 12, 14 };
            double[] _n2 = { 0.76, 0.78, 0.82, 0.85, 0.89, 0.93, 0.97, 1 };
            var _linearSolver = Interpolate.Linear(_h_d, _n2);
            _result = _linearSolver.Interpolate(H_D);
            if (_result > 1)
                _result = 1;
            return _result;
        }

        public static double CalculatedHorizontalAffectCoeff(double B_D, PileTypeManaged pileGeoType)
        {
            double _result = 0;
            if (pileGeoType == PileTypeManaged.SqaurePile)
            {
                double[] _b_d = { 4, 6, 8, 10, 12 };
                double[] _m2 = { 1.21, 1.08, 1.06, 1.03, 1 };
                var _linearSolver = LinearSpline.InterpolateSorted(_b_d, _m2);
                _result = _linearSolver.Interpolate(B_D);
            }
            else
            {
                double[] _b_d = { 3, 7, 10, 15 };
                double[] _m2 = { 1.83, 1.25, 1.15, 1 };
                var _linearSolver = LinearSpline.InterpolateSorted(_b_d, _m2);
                _result = _linearSolver.Interpolate(B_D);
            }

            if (_result < 1)
                _result = 1;
            return _result;
        }

        public static double CalculateIncliningAffectCoeff(double alpha, PileTypeManaged pileGeoType)
        {
            double _result = 1;
            if (pileGeoType == PileTypeManaged.SqaurePile)
            {
                double[] _alpha = { 0, 10, 20, 30, 45 };
                double[] _m3 = { 1, 0.67, 0.67, 0.71, 0.75 };
                var _linerSolver = Interpolate.Linear(_alpha, _m3);
                _result = _linerSolver.Interpolate(alpha);
            }
            return _result;
        }
    }

    /// <summary>
    /// Formulas for calculating Wave force imposing pile
    /// </summary>
    public static class WaveForce
    {
        public static double CalculateWaveLength(double wavePeriod, double waterDepth, double gravitationalAcceleration = 9.8)
        {
            return FindRoots.OfFunction(L =>
           {
               return L - gravitationalAcceleration * wavePeriod * wavePeriod / (2 * Math.PI) * Math.Tanh(2 * Math.PI * waterDepth / L);
           }, 0, 1000);
        }

        public static double CalculatePDMax(
            double CD,
            double gamma,
            double D,
            double H,
            double K1) =>
            CD * gamma * D * H * H / 2 * K1;

        public static double CalculatePIMax(
            double CM,
            double gamma,
            double A,
            double H,
            double K2) =>
            CM * gamma * A * H / 2 * K2;

        public static double CalculateMDMax(
            double CD,
            double gamma,
            double D,
            double H,
            double L,
            double K3) =>
            CD * gamma * D * H * H * L / (2 * Math.PI) * K3;

        public static double CalculateMIMax(
            double CM,
            double gamma,
            double A,
            double H,
            double L,
            double K4) =>
            CM * gamma * A * H * L / (4 * Math.PI) * K4;

        public static void CalculateFinalCompOfWaveForce(
            double diameter,
            double L,
            double H1,
            double waterDepth,
            double alpha,
            double beta,
            double gammaP,
            double gammaM,
            double PDMax,
            double MDMax,
            double PIMax,
            double MIMax,
            out double PDMax_Final,
            out double MDMax_Final,
            out double PIMax_Final,
            out double MIMax_Final)
        {
            if(diameter/L <= 0.2)
            {
                if((H1/waterDepth <= 0.2 && waterDepth/L >= 0.2) ||
                    (H1/waterDepth >0.2 && waterDepth/L >= 0.35))
                {
                    PDMax_Final = PDMax;
                    MDMax_Final = MDMax;
                    PIMax_Final = PDMax;
                    MIMax_Final = MIMax;
                }
                else
                {
                    PDMax_Final = alpha * PDMax;
                    MDMax_Final = beta * MDMax;
                    if(0.04 <= waterDepth /L && waterDepth / L <= 0.2)
                    {
                        PIMax_Final = gammaP * PIMax;
                        MIMax_Final = gammaM * MIMax;
                    }
                    else
                    {
                        PIMax_Final = PIMax;
                        MIMax_Final = MIMax;
                    }
                }
            }
            else
            {
                //if(H1 / waterDepth <= 0.1)
                //{
                //    PIMax_Final = PIMax;
                //    MIMax_Final = MIMax;
                //    PDMax_Final = PDMax;
                //    MDMax_Final = MDMax;
                //}
                //else
                //{

                //}
                //PDMax_Final = MDMax_Final = PIMax_Final = MIMax_Final = 0;
                PIMax_Final = PIMax;
                MIMax_Final = MIMax;
                PDMax_Final = PDMax;
                MDMax_Final = MDMax;
                //throw new NotImplementedException("Big Scale calculation algorithm hasn't been added!");
                //Todo: Need to complement big scale pile calculation process.
            }
        }

        public static void CalculateMaxWaveAndMoment(
            double diameter,
            double L,
            double H1,
            double d,
            double pileSpan,
            double PDMax_Final,
            double MDMax_FInal,
            double PIMax_Final, 
            double MIMax_Final,
            out double PMax,
            out double MMax,
            out double omgaT/*angel*/)
        {
            if(diameter / L  <= 0.2)
            {
                if(PDMax_Final <= PIMax_Final / 2)
                {
                    PMax = PIMax_Final;
                    MMax = MIMax_Final;
                    omgaT = 270;
                }
                else
                {
                    PMax = PDMax_Final * (1 + Math.Pow(PIMax_Final / 2 / PDMax_Final, 2));
                    MMax = MDMax_FInal * (1 + Math.Pow(MIMax_Final / 2 / MDMax_FInal, 2));
                    omgaT = Math.Asin(-0.5 * PIMax_Final / PDMax_Final) * 180 / Math.PI;
                }
                if(pileSpan < 4*diameter)
                {
                    double _K = CalculateK(pileSpan, diameter);
                    PMax *= _K;
                    MMax *= _K;
                }
            }
            else
            {
                if (H1 / d < 0.1)
                {
                    PMax = PIMax_Final;
                    MMax = MIMax_Final;
                    omgaT = double.NaN;
                }
                else
                {
                    if(H1/ d >= 0.1 && diameter / d >=0.4)
                    {
                        double _alphaP = 0;
                        double _alphaM = 0;
                        PMax = _alphaP * PIMax_Final;
                        MMax = _alphaM * MIMax_Final;
                        omgaT = double.NaN;
                    }
                    else
                    {

                    }
                    PMax = MMax = omgaT = 0;
                    throw new NotImplementedException("Big Scale calculation algorithm hasn't been added!");
                    //Todo: To complement big scale pile calculation algorithm.
                }
            }
        }


        public static double CalculateK1(
            double Z1,
            double Z2,
            double L,
            double d) =>
            (4 * Math.PI * Z2 / L - 4 * Math.PI * Z1 / L + Math.Sinh(4 * Math.PI * Z2 / L)
            - Math.Sinh(4 * Math.PI * Z1 / L)) / (8 * Math.Sinh(4 * Math.PI * d / L));

        public static double CalculateK2(
            double Z1,
            double Z2,
            double L,
            double d) =>
            (Math.Sinh(2 * Math.PI * Z2 / L) - Math.Sinh(2 * Math.PI * Z1 / L)) / Math.Cosh(2 * Math.PI * d / L);

        public static double CalculateK3(
            double Z1,
            double Z2,
            double L,
            double d) =>
            (Math.Pow(Math.PI * (Z2 - Z1) / (2 * L), 2) + Math.PI * (Z2 - Z1) / 8 / L * Math.Sinh(4 * Math.PI * Z2 / L) -
            (Math.Cosh(4 * Math.PI * Z2 / L) - Math.Cosh(4 * Math.PI * Z1 / L)) / 32) / Math.Sinh(4 * Math.PI * d / L);

        public static double CalculateK4(
            double Z1,
            double Z2,
            double L,
            double d) =>
            (2 * Math.PI * (Z2 - Z1) / L * Math.Sinh(2 * Math.PI * Z2 / L) - (Math.Cosh(2 * Math.PI * Z2 / L) - Math.Cosh(2 * Math.PI * Z1 / L))) /
            Math.Cosh(2 * Math.PI * d / L);

        public static double CalculatePu(
            double gamma,
            double H,
            double Diameter,
            double Z,
            double L,
            double d,
            double f2,
            double f0,
            double omgat) =>
            gamma * H * Math.Pow(Diameter, 2) / 4 * Math.Cosh(2 * Math.PI * Z / L) / Math.Cosh(2 * Math.PI * d / L) *
            (f2 * Math.Sin(omgat * Math.PI / 180) - f0 * Math.Cos(omgat * Math.PI / 180));

        public static double CalculateMu(
            double gamma,
            double H,
            double Diameter,
            double Z,
            double L,
            double d,
            double f3,
            double f1,
            double omgat) =>
            gamma * H * Math.Pow(Diameter, 3) / 32 * Math.Cosh(2 * Math.PI * Z / L) / Math.Cosh(2 * Math.PI * d / L) *
            (f3 * Math.Sin(omgat * Math.PI / 180) + f1 * Math.Cos(omgat * Math.PI / 180));


        private static void GetXYDataFromDatabase(string databasename, string tablename, out List<double> xData, out List<double> yData)
        {
            xData = new List<double>();
            yData = new List<double>();
#if DEBUG
            var _dbFilePath = @"D:\softwarefile\Bentley\Microstation\Configuration\Organization\Data\";
#else
            var _dbFilePath = BD.ConfigurationManager.GetVariable("PDIWT_ORGANIZATION_DATABASEPATH", BD.ConfigurationVariableLevel.Organization);
#endif
            using (SQLiteConnection _conn = new SQLiteConnection(string.Format($"Data Source={_dbFilePath}{databasename};Version=3;")))
            {
                _conn.Open();
                using (SQLiteCommand _cmd = new SQLiteCommand())
                {
                    _cmd.Connection = _conn;
                    _cmd.CommandText = string.Format($"select * from {tablename}");
                    using (SQLiteDataReader _dr = _cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection))
                    {
                        while (_dr.Read())
                        {
                            xData.Add(_dr.GetDouble(1));
                            yData.Add(_dr.GetDouble(2));
                        }
                    }
                }
            }
        }

        private static readonly string _databasename = "CodeOfHydrologyForHarbourAndWaterway.db";

        private static double CalculateInterploateFromDB(double value, string tablename)
        {
            List<double> _xdata, _ydata;
            GetXYDataFromDatabase(_databasename, tablename, out _xdata, out _ydata);
            return Interpolate.Linear(_xdata, _ydata).Interpolate(value);
        }

        public static double CalculateYitaMax(double H1, double d,  double L, double diameter, double relativeperiod)
        {
            if(diameter / L <= 0.2 )
                return CalculateInterploateFromDB(H1 / d, "Yitamax") * H1;
            else
            {
                double _r = diameter / 2;
                if(H1/d >= 0.1 && diameter / d >= 0.4)
                {

                    if( relativeperiod >= 8)
                    {
                        var rp = new List<double>();
                        var C1 = new List<double>();
                        var C2 = new List<double>();
                        var C3 = new List<double>();
                        var alpha = new List<double>();
                        var beta = new List<double>();
#if DEBUG
                        var _dbFilePath = @"D:\softwarefile\Bentley\Microstation\Configuration\Organization\Data\";
#else
                        var _dbFilePath = BD.ConfigurationManager.GetVariable("PDIWT_ORGANIZATION_DATABASEPATH", BD.ConfigurationVariableLevel.Organization);
#endif
                        using (SQLiteConnection _conn = new SQLiteConnection(string.Format($"Data Source={_dbFilePath}{_databasename};Version=3;")))
                        {
                            _conn.Open();
                            using (SQLiteCommand _cmd = new SQLiteCommand())
                            {
                                _cmd.Connection = _conn;
                                _cmd.CommandText = string.Format(@"select * from RelativePeriod");
                                using (SQLiteDataReader _dr = _cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection))
                                {
                                    while (_dr.Read())
                                    {
                                        rp.Add(_dr.GetDouble(1));
                                        C1.Add(_dr.GetDouble(2));
                                        C2.Add(_dr.GetDouble(2));
                                        C3.Add(_dr.GetDouble(2));
                                        alpha.Add(_dr.GetDouble(2));
                                        beta.Add(_dr.GetDouble(2));
                                    }
                                }
                            }
                        }
                        double _c1 = Interpolate.Linear(rp, C1).Interpolate(relativeperiod);
                        double _c2 = Interpolate.Linear(rp, C2).Interpolate(relativeperiod);
                        double _c3 = Interpolate.Linear(rp, C3).Interpolate(relativeperiod);
                        double _alpha = Interpolate.Linear(rp, alpha).Interpolate(relativeperiod);
                        double _beta = Interpolate.Linear(rp, beta).Interpolate(relativeperiod);
                        return H1 * (_c1 - _c2 * Math.Exp(-_alpha * _r / d)) * (1 + _c3 * Math.Pow(H1 / d - 0.1, _beta));
                    }
                    else
                    {
                        return CalculateInterploateFromDB(H1 / d, "Yitamax") * H1;
                    }
                }
                else
                {
                    return CalculateInterploateFromDB(H1 / d, "Yitamax") * H1;
                }
            }
        }

        public static double CalculateReltiavePeriod(double period, double d, double gravitationalAcceleration = 9.8)
        {
            return period * Math.Sqrt(gravitationalAcceleration / d); 
        }

        public static double CalculateAlpha(double H1, double d, double L)
        {
            List<double> _Hd = new List<double> { 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7 };
            List<double> _results = new List<double>(7);
            for (int i = 0; i < 7; i++)
            {
                _results.Add(CalculateInterploateFromDB(d / L, "alpha_hd_0_" + (i + 1).ToString()));
            }
            return Interpolate.Linear(_Hd, _results).Interpolate(H1 / d);
        }

        public static double CalculateBeta(double H1, double d, double L)
        {
            List<double> _Hd = new List<double> { 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7 };
            List<double> _results = new List<double>(7);
            for (int i = 0; i < 7; i++)
            {
                _results.Add(CalculateInterploateFromDB(d / L, "beta_hd_0_" + (i + 1).ToString()));
            }
            return Interpolate.Linear(_Hd, _results).Interpolate(H1 / d);
        }

        public static double CalculateOmgaT(double H1, double d, double L)
        {
            List<double> _Hd = new List<double> { 0.1, 0.15, 0.2, 0.25, 0.3, 0.35, 0.4, 0.45, 0.5 };
            List<double> _results = new List<double>();
            foreach (var _hd in _Hd)
            {
                _results.Add(CalculateInterploateFromDB(d / L, "omga_dL_0_" + (_hd * 100).ToString()));
            }
            return Interpolate.Linear(_Hd, _results).Interpolate(H1 / d);
        }


        public static double CalculateGammaP(double d, double L)
        {
            double _result = CalculateInterploateFromDB(d / L, "GammaP");
            if (_result < 1) _result = 1;
            return _result;
        }
        public static double CalculateGammaM(double d, double L)
        {
            double _result = CalculateInterploateFromDB(d / L, "GammaM");
            if (_result < 1) _result = 1;
            return _result;
        }
        public static double Calculatef0(double Diamter, double L)
        {
            return CalculateInterploateFromDB(Diamter / L, "f0");
        }
        public static double Calculatef1(double Diamter, double L)
        {
            return CalculateInterploateFromDB(Diamter / L, "f1");
        }
        public static double Calculatef2(double Diamter, double L)
        {
            return CalculateInterploateFromDB(Diamter / L, "f2");
        }
        public static double Calculatef3(double Diamter, double L)
        {
            return CalculateInterploateFromDB(Diamter / L, "f3");
        }

        public static double CalculateK(double l, double Diameter)
        {
            double[] _xdata = new double[] { 2, 3, 4 };
            double[] _ydata = new double[] { 1.5, 1.2, 1.1 };
            double _result = Interpolate.Linear(_xdata, _ydata).Interpolate(l / Diameter);
            if (l / Diameter > 4 || _result < 1)
                _result = 1;
            return _result;
        }
        public static double CalculateCM(double Diameter, double L,PileTypeManaged pileType)
        {
            if(Diameter / L <= 0.2)
            {
                if (pileType == PileTypeManaged.SqaurePile)
                    return 1.5;
                else
                    return 2.0;
            }
            else
            {
                return CalculateInterploateFromDB(Diameter / L, "CM");
            }
        }
        public static double CalculateCD(PileTypeManaged pileType)
        {
            if (pileType == PileTypeManaged.SqaurePile)
                return 2.0;
            else
                return 1.2;
        }
    }
}
