using NUnit.Framework;
using PDIWT.Formulas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BG = Bentley.GeometryNET;

namespace PDIWT.Formulas.Tests
{
    [TestFixture]
    public class WaveForceTests
    {
        double _diameter, _l, _topelevation, _bottomelevation, _waterlevel, _H1, _H13, _T, _Waterdepth, _tolerance;



        [SetUp]
        public void Init()
        {
            _diameter = 11.5;
            _l = 26;
            _topelevation = 10;
            _bottomelevation = -20.5;
            _waterlevel = 5.52;
            _H1 = 4.2;
            _H13 = 3.10;
            _T = 11.2;
            _Waterdepth = 26.02;
            _tolerance = 0.01;
        }

        [Test()]
        public void CalculateWaveLengthTest()
        {
            Assert.AreEqual(153.89, WaveForce.CalculateWaveLength(_T, _Waterdepth), _tolerance);
        }

        [Test()]
        public void GetYitaMaxTest()
        {
            double _wavelength = WaveForce.CalculateWaveLength(_T, _Waterdepth);
            Assert.AreEqual(2.52, WaveForce.CalculateYitaMax(_H1, _Waterdepth, _wavelength,_diameter,0), 0.05);
        }
        [Test()]
        public void CalculateAlphaTest()
        {
            double _wavelength = WaveForce.CalculateWaveLength(_T, _Waterdepth);
            Assert.AreEqual(1.19, WaveForce.CalculateAlpha(_H1, _Waterdepth, _wavelength),_tolerance + 0.1);
        }
        [Test()]
        public void CalculateBetaTest()
        {
            double _wavelength = WaveForce.CalculateWaveLength(_T, _Waterdepth);
            Assert.AreEqual(1.2, WaveForce.CalculateBeta(_H1, _Waterdepth, _wavelength),_tolerance +0.1);
        }

        [Test()]
        public void CalculateGammaPTest()
        {
            Assert.AreEqual(1, WaveForce.CalculateGammaP(_Waterdepth, WaveForce.CalculateWaveLength(_T, _waterlevel)), _tolerance);
            Assert.AreEqual(1.02, WaveForce.CalculateGammaP(0.15, 1), 0.05);
            Assert.AreEqual(1.24, WaveForce.CalculateGammaP(0.1, 1), 0.05);
            Assert.AreEqual(2.12, WaveForce.CalculateGammaP(0.05, 1), 0.05);
        }
        [Test()]
        public void CalculateGammaMTest()
        {
            Assert.AreEqual(1, WaveForce.CalculateGammaM(_Waterdepth, WaveForce.CalculateWaveLength(_T, _waterlevel)), _tolerance);
        }

        [Test()]
        public void CalculateKTest()
        {
            Assert.AreEqual(1.42, WaveForce.CalculateK(_l,_diameter), _tolerance);
        }
        [Test()]
        public void Calculatef0_f3Test()
        {
            double _wavelength = WaveForce.CalculateWaveLength(_T, _Waterdepth);

            Assert.AreEqual(-0.1, WaveForce.Calculatef0(_diameter,_wavelength), _tolerance +0.1);
            Assert.AreEqual(0.8, WaveForce.Calculatef1(_diameter,_wavelength), _tolerance + 0.1);
            Assert.AreEqual(1.55, WaveForce.Calculatef2(_diameter,_wavelength), _tolerance + 0.1);
            Assert.AreEqual(0.05, WaveForce.Calculatef3(_diameter,_wavelength), _tolerance + 0.1);
        }

        [Test]
        public void CalculateK1_K4()
        {
            double _wavelength = 153.89;
            double _z1 = 0;
            double _z2_0 = 28.54;
            double _z2_90 = 26.44;
            //double _gammaP = 1;
            //double _gammaM = 1;
            //double _gamma = 10.25;
            Assert.AreEqual(0.2249, WaveForce.CalculateK1(_z1, _z2_0, _wavelength, _Waterdepth), _tolerance);
            Assert.AreEqual(0.8038, WaveForce.CalculateK2(_z1, _z2_90, _wavelength, _Waterdepth), _tolerance);
            Assert.AreEqual(0.0787, WaveForce.CalculateK3(_z1, _z2_0, _wavelength, _Waterdepth), _tolerance);
            Assert.AreEqual(0.4716, WaveForce.CalculateK4(_z1, _z2_90, _wavelength, _Waterdepth), _tolerance);
        }
        [Test]
        public void CalculatePu_Mu()
        {
            double _wavelength = 153.89;

            double f0 = -0.08;
            double f1 = 0.68;
            double f2 = 1.55;
            double f3 = 0.05;
            double omgat = 17;
            Assert.AreEqual(465.552, WaveForce.CalculatePu(10.25, _H1, _diameter, 0, _wavelength, _Waterdepth, f2, f0, omgat),1);
            Assert.AreEqual(840.08, WaveForce.CalculateMu(10.25, _H1, _diameter, 0, _wavelength, _Waterdepth, f3, f1, omgat),1);
        }

        [Test]
        public void TestVector()
        {
            BG.DVector3d _dVector3d = BG.DVector3d.FromXYZ(0, 0, -1);
            BG.DVector3d _pileVector3d = BG.DVector3d.FromXYZ(1, 0, 0);
            BG.Angle _angle = _pileVector3d.AngleTo(_dVector3d);
            BG.Angle _singedangle = _pileVector3d.SignedAngleTo(_dVector3d, BG.DVector3d.FromXYZ(1, -1, 0));
            Assert.AreEqual(90, _angle.Degrees);
            Assert.AreEqual(-90, _singedangle.Degrees);
        }

        [Test]
        public void TestDSegment3d()
        {
            BG.DSegment3d _first = new BG.DSegment3d(0, 0, 0, 1, 0, 0);
            BG.DSegment3d _second = new BG.DSegment3d(0.5, 0, 1, 1, 0, 1);
            BG.DSegment3d.ClosestApproachSegment(_first, _second, out BG.DSegment3d _mind, out double _firstfraction, out double _secondfraction);
            Assert.AreEqual(1,_mind.Length);
        }

    }
}