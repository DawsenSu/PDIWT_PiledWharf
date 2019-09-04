using MathNet.Numerics;
using NUnit.Framework;
using PDIWT.Formulas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PDIWT.Formulas.Tests
{
    [TestFixture()]
    public class AxialBearingCapacityTests
    {
        double _pileLength, _pileTopElevation, _pileSkewness;
        List<double> _qfi, _li, _psii, _betasi;

        double _U, _qr, _A, _psip, _betap, _gammaR;
        double _outerDiameter;

        [SetUp]
        public void Init()
        {
            _gammaR = 1.5;
            _pileLength = 40;
            _pileTopElevation = 2.71;
            _pileSkewness = double.NaN;

            _qfi = new List<double> { 33, 64, 42, 32, 60, 90, 120 };
            _li = new List<double> { 3.10, 3.00, 4.40, 3.00, 3.00, 3.00, 2.70 };
            _psii = new List<double> { 0.7, 0.7, 0.7, 0.7, 0.7, 0.7, 0.5 };

            _outerDiameter = 0.65;
            //var _shape = pi;
            //_U = AxialBearingCapacity.CalculatePilePrimeter(_shape, _outerDiameter);
            //_qr = 6100;
            //_A = AxialBearingCapacity.CalculatePileEndOutsideArea(_shape, _outerDiameter);

        }

        [Test()]
        public void CalculateCosAlphaTest()
        {
            double _skewness = 7;
            Assert.AreEqual(1, AxialBearingCapacity.CalculateCosAlpha(double.NaN));
            Assert.Catch(typeof(InvalidParameterException), () => AxialBearingCapacity.CalculateCosAlpha(0));
            Assert.AreEqual(Math.Cos( Math.Atan(1 / _skewness)), AxialBearingCapacity.CalculateCosAlpha(_skewness),1e-5);
        }

        [Test()]
        public void CalculatePileBottomElevationTest()
        {
            Assert.AreEqual(-37.29, AxialBearingCapacity.CalculatePileBottomElevation(_pileTopElevation, _pileLength, _pileSkewness));
        }

        [Test()]
        public void DrivenPileBearingCapacityTest()
        {
            //Assert.AreEqual(4057, AxialBearingCapacity.DrivenPileBearingCapacity(_gammaR, _qfi, _li, _U, _qr, _A),1);
        }

    }
}