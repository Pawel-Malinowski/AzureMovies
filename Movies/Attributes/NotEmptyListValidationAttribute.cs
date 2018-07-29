using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Movies.Attributes
{
    public class NotNullOrEmptyCollectionAttribute : ValidationAttribute
    {
        public override string FormatErrorMessage(string name)
        {
            return $"{name} collection cannot be null or empty.";
        }

        public override bool IsValid(object value)
        {
            if (value is ICollection collection)
            {
                return collection.Count > 0;
            }

            return value is IEnumerable enumerable && enumerable.GetEnumerator().MoveNext();
        }
    }
}
