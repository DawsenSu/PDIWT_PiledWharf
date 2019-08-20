using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

using PDIWT.Resources.Localization.MainModule;
using GalaSoft.MvvmLight.Messaging;

namespace PDIWT_PiledWharf_Core.Model.Tools
{
    using PDIWT_PiledWharf_Core.ViewModel;

    using BD = Bentley.DgnPlatformNET;
    using BM = Bentley.MstnPlatformNET;
    using BDEC = Bentley.DgnPlatformNET.DgnEC;
    using BES = Bentley.ECObjects.Schema;
    using BEI = Bentley.ECObjects.Instance;
    using BDE = Bentley.DgnPlatformNET.Elements;
    using BMW = Bentley.MstnPlatformNET.WPF;
    using BG = Bentley.GeometryNET;
    using System.Collections.ObjectModel;

    public class LoadPileParametersTool<TResult> : BD.DgnElementSetTool where TResult : new()
    {
        public LoadPileParametersTool(ObtainInfoFromPileAndEnviroment<TResult> _method) : base()
        {
            _obtainMethod = _method;
        }

        protected override bool OnDataButton(BD.DgnButtonEvent ev)
        {
            BD.HitPath _hitPath = DoLocate(ev, true, 1);
            if (_hitPath == null || _hitPath.GetHeadElement() == null)
                return false;
            BDE.CellHeaderElement _pileCell = _hitPath.GetHeadElement() as BDE.CellHeaderElement;
            //var _pileInfo = new PDIWT_CurrentForePileInfo();
            //if (BD.StatusInt.Success == BuildUpPileInfo(_pileCell, out _pileInfo))
            //    Messenger.Default.Send(new NotificationMessage<PDIWT_CurrentForePileInfo>(_pileInfo, Resources.Success));
            //else
            //    Messenger.Default.Send(new NotificationMessage<PDIWT_CurrentForePileInfo>(_pileInfo, Resources.Error));
            TResult _pileInfo = new TResult();
            if (BD.StatusInt.Success == _obtainMethod(_pileCell, out _pileInfo))
                Messenger.Default.Send(new NotificationMessage<TResult>(_pileInfo, Resources.Success));
            else
                Messenger.Default.Send(new NotificationMessage<TResult>(_pileInfo, Resources.Error));
            ExitTool();
            return IsSingleShot();
        }

        protected override bool OnPostLocate(BD.HitPath path, out string cantAcceptReason)
        {
            if (!base.OnPostLocate(path, out cantAcceptReason))
                return false;

            cantAcceptReason = "It's not pile";
            BDE.Element _ele = path.GetHeadElement();
            if (_ele.ElementType == BD.MSElementType.CellHeader && ((BDE.CellHeaderElement)_ele).CellName == "Pile")
                return true;
            return false; ;
        }

        protected override bool IsSingleShot()
        {
            return true;
        }

        public override BD.StatusInt OnElementModify(BDE.Element element)
        {
            return BD.StatusInt.Error;
        }

        protected override bool OnResetButton(BD.DgnButtonEvent ev)
        {
            ExitTool();
            return true;
        }

        protected override void OnRestartTool()
        {
            InstallNewInstance(_obtainMethod);
        }


        protected override void OnCleanup()
        {
            base.OnCleanup();
        }

        protected override void OnPostInstall()
        {
            _mc.StatusCommand = "Pick up one Pile in the view";
            base.OnPostInstall();
        }

        public static void InstallNewInstance(ObtainInfoFromPileAndEnviroment<TResult> method)
        {
            var _tool = new LoadPileParametersTool<TResult>(method);
            _tool.InstallTool();
        }


        BM.MessageCenter _mc = BM.MessageCenter.Instance;
        ObtainInfoFromPileAndEnviroment<TResult> _obtainMethod;

        //private BD.StatusInt BuildUpPileInfo(BDE.CellHeaderElement pile, out PDIWT_CurrentForePileInfo pileInfo)
        //{
        //    pileInfo = new PDIWT_CurrentForePileInfo();

        //    var _dgnECManager = BDEC.DgnECManager.Manager;
        //    var _activeModelRef = BM.Session.Instance.GetActiveDgnModelRef();

        //    string _environmentECSchemaName = "PDIWT";
        //    string _materialECClassName = "PileMaterialSettings";
        //    Dictionary<string, object> _materialProps;
        //    List<string> _requireMaterialPropNameList = new List<string>()
        //    {
        //        "WaterDensity"
        //    };
        //    if (BD.StatusInt.Error == ESChemaReader.ReadECInstanceProperties(_environmentECSchemaName, _materialECClassName, _requireMaterialPropNameList, _activeModelRef, out _materialProps))
        //        return BD.StatusInt.Error;
        //    pileInfo.WaterDensity = double.Parse(_materialProps["WaterDensity"].ToString());

        //    string _waterLevelECClassName = "WaterLevelSettings";
        //    Dictionary<string, object> _waterLevelProps;
        //    List<string> _requireWaterLevelPropNameList = new List<string>()
        //    {
        //        "HAT"
        //    };
        //    if (BD.StatusInt.Error == ESChemaReader.ReadECInstanceProperties(_environmentECSchemaName, _waterLevelECClassName, _requireWaterLevelPropNameList, _activeModelRef, out _waterLevelProps))
        //        return BD.StatusInt.Error;
        //    pileInfo.HAT = double.Parse(_waterLevelProps["HAT"].ToString());

        //    string _ifcECSchemaName = "IfcPort";
        //    string _ifcPileECClassName = "IfcPile";

        //    Dictionary<string, object> _pileProps;
        //    List<string> _requirePilePropNameList = new List<string>()
        //    {
        //        "TopElevation",
        //        "Type",
        //        "CrossSectionWidth",
        //        "OutsideDiameter"
        //    };
        //    if (BD.StatusInt.Error == ESChemaReader.ReadECInstanceProperties(_ifcECSchemaName, _ifcPileECClassName, _requirePilePropNameList, pile, out _pileProps))
        //        return BD.StatusInt.Error;
        //    pileInfo.TopElevation = double.Parse(_pileProps["TopElevation"].ToString());
        //    pileInfo.Shape = _pileProps["Type"].ToString();
        //    //pileInfo.ProjectedWidth = double.Parse(_pileProps["CrossSectionWidth"].ToString());
        //    if (pileInfo.Shape == Resources.SquarePile)
        //        pileInfo.ProjectedWidth = double.Parse(_pileProps["CrossSectionWidth"].ToString());
        //    else
        //        pileInfo.ProjectedWidth = double.Parse(_pileProps["OutsideDiameter"].ToString());

        //    return BD.StatusInt.Success;

        //    ////Get Environment parameter - Water 
        //    //var _ativeModelRef = BM.Session.Instance.GetActiveDgnModelRef();
        //    //BES.IECSchema _pdiwtSchema = _dgnECManager.LocateSchemaInScope(BDEC.FindInstancesScope.CreateScope(_ativeModelRef, new BDEC.FindInstancesScopeOption(BDEC.DgnECHostType.Model, false)),
        //    //                                                               _environmentECSchemaName,1,0, BES.SchemaMatchType.Exact);
        //    //if (_pdiwtSchema == null)
        //    //    return BD.StatusInt.Error;
        //    //BES.IECClass _pileMaterialClass = _pdiwtSchema.GetClass(_materialECClassName);
        //    //if (_pileMaterialClass == null)
        //    //    return BD.StatusInt.Error;
        //    //var _scope = BDEC.FindInstancesScope.CreateScope(_ativeModelRef, new BDEC.FindInstancesScopeOption(BDEC.DgnECHostType.Model, false));
        //    //var _query = new Bentley.EC.Persistence.Query.ECQuery(_pileMaterialClass);
        //    //_query.SelectClause.SelectAllProperties = true;
        //    //var _materialSettingECInstances = _dgnECManager.FindInstances(_scope, _query);
        //    //foreach (var _eci in _materialSettingECInstances)
        //    //{
        //    //    pileInfo.WaterDensity = _eci.GetPropertyValue("WaterDensity").DoubleValue;
        //    //}

        //    //BES.IECClass _waterLevelClass = _pdiwtSchema.GetClass(_waterLevelECClassName);
        //    //if (_waterLevelClass == null)
        //    //    return BD.StatusInt.Error;
        //    //_query = new Bentley.EC.Persistence.Query.ECQuery(_waterLevelClass);
        //    //_query.SelectClause.SelectAllProperties = true;
        //    //var _waterLevelInstances = _dgnECManager.FindInstances(_scope, _query);
        //    //foreach (var _eci in _waterLevelInstances)
        //    //{
        //    //    pileInfo.HAT = _eci.GetPropertyValue("HAT").DoubleValue;
        //    //}
        //    //// Obtain Pile relate info 
        //    //BES.IECSchema _ifcSchema = _dgnECManager.LocateSchemaInScope(BDEC.FindInstancesScope.CreateScope(_ativeModelRef, new BDEC.FindInstancesScopeOption(BDEC.DgnECHostType.Element, false)),
        //    //                                                               _ifcECSchemaName, 1, 0, BES.SchemaMatchType.Exact);
        //    //if (_ifcSchema == null)
        //    //    return BD.StatusInt.Error;
        //    //BES.IECClass _ifcPileClass = _ifcSchema.GetClass(_ifcPileECClassName);
        //    //if (_ifcPileClass == null)
        //    //    return BD.StatusInt.Error;
        //    //_scope = BDEC.FindInstancesScope.CreateScope(pile, new BDEC.FindInstancesScopeOption(BDEC.DgnECHostType.Element, false));
        //    //_query = new Bentley.EC.Persistence.Query.ECQuery(_ifcPileClass);
        //    //_query.SelectClause.SelectAllProperties = true;
        //    //var _pileInfoInstances = _dgnECManager.FindInstances(_scope, _query);
        //    //foreach (var _eci in _pileInfoInstances)
        //    //{
        //    //    pileInfo.TopElevation = _eci.GetPropertyValue("TopElevation").DoubleValue;
        //    //    pileInfo.Shape = _eci.GetPropertyValue("Type").StringValue;
        //    //    if (pileInfo.Shape == Resources.SquarePile)
        //    //        pileInfo.ProjectedWidth = _eci.GetPropertyValue("CrossSectionWidth").DoubleValue;
        //    //    else
        //    //        pileInfo.ProjectedWidth = _eci.GetPropertyValue("OutsideDiameter").DoubleValue;
        //    //    break;
        //    //}

        //}
    }


    public static class ECSChemaReader
    {
        /// <summary>
        /// Read all list prop from host element
        /// </summary>
        /// <param name="ecshemaname">the schema name</param>
        /// <param name="ecclassname">the class name</param>
        /// <param name="ecpropertynames">the list contains the names of property for which user wants to look</param>
        /// <param name="hostElement">can be Element and ModelRef</param>
        /// <param name="result">Dictionary contains the <propertyname, value> pairs, if not find value equals null</param>
        /// <returns></returns>
        public static BD.StatusInt ReadECInstanceProperties(
            string ecshemaname,
            string ecclassname,
            IEnumerable<string> ecpropertynames,
            object hostElement,
            out Dictionary<string, object> result)
        {
            result = new Dictionary<string, object>();
            foreach (var _prop in ecpropertynames)
            {
                result.Add(_prop, null);
            }

            var _dgnECManager = BDEC.DgnECManager.Manager;
            BES.IECSchema _ecSchema = null;
            if (hostElement is BD.DgnModelRef)
            {
                _ecSchema = _dgnECManager.LocateSchemaInScope(BDEC.FindInstancesScope.CreateScope((BD.DgnModelRef)hostElement, new BDEC.FindInstancesScopeOption(BDEC.DgnECHostType.All, false)),
                                                               ecshemaname, 1, 0, BES.SchemaMatchType.Exact);
            }
            else if (hostElement is BDE.Element)
            {
                _ecSchema = _dgnECManager.LocateSchemaInScope(BDEC.FindInstancesScope.CreateScope(((BDE.Element)hostElement).DgnModelRef, new BDEC.FindInstancesScopeOption(BDEC.DgnECHostType.All, false)),
                                               ecshemaname, 1, 0, BES.SchemaMatchType.Exact);
            }

            if (_ecSchema == null)
                return BD.StatusInt.Error;
            BES.IECClass _ecClass = _ecSchema.GetClass(ecclassname);
            if (_ecClass == null)
                return BD.StatusInt.Error;

            BDEC.FindInstancesScope _scope = null;
            if (hostElement is BD.DgnModelRef)
            {
                _scope = BDEC.FindInstancesScope.CreateScope((BD.DgnModelRef)hostElement, new BDEC.FindInstancesScopeOption(BDEC.DgnECHostType.Model, false));
            }
            else if (hostElement is BDE.Element)
            {
                _scope = BDEC.FindInstancesScope.CreateScope((BDE.Element)hostElement, new BDEC.FindInstancesScopeOption(BDEC.DgnECHostType.Element, false));
            }

            if (_scope == null)
            {
                return BD.StatusInt.Error;
            }

            var _query = new Bentley.EC.Persistence.Query.ECQuery(_ecClass);
            _query.SelectClause.SelectAllProperties = true;

            var _ecInstances = _dgnECManager.FindInstances(_scope, _query);
            foreach (var _prop in ecpropertynames)
            {
                foreach (var _eci in _ecInstances)
                {
                    foreach (var _pr in _eci)
                    {
                        if (_pr.Property.Name == _prop)
                        {
                            object _result;
                            if (_pr.TryGetNativeValue(out _result))
                                result[_prop] = _result;
                        }
                    }

                    //if (_eci.GetPropertyValue(_prop).IsNull)
                    //    result[_prop] = string.Empty;
                    //else
                    //    result[_prop] = _eci.GetPropertyValue(_prop).NativeValue;
                }
            }
            

            return BD.StatusInt.Success;
        }
    }

    public static class LoadParametersToolEnablerProvider
    {
        public static BD.StatusInt CurrentForceInfoEnabler(BDE.CellHeaderElement pile, out PDIWT_CurrentForePileInfo pileInfo)
        {
            pileInfo = new PDIWT_CurrentForePileInfo();

            var _activeModelRef = BM.Session.Instance.GetActiveDgnModelRef();

            string _environmentECSchemaName = "PDIWT";
            string _materialECClassName = "PileMaterialSettings";
            Dictionary<string, object> _materialProps;
            List<string> _requireMaterialPropNameList = new List<string>()
            {
                "WaterUnitWeight"
            };
            if (BD.StatusInt.Error == ECSChemaReader.ReadECInstanceProperties(_environmentECSchemaName, _materialECClassName, _requireMaterialPropNameList, _activeModelRef, out _materialProps))
                return BD.StatusInt.Error;
            pileInfo.WaterDensity = double.Parse(_materialProps["WaterUnitWeight"].ToString());

            string _waterLevelECClassName = "WaterLevelSettings";
            Dictionary<string, object> _waterLevelProps;
            List<string> _requireWaterLevelPropNameList = new List<string>()
            {
                "HAT"
            };
            if (BD.StatusInt.Error == ECSChemaReader.ReadECInstanceProperties(_environmentECSchemaName, _waterLevelECClassName, _requireWaterLevelPropNameList, _activeModelRef, out _waterLevelProps))
                return BD.StatusInt.Error;
            pileInfo.HAT = double.Parse(_waterLevelProps["HAT"].ToString());

            string _ifcECSchemaName = "IfcPort";
            string _ifcPileECClassName = "IfcPile";

            Dictionary<string, object> _pileProps;
            List<string> _requirePilePropNameList = new List<string>()
            {
                "TopElevation",
                "Type",
                "CrossSectionWidth",
                "OutsideDiameter"
            };
            if (BD.StatusInt.Error == ECSChemaReader.ReadECInstanceProperties(_ifcECSchemaName, _ifcPileECClassName, _requirePilePropNameList, pile, out _pileProps))
                return BD.StatusInt.Error;
            pileInfo.TopElevation = double.Parse(_pileProps["TopElevation"].ToString());
            pileInfo.Shape = _pileProps["Type"].ToString();
            //pileInfo.ProjectedWidth = double.Parse(_pileProps["CrossSectionWidth"].ToString());
            if (pileInfo.Shape == Resources.SquarePile)
                pileInfo.ProjectedWidth = double.Parse(_pileProps["CrossSectionWidth"].ToString());
            else
                pileInfo.ProjectedWidth = double.Parse(_pileProps["OutsideDiameter"].ToString());

            return BD.StatusInt.Success;
        }

        public static BD.StatusInt WaveForceInfoEnabler(BDE.CellHeaderElement pile, out PDIWT_WaveForcePileInfo pileInfo)
        {
            pileInfo = new PDIWT_WaveForcePileInfo();
            var _activeModelRef = BM.Session.Instance.GetActiveDgnModelRef();

            string _environmentECSchemaName = "PDIWT";
            string _materialECClassName = "PileMaterialSettings";
            Dictionary<string, object> _materialProps;
            List<string> _requireMaterialPropNameList = new List<string>()
            {
                "WaterUnitWeight"
            };
            if (BD.StatusInt.Error == ECSChemaReader.ReadECInstanceProperties(_environmentECSchemaName, _materialECClassName, _requireMaterialPropNameList, _activeModelRef, out _materialProps))
                return BD.StatusInt.Error;
            pileInfo.WaterDensity = double.Parse(_materialProps["WaterUnitWeight"].ToString());

            string _waterLevelECClassName = "WaterLevelSettings";
            Dictionary<string, object> _waterLevelProps;
            List<string> _requiredWaterLevelPropNamtList = new List<string>() { "HAT", "MHW", "MLW", "LAT" };
            if (BD.StatusInt.Error ==
                ECSChemaReader.ReadECInstanceProperties(_environmentECSchemaName, _waterLevelECClassName, _requiredWaterLevelPropNamtList, _activeModelRef, out _waterLevelProps))
                return BD.StatusInt.Error;
            pileInfo.HAT = double.Parse(_waterLevelProps["HAT"].ToString());
            pileInfo.MHW = double.Parse(_waterLevelProps["MHW"].ToString());
            pileInfo.MLW = double.Parse(_waterLevelProps["MLW"].ToString());
            pileInfo.LAT = double.Parse(_waterLevelProps["LAT"].ToString());

            string _waveECClassName = "WaveSettings";
            Dictionary<string, object> _waveProps;
            List<string> _requiredWavePropNameList = new List<string>();
            foreach (var _waterLevel in _requiredWaterLevelPropNamtList)
            {
                _requiredWavePropNameList.Add($"WaveHeight_{_waterLevel}");
                _requiredWavePropNameList.Add($"WavePeriod_{_waterLevel}");
            }
            if (BD.StatusInt.Error ==
                ECSChemaReader.ReadECInstanceProperties(_environmentECSchemaName, _waveECClassName, _requiredWavePropNameList, _activeModelRef, out _waveProps))
                return BD.StatusInt.Error;
            foreach (var _prop in _waveProps)
            {
                if (_prop.Key.StartsWith("WaveHeight"))
                    pileInfo.WaveHeight.Add(double.Parse(_prop.Value.ToString()));
                else
                    pileInfo.WavePeriod.Add(double.Parse(_prop.Value.ToString()));
            }

            string _ifcECSchemaName = "IfcPort";
            string _ifcPileECClassName = "IfcPile";
            Dictionary<string, object> _pileProps;
            List<string> _requirePilePropNameList = new List<string>()
            {
                "Type",
                "CrossSectionWidth",
                "OutsideDiameter"
            };
            if (BD.StatusInt.Error == ECSChemaReader.ReadECInstanceProperties(_ifcECSchemaName, _ifcPileECClassName, _requirePilePropNameList, pile, out _pileProps))
                return BD.StatusInt.Error;
            pileInfo.Shape = _pileProps["Type"].ToString();
            if (pileInfo.Shape == Resources.SquarePile)
                pileInfo.PileDiameter = double.Parse(_pileProps["CrossSectionWidth"].ToString());
            else
                pileInfo.PileDiameter = double.Parse(_pileProps["OutsideDiameter"].ToString());

            return BD.StatusInt.Success;
        }

        public static BD.StatusInt AxialBearingCapacityPileInfoEnabler(BDE.CellHeaderElement pile, out PDIWT_BearingCapacity_PileInfo pileInfo)
        {
            pileInfo = new PDIWT_BearingCapacity_PileInfo();            

            var _activeModel = BM.Session.Instance.GetActiveDgnModel();
            double _uorpermeter = _activeModel.GetModelInfo().UorPerMeter;
            var _mc = BM.MessageCenter.Instance;

            BDE.LineElement _axisLine = pile.GetChildren().Where(_line => _line is BDE.LineElement).First() as BDE.LineElement;
            BG.DPoint3d _lineStartPoint, _lineEndPoint;
            if (!_axisLine.AsCurvePathEdit().GetCurveVector().GetStartEnd(out _lineStartPoint, out _lineEndPoint))
            {
                _mc.ShowErrorMessage("The Pile doesn't contain axis line Elements", "", BM.MessageAlert.Balloon);
                return BD.StatusInt.Error;
            }
            //Swap start and end point if necessary, start point => top point, end point => bottom point;
            if(_lineEndPoint.Z > _lineStartPoint.Z)
            {
                BG.DPoint3d _temp = _lineEndPoint;
                _lineEndPoint = _lineStartPoint;
                _lineStartPoint = _temp;
            }
            BG.DRay3d _axisRay = new BG.DRay3d(_lineStartPoint, _lineEndPoint);

            //Pile Material and Pile Self Related Parameters
            string _environmentECSchemaName = "PDIWT";
            string _materialECClassName = "PileMaterialSettings";
            Dictionary<string, object> _materialProps;
            List<string> _requireMaterialPropNameList = new List<string>()
            {
                "ConcreteUnitWeight",
                "ConcreteUnderWaterUnitWeight",
                "SteelUnitWeight",
                "WaterUnitWeight"
            };
            if (BD.StatusInt.Error == ECSChemaReader.ReadECInstanceProperties(_environmentECSchemaName, _materialECClassName, _requireMaterialPropNameList, _activeModel, out _materialProps))
                return BD.StatusInt.Error;
            pileInfo.ConcreteWeight = double.Parse(_materialProps["ConcreteUnitWeight"].ToString());
            pileInfo.ConcreteUnderwaterWeight = double.Parse(_materialProps["ConcreteUnderWaterUnitWeight"].ToString());
            pileInfo.SteelWeight = double.Parse(_materialProps["SteelUnitWeight"].ToString());
            pileInfo.SteelUnderwaterWeight = double.Parse(_materialProps["SteelUnitWeight"].ToString()) - double.Parse(_materialProps["WaterUnitWeight"].ToString());

            string _ifcECSchemaName = "IfcPort";
            string _ifcPileECClassName = "IfcPile";
            Dictionary<string, object> _pileProps;
            List<string> _requirePilePropNameList = new List<string>()
            {
                "Code",
                "Type",
                "CrossSectionWidth",
                "OutsideDiameter",
                "InnerDiameter",
                "Length"
            };
            if (BD.StatusInt.Error == ECSChemaReader.ReadECInstanceProperties(_ifcECSchemaName, _ifcPileECClassName, _requirePilePropNameList, pile, out _pileProps))
                return BD.StatusInt.Error;
            pileInfo.SelectedPileGeoType = PDIWT.Resources.PDIWT_Helper.GetEnumDescriptionDictionary<PDIWT_PiledWharf_Core_Cpp.PileTypeManaged>()
                                                                       .Where(e => e.Value == _pileProps["Type"].ToString())
                                                                       .First().Key;
            switch (pileInfo.SelectedPileGeoType)
            {
                case PDIWT_PiledWharf_Core_Cpp.PileTypeManaged.SqaurePile:
                    pileInfo.PileDiameter = double.Parse(_pileProps["CrossSectionWidth"].ToString()) / 1000;
                    break;
                case PDIWT_PiledWharf_Core_Cpp.PileTypeManaged.TubePile:
                    pileInfo.PileDiameter = double.Parse(_pileProps["OutsideDiameter"].ToString()) / 1000;
                    break;
                case PDIWT_PiledWharf_Core_Cpp.PileTypeManaged.PHCTubePile:
                    pileInfo.PileDiameter = double.Parse(_pileProps["OutsideDiameter"].ToString()) / 1000;
                    pileInfo.PileInsideDiameter = double.Parse(_pileProps["InnerDiameter"].ToString()) / 1000;
                    break;
                case PDIWT_PiledWharf_Core_Cpp.PileTypeManaged.SteelTubePile:
                    pileInfo.PileDiameter = double.Parse(_pileProps["OutsideDiameter"].ToString()) / 1000;
                    pileInfo.PileInsideDiameter = double.Parse(_pileProps["InnerDiameter"].ToString()) / 1000;
                    break;
                default:
                    break;
            }

            pileInfo.PileLength = double.Parse(_pileProps["Length"].ToString()) / 1000;
            pileInfo.PileCode = _pileProps["Code"].ToString();

            BG.DVector3d _pileAxisVector = new BG.DVector3d(_lineStartPoint, _lineEndPoint);
            if(_pileAxisVector.IsParallelOrOppositeTo(BG.DVector3d.UnitZ))
            {
                pileInfo.IsVerticalPile = true;
                pileInfo.PileSkewness = double.NaN;
            }
            else
            {
                pileInfo.IsVerticalPile = false;
                pileInfo.PileSkewness = Math.Abs(_pileAxisVector.Z / Math.Sqrt(_pileAxisVector.X * _pileAxisVector.X + _pileAxisVector.Y * _pileAxisVector.Y));
            }
            pileInfo.PileTopElevation = _lineStartPoint.Z / _uorpermeter;

            List<BDE.MeshHeaderElement> _meshHeaderElements = new List<BDE.MeshHeaderElement>();
            BD.ScanCriteria _sc = new BD.ScanCriteria();
            _sc.SetModelRef(_activeModel);
            _sc.SetModelSections(BD.DgnModelSections.GraphicElements);
            BD.BitMask _meshBitMask = new BD.BitMask(false);
            _meshBitMask.Capacity = 400;
            _meshBitMask.ClearAll();
            _meshBitMask.SetBit(104,true);
            _sc.SetElementTypeTest(_meshBitMask);
            _sc.Scan((_element, _model) =>
            {
                _meshHeaderElements.Add((BDE.MeshHeaderElement)_element);
                return BD.StatusInt.Success;
            });
            if(_meshHeaderElements.Count == 0)
            {
                _mc.ShowErrorMessage("The active Model doesn't contain any mesh elements", "", BM.MessageAlert.Balloon);
                return BD.StatusInt.Error;
            }

            ObservableCollection<PDIWT_BearingCapacity_SoilLayerInfo> _soilLayers = new ObservableCollection<PDIWT_BearingCapacity_SoilLayerInfo>();
            foreach (var _mesh in _meshHeaderElements)
            {
                BG.PolyfaceVisitor _polyfaceVisitor = BG.PolyfaceVisitor.Attach(_mesh.AsMeshEdit().GetMeshData(), true);
                BG.DPoint3d _factPoint;
                double _fractionInAxis;
                List<BG.DPoint3d> _interSectionPoints = new List<BG.DPoint3d>();
                while(_polyfaceVisitor.AdvanceToFacetBySearchRay(_axisRay, 1e-2, out _factPoint, out _fractionInAxis))
                {
                    if (_fractionInAxis >= 0 & _fractionInAxis <= 1)
                        _interSectionPoints.Add(_factPoint);
                }
                _interSectionPoints = _interSectionPoints.Distinct().ToList();
                if (_interSectionPoints.Count == 0)
                    continue;
                else if(_interSectionPoints.Count == 1) // another point is considered to be bottom point of pile
                {
                    //! There could be potential problem about this logical.
                    //! Sometimes it can't locate the mesh.
                    var _soilInfo = new PDIWT_BearingCapacity_SoilLayerInfo()
                    {
                        SoilLayerName = "None",
                        SoilLayerNumber = "UnKonw",
                        SoilLayerTopElevation = _interSectionPoints[0].Z / _uorpermeter,
                        SoilLayerBottomElevation = _lineEndPoint.Z / _uorpermeter,
                        SoilLayerThickness = (new BG.DSegment3d(_interSectionPoints[0], _lineEndPoint)).Length / _uorpermeter
                    };
                    GetSoilLayerInfo(_soilInfo, _mesh);
                    _soilLayers.Add(_soilInfo);


                }
                else if(_interSectionPoints.Count == 2)
                {
                    _interSectionPoints.OrderByDescending(_point => _point.Z);
                    var _soilInfo = new PDIWT_BearingCapacity_SoilLayerInfo()
                    {
                        SoilLayerName = "None",
                        SoilLayerNumber = "UnKonw",
                        SoilLayerTopElevation = _interSectionPoints[0].Z / _uorpermeter,
                        SoilLayerBottomElevation = _interSectionPoints[1].Z / _uorpermeter,
                        SoilLayerThickness = (new BG.DSegment3d(_interSectionPoints[0], _interSectionPoints[1])).Length / _uorpermeter
                    };
                    GetSoilLayerInfo(_soilInfo, _mesh);
                    _soilLayers.Add(_soilInfo);
                }
                else
                {
                    _mc.ShowInfoMessage("The pile has more than two intersection point with soil layer", "", false);
                }
            }
            
            if(_soilLayers.Count == 0)
            {
                _mc.ShowInfoMessage("There is no intersection between selected pile and existing soil layer",$"The pile axis is {_lineStartPoint} to {_lineEndPoint}", false);
                return BD.StatusInt.Error;
            }

            _soilLayers = new ObservableCollection<PDIWT_BearingCapacity_SoilLayerInfo>(_soilLayers.OrderByDescending(_layer => _layer.SoilLayerTopElevation));
            pileInfo.PileSoilLayersInfo = _soilLayers;

            return BD.StatusInt.Success;
        }
        /// <summary>
        /// Get soil info from mesh element
        /// </summary>
        /// <param name="soilInfo">The soil info to get</param>
        /// <param name="mesh">the ecisntance attached element</param>
        private static void GetSoilLayerInfo(PDIWT_BearingCapacity_SoilLayerInfo soilInfo, BDE.MeshHeaderElement mesh)
        {
            string _environmentECSchemaName = "PDIWT";
            string _materialECClassName = "BearingCapacitySoilLayerInfo";
            Dictionary<string, object> _soilProps;
            List<string> _requireMaterialPropNameList = new List<string>()
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
            if (BD.StatusInt.Error == ECSChemaReader.ReadECInstanceProperties(_environmentECSchemaName, _materialECClassName, _requireMaterialPropNameList,
                mesh, out _soilProps) || _soilProps.Values.Contains(null))
                return;
            soilInfo.SoilLayerNumber = _soilProps["LayerNumber"].ToString();
            soilInfo.SoilLayerName = _soilProps["LayerName"].ToString();
            soilInfo.Betasi = double.Parse(_soilProps["Betasi"].ToString());
            soilInfo.Psii = double.Parse(_soilProps["Psisi"].ToString());
            soilInfo.SideFrictionStandardValue = double.Parse(_soilProps["qfi"].ToString());
            soilInfo.Betap = double.Parse(_soilProps["Betap"].ToString());
            soilInfo.Psip = double.Parse(_soilProps["Psip"].ToString());
            soilInfo.EndResistanceStandardValue = double.Parse(_soilProps["qr"].ToString());
        }

    }

    public delegate BD.StatusInt ObtainInfoFromPileAndEnviroment<TInfoClass>(BDE.CellHeaderElement pilecell, out TInfoClass infoClass);

    public class PDIWT_CurrentForePileInfo
    {
        public double TopElevation { get; set; }
        public double HAT { get; set; }
        public double SoilElevation { get; set; }
        public double ProjectedWidth { get; set; }
        public string Shape { get; set; }
        public double WaterDensity { get; set; }
    }

    public class PDIWT_WaveForcePileInfo
    {
        public string Shape { get; set; }
        public double PileDiameter { get; set; }
        public double HAT { get; set; }
        public double MHW { get; set; }
        public double MLW { get; set; }
        public double LAT { get; set; }
        public double WaterDensity { get; set; }
        public List<double> WaveHeight { get; set; } = new List<double>();
        public List<double> WavePeriod { get; set; } = new List<double>();
    }


}
