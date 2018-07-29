using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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
            int intValue = (int) value;

            return intValue <= DateTime.Today.Year;
        }
    }
}
