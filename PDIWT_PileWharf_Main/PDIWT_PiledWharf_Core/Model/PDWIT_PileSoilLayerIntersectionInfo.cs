using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;

using BD = Bentley.DgnPlatformNET;
using BM = Bentley.MstnPlatformNET;
using BDEC = Bentley.DgnPlatformNET.DgnEC;
using BES = Bentley.ECObjects.Schema;
using BEI = Bentley.ECObjects.Instance;
using BDE = Bentley.DgnPlatformNET.Elements;
using BMW = Bentley.MstnPlatformNET.WPF;
using BG = Bentley.GeometryNET;
using System.Collections.ObjectModel;

namespace PDIWT_PiledWharf_Core.Model
{
    public class PileSoilLayersIntersectionGetter  : ObservableObject
    {
        public PileSoilLayersIntersectionGetter (
            IPile pile,
            SoilLayerCollection soilLayers)
        {
            _pile = pile;
            _soilLayers = soilLayers;
        }

        private IPile _pile;
        /// <summary>
        /// Property Description
        /// </summary>
        public IPile Pile
        {
            get { return _pile; }
            set { Set(ref _pile, value); }
        }

        private SoilLayerCollection _soilLayers;
        /// <summary>
        /// The Soil layers you want to test with given pile
        /// </summary>
        public SoilLayerCollection SoilLayers
        {
            get { return _soilLayers; }
            set { Set(ref _soilLayers, value); }
        }
        
        /// <summary>
        /// Get Given pile and SoilLayer Intersection Information. the outcome is sorted collection.
        /// </summary>
        /// <param name="Infos">inter section information</param>
        /// <param name="isWithinAxisRayRange">Is only interested in range of pile length</param>
        /// <returns>return status</returns>
        public PileSoilLayersInsectionStatus GetInterSectionInfo(out ObservableCollection<IntersectionInfo> Infos, bool isWithinAxisRayRange = true)
        {
            Infos = new ObservableCollection<IntersectionInfo>();

            if (SoilLayers == null || SoilLayers.Count == 0)
                return PileSoilLayersInsectionStatus.NoSoilLayer;

            foreach (var _layer in SoilLayers)
                if (null == _layer.Mesh) return PileSoilLayersInsectionStatus.NotAllSoilLayersContainMeshElement;

            foreach (var _layer in SoilLayers)
            {
                BG.DPoint3d _facetPoint;
                double _fractionInAxis;
                BG.PolyfaceVisitor _polyfaceVisitor = BG.PolyfaceVisitor.Attach(_layer.Mesh.AsMeshEdit().GetMeshData(), true);
                List<double> _insertFractions = new List<double>();
                var _axisRay = Pile.GetPileRay3D();
                //todo Sometimes it will count more than two points, ask the problem on the forum
                while (_polyfaceVisitor.AdvanceToFacetBySearchRay(_axisRay, 0, out _facetPoint, out _fractionInAxis))
                {
                    _insertFractions.Add(_fractionInAxis);
                }
                _insertFractions = _insertFractions.Distinct().ToList();

                double _topFraction, _bottomFraction;

                if (_insertFractions.Count == 0 || _insertFractions.Count == 1)
                    continue;
                else
                {
                    if (_insertFractions.Count > 2)
                        _insertFractions = _insertFractions.GetRange(0, 2);
                    _topFraction = _insertFractions.Min();
                    _bottomFraction = _insertFractions.Max();
                }

                if (_topFraction < 0 && _bottomFraction < 0)
                    continue;

                if (isWithinAxisRayRange)
                {
                    if (_topFraction > 1 && _bottomFraction > 1)
                        continue;
                    else if ((_topFraction - 1) * (_bottomFraction - 1) < 0)
                    {
                        _bottomFraction = 1;
                    }
                }

                IntersectionInfo _info = new IntersectionInfo(_topFraction, _bottomFraction, Pile, _layer);
                Infos.Add(_info);
            }

            if (0 == Infos.Count)
                return PileSoilLayersInsectionStatus.NoIntersection;
            else
            {
                Infos = new ObservableCollection<IntersectionInfo>(Infos.OrderBy(_info => _info.TopFraction));
                return PileSoilLayersInsectionStatus.Success;
            }
        }
    }


    /// <summary>
    /// Object containing intersection information
    /// </summary>
    public class IntersectionInfo : ObservableObject
    {
        public IntersectionInfo(double topFraction, double bottomFraction, IPile pile, SoilLayer soilLayer)
        {
            _topFraction = topFraction;
            _bottomFraction = bottomFraction;
            _pile = pile;
            _soilLayer = soilLayer;
        }

        private double _topFraction;
        /// <summary>
        /// unit: uor
        /// </summary>
        public double TopFraction
        {
            get { return _topFraction; }
            set { Set(ref _topFraction, value); }
        }

        private double _bottomFraction;
        /// <summary>
        /// unit: uor
        /// </summary>
        public double BottomFraction
        {
            get { return _bottomFraction; }
            set { Set(ref _bottomFraction, value); }
        }


        private IPile _pile;
        /// <summary>
        /// The pile object which is used to calculate the intersection point
        /// </summary>
        public IPile Pile
        {
            get { return _pile; }
            set { Set(ref _pile, value); }
        }

        private SoilLayer _soilLayer;
        /// <summary>
        /// The soil layer object which insects with given pile
        /// </summary>
        public SoilLayer SoilLayer
        {
            get { return _soilLayer; }
            set { Set(ref _soilLayer, value); }
        }

        public BG.DPoint3d GetTopPoint()
        {
            BG.DRay3d _axisRay = Pile.GetPileRay3D();
            return _axisRay.PointAtFraction(TopFraction);
        }

        public BG.DPoint3d GetBottomPoint()
        {
            BG.DRay3d _axisRay = Pile.GetPileRay3D();
            return _axisRay.PointAtFraction(BottomFraction);
        }

        /// <summary>
        /// Get the given pile length in this soil layer. Length unit: uor
        /// </summary>
        /// <returns></returns>
        public double GetPileLengthInSoilLayer()
        {
            if (Pile == null || SoilLayer == null) return 0;
            return new BG.DSegment3d(GetTopPoint(), GetBottomPoint()).Length;
        }
    }
}