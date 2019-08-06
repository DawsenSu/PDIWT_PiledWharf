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
namespace PDIWT.Formulas
{
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

        public static double CalculatedHorizontalAffectCoeff(double B_D, ShapeInfo shape)
        {
            double _result = 0;
            if (shape.Value == 1)
            {
                double[] _b_d = { 3, 7, 10, 15 };
                double[] _m2 = { 1.83, 1.25, 1.15, 1 };
                var _linearSolver = LinearSpline.InterpolateSorted(_b_d, _m2);
                _result = _linearSolver.Interpolate(B_D);
            }
            else
            {
                double[] _b_d = { 4, 6, 8, 10, 12 };
                double[] _m2 = { 1.21, 1.08, 1.06, 1.03, 1 };
                var _linearSolver = LinearSpline.InterpolateSorted(_b_d, _m2);
                _result = _linearSolver.Interpolate(B_D);
            }

            if (_result < 1)
                _result = 1;
            return _result;
        }

        public static double CalculateIncliningAffectCoeff(double alpha, ShapeInfo shape)
        {
            double _result = 1;
            if (shape.Value == 2)
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

        public static double CalculateMaxVelocityComponentOfWaveForce(
            double alpha,
            double CD,
            double gamma,
            double D,
            double H,
            double K1) =>
            alpha * CD * gamma * D * H * H / 2 * K1;

        public static double CalculateMaxInertiaComponentOfWaveForce(
            double gammap,
            double CM,
            double gamma,
            double A,
            double H,
            double K2) =>
            gammap * CM * gamma * A * H / 2 * K2;

        public static double CalculateMaxVelocityComponentOfWaveMonment(
            double beta,
            double Cp,
            double gamma,
            double D,
            double H,
            double L,
            double K3) =>
            beta * Cp * gamma * D * H * H * L / (2 * Math.PI) * K3;

        public static double CalculateMaxInertiaComponentOfWaveMonment(
            double gammaM,
            double CM,
            double gamma,
            double A,
            double H,
            double L,
            double K4) =>
            gammaM * CM * gamma * A * H * L / (4 * Math.PI) * K4;

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

        public static double CalculateYitaMax(double H1, double d)
        {
            return CalculateInterploateFromDB(H1 / d, "Yitamax") * H1;
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
        public static double CalcuateCM(double Diameter, double L)
        {
            return CalculateInterploateFromDB(Diameter / L, "CM");
        }
        public static double CalculateCD(ShapeInfo shapeInfo)
        {
            if (shapeInfo.Value == 1)
                return 1.2;
            else
                return 2.0;
        }
    }
}
