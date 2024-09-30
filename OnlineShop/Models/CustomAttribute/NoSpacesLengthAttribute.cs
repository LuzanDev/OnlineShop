using System.ComponentModel.DataAnnotations;
namespace OnlineShop.Models.CustomAttribute
{
    public class NoSpacesLengthAttribute : ValidationAttribute
    {
        private readonly int _minLength;
        private readonly int _maxLength;

        public NoSpacesLengthAttribute(int minLength, int maxLength)
        {
            _minLength = minLength;
            _maxLength = maxLength;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is string stringValue)
            {
                var trimmedLength = stringValue.Replace(" ", "").Length;

                if (trimmedLength < _minLength)
                {
                    return new ValidationResult($"Длина поля должна быть не менее {_minLength} символов, без учета пробелов.");
                }

                if (trimmedLength > _maxLength)
                {
                    return new ValidationResult($"Длина поля не должна превышать {_maxLength} символов, без учета пробелов.");
                }
            }

            return ValidationResult.Success;
        }
    }

}
