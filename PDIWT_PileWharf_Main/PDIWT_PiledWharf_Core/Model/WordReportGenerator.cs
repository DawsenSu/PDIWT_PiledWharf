using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.IO;
using Aspose.Words;
using WpfMath;
using PDIWT.Resources.Localization.MainModule;

using BD = Bentley.DgnPlatformNET;
using Aspose.Words.Tables;
using PDIWT.Formulas;
using PDIWT_PiledWharf_Core.ViewModel;

namespace PDIWT_PiledWharf_Core.Model
{
    /// <summary>
    /// The base calls for Generate Calculation Note
    /// </summary>
    public abstract class PDIWT_CalculationNoteBuilderBase
    {
        public PDIWT_CalculationNoteBuilderBase(string templateName, string outFilePath)
        {
            string _templatePath = BD.ConfigurationManager.GetVariable("PDIWT_ORGANIZATION_WORDTEMPLATE", BD.ConfigurationVariableLevel.Organization);
            if (string.IsNullOrEmpty(_templatePath))
                throw new FileNotFoundException("Can't found word template file.", templateName);
            TemplateInfo = new FileInfo(_templatePath + templateName);
            OutFileInfo = new FileInfo(outFilePath);

            _document = new Document(TemplateInfo.FullName);
        }

        abstract public void Build();

        protected void Save()
        {
            _document.Save(OutFileInfo.FullName);

        }

        protected byte[] GenerateFormulaPic(string latexstr, double scale = 14)
        {
            var _parser = new TexFormulaParser();
            var _forumla = _parser.Parse(latexstr);
            return _forumla.RenderToPng(scale, 0, 0, "Times New Roman");
        }

        protected void BuildCoverPart(PDIWT_CalculationNoteInfo calculationCoverInfo)
        {
            string[] _mergedFields = {"ProjectName", "ProjectPhase","NumberOfVolume","CalculatedItemName","Designer",
                                    "DesignDate","Checker","CheckDate","Reviewer","ReviewDate"};
            var _enumStrDic = PDIWT.Resources.PDIWT_Helper.GetEnumDescriptionDictionary<PDIWT_ProjectPhase>();
            string[] _infos = { calculationCoverInfo.ProjectName,
                                _enumStrDic[calculationCoverInfo.ProjectPhase],
                                calculationCoverInfo.NumberOfVolume,
                                calculationCoverInfo.CalculatedItemName,
                                calculationCoverInfo.Designer, calculationCoverInfo.DesignDate.ToString("yyyy-MM-dd"),
                                calculationCoverInfo.Checker, calculationCoverInfo.CheckDate.ToString("yyyy-MM-dd"),
                                calculationCoverInfo.Reviewer, calculationCoverInfo.ReviewDate.ToString("yyyy-MM-dd")
                               };
            _document.MailMerge.UseNonMergeFields = true;
            _document.MailMerge.Execute(_mergedFields, _infos);
        }

        protected void SetToHeading1(ParagraphFormat paragraphFormat)
        {
            Style _style = _document.Styles["PDIWT Heading 1"];
            if (_style == null) return;
            paragraphFormat.Style = _style;
        }

        protected void SetToHeading2(ParagraphFormat paragraphFormat)
        {
            Style _style = _document.Styles["PDIWT Heading 2"];
            if (_style == null) return;
            paragraphFormat.Style = _style;
        }
        protected void SetToMainBody(ParagraphFormat paragraphFormat)
        {
            Style _style = _document.Styles["PDIWT MainBody"];
            if (_style == null) return;
            paragraphFormat.Style = _style;
        }
        protected void SetToNoSpacing(ParagraphFormat paragraphFormat)
        {
            Style _style = _document.Styles["PDIWT No Spacing"];
            if (_style == null) return;
            paragraphFormat.Style = _style;
        }

        protected void SetToPDIWTTable(Table table)
        {
            TableStyle _style = (TableStyle)_document.Styles["PDIWT Table"];
            if (_style == null) return;
            table.Style = _style;
            table.AutoFit(AutoFitBehavior.AutoFitToContents);
        }

        public FileInfo TemplateInfo { get; private set; }
        public FileInfo OutFileInfo { get; private set; }

        protected Document _document;
    }

    public class PDIWT_CurrentForceCalculationNoteBuilder : PDIWT_CalculationNoteBuilderBase
    {
        public PDIWT_CurrentForceCalculationNoteBuilder(string templateName, string outFilePath, PDIWT_CalculationNoteInfo info, ViewModel.CurrentForceViewModel viewModel) : base(templateName, outFilePath)
        {
            _calculationNoteInfo = info;
            _vm = viewModel;
        }

        public override void Build()
        {
            BuildCoverPart(_calculationNoteInfo);
            BuildMainPart();
            Save();
        }

        private void BuildMainPart()
        {
            DocumentBuilder _docBuilder = new DocumentBuilder(_document);

            // The character of font for body part: 
            //小四号 华文中宋,(行间距：1.1倍行距  | 段前间距：0.5行)
            _docBuilder.MoveToBookmark("InsertPoint");
            //_docBuilder.CurrentSection.PageSetup.SectionStart = SectionStart.NewPage;

            //ParagraphFormat _bodyParagraphFormt = _docBuilder.ParagraphFormat;
            //_bodyParagraphFormt.LineSpacing = 13.2;
            //_bodyParagraphFormt.SpaceBefore = 6;
            //_bodyParagraphFormt.SpaceAfter = 0;
            //SetToMainBodyStyle(_docBuilder.Font.Style);
            //_docBuilder.Writeln();
            SetToHeading1(_docBuilder.ParagraphFormat);
            _docBuilder.Writeln("桩基水流力计算");
            SetToMainBody(_docBuilder.ParagraphFormat);
            _docBuilder.Writeln($"桩基顶面高程: {_vm.PileTopElevation}m");
            _docBuilder.Writeln($"极端高水位: {_vm.HAT}m");
            _docBuilder.Writeln($"基桩泥面高程: {_vm.SoilElevation}m");
            _docBuilder.Writeln($"基桩投影宽度b: {_vm.ProjectedWidth}m");
            Dictionary<PDIWT_PiledWharf_Core_Cpp.PileTypeManaged, string> _pileTypes = PDIWT.Resources.PDIWT_Helper.GetEnumDescriptionDictionary<PDIWT_PiledWharf_Core_Cpp.PileTypeManaged>();
            _docBuilder.Writeln($"形状: {_pileTypes[_vm.SelectedPileType]}");
            _docBuilder.Writeln($"水流速: V = {_vm.CurrentVelocity}m/s");
            _docBuilder.Writeln($"横向桩中心间距: B = {_vm.PileHorizontalCentraSpan}m");
            _docBuilder.Writeln($"纵向桩中心间距: L = {_vm.PileVerticalCentraSpan}m");
            _docBuilder.Writeln($"水密度: ρ = {_vm.WaterDensity / 1000}t/m3");

            SetToHeading1(_docBuilder.ParagraphFormat);
            _docBuilder.Writeln("计算参数");
            SetToMainBody(_docBuilder.ParagraphFormat);
            _docBuilder.Writeln($"高度: H = {_vm.PileHeight}m");
            _docBuilder.Writeln($"投影面积: A = {_vm.ProjectedArea}m2/m");
            _docBuilder.Writeln($"(1) 水流阻力系数: 水流阻力系数Cw可按表13.0.3-1选用");
            _docBuilder.Writeln($"CW = {_vm.CurrentResistentCoeff}");
            _docBuilder.Writeln($"(2) 遮流影响系数: 遮流影响系数m1可按表13.0.3-2选用");
            _docBuilder.Writeln($"m1 = {_vm.ShelteringCoeff}");
            _docBuilder.Writeln($"(3) 淹没深度影响系数: 淹没深度影响系数n1可按表13.0.3-3选用");
            _docBuilder.Writeln($"n1 = {_vm.SubmergedCoeff}");
            _docBuilder.Writeln($"(4) 水深影响系数: 水深影响系数n2可按表13.0.3-4选用");
            _docBuilder.Writeln($"n2 = {_vm.WaterDepthCoeff}");
            _docBuilder.Writeln($"(5) 横向影响系数: 横向影响系数m2可按表13.0.3-4选用");
            _docBuilder.Writeln($"m2 = {_vm.HorizontalAffectCoeff}");
            _docBuilder.Writeln($"(6) 斜向影响系数: 影响系数m3可按表13.0.3-4选用");
            _docBuilder.Writeln($"m3 = {_vm.IncliningAffectCoeff}");

            SetToHeading1(_docBuilder.ParagraphFormat);
            _docBuilder.Writeln("计算结果");
            SetToMainBody(_docBuilder.ParagraphFormat);
            _docBuilder.Writeln($"桩基所受水流力准值:");

            string _frontPileLatex = string.Format("F_{{w}} = C_{{w}}\\frac{{\\rho}}{{2}}V^2A={0:N2}kN", _vm.CurrentForceForFrontPile);
            var _bytes = GenerateFormulaPic(_frontPileLatex);
            _docBuilder.Write("前排:"); _docBuilder.InsertImage(_bytes); _docBuilder.Writeln();
            string _rearPilelatex = string.Format("F_{{w}} = C_{{w}}\\frac{{\\rho}}{{2}}V^2A={0:N2}kN", _vm.CurrentForceForRearPile);
            _bytes = GenerateFormulaPic(_rearPilelatex);
            _docBuilder.Write("后排:"); _docBuilder.InsertImage(_bytes); _docBuilder.Writeln();

            _docBuilder.Writeln(string.Format("作用点:{0:N2}m", _vm.ActionPointForFrontPile));
        }

        private ViewModel.CurrentForceViewModel _vm;
        private PDIWT_CalculationNoteInfo _calculationNoteInfo;
    }

    public class PDIWT_WaveForceCalculationNoteBuilder : PDIWT_CalculationNoteBuilderBase
    {
        public PDIWT_WaveForceCalculationNoteBuilder(string templateName, string outFilePath, PDIWT_CalculationNoteInfo info, ViewModel.WaveForceViewModel viewModel)
            : base(templateName, outFilePath)
        {
            _calInfo = info;
            _vm = viewModel;
            _db = new DocumentBuilder(_document);
            _enumDict = PDIWT.Resources.PDIWT_Helper.GetEnumDescriptionDictionary<ViewModel.DesignWaterLevelCondition>();
        }

        private void BuildInputParametersPart()
        {
            _db.MoveToBookmark("InsertPoint");

            SetToHeading1(_db.ParagraphFormat);
            _db.Writeln("设计参数");

            SetToHeading2(_db.ParagraphFormat);
            _db.Writeln("几何属性");
            SetToMainBody(_db.ParagraphFormat);
            Dictionary<PDIWT_PiledWharf_Core_Cpp.PileTypeManaged, string> _dict = PDIWT.Resources.PDIWT_Helper.GetEnumDescriptionDictionary<PDIWT_PiledWharf_Core_Cpp.PileTypeManaged>();
            _db.Writeln($"柱体界面形状：{_dict[ _vm.SelectedPileType]}");
            _db.Writeln($"柱体的直径(边长)：D(a)={_vm.PileDiameter.ToString("F2")}m");
            _db.Writeln($"柱体的中心距：l={_vm.PileDiameter.ToString("F2")}m");

            SetToHeading2(_db.ParagraphFormat);
            _db.Writeln("计算系数");
            SetToMainBody(_db.ParagraphFormat);
            _db.Writeln($"速度力系数：CD={_vm.VelocityForceCoeffCD.ToString("F2")}");
            _db.Writeln($"速度力系数：CM={_vm.VelocityForceCoeffCM.ToString("F2")}");
            _db.Writeln($"水重度：γ={_vm.WaterWeight.ToString("F2")}kN/m3");

            SetToHeading2(_db.ParagraphFormat);
            _db.Writeln("码头标高");
            SetToMainBody(_db.ParagraphFormat);
            _db.Writeln($"码头顶标高：Δ={_vm.TopElevation.ToString("F2")}m");
            _db.Writeln($"码头底标高：Δ={_vm.BottomElevation.ToString("F2")}m");

            SetToHeading2(_db.ParagraphFormat);
            _db.Writeln("设计水位");
            SetToMainBody(_db.ParagraphFormat);
            _db.Writeln($"{_enumDict[ViewModel.DesignWaterLevelCondition.HAT]}={_vm.HAT.ToString("F2")}m");
            _db.Writeln($"{_enumDict[ViewModel.DesignWaterLevelCondition.MHW]}={_vm.MHW.ToString("F2")}m");
            _db.Writeln($"{_enumDict[ViewModel.DesignWaterLevelCondition.MLW]}={_vm.MLW.ToString("F2")}m");
            _db.Writeln($"{_enumDict[ViewModel.DesignWaterLevelCondition.LAT]}={_vm.LAT.ToString("F2")}m");

            SetToHeading2(_db.ParagraphFormat);
            _db.Writeln("设计波浪要素(50年一遇)");
            SetToMainBody(_db.ParagraphFormat);
            _db.Write("波长：");
            _db.InsertImage(GenerateFormulaPic(_waveLengthFormula));
            _db.Writeln();
            _db.Writeln($"重力加速度g={_vm.GravitationalAcceleration.ToString("F2")}m/s2");
            SetToNoSpacing(_db.ParagraphFormat);
            Table _table = _db.StartTable();
            List<string> _headstr = new List<string> { "水位", "H1%(m)", "H13%(m)", "T(s)", "水深d(m)", "波长L(m)" };
            foreach (var _cell in _headstr)
            {
                _db.InsertCell();
                _db.Write(_cell);
            }
            _db.EndRow();
            foreach (var _inputparam in _vm.DesignInputParameters)
            {
                _db.InsertCell(); _db.Write(_enumDict[_inputparam.DesignWaterLevel]);
                _db.InsertCell(); _db.Write(_inputparam.H1.ToString("F2"));
                _db.InsertCell(); _db.Write(_inputparam.H13.ToString("F2"));
                _db.InsertCell(); _db.Write(_inputparam.T.ToString("F2"));
                _db.InsertCell(); _db.Write(_inputparam.WaterDepth.ToString("F2"));
                _db.InsertCell(); _db.Write(_inputparam.WaveLength.ToString("F2"));
                _db.EndRow();
            }
            _db.EndTable();
            SetToPDIWTTable(_table);
            _db.Writeln();

            SetToHeading2(_db.ParagraphFormat);
            _db.Writeln("计算参数");
            SetToNoSpacing(_db.ParagraphFormat);
            _headstr = new List<string> { "水位", "D/L", "H/d", "d/L" };
            _table = _db.StartTable();
            foreach (var _cell in _headstr)
            {
                _db.InsertCell();
                _db.Write(_cell);
            }
            _db.EndRow();
            foreach (var _calParams in _vm.CalculatedParameters)
            {
                _db.InsertCell(); _db.Write(_enumDict[_calParams.DesignWaterLevel]);
                _db.InsertCell(); _db.Write(_calParams.D_L.ToString("F2"));
                _db.InsertCell(); _db.Write(_calParams.H_D.ToString("F2"));
                _db.InsertCell(); _db.Write(_calParams.DD_L.ToString("F2"));
                _db.EndRow();
            }
            _db.EndTable();
            SetToPDIWTTable(_table);
            _db.Writeln();

            SetToHeading2(_db.ParagraphFormat);
            _db.Writeln("查图所得参数");
            SetToMainBody(_db.ParagraphFormat);
            _db.Writeln("由图10.3.2-1查得当ωt＝0°时ηmax值");
            SetToNoSpacing(_db.ParagraphFormat);
            _headstr = new List<string> { "水位", "ηmax" };
            _table = _db.StartTable();
            foreach (var _cell in _headstr)
            {
                _db.InsertCell();
                _db.Write(_cell);
            }
            _db.EndRow();
            foreach (var _calParams in _vm.CalculatedParameters)
            {
                _db.InsertCell(); _db.Write(_enumDict[_calParams.DesignWaterLevel]);
                _db.InsertCell(); _db.Write(_calParams.YitaMax.ToString("F2"));
                _db.EndRow();
            }
            _db.EndTable();
            SetToPDIWTTable(_table);
            _db.Writeln();

            SetToMainBody(_db.ParagraphFormat);
            _db.Writeln("α、β分别按图10.3.2-8和图10.3.2-9查得");
            SetToNoSpacing(_db.ParagraphFormat);
            _headstr = new List<string> { "水位", "α", "β" };
            _table = _db.StartTable();
            foreach (var _cell in _headstr)
            {
                _db.InsertCell();
                _db.Write(_cell);
            }
            _db.EndRow();
            foreach (var _calParams in _vm.CalculatedParameters)
            {
                _db.InsertCell(); _db.Write(_enumDict[_calParams.DesignWaterLevel]);
                _db.InsertCell(); _db.Write(_calParams.Alpha.ToString("F2"));
                _db.InsertCell(); _db.Write(_calParams.Beta.ToString("F2"));
                _db.EndRow();
            }
            _db.EndTable();
            SetToPDIWTTable(_table);
            _db.Writeln();

            SetToMainBody(_db.ParagraphFormat);
            _db.Writeln("系数γp和γM按图10.3.2-10查得");
            SetToNoSpacing(_db.ParagraphFormat);
            _headstr = new List<string> { "水位", "γp", "γM" };
            _table = _db.StartTable();
            foreach (var _cell in _headstr)
            {
                _db.InsertCell();
                _db.Write(_cell);
            }
            _db.EndRow();
            foreach (var _calParams in _vm.CalculatedParameters)
            {
                _db.InsertCell(); _db.Write(_enumDict[_calParams.DesignWaterLevel]);
                _db.InsertCell(); _db.Write(_calParams.GammaP.ToString("F2"));
                _db.InsertCell(); _db.Write(_calParams.GammaM.ToString("F2"));
                _db.EndRow();
            }
            _db.EndTable();
            SetToPDIWTTable(_table);
            _db.Writeln();

            if (_vm.PileCentraSpan / _vm.PileDiameter <= 4)
            {
                SetToMainBody(_db.ParagraphFormat);
                _db.Writeln($"l/D = {_vm.PileCentraSpan / _vm.PileDiameter}");
                _db.Writeln("当桩的中心距l小于4倍桩的直径D时，应乘以群桩系数K，K值按表10.3.5查得");
                SetToNoSpacing(_db.ParagraphFormat);
                _headstr = new List<string> { "水位", "K" };
                _table = _db.StartTable();
                foreach (var _cell in _headstr)
                {
                    _db.InsertCell();
                    _db.Write(_cell);
                }
                _db.EndRow();
                foreach (var _calParams in _vm.CalculatedParameters)
                {
                    _db.InsertCell(); _db.Write(_enumDict[_calParams.DesignWaterLevel]);
                    _db.InsertCell(); _db.Write(_calParams.K.ToString("F2"));
                    _db.EndRow();
                }
                _db.EndTable();
                SetToPDIWTTable(_table);
                _db.Writeln();
            }

            SetToMainBody(_db.ParagraphFormat);
            _db.Writeln("算圆柱底面上波浪浮托力时f0、f1、f2、f3系数，按图10.3.6-3查得");
            SetToNoSpacing(_db.ParagraphFormat);
            _headstr = new List<string> { "水位", "f0", "f1", "f2", "f3", "ωt(°)" };
            _table = _db.StartTable();
            foreach (var _cell in _headstr)
            {
                _db.InsertCell();
                _db.Write(_cell);
            }
            _db.EndRow();
            foreach (var _calParams in _vm.CalculatedParameters)
            {
                _db.InsertCell(); _db.Write(_enumDict[_calParams.DesignWaterLevel]);
                _db.InsertCell(); _db.Write(_calParams.F0.ToString("F2"));
                _db.InsertCell(); _db.Write(_calParams.F1.ToString("F2"));
                _db.InsertCell(); _db.Write(_calParams.F2.ToString("F2"));
                _db.InsertCell(); _db.Write(_calParams.F3.ToString("F2"));
                _db.InsertCell(); _db.Write(_calParams.OmgaT.ToString("F2"));
                _db.EndRow();
            }
            _db.EndTable();
            SetToPDIWTTable(_table);
            _db.Writeln();
        }

        private void BuildResultsPart()
        {
            SetToHeading1(_db.ParagraphFormat);
            _db.Writeln("计算结果");
            SetToMainBody(_db.ParagraphFormat);
            _db.Writeln("作用于整个柱体高度上的最大总波浪力Pmax和最大总波浪力矩Mmax分别如下;" +
                "圆柱底面上波浪力浮托力Pu和和波流浮托力矩Mu分别如下");
            SetToNoSpacing(_db.ParagraphFormat);
            var _headstr = new List<string> { "水位", "Pmax(kN)", "Mmax(kN•m)", "Pu(kN)", "Mu(kN•m)" };
            var _table = _db.StartTable();
            foreach (var _cell in _headstr)
            {
                _db.InsertCell();
                _db.Write(_cell);
            }
            _db.EndRow();
            foreach (var _result in _vm.Results)
            {
                _db.InsertCell(); _db.Write(_enumDict[_result.DesignWaterLevel]);
                _db.InsertCell(); _db.Write(_result.PMax.ToString("F2"));
                _db.InsertCell(); _db.Write(_result.MMax.ToString("F2"));
                _db.InsertCell(); _db.Write(_result.Pu.ToString("F2"));
                _db.InsertCell(); _db.Write(_result.Mu.ToString("F2"));
                _db.EndRow();
            }
            _db.EndTable();
            SetToPDIWTTable(_table);
            _db.Writeln();
        }

        private int count = 0;
        private void BuildCalculationProcessPart(ViewModel.DesignWaterLevelCondition levelCondition)
        {
            var _inputparams = (from input in _vm.DesignInputParameters where input.DesignWaterLevel == levelCondition select input).FirstOrDefault();
            var _calparams = (from cal in _vm.CalculatedParameters where cal.DesignWaterLevel == levelCondition select cal).FirstOrDefault();
            var _result = (from res in _vm.Results where res.DesignWaterLevel == levelCondition select res).FirstOrDefault();

            if (count == 0)
            {
                SetToHeading1(_db.ParagraphFormat);
                _db.Writeln("计算过程");
                count++;
            }
            SetToHeading2(_db.ParagraphFormat);
            _db.Writeln(_enumDict[levelCondition]);
            SetToMainBody(_db.ParagraphFormat);
            _db.Writeln("[1]、已知条件");
            _db.Writeln(string.Format("柱体直径(边长)D(a)={0:F2}m， 波长L={0:F2}m，D/L={0:F2}m", _vm.PileDiameter, _inputparams.WaveLength, _calparams.D_L));
            _db.Writeln($"水深d={_inputparams.WaterDepth.ToString("F2")}m，d/L={_calparams.DD_L.ToString("F2")}m");
            _db.Writeln($"波高H1%={_inputparams.H1.ToString("F2")}m，H/d={_calparams.H_D.ToString("F2")}m");
            _db.Writeln($"查图10.3.2-1，当ωt=0°时，波峰在静水面以上的高度ηmax={_calparams.YitaMax.ToString("F2")}m");
            _db.Write("因为");
            if (_calparams.D_L <= 0.2)
            {
                _db.Write("D/L≤0.2, 小尺度桩。");
                if (_calparams.H_D <= 0.2 && _calparams.DD_L >= 0.2)
                {
                    _db.Writeln("H/d≦0.2且d/L≧0.2，按照第10.3.2.1条款计算作用于柱体上的正向波浪力。");
                }
                else if (_calparams.H_D > 0.2 && _calparams.DD_L >= 0.35)
                {
                    _db.Writeln("H/d>0.2且d/L≧0.35，按照第10.3.2.1条款计算作用于柱体上的正向波浪力。");

                }
                else if (_calparams.H_D <= 0.2 && _calparams.DD_L < 0.2)
                {
                    _db.Writeln("H/d≦0.2且d/L<0.2，按照第10.3.2.2条款计算作用于柱体上的正向波浪力。");
                }
                else
                {
                    _db.Writeln("H/d>0.2且d/L<0.35，按照第10.3.2.2条款计算作用于柱体上的正向波浪力。");
                }
            }
            else
            {
                _db.Write("D/L>0.2, 大尺度桩。");
                if (_calparams.H_D < 0.1)
                {
                    _db.Writeln("H/d<0.1，按照第10.3.6条款计算作用于柱体上的正向波浪力。");

                }
                else
                {
                    if (_vm.PileDiameter / _inputparams.WaterDepth >= 0.4)
                    {
                        _db.Writeln("H/d>=0.1q且D/d≧0.4，按照第10.3.7条款计算作用于柱体上的正向波浪力。");
                    }
                    else
                    {
                        _db.Writeln("按照第10.3.2.1条款计算作用于柱体上的正向波浪力");
                    }
                }
            }
            _db.Writeln("[2]、波浪力计算");
            _db.Writeln("波浪力的最大速度分力PDmax，波浪力的最大惯性分力PImax，以及PDmax和PImax对Z1断面的力矩MDmax和MImax的计算公式入下：");
            _db.InsertImage(GenerateFormulaPic(_PDMaxFormula)); _db.Writeln();
            _db.InsertImage(GenerateFormulaPic(_PIMaxFormula)); _db.Writeln();
            _db.InsertImage(GenerateFormulaPic(_MDMaxFormula)); _db.Writeln();
            _db.InsertImage(GenerateFormulaPic(_MIMaxFormula)); _db.Writeln();

            _db.Writeln("1)、波浪力的最大速度分力计算");
            _db.Writeln("沿整个柱体高度断面相同，则在计算整个柱体的最大速度分力时，已知：");
            _db.Writeln(string.Format("z1=0m， z2=d+ηmax={0:F2}m", _inputparams.WaterDepth + _calparams.YitaMax));
            _db.Writeln(string.Format("CD={0:F2}， CM={1:F2}", WaveForce.CalculateCD(_vm.SelectedPileType),
                                                             WaveForce.CalculateCM(_vm.PileDiameter, _inputparams.WaveLength, _vm.SelectedPileType)));

            _db.Writeln("系数K1与K3的计算公式如下：");
            _db.InsertImage(GenerateFormulaPic(_K1Formula)); _db.Writeln();
            _db.InsertImage(GenerateFormulaPic(_K3Formula)); _db.Writeln();
            _db.Writeln($"K1={_calparams.K1.ToString("F2")}, K3={_calparams.K3.ToString("F2")}");
            _db.Writeln(string.Format("PDmax={0:F2}kN，MDmax={1:F2}kN•m", _result.PDMax, _result.MDMax));

            _db.Writeln("2)、波浪力的最大惯性分力计算");
            _db.Writeln("沿整个柱体高度断面相同，则在计算整个柱体的最大速度分力时,已知：");
            _db.Writeln(string.Format("z1=0m， z2=d+ηmax-H/2={0:F2}m", _inputparams.WaterDepth + _calparams.YitaMax - _inputparams.H1 / 2));
            _db.Writeln(string.Format("CD={0:F2}， CM={1:F2}", WaveForce.CalculateCD(_vm.SelectedPileType),
                                                             WaveForce.CalculateCM(_vm.PileDiameter, _inputparams.WaveLength, _vm.SelectedPileType)));
            _db.Writeln("系数K2与K4的计算公式如下：");
            _db.InsertImage(GenerateFormulaPic(_K2Formula)); _db.Writeln();
            _db.InsertImage(GenerateFormulaPic(_K4Formula)); _db.Writeln();
            _db.Writeln($"K2={_calparams.K1.ToString("F2")}, K4={_calparams.K3.ToString("F2")}");

            _db.Writeln(string.Format("PImax={0:F2}kN， MImax={1:F2}kN•m", _result.PDMax, _result.MIMax));
            _db.Writeln("3)、PDMax，MDMax，PIMax，MIMax乘以相应系数");
            if ((_calparams.H_D <= 0.2 && _calparams.DD_L < 0.2) ||
                (_calparams.H_D > 0.2 && _calparams.DD_L < 0.35))
            {
                _db.Writeln("H/d≦0.2且d/L<0.2，或H/d>0.2且d/L<0.35时，按10.3.2.2条款，PDMax乘以α，MDMax乘以β");
                _db.Write(string.Format("α={0:F2}， β={1:F2}", _calparams.Alpha, _calparams.Beta));
                _db.Write(string.Format("PDMax={0:F2}kN， MDMax={1:F2}kN•m", _result.PDMax_Final, _result.MDMax_Final));
            }
            if (0.04 <= _calparams.DD_L && _calparams.DD_L <= 0.2)
            {
                _db.Writeln("0.04≦d/L≦0.2时，按10.3.2.3条款，PIMax乘以γP，MIMax乘以γM");
                _db.Write(string.Format("γP={0:F2}， γM={1:F2}", _calparams.GammaP, _calparams.GammaM));
                _db.Write(string.Format("PIMax={0:F2}kN， MIMax={1:F2}kN•m", _result.PIMax_Final, _result.MIMax_Final));
            }
            _db.Writeln();

            _db.Writeln("4)、最大总波浪力和最大总波浪力矩计算");
            if (_calparams.D_L <= 0.2)
            {
                if (_vm.PileCentraSpan < 4 * _vm.PileDiameter)
                {
                    _db.Writeln("l ≦ 4D， 按照10.3.5条款得到需乘以群桩系数K");
                    _db.Writeln(string.Format("按表10.3.5得到，K={0:F2}", _calparams.K));
                }
                if (_result.PDMax_Final <= _result.PIMax_Final / 2)
                {
                    _db.Writeln("PDMax ≦ 0.5PIMax， 按照10.3.4(1)条款得到");
                    _db.Writeln(string.Format("PMax = PIMAx = {0:F2}kN", _result.PMax));
                    _db.Writeln(string.Format("MMax = MIMAx = {0:F2}kN•m", _result.MMax));
                    _db.Writeln("ωt=270°");
                }
                else
                {
                    _db.Writeln("PDMax > 0.5PIMax，按照10.3.4(2)条款得到");
                    _db.Writeln("Pmax，Dmax，ωt 计算公式如下：");
                    _db.InsertImage(GenerateFormulaPic(_PMaxFormula)); _db.Writeln();
                    _db.InsertImage(GenerateFormulaPic(_MMaxFormula)); _db.Writeln();
                    _db.InsertImage(GenerateFormulaPic(_OmgaTFormula)); _db.Writeln();
                    _db.Writeln(string.Format("PMax={0:F2}kN，MMAx = {1:F2}kN•m", _result.PMax, _result.MMax));
                }
            }
            else
            {
                _db.Writeln("PDMax > 0.5PIMax， 按照10.3.6条款得到");
                _db.Writeln(string.Format("PMax={0:F2}kN，MMAx = {1:F2}kN•m，ωt={2:F2}°", _result.PMax, _result.MMax, _result.OmgaT));
            }
            _db.Writeln("[3]、波浪付托力计算");
            _db.Writeln("波浪浮托力Pu,波浪浮托力矩Mu的计算公式如下");
            _db.InsertImage(GenerateFormulaPic(_PuFormula)); _db.Writeln();
            _db.InsertImage(GenerateFormulaPic(_MuFormula)); _db.Writeln();
            _db.Writeln(string.Format("根据附录Q，f0={0:F2}，f1={0:F1}，f2={0:F2}，f3={0:F2}，ωt={0:F2}°",
                _calparams.F0, _calparams.F1, _calparams.F2, _calparams.F3, _calparams.OmgaT));
            _db.Writeln("计算结果如下：");
            _db.Writeln(string.Format("Pu={0:F2}kN， Mu = {1:F2}kN•m， ωt={2:F2}°", _result.Pu, _result.Mu, _result.OmgaT));

        }

        public override void Build()
        {
            BuildCoverPart(_calInfo);
            BuildInputParametersPart();
            BuildResultsPart();
            BuildCalculationProcessPart(ViewModel.DesignWaterLevelCondition.HAT);
            BuildCalculationProcessPart(ViewModel.DesignWaterLevelCondition.MHW);
            BuildCalculationProcessPart(ViewModel.DesignWaterLevelCondition.MLW);
            BuildCalculationProcessPart(ViewModel.DesignWaterLevelCondition.LAT);
            //var _tables = _document.GetChildNodes(NodeType.Table, false);
            //foreach (var _t in _tables)
            //{
            //    Table _table = (Table)_t;
            //    _table.AutoFit(AutoFitBehavior.AutoFitToContents);
            //}
            Save();
        }
        readonly string _waveLengthFormula = @"L=\frac{gT^{2}}{2\pi}Tanh(\frac{2\pi d}{L})";
        readonly string _K1Formula = @"K_{1}=\frac{\frac{4\pi z_{2}}{L}-\frac{4\pi z_{1}}{L}+sh\frac{4\pi z_{2}}{L}-sh\frac{4\pi z_{1}}{L}}{8sh\frac{4\pi d}{L}}";
        readonly string _K2Formula = @"K_{2}=\frac{sh\frac{2\pi z_{2}}{L}-sh\frac{2\pi z_{1}}{L}}{ch\frac{2\pi d}{L}}";
        readonly string _K3Formula = @"K_{3}=\frac{1}{sh\frac{4\pi d}{L}}\left [ \frac{\pi^{2}(z_{2}-z_{1})^{2}}{4L^{2}}+\frac{\pi(z_{2}-z{1})}{8L}sh\frac{4\pi z_{2}}{L}-\frac{1}{32}\left ( ch\frac{4\pi z_{2}}{L}-ch\frac{4\pi z_{1}}{L} \right ) \right ]";
        readonly string _K4Formula = @"K_{4}=\frac{1}{ch\frac{2\pi d}{L}}\left [ \frac{2\pi (z_{2}-z_{1})}{L}sh\frac{2\pi z_{2}}{L}-\left( ch\frac{2\pi z_{2}}{L}-ch\frac{2\pi z_{1}}{L} \right) \right]";
        readonly string _PDMaxFormula = @"P_{Dmax}=C_{D}\frac{\gamma DH^{2}}{2}K_{1}";
        readonly string _MDMaxFormula = @"M_{Dmax}=C_{D}\frac{\gamma DH^{2}L}{2\pi}K_{3}";
        readonly string _PIMaxFormula = @"P_{Imax}=C_{M}\frac{\gamma AH}{2}K_{2}";
        readonly string _MIMaxFormula = @"M_{Imax}=C_{M}\frac{\gamma AHL}{4\pi}K_{4}";

        readonly string _PMaxFormula = @"P_{max}=P_{Dmax}\left( 1+0.25\frac{P_{Imax}^{2}}{P_{Dmax}^{2}} \right)";
        readonly string _MMaxFormula = @"M_{max}=M_{Dmax}\left( 1+0.25\frac{M_{Imax}^{2}}{M_{Dmax}^{2}} \right)";
        readonly string _OmgaTFormula = @"sin\omega t=-0.5\frac{P_{Imax}}{P_{Dmax}}";
        readonly string _PuFormula = @"P_{u}=\frac{\gamma HD^{2}}{4}\frac{ch(2\pi z/L)}{ch(2\pi d/L)}(f_{2}sin\omega t -f_{0}cos\omega t)";
        readonly string _MuFormula = @"M_{u}=\frac{\gamma HD^{3}}{32}\frac{ch(2\pi z/L)}{ch(2\pi d/L)}(f_{3}sin\omega t +f_{1}cos\omega t)";

        PDIWT_CalculationNoteInfo _calInfo;
        ViewModel.WaveForceViewModel _vm;
        DocumentBuilder _db;
        Dictionary<ViewModel.DesignWaterLevelCondition, string> _enumDict;

    }

    public class PDIWT_AxialBearingCapacityCalculationNoteBuilder : PDIWT_CalculationNoteBuilderBase
    {
        public PDIWT_AxialBearingCapacityCalculationNoteBuilder(string templateName, string outFilePath, PDIWT_CalculationNoteInfo info, ViewModel.BearingCapacityViewModel viewModel) : base(templateName, outFilePath)
        {
            _calInfo = info;
            _vm = viewModel;
            _db = new DocumentBuilder(_document);
        }
        public override void Build()
        {
            BuildCoverPart(_calInfo);
            BuildInputParametersPart();
            BuildSummaryofResultsPart();
            BuildCalculationProcessPart();
            Save();
        }

        private void BuildInputParametersPart()
        {
            _db.MoveToBookmark("InsertPoint");

            SetToHeading1(_db.ParagraphFormat);
            _db.Writeln("地质参数");

            SetToMainBody(_db.ParagraphFormat);
            _db.Writeln("地质参数表格如下：");
            SetToNoSpacing(_db.ParagraphFormat);
            var _headstr = new List<string> { "土层号", "土层", "qf/kPa", "qr/kPa"};
            var _table = _db.StartTable();
            foreach (var _cell in _headstr)
            {
                _db.InsertCell();
                _db.Write(_cell);
            }
            _db.EndRow();
            foreach (var _soillayer in _vm.SoilLayersLibrary)
            {
                //todo need to revise depend on pile type
                _db.InsertCell(); _db.Write(_soillayer.Number);
                _db.InsertCell(); _db.Write(_soillayer.Name);
                _db.InsertCell(); _db.Write(_soillayer.DPSE_Qfi.Value.ToString("F2"));
                _db.InsertCell(); _db.Write(_soillayer.DPSE_Qr.Value.ToString("F2"));
                _db.EndRow();
            }
            _db.EndTable();
            SetToPDIWTTable(_table);
            _db.Writeln();
        }

        private void BuildSummaryofResultsPart()
        {
            SetToHeading1(_db.ParagraphFormat);
            _db.Writeln("计算结果汇总");

            SetToMainBody(_db.ParagraphFormat);
            _db.Writeln("计算结果汇总表如下：");
            SetToNoSpacing(_db.ParagraphFormat);
            var _headstr = new List<string> { "桩基编号", "桩长", "抗压承载力(kN)", "抗拔承载力(kN)" };
            var _table = _db.StartTable();
            foreach (var _cell in _headstr)
            {
                _db.InsertCell();
                _db.Write(_cell);
            }
            _db.EndRow();
            foreach (var _pile in _vm.PileInfos)
            {
                _db.InsertCell(); _db.Write(_pile.PileName);
                _db.InsertCell(); _db.Write(_pile.PileLength.ToString("F2"));
                _db.InsertCell(); _db.Write(_pile.DesignAxialBearingCapacity.Value.ToString("F2"));
                _db.InsertCell(); _db.Write(_pile.DesignAxialUpliftCapacity.Value.ToString("F2"));
                _db.EndRow();
            }
            _db.EndTable();
            SetToPDIWTTable(_table);
            _db.Writeln();
        }

        private void BuildCalculationProcessPart()
        {
            SetToHeading1(_db.ParagraphFormat);
            _db.Writeln("计算过程");

            foreach (var _pile in _vm.PileInfos)
            {
                SetToHeading2(_db.ParagraphFormat);
                _db.Writeln($"桩{_pile.PileName}计算结果计算过程");

                SetToMainBody(_db.ParagraphFormat);
                switch (_pile.SelectedBearingCapacityPileType)
                {
                    case BearingCapacityPileTypes.DrivenPileWithSealedEnd:
                        _db.Writeln("桩身实心或者桩端封闭的打入桩的轴向抗压承载力设计值与抗拔承载力设计值计算分别按照规范4.2.8.1及4.2.10条进行，其计算公式如下：");
                        _db.InsertImage(GenerateFormulaPic(_drivenSealPileFormula)); _db.Writeln();
                        break;
                    case BearingCapacityPileTypes.TubePileOrSteelPile:
                        _db.Writeln("钢管桩和预制混凝土管桩的轴向抗压承载力设计值与抗拔承载力设计值计算分别按照规范4.2.8.2及4.2.10条进行，其计算公式如下：");
                        _db.InsertImage(GenerateFormulaPic(_steelandtubepileFormula)); _db.Writeln();
                        break;
                    case BearingCapacityPileTypes.CastInSituPile:
                        _db.Writeln("灌注桩的轴向抗压承载力设计值与抗拔承载力设计值计算分别按照规范4.2.8.1及4.2.10条进行，其计算公式如下：");
                        _db.InsertImage(GenerateFormulaPic(_castinsitupileFormula)); _db.Writeln();
                        break;
                    case BearingCapacityPileTypes.CastInSituAfterGrountingPile:
                        _db.Writeln("后注浆灌注桩端封闭的打入桩的轴向抗压承载力设计值与抗拔承载力设计值计算分别按照规范4.2.8.1及4.2.10条进行，其计算公式如下：");
                        _db.InsertImage(GenerateFormulaPic(_castinsitugroutingpileForumla)); _db.Writeln();
                        break;
                    default:
                        break;
                }
                _db.InsertImage(GenerateFormulaPic(_liftupFormula)); _db.Writeln();

                _db.Writeln($"分项系数γR = {_pile.PartialCoeff.ToString("F1")}；闭塞系数η = {_pile.BlockCoeff.ToString("F1")}； 计算水位 WL = {_pile.CalculatedWaterLevel.ToString("F2")}m；");
                _db.Writeln($"桩顶高程 = {_pile.PileTopElevation.ToString("F2")}m；桩长 L = {_pile.PileLength.ToString("F2")}m；");
                if (_pile.IsVerticalPile)
                    _db.Writeln("桩斜度 m = NaN（直桩）；");
                else
                    _db.Writeln($"桩斜度 m = {_pile.PileSkewness.ToString("F2")}；");
                switch (_pile.SelectedPileGeoType)
                {
                    case PDIWT_PiledWharf_Core_Cpp.PileTypeManaged.SqaurePile:
                        _db.Writeln($"方桩边长 b = {_pile.PileDiameter.ToString("F2")}m；");
                        break;
                    case PDIWT_PiledWharf_Core_Cpp.PileTypeManaged.TubePile:
                        _db.Writeln($"圆桩直径 D = {_pile.PileDiameter.ToString("F2")}m；");
                        break;
                    case PDIWT_PiledWharf_Core_Cpp.PileTypeManaged.PHCTubePile:
                        _db.Writeln($"管桩外直径 D = {_pile.PileDiameter.ToString("F2")}m；管桩内直径 d = {_pile.PileInnerDiameter.ToString("F2")}m；");
                        break;
                    case PDIWT_PiledWharf_Core_Cpp.PileTypeManaged.SteelTubePile:
                        _db.Writeln($"钢管桩外直径 D = {_pile.PileDiameter.ToString("F2")}m；钢管桩内直径 d = {_pile.PileInnerDiameter.ToString("F2")}m；钢管桩灌芯长度 Lc = {_pile.ConcreteCoreLength.ToString("F2")}m；");
                        break;
                    default:
                        break;
                }
                _db.Writeln($"混凝土重度 γc = {_pile.ConcreteWeight.ToString("F2")}kN/m3；混凝土浮重度 γc浮 = {_pile.ConcreteUnderwaterWeight.ToString("F2")}kN/m3；");
                if (_pile.SelectedPileGeoType == PDIWT_PiledWharf_Core_Cpp.PileTypeManaged.SteelTubePile)
                    _db.Writeln($"钢材重度 γs = {_pile.SteelWeight.ToString("F2")}kN/m3；钢材浮重度 γs浮 = {_pile.SteelUnderwaterWeight.ToString("F2")}kN/m3；");

                _db.Writeln($"桩周长 U = {_pile.PilePerimeter.ToString("F2")}m；桩的外截面积 A = {_pile.PileCrossSectionArea.ToString("F2")}m2");
                _db.Writeln($"桩底高程 = {_pile.PileBottomElevation.ToString("F2")}m；桩自重 A = {_pile.PileSelfWeight.ToString("F2")}kN； Cosα = {AxialBearingCapacity.CalculateCosAlpha(_pile.PileSkewness).ToString("F2")}；");

                switch (_pile.SelectedBearingCapacityPileType)
                {
                    case BearingCapacityPileTypes.DrivenPileWithSealedEnd:
                        DrivenSealPileCalculationTable(_pile);
                        break;
                    case BearingCapacityPileTypes.TubePileOrSteelPile:
                        SteelTubePileCalculationTable(_pile);
                        break;
                    case BearingCapacityPileTypes.CastInSituPile:
                        CastInSituPileCalculationTable(_pile);
                        break;
                    case BearingCapacityPileTypes.CastInSituAfterGrountingPile:
                        CastInSituGroutingPileCalculationTable(_pile);
                        break;
                    default:
                        break;
                }

                _db.Writeln($"单桩极限抗压承载力设计值 Qd = {_pile.DesignAxialBearingCapacity.Value.ToString("F2")}kN");
                _db.Writeln($"单桩抗拔极限承载力设计值 Td = {_pile.DesignAxialUpliftCapacity.Value.ToString("F2")}kN");
            }

        }

        private void DrivenSealPileCalculationTable(PDIWT_BearingCapacity_PileInfo _pile)
        {
            SetToMainBody(_db.ParagraphFormat);
            _db.Writeln("计算结果汇总表如下：");
            SetToNoSpacing(_db.ParagraphFormat);
            var _headstr = new List<string> { "土层号", "土层", "层顶高程(m)", "土层厚度li(m)","极限侧摩阻力标准值qfi(kPa)", "极限桩端阻力标准值qr(kPa)","抗拔折减系数ξi" };
            var _table = _db.StartTable();
            foreach (var _cell in _headstr)
            {
                _db.InsertCell();
                _db.Write(_cell);
            }
            _db.EndRow();
            foreach (var _layer in _pile.PileSoilLayersInfo)
            {
                _db.InsertCell(); _db.Write(_layer.SoilLayerObject.Number);
                _db.InsertCell(); _db.Write(_layer.SoilLayerObject.Name);
                _db.InsertCell(); _db.Write(_layer.PileIntersectionTopEle.Value.ToString("F2"));
                _db.InsertCell(); _db.Write((_layer.PileIntersectionTopEle - _layer.PileIntersectionBottomEle).Value.ToString("F2"));
                _db.InsertCell(); _db.Write(_layer.SoilLayerObject.DPSE_Qfi.Value.ToString("F2"));
                _db.InsertCell(); _db.Write(_layer.SoilLayerObject.DPSE_Qr.Value.ToString("F2"));
                _db.InsertCell(); _db.Write(_layer.SoilLayerObject.Xii.Value.ToString("F2"));
                _db.EndRow();
            }
            _db.EndTable();
            SetToPDIWTTable(_table);
            _db.Writeln();
        }
        private void SteelTubePileCalculationTable(PDIWT_BearingCapacity_PileInfo _pile)
        {
            SetToMainBody(_db.ParagraphFormat);
            _db.Writeln("计算结果汇总表如下：");
            SetToNoSpacing(_db.ParagraphFormat);
            var _headstr = new List<string> { "土层号", "土层", "层顶高程(m)", "土层厚度li(m)", "极限侧摩阻力标准值qfi(kPa)", "极限桩端阻力标准值qr(kPa)", "抗拔折减系数ξi" };
            var _table = _db.StartTable();
            foreach (var _cell in _headstr)
            {
                _db.InsertCell();
                _db.Write(_cell);
            }
            _db.EndRow();
            foreach (var _layer in _pile.PileSoilLayersInfo)
            {
                _db.InsertCell(); _db.Write(_layer.SoilLayerObject.Number);
                _db.InsertCell(); _db.Write(_layer.SoilLayerObject.Name);
                _db.InsertCell(); _db.Write(_layer.PileIntersectionTopEle.Value.ToString("F2"));
                _db.InsertCell(); _db.Write((_layer.PileIntersectionTopEle - _layer.PileIntersectionBottomEle).Value.ToString("F2"));
                _db.InsertCell(); _db.Write(_layer.SoilLayerObject.TPSP_Qfi.Value.ToString("F2"));
                _db.InsertCell(); _db.Write(_layer.SoilLayerObject.TPSP_Qr.Value.ToString("F2"));
                _db.InsertCell(); _db.Write(_layer.SoilLayerObject.Xii.Value.ToString("F2"));
                _db.EndRow();
            }
            _db.EndTable();
            SetToPDIWTTable(_table);
            _db.Writeln();
        }
        private void CastInSituPileCalculationTable(PDIWT_BearingCapacity_PileInfo _pile)
        {
            SetToMainBody(_db.ParagraphFormat);
            _db.Writeln("计算结果汇总表如下：");
            SetToNoSpacing(_db.ParagraphFormat);
            var _headstr = new List<string> { "土层号", "土层", "层顶高程(m)", "尺寸效应系数ψsi","土层厚度li(m)", "极限侧摩阻力标准值qfi(kPa)","尺寸效应系数ψp", "极限桩端阻力标准值qr(kPa)", "抗拔折减系数ξi" };
            var _table = _db.StartTable();
            foreach (var _cell in _headstr)
            {
                _db.InsertCell();
                _db.Write(_cell);
            }
            _db.EndRow();
            foreach (var _layer in _pile.PileSoilLayersInfo)
            {
                _db.InsertCell(); _db.Write(_layer.SoilLayerObject.Number);
                _db.InsertCell(); _db.Write(_layer.SoilLayerObject.Name);
                _db.InsertCell(); _db.Write(_layer.PileIntersectionTopEle.Value.ToString("F2"));
                _db.InsertCell(); _db.Write(_layer.SoilLayerObject.CISP_Psisi.Value.ToString("F2"));
                _db.InsertCell(); _db.Write((_layer.PileIntersectionTopEle - _layer.PileIntersectionBottomEle).Value.ToString("F2"));
                _db.InsertCell(); _db.Write(_layer.SoilLayerObject.CISP_Qfi.Value.ToString("F2"));
                _db.InsertCell(); _db.Write(_layer.SoilLayerObject.CISP_Psip.Value.ToString("F2"));
                _db.InsertCell(); _db.Write(_layer.SoilLayerObject.CISP_Qr.Value.ToString("F2"));
                _db.InsertCell(); _db.Write(_layer.SoilLayerObject.Xii.Value.ToString("F2"));
                _db.EndRow();
            }
            _db.EndTable();
            SetToPDIWTTable(_table);
            _db.Writeln();
        }
        private void CastInSituGroutingPileCalculationTable(PDIWT_BearingCapacity_PileInfo _pile)
        {
            SetToMainBody(_db.ParagraphFormat);
            _db.Writeln("计算结果汇总表如下：");
            SetToNoSpacing(_db.ParagraphFormat);
            var _headstr = new List<string> { "土层号", "土层", "层顶高程(m)","增强系数βsi", "尺寸效应系数ψsi", "土层厚度li(m)", "极限侧摩阻力标准值qfi(kPa)", "增强系数βp","尺寸效应系数ψp", "极限桩端阻力标准值qr(kPa)", "抗拔折减系数ξi" };
            var _table = _db.StartTable();
            foreach (var _cell in _headstr)
            {
                _db.InsertCell();
                _db.Write(_cell);
            }
            _db.EndRow();
            foreach (var _layer in _pile.PileSoilLayersInfo)
            {
                _db.InsertCell(); _db.Write(_layer.SoilLayerObject.Number);
                _db.InsertCell(); _db.Write(_layer.SoilLayerObject.Name);
                _db.InsertCell(); _db.Write(_layer.PileIntersectionTopEle.Value.ToString("F2"));
                _db.InsertCell(); _db.Write(_layer.SoilLayerObject.CISAGP_Betasi.Value.ToString("F2"));
                _db.InsertCell(); _db.Write(_layer.SoilLayerObject.CISAGP_Psisi.Value.ToString("F2"));
                _db.InsertCell(); _db.Write((_layer.PileIntersectionTopEle - _layer.PileIntersectionBottomEle).Value.ToString("F2"));
                _db.InsertCell(); _db.Write(_layer.SoilLayerObject.CISAGP_Qfi.Value.ToString("F2"));
                _db.InsertCell(); _db.Write(_layer.SoilLayerObject.CISAGP_Betap.Value.ToString("F2"));
                _db.InsertCell(); _db.Write(_layer.SoilLayerObject.CISAGP_Psip.Value.ToString("F2"));
                _db.InsertCell(); _db.Write(_layer.SoilLayerObject.CISAGP_Qr.Value.ToString("F2"));
                _db.InsertCell(); _db.Write(_layer.SoilLayerObject.Xii.Value.ToString("F2"));
                _db.EndRow();
            }
            _db.EndTable();
            SetToPDIWTTable(_table);
            _db.Writeln();
        }

        readonly string _drivenSealPileFormula = @"Q_{d}=\frac{1}{\gamma _{R}}\left(U\sum q_{fi}l{i}+q_{R}A \right )";
        readonly string _steelandtubepileFormula = @"Q_{d}=\frac{1}{\gamma _{R}}\left(U\sum q_{fi}l{i}+\eta  q_{R}A \right )";
        readonly string _castinsitupileFormula = @"Q_{d}=\frac{1}{\gamma _{R}}\left(U\sum \psi _{si}q_{fi}l{i}+\psi_{p} q_{R}A \right )";
        readonly string _castinsitugroutingpileForumla = @"Q_{d}=\frac{1}{\gamma _{R}}\left(U\sum \beta_{si} \psi _{si}q_{fi}l{i}+ \beta_{p}\psi_{p} q_{R}A \right )";
        readonly string _liftupFormula = @"T_{d} = \frac{1}{\gamma _{R}}\left(U\sum \xi _{i}q_{fi}l_{i}+Gcos\alpha \right )";
        PDIWT_CalculationNoteInfo _calInfo;
        ViewModel.BearingCapacityViewModel _vm;
        DocumentBuilder _db;
    }

    public class PDIWT_CalculationNoteInfo
    {
        public string ProjectName { get; set; }
        public PDIWT_ProjectPhase ProjectPhase { get; set; }
        public string NumberOfVolume { get; set; }
        public string CalculatedItemName { get; set; }

        public string Designer { get; set; }
        public DateTime DesignDate { get; set; }
        public string Checker { get; set; }
        public DateTime CheckDate { get; set; }
        public string Reviewer { get; set; }
        public DateTime ReviewDate { get; set; }

        private static readonly Dictionary<PDIWT_ProjectPhase, string> _nameDict = new Dictionary<PDIWT_ProjectPhase, string>
        {
            {PDIWT_ProjectPhase.Preliminary_Feasibility_Study, Resources.PP_PreliminaryFeasibilityStudy},
            {PDIWT_ProjectPhase.Feasibility_Study, Resources.PP_FeasibilityStudy },
            {PDIWT_ProjectPhase.PreliminaryDesign,Resources.PP_PreliminaryDesign },
            {PDIWT_ProjectPhase.Construction_Design, Resources.PP_ConstructionDesign }
        };

        public static string GetProjectPhaseString(PDIWT_ProjectPhase pDIWT_ProjectPhase)
        {
            return _nameDict[pDIWT_ProjectPhase];
        }
        public static PDIWT_ProjectPhase GetProjectPhaseEnumFromString(string _phasestring)
        {
            return _nameDict.Where(e => e.Value == _phasestring).First().Key;
        }
    }

}
