using System;
using System.Collections.Generic;
using System.Linq;

using System.IO;
using Aspose.Words;
using WpfMath;
using PDIWT.Resources.Localization.MainModule;

using BD = Bentley.DgnPlatformNET;

namespace PDIWT_PiledWharf_Core.Model
{

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

        public void Save()
        {
            _document.Save(OutFileInfo.FullName);

        }
        public FileInfo TemplateInfo { get; private set; }
        public FileInfo OutFileInfo { get; private set; }

        protected Document _document;
    }

    public class PDIWT_CurrentForceCalculationNoteBuilder : PDIWT_CalculationNoteBuilderBase 
    {
        public PDIWT_CurrentForceCalculationNoteBuilder(string templateName,string outFilePath, PDIWT_CalculationNoteInfo info, ViewModel.CurrentForceViewModel viewModel) : base(templateName, outFilePath)
        {
            _calculationNoteInfo = info;
            _vm = viewModel;
        }

        public override void Build()
        {
            BuildCoverPart();
            BuildMainPart();
            Save();           
        }

        private void BuildCoverPart()
        {
            string[] _mergedFields = {"ProjectName", "ProjectPhase","NumberOfVolume","CalculatedItemName","Designer",
                                    "DesignDate","Checker","CheckDate","Reviewer","ReviewDate"};
            string[] _infos = { _calculationNoteInfo.ProjectName, PDIWT_CalculationNoteInfo.GetProjectPhaseString(_calculationNoteInfo.ProjectPhase), _calculationNoteInfo.NumberOfVolume,
                                _calculationNoteInfo.CalculatedItemName,
                                _calculationNoteInfo.Designer, _calculationNoteInfo.DesignDate.ToString("yyyy-MM-dd"),
                                _calculationNoteInfo.Checker, _calculationNoteInfo.CheckDate.ToString("yyyy-MM-dd"),
                                _calculationNoteInfo.Reviewer, _calculationNoteInfo.ReviewDate.ToString("yyyy-MM-dd")
                               };
            _document.MailMerge.UseNonMergeFields = true;
            _document.MailMerge.Execute(_mergedFields, _infos);
        }

        private void BuildMainPart()
        {
            DocumentBuilder _docBuilder = new DocumentBuilder(_document);

            // The character of font for body part: 
            //小四号 华文中宋,(行间距：1.1倍行距  | 段前间距：0.5行)
            _docBuilder.MoveToBookmark("InsertPoint");

            Font _bodyFont = _docBuilder.Font;
            _bodyFont.Size = 12;
            _bodyFont.Name = "华文中宋";

            ParagraphFormat _bodyParagraphFormt = _docBuilder.ParagraphFormat;
            _bodyParagraphFormt.LineSpacing = 13.2;
            _bodyParagraphFormt.SpaceBefore = 6;
            _bodyParagraphFormt.SpaceAfter = 0;

            _bodyFont.Bold = true;
            _docBuilder.Writeln("1. 桩基水流力计算");
            _bodyFont.Bold = false;
            _docBuilder.Writeln($"\t桩基顶面高程:{_vm.PileTopElevation}m");
            _docBuilder.Writeln($"\t极端高水位:{_vm.HAT}m");
            _docBuilder.Writeln($"\t基桩泥面高程:{_vm.SoilElevation}m");
            _docBuilder.Writeln($"\t基桩投影宽度b:{_vm.ProjectedWidth}m");
            _docBuilder.Writeln($"\t形状:{_vm.SelectedShape.Shape}");
            _docBuilder.Writeln($"\t水流速: V = {_vm.CurrentVelocity}m/s");
            _docBuilder.Writeln($"\t横向桩中心间距: B = {_vm.PileHorizontalCentraSpan}m");
            _docBuilder.Writeln($"\t纵向桩中心间距: L = {_vm.PileVerticalCentraSpan}m");
            _docBuilder.Writeln($"\t水密度: ρ = {_vm.WaterDensity / 1000}t/m3");
            _bodyFont.Bold = true;
            _docBuilder.Writeln("2. 计算参数");
            _bodyFont.Bold = false;
            _docBuilder.Writeln($"\t高度: H = {_vm.PileHeight}m");
            _docBuilder.Writeln($"\t投影面积: A = {_vm.ProjectedArea}m2/m");
            _docBuilder.Writeln($"\t(1) 水流阻力系数: 水流阻力系数Cw可按表13.0.3-1选用");
            _bodyParagraphFormt.Alignment = ParagraphAlignment.Center;
            _docBuilder.Writeln($"\t\tCW = {_vm.CurrentResistentCoeff}");
            _bodyParagraphFormt.Alignment = ParagraphAlignment.Left;
            _docBuilder.Writeln($"\t(2) 遮流影响系数: 遮流影响系数m1可按表13.0.3-2选用");
            _bodyParagraphFormt.Alignment = ParagraphAlignment.Center;
            _docBuilder.Writeln($"\t\tm1 = {_vm.ShelteringCoeff}");
            _bodyParagraphFormt.Alignment = ParagraphAlignment.Left;
            _docBuilder.Writeln($"\t(3) 淹没深度影响系数: 淹没深度影响系数n1可按表13.0.3-3选用");
            _bodyParagraphFormt.Alignment = ParagraphAlignment.Center;
            _docBuilder.Writeln($"\t\tn1 = {_vm.SubmergedCoeff}");
            _bodyParagraphFormt.Alignment = ParagraphAlignment.Left;
            _docBuilder.Writeln($"\t(4) 水深影响系数: 水深影响系数n2可按表13.0.3-4选用");
            _bodyParagraphFormt.Alignment = ParagraphAlignment.Center;
            _docBuilder.Writeln($"\t\tn2 = {_vm.WaterDepthCoeff}");
            _bodyParagraphFormt.Alignment = ParagraphAlignment.Left;
            _docBuilder.Writeln($"\t(5) 横向影响系数: 横向影响系数m2可按表13.0.3-4选用");
            _bodyParagraphFormt.Alignment = ParagraphAlignment.Center;
            _docBuilder.Writeln($"\t\tm2 = {_vm.HorizontalAffectCoeff}");
            _bodyParagraphFormt.Alignment = ParagraphAlignment.Left;
            _docBuilder.Writeln($"\t(6) 斜向影响系数: 影响系数m3可按表13.0.3-4选用");
            _bodyParagraphFormt.Alignment = ParagraphAlignment.Center;
            _docBuilder.Writeln($"\t\tm3 = {_vm.IncliningAffectCoeff}");
            _bodyParagraphFormt.Alignment = ParagraphAlignment.Left;


            _bodyFont.Bold = true;
            _docBuilder.Writeln("3. 计算结果");
            _bodyFont.Bold = false;
            _docBuilder.Writeln($"\t桩基所受水流力准值:");

            var _parser = new TexFormulaParser();
            string _frontPileLatex = string.Format("F_{{w}} = C_{{w}}\\frac{{\\rho}}{{2}}V^2A={0:N2}kN", _vm.CurrentForceForFrontPile);
            var _forumla = _parser.Parse(_frontPileLatex);
            var _bytes = _forumla.RenderToPng(18, 0, 0, "Arial");
            _docBuilder.Write("\t前排:"); _docBuilder.InsertImage(_bytes);_docBuilder.Writeln();
            string _rearPilelatex = string.Format("F_{{w}} = C_{{w}}\\frac{{\\rho}}{{2}}V^2A={0:N2}kN", _vm.CurrentForceForRearPile);
            _forumla = _parser.Parse(_rearPilelatex);
            _bytes = _forumla.RenderToPng(18, 0, 0, "Arial");
            _docBuilder.Write("\t后排:"); _docBuilder.InsertImage(_bytes);_docBuilder.Writeln();
            _docBuilder.Writeln(string.Format("\t作用点:{0:N2}m", _vm.ActionPointForFrontPile));
        }

        private ViewModel.CurrentForceViewModel _vm;
        private PDIWT_CalculationNoteInfo _calculationNoteInfo;
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
            { PDIWT_ProjectPhase.Preliminary_Feasibility_Study, Resources.PreliminaryFeasibilityStudy},
            {PDIWT_ProjectPhase.Feasibility_Study, Resources.FeasibilityStudy },
            {PDIWT_ProjectPhase.PreliminaryDesign,Resources.PreliminaryDesign },
            {PDIWT_ProjectPhase.Construction_Design, Resources.ConstructionDesign }
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

    public enum PDIWT_ProjectPhase
    {
        Preliminary_Feasibility_Study,
        Feasibility_Study,
        PreliminaryDesign,
        Construction_Design
    }
}
