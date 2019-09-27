using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using System.Windows.Forms;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Controls;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;

using PDIWT.Formulas;
using PDIWT_PiledWharf_Core.Model.Tools;
using PDIWT.Resources.Localization.MainModule;
using BD = Bentley.DgnPlatformNET;
using BM = Bentley.MstnPlatformNET;
using BG = Bentley.GeometryNET;
using PDIWT_PiledWharf_Core_Cpp;
using PDIWT_PiledWharf_Core.Model;

namespace PDIWT_PiledWharf_Core.ViewModel
{
    public class PileFrameDividerViewModel : ViewModelBase
    {
        public PileFrameDividerViewModel()
        {
#if DEBUG
            WharfLength = 808;
#endif
            Sections = new ObservableCollection<WharfSection>()
            {
                new WharfSection(){LeftLength = 2.25, Span=8, Number = 6, RightLength = 2.25}
            };
        }

        /// <summary>
        /// The <see cref="WharfLength" /> property's name.
        /// </summary>
        public const string WharfLengthPropertyName = "WharfLength";

        private double _wharfLength = 0;

        /// <summary>
        /// Sets and gets the WharfLength property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double WharfLength
        {
            get
            {
                return _wharfLength;
            }
            set
            {
                Set(() => WharfLength, ref _wharfLength, value);
            }
        }
        public ObservableCollection<WharfSection> Sections { get; set; }

        /// <summary>
        /// The <see cref="SelectedWharfSection" /> property's name.
        /// </summary>
        public const string SelectedWharfSectionPropertyName = "SelectedWharfSection";

        private WharfSection _selectedWharfSection = null;

        /// <summary>
        /// The <see cref="ResultText" /> property's name.
        /// </summary>
        public const string ResultTextPropertyName = "ResultText";

        private string _resultText = string.Empty;

        /// <summary>
        /// Sets and gets the ResultText property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string ResultText
        {
            get
            {
                return _resultText;
            }

            set
            {
                if (_resultText == value)
                {
                    return;
                }

                _resultText = value;
                RaisePropertyChanged(() => ResultText);
            }
        }

        /// <summary>
            /// The <see cref="SelectedMethod" /> property's name.
            /// </summary>
        public const string SelectedMethodPropertyName = "SelectedMethod";

        private LastSectionHandleMethod _selectedMethod = LastSectionHandleMethod.Separate;

        /// <summary>
        /// Sets and gets the SelectedMethod property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public LastSectionHandleMethod SelectedMethod
        {
            get
            {
                return _selectedMethod;
            }
            set
            {
                Set(() => SelectedMethod, ref _selectedMethod, value);
            }
        }

        /// <summary>
        /// Sets and gets the SelectedWharfSection property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public WharfSection SelectedWharfSection
        {
            get
            {
                return _selectedWharfSection;
            }
            set
            {
                Set(() => SelectedWharfSection, ref _selectedWharfSection, value);
            }
        }

        private RelayCommand _add;

        /// <summary>
        /// Gets the Add.
        /// </summary>
        public RelayCommand Add
        {
            get
            {
                return _add
                    ?? (_add = new RelayCommand(ExecuteAdd));
            }
        }

        private void ExecuteAdd()
        {
            Sections.Add(new WharfSection());
        }

        private RelayCommand _copy;

        /// <summary>
        /// Gets the Add.
        /// </summary>
        public RelayCommand Copy
        {
            get
            {
                return _copy
                    ?? (_copy = new RelayCommand(ExecuteCopy, () => SelectedWharfSection != null));
            }
        }

        private void ExecuteCopy()
        {
            Sections.Add(new WharfSection()
            {
                LeftLength = SelectedWharfSection.LeftLength,
                Span = SelectedWharfSection.Span,
                Number = SelectedWharfSection.Number + 1,
                RightLength = SelectedWharfSection.RightLength
            });
            SelectedWharfSection = Sections.Last();
        }

        private RelayCommand _delete;

        /// <summary>
        /// Gets the Delete.
        /// </summary>
        public RelayCommand Delete
        {
            get
            {
                return _delete
                    ?? (_delete = new RelayCommand(ExecuteDelete, () => SelectedWharfSection != null));
            }
        }

        private void ExecuteDelete()
        {
            int _index = Sections.IndexOf(SelectedWharfSection);
            var _selected = SelectedWharfSection;
            if (Sections.Count == 1)
            {
                SelectedWharfSection = null;
            }
            else
            {
                if (_index == 0)
                    SelectedWharfSection = Sections[1];
                else
                    SelectedWharfSection = Sections[_index - 1];

            }
            Sections.Remove(_selected);
        }
        private RelayCommand _calculate;

        /// <summary>
        /// Gets the Calculate.
        /// </summary>
        public RelayCommand Calculate
        {
            get
            {
                return _calculate
                    ?? (_calculate = new RelayCommand(ExecuteCalculate));
            }
        }

        private void ExecuteCalculate()
        {
            ResultText = string.Empty;
            List<List<WharfSection>> _res = new List<List<WharfSection>>();

            StringBuilder _sb = new StringBuilder();
            int _index = 1;
            foreach (var _section in Sections)
            {
                double _sectionLength = _section.GetSectionLength();
                int _numberofSection = (int)Math.Truncate(_wharfLength / _sectionLength);

                int _maxNumberNormalSection;
                if (SelectedMethod == LastSectionHandleMethod.Separate)
                    _maxNumberNormalSection = _numberofSection;
                else
                    _maxNumberNormalSection = _numberofSection - 1;

                double _lastsectionLength = _wharfLength - _maxNumberNormalSection * _sectionLength;

                int _pileSpanNumberInLastSection = (int)Math.Truncate((_lastsectionLength - _section.LeftLength - _section.RightLength) / _section.Span);

                double _residueLength = _lastsectionLength - _pileSpanNumberInLastSection * _section.Span - _section.LeftLength - _section.RightLength;

                List<WharfSection> _resultsSections = new List<WharfSection>();

                var _firstSection = (WharfSection)_section.Clone();
                _firstSection.LeftLength += _residueLength / 2;
                _resultsSections.Add(_firstSection);

                for (int i = 0; i < _maxNumberNormalSection - 1; i++)
                {
                    _resultsSections.Add((WharfSection)_section.Clone());
                }
                var _lastSection = (WharfSection)_section.Clone();
                _lastSection.Number = _pileSpanNumberInLastSection;
                _lastSection.RightLength += _residueLength / 2;
                _resultsSections.Add(_lastSection);

                _sb.AppendLine($"*********** No.{_index} combination***********");
                _sb.AppendLine($"The Wharf's total length is {WharfLength}m");
                _sb.AppendLine($"Wharf is divided into {_resultsSections.Count} Sections. Each section is list Below:");
                _sb.AppendLine($"[No.] [Left Length] [Frame span] [Number of Frame] [Right Length] [Total Length]");
                int _numberofResult = 1;
                foreach (var _result in _resultsSections)
                {
                    _sb.AppendFormat("{0,-10:D2} {1,-20:F2} {2,-20:F2} {3,-20:F2} {4,-20:F2} {5,-20:F2}\n", _numberofResult, _result.LeftLength, _result.Span, _result.Number, _result.RightLength, _result.GetSectionLength());
                    _numberofResult++;
                }
                _sb.AppendLine($"Total Length: {_resultsSections.Select(s => s.GetSectionLength()).Sum()}");
                _sb.AppendLine();
                _index++;

                _res.Add(_resultsSections);
            }
            MessengerInstance.Send(_res);

            ResultText += _sb.ToString();
        }
    }
}
