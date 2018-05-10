using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Gym.Domain
{
    public class BeANumberValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            int val;
            return int.TryParse(value.ToString(), out val) && val == 0
                ? new ValidationResult(false, "حداقل تعداد مورد قبول : 1")
                : ValidationResult.ValidResult;
        }
    }
}

