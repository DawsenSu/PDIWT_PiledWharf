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
    using System.Windows.Media.Imaging;
    //Todo Need to add the judgment which determine if this mesh has been attached BearingCapacityECInstance.
    public class AttachBCInstanceTool : BD.DgnElementSetTool
    {

        public AttachBCInstanceTool(BM.AddIn addIn) : base()
        {
            _addIn = addIn;
        }

        protected override bool OnDataButton(BD.DgnButtonEvent ev)
        {
            BD.HitPath _hitPath = DoLocate(ev, true, 1);
            if (_hitPath == null || _hitPath.GetHeadElement() == null)
                return false;

            BDE.MeshHeaderElement _mesh = _hitPath.GetHeadElement() as BDE.MeshHeaderElement;

            if( SetMeshInstance(_mesh) == BD.StatusInt.Error)
            {
                _mc.ShowErrorMessage("Fail to create instance on mesh " + _mesh.ElementId, "", false);
                return false;
            }
            else
            {
                _mc.ShowInfoMessage("Attach ECInstance Successfully On Selected mesh " + _mesh.ElementId, "", false);
                return true;
            }
        }

        protected override bool OnResetButton(BD.DgnButtonEvent ev)
        {
            ExitTool();
            return true;
        }

        public override BD.StatusInt OnElementModify(BDE.Element element)
        {
            return BD.StatusInt.Success;
        }

        protected override void OnRestartTool()
        {
            InstallNewInstance();
        }
        protected override bool OnPostLocate(BD.HitPath path, out string cantAcceptReason)
        {
            if (!base.OnPostLocate(path, out cantAcceptReason))
                return false;

            BDE.Element _ele = path.GetHeadElement();
            if (_ele.ElementType == BD.MSElementType.MeshHeader)
            {
                string _ecSchemaName = "PDIWT";
                string _ecClassName = "BearingCapacitySoilLayerInfo";

                var _ecMgr = BDEC.DgnECManager.Manager;
                BES.IECSchema _eCSchema = _ecMgr.LocateSchemaInScope(BDEC.FindInstancesScope.CreateScope(BM.Session.Instance.GetActiveDgnFile(), new BDEC.FindInstancesScopeOption(BDEC.DgnECHostType.File, false)),
                                          _ecSchemaName, 1, 0, BES.SchemaMatchType.Latest);
                if (_eCSchema == null)
                {
                    cantAcceptReason = $"Can't find {_ecSchemaName} ECSchema";
                    return false;
                }
                BES.IECClass _eCClass = _eCSchema.GetClass(_ecClassName);
                if(_eCClass == null)
                {
                    cantAcceptReason = $"Can't find {_ecClassName} ECClass in {_ecSchemaName}";
                    return false;
                }
                Bentley.EC.Persistence.Query.ECQuery _eCQuery = new Bentley.EC.Persistence.Query.ECQuery(_eCClass);
                BDEC.DgnECInstanceCollection _eCInstances = _ecMgr.FindInstances(BDEC.FindInstancesScope.CreateScope(_ele, new BDEC.FindInstancesScopeOption(BDEC.DgnECHostType.Element, false)),_eCQuery);
                if (_eCInstances.Count() != 0)
                {
                    cantAcceptReason = $"Mesh {_ele.ElementId} has already assign soil layer properties";
                    return false;
                }
                else
                    return true;

            }
            cantAcceptReason = "It's not soil layer element[Mesh]";
            return false;
        }
        protected override void OnPostInstall()
        {
            _mc.StatusCommand = "Pick up soil layer to attach info";
            BM.InteropServices.Utilities.ComApp.CadInputQueue.SendKeyin("POWERSELECTOR DESELECT ");
            base.OnPostInstall();
        }


        protected override void OnCleanup()
        {
            if (_attachBCInstanceToolHost != null)
            {
                _attachBCInstanceToolHost.Detach();
                _attachBCInstanceToolHost.Dispose();
                _attachBCInstanceToolHost = null;
            }
            base.OnCleanup();
        }

        public void InstallNewInstance()
        {
            AttachBCInstanceTool _tool = new AttachBCInstanceTool(_addIn);
            _tool.InstallTool();

            _mc.StatusCommand = "Bearing Capacity EC Tool";
            _mc.StatusPrompt = "Pick Soil Layer[Mesh]";

            if (_attachBCInstanceToolHost == null)
            {
                _attachBCInstanceToolHost = new BMW.ToolSettingsHost();
                _attachBCInstanceToolHost.Width = 270;
                _attachBCInstanceToolHost.Height = 200;
                _attachBCInstanceToolHost.ResizeMode = System.Windows.ResizeMode.CanResize;
                _attachBCInstanceToolHost.Title = Resources.TN_AttachBCEC;
                _attachBCInstanceToolHost.Icon = new BitmapImage(new Uri("pack://application:,,,/PDIWT.Resources;component/Images/Icons/AttachEC.ico", UriKind.RelativeOrAbsolute));
                _attachBCInstanceToolHost.Closing += _attachBCInstanceToolHost_Closing;
                _attachBCInstanceToolHost.Content = new AttachBCInstanceUserControl();
                _attachBCInstanceToolHost.Attach(_addIn);
                _attachBCInstanceToolHost.Show();
            }
            else
            {
                _attachBCInstanceToolHost.Focus();
            }
        }

        private void _attachBCInstanceToolHost_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ExitTool();
        }

        private BD.StatusInt SetMeshInstance(BDE.MeshHeaderElement _mesh)
        {
            string _ecSchemaName = "PDIWT";
            string _ecClassName = "BearingCapacitySoilLayerInfo";
            var _activedgnfile = BM.Session.Instance.GetActiveDgnFile();
            BDEC.DgnECManager _mgr = BDEC.DgnECManager.Manager;
            if(!_mgr.DiscoverSchemas(_activedgnfile, BDEC.ReferencedModelScopeOption.All, false).Contains(_ecSchemaName+".01.00"))
            {
                _mc.ShowErrorMessage(_ecSchemaName + " is not in current dgn file", "", BM.MessageAlert.None);
                return BD.StatusInt.Error;
            }

            BDEC.FindInstancesScope _scope = BDEC.FindInstancesScope.CreateScope(_activedgnfile, new BDEC.FindInstancesScopeOption(BDEC.DgnECHostType.All, false));
            BES.IECSchema _pdiECSchema = _mgr.LocateSchemaInScope(_scope, _ecSchemaName, 1, 0, BES.SchemaMatchType.Exact);
            if (_pdiECSchema == null)
                return BD.StatusInt.Error;

            BES.IECClass _bcECCalss = _pdiECSchema.GetClass(_ecClassName);
            if(_bcECCalss == null)
            {
                _mc.ShowErrorMessage(_ecClassName + "can't be find", "", false);
                return BD.StatusInt.Error;
            }

            BDEC.DgnECInstanceEnabler _enabler = _mgr.ObtainInstanceEnabler(_activedgnfile, _bcECCalss);
            BEI.StandaloneECDInstance _instance = _enabler.SharedWipInstance;

            AttachBCInstanceViewModel _vm = ((AttachBCInstanceUserControl)_attachBCInstanceToolHost.Content).DataContext as AttachBCInstanceViewModel;
            //_instance.MemoryBuffer.SetStringValue(new ,_vm.SoilLayerNumber);
            _instance.MemoryBuffer.SetStringValue("LayerNumber", -1, _vm.SoilLayerNumber);
            _instance.MemoryBuffer.SetStringValue("LayerName", -1, _vm.SoilLayerName);
            _instance.MemoryBuffer.SetDoubleValue("Betasi", -1, _vm.Betasi);
            _instance.MemoryBuffer.SetDoubleValue("Psisi", -1, _vm.Psisi);
            _instance.MemoryBuffer.SetDoubleValue("qfi", -1, _vm.Qfi);
            _instance.MemoryBuffer.SetDoubleValue("Betap", -1, _vm.Betap);
            _instance.MemoryBuffer.SetDoubleValue("Psip", -1, _vm.Psip);
            _instance.MemoryBuffer.SetDoubleValue("qr", -1, _vm.Qr);

            var _dgnECInstance = _enabler.CreateInstanceOnElement(_mesh, _instance, false);
            if (_dgnECInstance == null)
                return BD.StatusInt.Error;

            return BD.StatusInt.Success;
        }


        BM.MessageCenter _mc = BM.MessageCenter.Instance;
        readonly BM.AddIn _addIn;
        static BMW.ToolSettingsHost _attachBCInstanceToolHost;
    }
}
