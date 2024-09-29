using System.ComponentModel.DataAnnotations;

namespace EAD_Backend_Application__.NET.Helpers
{
    public class MinLengthCollectionAttribute : ValidationAttribute
    {
        private readonly int _minLength;

        public MinLengthCollectionAttribute(int minLength)
        {
            _minLength = minLength;
        }

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value is IList<string> list)
            {
                if (list.Count < _minLength)
                {
                    return new ValidationResult(ErrorMessage ?? $"The collection must contain at least {_minLength} items.");
                }
            }
            return ValidationResult.Success!;
        }
    }
}
