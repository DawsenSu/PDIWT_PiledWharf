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
                _mc.ShowErrorMessage("Fail to create instance on mesh elements", "", false);
                return false;
            }
            else
            {
                _mc.ShowInfoMessage("Attach ECInstance On Selected mesh", "", false);
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

            cantAcceptReason = "It's not soil layer element[Mesh]";
            BDE.Element _ele = path.GetHeadElement();
            if (_ele.ElementType == BD.MSElementType.MeshHeader)
                return true;
            return false; ;
        }
        protected override void OnPostInstall()
        {
            _mc.StatusCommand = "Pick up soil layer to attach info";
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
                _attachBCInstanceToolHost.Height = 270;
                _attachBCInstanceToolHost.ResizeMode = System.Windows.ResizeMode.CanResize;
                _attachBCInstanceToolHost.Title = Resources.TN_AttachBCEC;
                _attachBCInstanceToolHost.Icon = new BitmapImage(new Uri("pack://application:,,,/PDIWT.Resources;component/Images/Icons/AttachEC.ico", UriKind.RelativeOrAbsolute));

                _attachBCInstanceToolHost.Content = new AttachBCInstanceUserControl();
                _attachBCInstanceToolHost.Attach(_addIn);
                _attachBCInstanceToolHost.Show();
            }
            else
            {
                _attachBCInstanceToolHost.Focus();
            }
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
        BM.AddIn _addIn;
        static BMW.ToolSettingsHost _attachBCInstanceToolHost;
    }
}
