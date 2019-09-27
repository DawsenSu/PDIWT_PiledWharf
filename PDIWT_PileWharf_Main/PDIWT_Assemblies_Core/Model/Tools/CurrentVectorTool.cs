using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GalaSoft.MvvmLight.Messaging;
using System.Windows;

using BD = Bentley.DgnPlatformNET;
using BM = Bentley.MstnPlatformNET;
using BDEC = Bentley.DgnPlatformNET.DgnEC;
using BES = Bentley.ECObjects.Schema;
using BEI = Bentley.ECObjects.Instance;
using BDE = Bentley.DgnPlatformNET.Elements;
using BMW = Bentley.MstnPlatformNET.WPF;
using BG = Bentley.GeometryNET;

namespace PDIWT_Assemblies_Core.Model.Tools
{
    public class CurrentVectorTool : BD.DgnPrimitiveTool
    {
        public CurrentVectorTool(BM.AddIn addIn) : this(0, 0, addIn)
        { }

        public CurrentVectorTool(int toolName, int toolPrompt, BM.AddIn addIn) : base(toolName, toolPrompt)
        {
            _addIn = addIn;
        }

        BM.AddIn _addIn;
        BM.MessageCenter _mc = BM.MessageCenter.Instance;
        static BMW.ToolSettingsHost _currentVectorToolHost;


        List<List<Tuple<DateTime, double, double>>> _info;
        string _stationName;
        List<string> _stationNames;

        protected override bool OnDataButton(BD.DgnButtonEvent ev)
        {
            BD.DgnModel _activeDgnModel = BM.Session.Instance.GetActiveDgnModel();

            var _infomationToDraw = _info.ElementAt(_stationNames.IndexOf(_stationName));
            BDE.CellHeaderElement _arrowsCell = DrawCurrentArrows(ev.Point, _infomationToDraw);
            _arrowsCell.AddToModel();

            BDE.TextElement _stationText = DrawStationName(new BG.DPoint3d(ev.Point.X + 100, ev.Point.Y, ev.Point.Z));
            _stationText.AddToModel();

            ExitTool();
            return true;
        }
        protected override void OnDynamicFrame(BD.DgnButtonEvent ev)
        {
            var _infomationToDraw = _info.ElementAt(_stationNames.IndexOf(_stationName));
            BDE.CellHeaderElement _arrowsCell = DrawCurrentArrows(ev.Point, _infomationToDraw);

            BD.RedrawElems redrawElems = new BD.RedrawElems();
            redrawElems.SetDynamicsViewsFromActiveViewSet(BM.Session.GetActiveViewport());
            redrawElems.DrawMode = BD.DgnDrawMode.TempDraw;
            redrawElems.DrawPurpose = BD.DrawPurpose.Dynamics;

            redrawElems.DoRedraw(_arrowsCell);
        }

        protected override void OnPostInstall()
        {
            //BD.AccuDraw.Active = true;
            //BD.AccuSnap.LocateEnabled = true;
            BD.AccuSnap.SnapEnabled = true;
            base.OnPostInstall();
            BeginDynamics();
        }

        protected override bool OnResetButton(BD.DgnButtonEvent ev)
        {
            ExitTool();
            return true;
        }

        protected override void OnRestartTool()
        {
            InstallNewInstance(_stationNames,_stationName, _info);
        }

        protected override void OnCleanup()
        {
            if (_currentVectorToolHost != null)
            {
                _currentVectorToolHost.Detach();
                _currentVectorToolHost.Dispose();
                _currentVectorToolHost = null;
            }
            base.OnCleanup();
        }

        public void InstallNewInstance(List<string> stationNames, string stationName, List<List<Tuple<DateTime, double, double>>> infos)
        {
            CurrentVectorTool _tool = new CurrentVectorTool(_addIn);
            _tool._info = infos;
            _tool._stationNames = stationNames;
            _tool._stationName = stationName;
            _tool.InstallTool();

            _mc.StatusCommand = "Current Vectors";
            _mc.StatusPrompt = "Pick the Point to draw";


            if (_currentVectorToolHost == null)
            {
                _currentVectorToolHost = new BMW.ToolSettingsHost
                {
                    //_currentVectorToolHost.Width = 300;
                    //_currentVectorToolHost.Height = 130;
                    //_currentVectorToolHost.ResizeMode = ResizeMode.CanResize; 
                    Title = "绘制流矢图"
                };
                //_currentVectorToolHost.Icon = new BitmapImage(new Uri("pack://application:,,,/PDIWT.Resources;component/Images/Icons/DrawPile.ico", UriKind.RelativeOrAbsolute));
                _currentVectorToolHost.Closing += _currentVectorToolHost_Closing;
                //_currentVectorToolHost.Content =new PilePlacementUserControl();
                _currentVectorToolHost.Attach(_addIn);
                _currentVectorToolHost.Show();
            }
            else
            {
                _currentVectorToolHost.Focus();
            }
        }

        private void _currentVectorToolHost_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ExitTool();
        }

        BDE.CellHeaderElement DrawCurrentArrows(BG.DPoint3d origin, List<Tuple<DateTime, double, double>> info)
        {
            BD.DgnModel _activeDgnModel = BM.Session.Instance.GetActiveDgnModel();
            List<BDE.Element> _eles = new List<BDE.Element>();
            foreach (var _i in info)
                _eles.AddRange(DrawArrow(origin, _i.Item2, _i.Item3));

            BDE.CellHeaderElement _arrowCell = new BDE.CellHeaderElement(_activeDgnModel, "CurrentArrow", origin, BG.DMatrix3d.Identity, _eles);
            return _arrowCell;
        }

        List<BDE.LineElement> DrawArrow(BG.DPoint3d origin, double length, double direction)
        {
            BD.DgnModel _activeDgnModel = BM.Session.Instance.GetActiveDgnModel();
            double _uorpercm = _activeDgnModel.GetModelInfo().UorPerMeter / 100;
            List<BDE.LineElement> _arrow = new List<BDE.LineElement>();
            BDE.LineElement _mainLine = new BDE.LineElement(_activeDgnModel, null, new BG.DSegment3d(BG.DPoint3d.Zero, BG.DPoint3d.FromXY(0, length * _uorpercm)));
            BDE.LineElement _upwing = new BDE.LineElement(_activeDgnModel, null, new BG.DSegment3d(BG.DPoint3d.FromXY(0, (length - 5) * _uorpercm), BG.DPoint3d.FromXY(0, length * _uorpercm)));
            BDE.LineElement _downwing = new BDE.LineElement(_activeDgnModel, null, new BG.DSegment3d(BG.DPoint3d.FromXY(0, (length - 5) * _uorpercm), BG.DPoint3d.FromXY(0, length * _uorpercm)));

            BD.TransformInfo _upTansinfo = new BD.TransformInfo(BG.DTransform3d.FromMatrixAndFixedPoint(BG.DMatrix3d.FromEulerAngles(BG.EulerAngles.FromDegrees(-15, 0, 0)), BG.DPoint3d.FromXY(0, length * _uorpercm)));
            BD.TransformInfo _downTansinfo = new BD.TransformInfo(BG.DTransform3d.FromMatrixAndFixedPoint(BG.DMatrix3d.FromEulerAngles(BG.EulerAngles.FromDegrees(15, 0, 0)), BG.DPoint3d.FromXY(0, length * _uorpercm)));

            _upwing.ApplyTransform(_upTansinfo);
            _downwing.ApplyTransform(_downTansinfo);

            BD.TransformInfo _totalTansinfo = new BD.TransformInfo(BG.DTransform3d.FromMatrixAndFixedPoint(BG.DMatrix3d.FromEulerAngles(BG.EulerAngles.FromDegrees(direction, 0, 0)), BG.DPoint3d.Zero));
            _mainLine.ApplyTransform(_totalTansinfo);
            _upwing.ApplyTransform(_totalTansinfo);
            _downwing.ApplyTransform(_totalTansinfo);

            BD.TransformInfo _tarnslationTrans = new BD.TransformInfo(BG.DTransform3d.FromTranslation(origin));

            _mainLine.ApplyTransform(_tarnslationTrans);
            _upwing.ApplyTransform(_tarnslationTrans);
            _downwing.ApplyTransform(_tarnslationTrans);
            return new List<BDE.LineElement>() { _mainLine, _upwing, _downwing };
        }

        BDE.TextElement DrawStationName(BG.DPoint3d origin)
        {
            BD.DgnModel _activeDgnModel = BM.Session.Instance.GetActiveDgnModel();
            double _uorpercm = _activeDgnModel.GetModelInfo().UorPerMeter / 100;

            BD.DgnTextStyle dts = new BD.DgnTextStyle("test", BM.Session.Instance.GetActiveDgnFile());
            dts.SetProperty(BD.TextStyleProperty.Width, _uorpercm*4);
            dts.SetProperty(BD.TextStyleProperty.Height, _uorpercm*5);

            BD.TextBlockProperties tbProps = new BD.TextBlockProperties(dts, _activeDgnModel);
            BD.ParagraphProperties pProps = new BD.ParagraphProperties(dts, _activeDgnModel);
            BD.RunProperties runProps = new BD.RunProperties(dts, _activeDgnModel);
            BD.TextBlock TxtBlock = new BD.TextBlock(tbProps, pProps, runProps, _activeDgnModel);
            TxtBlock.AppendText(_stationName);
            TxtBlock.SetUserOrigin(origin);
            BDE.TextHandlerBase _stationText = BDE.TextHandlerBase.CreateElement(null, TxtBlock);
            return (BDE.TextElement)_stationText;
        }

    }
}
