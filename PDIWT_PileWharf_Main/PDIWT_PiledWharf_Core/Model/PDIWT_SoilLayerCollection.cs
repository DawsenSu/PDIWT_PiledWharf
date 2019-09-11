using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;

using GalaSoft.MvvmLight;

using BD = Bentley.DgnPlatformNET;
using BM = Bentley.MstnPlatformNET;
using BDEC = Bentley.DgnPlatformNET.DgnEC;
using BES = Bentley.ECObjects.Schema;
using BEI = Bentley.ECObjects.Instance;
using BDE = Bentley.DgnPlatformNET.Elements;
using BMW = Bentley.MstnPlatformNET.WPF;
using BG = Bentley.GeometryNET;
using PDIWT_PiledWharf_Core.Model.Tools;
using PDIWT_PiledWharf_Core.ViewModel;

namespace PDIWT_PiledWharf_Core.Model
{
    public class SoilLayerCollection : ObservableCollection<SoilLayer>
    {
        public SoilLayerCollection() { }
        public SoilLayerCollection(IEnumerable<SoilLayer> soilLayers)
        {
            foreach (var _layer in soilLayers)
            {
                Add(_layer);
            }
        }
        /// <summary>
        /// Construct soillayercollection from given model, which can be used to build up soil layer library.
        /// </summary>
        /// <param name="dgnModel"></param>
        /// <returns></returns>
        public static SoilLayerCollection ObtainFromModel(BD.DgnModel dgnModel)
        {
            try
            {
                SoilLayerCollection _soilLayers = new SoilLayerCollection();

                BD.ScanCriteria _sc = new BD.ScanCriteria();
                _sc.SetModelRef(dgnModel);
                _sc.SetModelSections(BD.DgnModelSections.GraphicElements);
                BD.BitMask _meshBitMask = new BD.BitMask(false);
                _meshBitMask.Capacity = 400;
                _meshBitMask.ClearAll();
                _meshBitMask.SetBit(104, true);
                _sc.SetElementTypeTest(_meshBitMask);
                _sc.Scan((_element, _model) =>
                {
                    var _layerInfo = new PDIWT_BearingCapacity_SoilLayerInfo();
                    BDE.MeshHeaderElement _mesh = (BDE.MeshHeaderElement)_element;
                    SoilLayer _soilLayer = SoilLayer.ObtainInfoFromMesh(_mesh);
                    if (_soilLayer != null)
                        _soilLayers.Add(_soilLayer);
                    return BD.StatusInt.Success;
                });
                _soilLayers = new SoilLayerCollection(_soilLayers.OrderBy(_layer => _layer.Number));
                return _soilLayers;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Can't build soil layers from given model {dgnModel.ModelName}", e);
            }

        }
    }
}
