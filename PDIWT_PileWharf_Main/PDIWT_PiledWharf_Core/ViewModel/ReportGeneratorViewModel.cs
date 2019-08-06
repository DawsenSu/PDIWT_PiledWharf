using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using PDIWT.Resources.Localization.MainModule;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using PDIWT_PiledWharf_Core.Model;
using System.Windows;

namespace PDIWT_PiledWharf_Core.ViewModel
{
    public class ReportGeneratorViewModel : ViewModelBase
    {
        public ReportGeneratorViewModel()
        {
            //_projectPhaseCategory = new List<string> { Resources.PreliminaryFeasibilityStudy, Resources.FeasibilityStudy,Resources.PreliminaryDesign,Resources.ConstructionDesign};
            _projectPhaseCategory = PDIWT.Resources.PDIWT_Helper.GetEnumDescriptionDictionary<Model.PDIWT_ProjectPhase>().Values.ToList();
            _selectedPhase = _projectPhaseCategory[0];
            _calculatedItemName = "桩基水流力计算";
            _designDate = DateTime.Now;
            _checkDate = DateTime.Now;
            _reviewDate = DateTime.Now;

            Messenger.Default.Register<NotificationMessage<CurrentForceViewModel>>(this, "ViewModelForReport", notification => _currentViewModelForReport = notification.Content);
        }


        private string _projectName;
        /// <summary>
        /// Property Description
        /// </summary>
        public string ProjectName
        {
            get { return _projectName; }
            set { Set(ref _projectName, value); }
        }


        private List<string> _projectPhaseCategory;
        /// <summary>
        /// Property Description
        /// </summary>
        public List<string> ProjectPhaseCategory
        {
            get { return _projectPhaseCategory; }
            set { Set(ref _projectPhaseCategory, value); }
        }

        private string _selectedPhase;
        /// <summary>
        /// Property Description
        /// </summary>
        public string SelectedPhase
        {
            get { return _selectedPhase; }
            set { Set(ref _selectedPhase, value); }
        }


        private string _numberOfVolume;
        /// <summary>
        /// Property Description
        /// </summary>
        public string NumberOfVolume
        {
            get { return _numberOfVolume; }
            set { Set(ref _numberOfVolume, value); }
        }

        private string _calculatedItemName;
        /// <summary>
        /// Property Description
        /// </summary>
        public string CalculatedItemName
        {
            get { return _calculatedItemName; }
            set { Set(ref _calculatedItemName, value); }
        }
        
        private string _designer;
        /// <summary>
        /// Property Description
        /// </summary>
        public string Designer
        {
            get { return _designer; }
            set { Set(ref _designer, value); }
        }

        private DateTime? _designDate;
        /// <summary>
        /// Property Description
        /// </summary>
        public DateTime? DesignDate
        {
            get { return _designDate; }
            set { Set(ref _designDate, value); }
        }

        private string _checker;
        /// <summary>
        /// Property Description
        /// </summary>
        public string Checker
        {
            get { return _checker; }
            set { Set(ref _checker, value); }
        }

        private DateTime? _checkDate;
        /// <summary>
        /// Property Description
        /// </summary>
        public DateTime? CheckDate
        {
            get { return _checkDate; }
            set { Set(ref _checkDate, value); }
        }

        private string _reviewer;
        /// <summary>
        /// Property Description
        /// </summary>
        public string Reviewer
        {
            get { return _reviewer; }
            set { Set(ref _reviewer, value); }
        }

        private DateTime? _reviewDate;
        /// <summary>
        /// Property Description
        /// </summary>
        public DateTime? ReviewDate
        {
            get { return _reviewDate; }
            set { Set(ref _reviewDate, value); }
        }


        private string _outFilePath;
        /// <summary>
        /// Property Description
        /// </summary>
        public string OutFilePath
        {
            get { return _outFilePath; }
            set { Set(ref _outFilePath, value); }
        }

        private RelayCommand _browseOutFilePath;

        /// <summary>
        /// Gets the BrowseOutFilePath.
        /// </summary>
        public RelayCommand BrowseOutFilePath
        {
            get
            {
                return _browseOutFilePath
                    ?? (_browseOutFilePath = new RelayCommand(ExecuteBrowseOutFilePath));
            }
        }

        private void ExecuteBrowseOutFilePath()
        {
            using (var _saveFileDialog = new SaveFileDialog())
            {
                _saveFileDialog.Title = Resources.ReportGenerator;
                _saveFileDialog.Filter = Resources.WordFileFilter;
                _saveFileDialog.DefaultExt = ".docx";
                _saveFileDialog.CheckPathExists = true;
                if(_saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    OutFilePath = _saveFileDialog.FileName;
                }
            }
        }

        private RelayCommand _generate;

        /// <summary>
        /// Gets the Generate.
        /// </summary>
        public RelayCommand Generate
        {
            get
            {
                return _generate
                    ?? (_generate = new RelayCommand(ExecuteGenerate, CanExcuteGenerate));
            }
        }

        private void ExecuteGenerate()
        {
            try
            {
                if (File.Exists(OutFilePath)) File.Delete(OutFilePath);
                PDIWT_CalculationNoteInfo _info = new PDIWT_CalculationNoteInfo
                {
                    ProjectName = this.ProjectName,
                    ProjectPhase = PDIWT_CalculationNoteInfo.GetProjectPhaseEnumFromString(SelectedPhase),
                    NumberOfVolume = NumberOfVolume,
                    CalculatedItemName = CalculatedItemName,
                    Designer = this.Designer,
                    DesignDate = DesignDate.GetValueOrDefault(),
                    Checker = Checker,
                    CheckDate = CheckDate.GetValueOrDefault(),
                    Reviewer = Reviewer,
                    ReviewDate = ReviewDate.GetValueOrDefault()
                };

                if (_currentViewModelForReport == null)
                    throw new NullReferenceException("currentforceviewmodel is null");

                PDIWT_CurrentForceCalculationNoteBuilder _docbuilder = new PDIWT_CurrentForceCalculationNoteBuilder("pdiwt_calculationnote.docx",
                            OutFilePath, _info, _currentViewModelForReport);
                _docbuilder.Build();
                System.Windows.Forms.MessageBox.Show(Resources.Success, Resources.Success, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message, Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private bool CanExcuteGenerate() => !string.IsNullOrEmpty(OutFilePath);

        private CurrentForceViewModel _currentViewModelForReport = null;
        
    }
}
