using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace Movies.Attributes
{
    public class NotEmptyCollectionAttribute : ValidationAttribute
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
