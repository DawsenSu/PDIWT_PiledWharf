using System;
using System.Collections;
using System.Collections.Generic;
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
using System.Linq;

namespace PDIWT_PiledWharf_Core.Model
{
    using Tools;


    [Obsolete]
    public class PDIWT_SoilLayerInfoReader
    {
        /// <summary>
        /// Obtain the Soil information from mesh element in active model. 
        /// The mesh element which is attached BearingCapacitySoilLayerInfo ECSchmea could be considered as valid soil layer element.
        /// </summary>
        /// <param name="tupleInfo">Get the soil layers info overall from active model. The item1 is mesh element itself, item2 is the dictionary which has 
        /// property and property's value pair stored in it.</param>
        /// <returns>if Success, obtain valid information.if error, not.</returns>
        public static BD.StatusInt ObtainSoilLayerInfoFromModel(out List<Tuple<BDE.MeshHeaderElement, Dictionary<string, object>>> tupleInfo)
        {
            var _activeModel = BM.Session.Instance.GetActiveDgnModel();
            var _activedgnfile = BM.Session.Instance.GetActiveDgnFile();
            tupleInfo = null;
            var _tupleInfo = new List<Tuple<BDE.MeshHeaderElement, Dictionary<string, object>>>(); // local variable to hold information in lambda expression.
            string _ecSchemaName = "PDIWT";
            string _ecClassName = "BearingCapacitySoilLayerInfo";
            List<string> _requiredSoilLayerProps = new List<string>()
            {
                "LayerNumber",
                "LayerName",
                "Betasi",
                "Psisi",
                "qfi",
                "Betap",
                "Psip",
                "qr"
            };

            //determine if the active model contains ECSchmea
            var _ecmgr = BDEC.DgnECManager.Manager;
            if (!_ecmgr.DiscoverSchemas(_activedgnfile, BDEC.ReferencedModelScopeOption.All, false).Contains(_ecSchemaName + ".01.00"))
                return BD.StatusInt.Error;

            // Scan the mesh element
            BD.ScanCriteria _sc = new BD.ScanCriteria();
            _sc.SetModelRef(_activeModel);
            _sc.SetModelSections(BD.DgnModelSections.GraphicElements);
            BD.BitMask _meshBitMask = new BD.BitMask(false);
            _meshBitMask.Capacity = 400;
            _meshBitMask.ClearAll();
            _meshBitMask.SetBit(104, true);
            _sc.SetElementTypeTest(_meshBitMask);
            _sc.Scan((_mesh, _model) =>
            {
                if (BD.StatusInt.Error == ECSChemaReader.ReadECInstanceProperties(_ecSchemaName, _ecClassName, _requiredSoilLayerProps, _mesh, out Dictionary<string, object>  _soilLayerProps) ||
                    _soilLayerProps.Values.Contains(null))
                    return BD.StatusInt.Error;
                _tupleInfo.Add(new Tuple<BDE.MeshHeaderElement, Dictionary<string, object>>((BDE.MeshHeaderElement)_mesh, _soilLayerProps));
                return BD.StatusInt.Success;
            });

            if (_tupleInfo.Count == 0)
                return BD.StatusInt.Error;
            tupleInfo = _tupleInfo;

            return BD.StatusInt.Success;
        }


    }
}
