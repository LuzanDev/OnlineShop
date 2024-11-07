using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Models.CustomAttribute
{
    public class RequiredIfAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;
        private readonly object _comparisonValue;

        public RequiredIfAttribute(string comparisonProperty, object comparisonValue)
        {
            _comparisonProperty = comparisonProperty;
            _comparisonValue = comparisonValue;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Получаем значение свойства для сравнения
            var comparisonPropertyInfo = validationContext.ObjectType.GetProperty(_comparisonProperty);
            if (comparisonPropertyInfo == null)
            {
                return new ValidationResult($"Unknown property: {_comparisonProperty}");
            }

            var comparisonValue = comparisonPropertyInfo.GetValue(validationContext.ObjectInstance);

            // Проверяем, равно ли значение свойству, и если да, проверяем, заполнено ли текущее значение
            if (comparisonValue != null && comparisonValue.Equals(_comparisonValue))
            {
                if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                {
                    return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} is required.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
