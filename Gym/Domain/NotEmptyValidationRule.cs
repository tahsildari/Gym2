﻿using System.Globalization;
using System.Windows.Controls;

namespace Gym.Domain
{
    public class NotEmptyValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return string.IsNullOrWhiteSpace((value ?? "").ToString())
                ? new ValidationResult(false, "اجباری")
                : ValidationResult.ValidResult;
        }
    }
}
