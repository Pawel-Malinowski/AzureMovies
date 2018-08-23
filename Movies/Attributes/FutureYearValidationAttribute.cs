using System;
using System.ComponentModel.DataAnnotations;

namespace Movies.Attributes
{
    public class FutureYearValidationAttribute : ValidationAttribute
    {
        public override string FormatErrorMessage(string name)
        {
            return $"Field {name} is a future year which is not allowed";
        }

        public override bool IsValid(object value)
        {
            int intValue = (int)value;

            return intValue <= DateTime.Today.Year;
        }
    }
}
