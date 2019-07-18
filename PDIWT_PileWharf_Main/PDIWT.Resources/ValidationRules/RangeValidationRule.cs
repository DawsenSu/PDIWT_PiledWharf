using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Controls;

namespace PDIWT.Resources.ValidationRules
{
    public class RangeValidationRule : ValidationRule
    {
        public double Min { get; set; }

        public double Max { get; set; }



        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if(double.TryParse(value.ToString(), out double result))
            {
                if ((result < Min) || (result > Max))
                    return new ValidationResult(false, $"Please enter an number in the range of [{Min} , {Max}].");
                return ValidationResult.ValidResult;
            }
            else
            {
                return new ValidationResult(false, $"Illegal Characters {value.ToString()} or can't be converted to double type ");
            }

        }
    }
}
