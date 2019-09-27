using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BD = Bentley.DgnPlatformNET;
using BM = Bentley.MstnPlatformNET;
using BDEC = Bentley.DgnPlatformNET.DgnEC;
using BES = Bentley.ECObjects.Schema;
using BEI = Bentley.ECObjects.Instance;
using BDE = Bentley.DgnPlatformNET.Elements;
using BMW = Bentley.MstnPlatformNET.WPF;
using BG = Bentley.GeometryNET;

using GalaSoft.MvvmLight.Messaging;

namespace PDIWT_Assemblies_Core.Model.Tools
{
    using ViewModel;

    public class StairCreationTool : BD.DgnPrimitiveTool
    {
        public StairCreationTool(BM.AddIn addIn) : this(0,0,addIn)
        {            
        }

        public StairCreationTool(int toolName, int toolprompt, BM.AddIn addIn) : base(toolName, toolprompt)
        {
            _addIn = addIn;
            _points = new List<BG.DPoint3d>();
        }
        protected override bool OnDataButton(BD.DgnButtonEvent ev)
        {
            if (0 == _points.Count)
                BeginDynamics();
            _points.Add(ev.Point);

            _mc.StatusPrompt = "Pick Pile [Bottom] Point";
            if (_points.Count < 2)
                return false;

            try
            {
                BD.DgnModel _activeDgnModel = BM.Session.Instance.GetActiveDgnModel();
                double uorpermeter = BM.Session.Instance.GetActiveDgnModel().GetModelInfo().UorPerMeter;

            }
            catch (Exception e)
            {
                _mc.ShowErrorMessage("Can't Create Pile", e.ToString(), false);
            }
            finally
            {
                base.OnReinitialize();
            }
            return true;
        }

        protected override void OnDynamicFrame(BD.DgnButtonEvent ev)
        {
            BDE.LineElement _lineElement = new BDE.LineElement(BM.Session.Instance.GetActiveDgnModel(), null, new BG.DSegment3d(_points[0], ev.Point));
            BD.ElementPropertiesSetter _lineElementSetter = new BD.ElementPropertiesSetter();
            _lineElementSetter.SetLinestyle(3, null); ;
            _lineElementSetter.SetTransparency(0.6);
            _lineElementSetter.Apply(_lineElement);

            if (null == _lineElement)
                return;

            BD.RedrawElems redrawElems = new BD.RedrawElems();
            redrawElems.SetDynamicsViewsFromActiveViewSet(BM.Session.GetActiveViewport());
            redrawElems.DrawMode = BD.DgnDrawMode.TempDraw;
            redrawElems.DrawPurpose = BD.DrawPurpose.Dynamics;

            redrawElems.DoRedraw(_lineElement);
        }
        protected override void OnPostInstall()
        {
            //BD.AccuDraw.Active = true;
            //BD.AccuSnap.LocateEnabled = true;
            BD.AccuSnap.SnapEnabled = true;
            base.OnPostInstall();
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
            if (_stairCreationToolHost != null)
            {
                _stairCreationToolHost.Detach();
                _stairCreationToolHost.Dispose();
                _stairCreationToolHost = null;
            }
            base.OnCleanup();
        }

        public void InstallNewInstance()
        {
            StairCreationTool _tool = new StairCreationTool(_addIn);

            _mc.StatusCommand = "StairCreation";
            _mc.StatusPrompt = "Pick a place point";


            if (_stairCreationToolHost == null)
            {
                _stairCreationToolHost = new BMW.ToolSettingsHost
                {
                    Width = 300,
                    Height = 130,
                    ResizeMode = System.Windows.ResizeMode.CanResize,
                    Title = PDIWT.Resources.Localization.MainModule.Resources.StairCreationToolTitle
                };
                //_stairCreationToolHost.Icon = new BitmapImage(new Uri("pack://application:,,,/PDIWT.Resources;component/Images/Icons/DrawPile.ico", UriKind.RelativeOrAbsolute));
                _stairCreationToolHost.Closing += (s,e)=> ExitTool();
                _stairCreationToolHost.Content = new StairCreationUserControl();
                _stairCreationToolHost.Attach(_addIn);
                _stairCreationToolHost.Show();
            }
            else
            {
                _stairCreationToolHost.Focus();
            }
        }

        readonly BM.AddIn _addIn;
        static BMW.ToolSettingsHost _stairCreationToolHost;
        BM.MessageCenter _mc = BM.MessageCenter.Instance;

        private List<BG.DPoint3d> _points;
        StairCreationViewModel _vm;
    }
}
