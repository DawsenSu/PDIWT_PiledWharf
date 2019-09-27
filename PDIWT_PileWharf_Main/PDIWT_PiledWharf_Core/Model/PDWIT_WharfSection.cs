using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace PDIWT_PiledWharf_Core.Model
{
    public class WharfSection : ObservableObject, ICloneable
    {
        /// <summary>
        /// The <see cref="Span" /> property's name.
        /// </summary>
        public const string SpanPropertyName = "Span";

        private int _span = 0;

        /// <summary>
        /// Sets and gets the Span property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int Span
        {
            get
            {
                return _span;
            }
            set
            {
                Set(() => Span, ref _span, value);
            }
        }
        /// <summary>
        /// The <see cref="Number" /> property's name.
        /// </summary>
        public const string NumberPropertyName = "Number";

        private double _number = 0;

        /// <summary>
        /// Sets and gets the Number property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double Number
        {
            get
            {
                return _number;
            }
            set
            {
                Set(() => Number, ref _number, value);
            }
        }
        /// <summary>
        /// The <see cref="LeftLength" /> property's name.
        /// </summary>
        public const string LeftLengthPropertyName = "LeftLength";

        private double _leftLength = 0;

        /// <summary>
        /// Sets and gets the LeftLength property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double LeftLength
        {
            get
            {
                return _leftLength;
            }
            set
            {
                Set(() => LeftLength, ref _leftLength, value);
            }
        }
        /// <summary>
        /// The <see cref="RightLength" /> property's name.
        /// </summary>
        public const string RightLengthPropertyName = "RightLength";

        private double _rightLength = 0;

        /// <summary>
        /// Sets and gets the RightLength property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double RightLength
        {
            get
            {
                return _rightLength;
            }
            set
            {
                Set(() => RightLength, ref _rightLength, value);
            }
        }

        public double GetSectionLength()
        {
            return LeftLength + Span * Number + RightLength;
        }

        public object Clone()
        {
            return new WharfSection() { RightLength = RightLength, Number = Number, Span = Span, LeftLength = LeftLength };
        }
    }
}
