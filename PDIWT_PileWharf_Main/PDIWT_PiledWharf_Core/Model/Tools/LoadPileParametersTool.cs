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
    using BD = Bentley.DgnPlatformNET;
    using BM = Bentley.MstnPlatformNET;
    using BDEC = Bentley.DgnPlatformNET.DgnEC;
    using BES = Bentley.ECObjects.Schema;
    using BEI = Bentley.ECObjects.Instance;
    using BDE = Bentley.DgnPlatformNET.Elements;
    using BMW = Bentley.MstnPlatformNET.WPF;
    using BG = Bentley.GeometryNET;
    public class LoadPileParametersTool : BD.DgnElementSetTool
    {
        public LoadPileParametersTool() :base()
        {

        }

        protected override bool OnDataButton(BD.DgnButtonEvent ev)
        {
            BD.HitPath _hitPath = DoLocate(ev, true, 1);
            if(_hitPath != null && _hitPath.GetHeadElement().ElementType == BD.MSElementType.CellHeader)
            {
                BDE.CellHeaderElement _pileCell = _hitPath.GetHeadElement() as BDE.CellHeaderElement;
                if (_pileCell.CellName != "Pile")
                {
                    Messenger.Default.Send(new NotificationMessage(Resources.Error));
                    return true;
                }
                var _pileInfo = new PDIWT_CurrentForePileInfo();
                if(BD.StatusInt.Success == BuildUpPileInfo(_pileCell, out _pileInfo))
                    Messenger.Default.Send(new NotificationMessage<PDIWT_CurrentForePileInfo>(_pileInfo, Resources.Success));
                else
                    Messenger.Default.Send(new NotificationMessage<PDIWT_CurrentForePileInfo>(_pileInfo, Resources.Error));
            }
            ExitTool();
            return IsSingleShot();
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
            InstallNewInstance();
        }


        protected override void OnCleanup()
        {
            base.OnCleanup();
        }

        protected override void OnPostInstall()
        {
            _mc.StatusCommand = "Pick up one of Pile in the view";
            base.OnPostInstall();
        }
       
        public static void InstallNewInstance()
        {
            var _tool = new LoadPileParametersTool();
            _tool.InstallTool();
        }


        BM.MessageCenter _mc = BM.MessageCenter.Instance;


        private BD.StatusInt BuildUpPileInfo(BDE.CellHeaderElement pile, out PDIWT_CurrentForePileInfo pileInfo)
        {
            pileInfo = new PDIWT_CurrentForePileInfo();

            var _dgnECManager = BDEC.DgnECManager.Manager;
            string _ifcECSchemaName = "IfcPort";
            string _ifcPileECClassName = "IfcPile";
            string _environmentECSchemaName = "PDIWT";
            string _environmentECClassName = "PileMaterialSettings";
            string _waterLevelECClassName = "WaterLevelSettings";

            //Get Environment parameter - Water 
            var _ativeModelRef = BM.Session.Instance.GetActiveDgnModelRef();
            BES.IECSchema _pdiwtSchema = _dgnECManager.LocateSchemaInScope(BDEC.FindInstancesScope.CreateScope(_ativeModelRef, new BDEC.FindInstancesScopeOption(BDEC.DgnECHostType.Model, false)),
                                                                           _environmentECSchemaName,1,0, BES.SchemaMatchType.Exact);
            if (_pdiwtSchema == null)
                return BD.StatusInt.Error;
            BES.IECClass _pileMaterialClass = _pdiwtSchema.GetClass(_environmentECClassName);
            if (_pileMaterialClass == null)
                return BD.StatusInt.Error;
            var _scope = BDEC.FindInstancesScope.CreateScope(_ativeModelRef, new BDEC.FindInstancesScopeOption(BDEC.DgnECHostType.Model, false));
            var _query = new Bentley.EC.Persistence.Query.ECQuery(_pileMaterialClass);
            _query.SelectClause.SelectAllProperties = true;
            var _materialSettingECInstances = _dgnECManager.FindInstances(_scope, _query);
            foreach (var _eci in _materialSettingECInstances)
            {
                pileInfo.WaterDensity = _eci.GetPropertyValue("WaterDensity").DoubleValue;
            }

            BES.IECClass _waterLevelClass = _pdiwtSchema.GetClass(_waterLevelECClassName);
            if (_waterLevelClass == null)
                return BD.StatusInt.Error;
            _query = new Bentley.EC.Persistence.Query.ECQuery(_waterLevelClass);
            _query.SelectClause.SelectAllProperties = true;
            var _waterLevelInstances = _dgnECManager.FindInstances(_scope, _query);
            foreach (var _eci in _waterLevelInstances)
            {
                pileInfo.HAT = _eci.GetPropertyValue("HAT").DoubleValue;
            }
            // Obtain Pile relate info 
            BES.IECSchema _ifcSchema = _dgnECManager.LocateSchemaInScope(BDEC.FindInstancesScope.CreateScope(_ativeModelRef, new BDEC.FindInstancesScopeOption(BDEC.DgnECHostType.Element, false)),
                                                                           _ifcECSchemaName, 1, 0, BES.SchemaMatchType.Exact);
            if (_ifcSchema == null)
                return BD.StatusInt.Error;
            BES.IECClass _ifcPileClass = _ifcSchema.GetClass(_ifcPileECClassName);
            if (_ifcPileClass == null)
                return BD.StatusInt.Error;
            _scope = BDEC.FindInstancesScope.CreateScope(pile, new BDEC.FindInstancesScopeOption(BDEC.DgnECHostType.Element, false));
            _query = new Bentley.EC.Persistence.Query.ECQuery(_ifcPileClass);
            _query.SelectClause.SelectAllProperties = true;
            var _pileInfoInstances = _dgnECManager.FindInstances(_scope, _query);
            foreach (var _eci in _pileInfoInstances)
            {
                pileInfo.TopElevation = _eci.GetPropertyValue("TopElevation").DoubleValue;
                pileInfo.Shape = _eci.GetPropertyValue("Type").StringValue;
                if (pileInfo.Shape == Resources.SquarePile)
                    pileInfo.ProjectedWidth = _eci.GetPropertyValue("CrossSectionWidth").DoubleValue;
                else
                    pileInfo.ProjectedWidth = _eci.GetPropertyValue("OutsideDiameter").DoubleValue;
                break;
            }
            return BD.StatusInt.Success;
        }
    }

    public class PDIWT_CurrentForePileInfo
    {
        public double TopElevation { get; set; }
        public double HAT { get; set; }
        public double SoilElevation { get; set; }
        public double ProjectedWidth { get; set; }
        public string Shape { get; set; }
        public double WaterDensity { get; set; }

    }
}
