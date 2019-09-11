using PDIWT_PiledWharf_Core_Cpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

using GalaSoft.MvvmLight.Messaging;
using System.Windows;

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
    public class PilePlacementTool : BD.DgnPrimitiveTool
    {
        public PilePlacementTool(BM.AddIn addIn) : this(0,0,addIn)
        {           
        }

        public PilePlacementTool(int toolName, int toolPrompt, BM.AddIn addIn) : base(toolName, toolPrompt)
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
            //var _pileplaceViewModel = (ViewModel.PilePlacementViewModel)((PilePlacementUserControl)_pilePlacementToolHost.Content).DataContext;

            try
            {
                BD.DgnModel _activeDgnModel = BM.Session.Instance.GetActiveDgnModel();
                double uorpermeter = BM.Session.Instance.GetActiveDgnModel().GetModelInfo().UorPerMeter;

                PileBase _pile = new PileBase(PileBase._unknowPileNumbering, _info.Item1,
                    new Joint(_points[0]), new Joint(_points[1]),
                    _info.Item2, _info.Item3 * uorpermeter, _info.Item4 * uorpermeter, _info.Item5 * uorpermeter, _info.Item6);
                _pile.DrawInActiveModel();
                _mc.ShowInfoMessage($"Pile Created Successfully",$"Top Point:{_pile.TopJoint}; Bottom Point:{_pile.BottomJoint}",false);
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
        protected override void ExitTool()
        {
            Messenger.Default.Send(Visibility.Visible, "ImportWindowVisibility");
            base.ExitTool();
        }

        protected override void OnRestartTool()
        {
            InstallNewInstance(_info);
        }

        protected override void OnCleanup()
        {
            if (_pilePlacementToolHost != null)
            {
                _pilePlacementToolHost.Detach();
                _pilePlacementToolHost.Dispose();
                _pilePlacementToolHost = null;
            }
            base.OnCleanup();
        }

        public void InstallNewInstance(Tuple<string, PileTypeManaged, double, double, double, PileTipType> info)
        {
            PilePlacementTool _tool = new PilePlacementTool(_addIn);
            _tool._info = info;
            _tool.InstallTool();

            _mc.StatusCommand = "Pile Placement";
            _mc.StatusPrompt = "Pick Pile [Top] Point";


            if (_pilePlacementToolHost == null)
            {
                _pilePlacementToolHost = new BMW.ToolSettingsHost
                {
                    //_pilePlacementToolHost.Width = 300;
                    //_pilePlacementToolHost.Height = 130;
                    //_pilePlacementToolHost.ResizeMode = ResizeMode.CanResize; 
                    Title = PDIWT.Resources.Localization.MainModule.Resources.PilePlacementTool
                };
                //_pilePlacementToolHost.Icon = new BitmapImage(new Uri("pack://application:,,,/PDIWT.Resources;component/Images/Icons/DrawPile.ico", UriKind.RelativeOrAbsolute));
                _pilePlacementToolHost.Closing += _pilePlacementToolHost_Closing;
                //_pilePlacementToolHost.Content =new PilePlacementUserControl();
                _pilePlacementToolHost.Attach(_addIn);
                _pilePlacementToolHost.Show();                
            }
            else
            {
                _pilePlacementToolHost.Focus();
            }
        }

        private void _pilePlacementToolHost_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ExitTool();
        }

        BM.AddIn _addIn;
        BM.MessageCenter _mc = BM.MessageCenter.Instance;
        static BMW.ToolSettingsHost _pilePlacementToolHost;

        private List<BG.DPoint3d> _points;
        private Tuple<string, PileTypeManaged, double, double, double, PileTipType> _info;

    }
}
