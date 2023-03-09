using System;
using System.ComponentModel.DataAnnotations;

namespace BlazorApp2.Shared.Models
{
    public class RequiredIfAttribute : ValidationAttribute
    {
        readonly RequiredAttribute _innerAttribute = new RequiredAttribute();
        private string _dependentProperty { get; }
        private object _targetValue { get; }

        public RequiredIfAttribute(string dependentProperty, object targetValue)
        {
            _dependentProperty = dependentProperty;
            _targetValue = targetValue;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var field = validationContext.ObjectType.GetProperty(_dependentProperty);
            if (field != null)
            {
                var dependentValue = field.GetValue(validationContext.ObjectInstance, null);
                if ((dependentValue == null && _targetValue == null) || dependentValue.Equals(_targetValue))
                {
                    if (!_innerAttribute.IsValid(value))
                    {
                        var name = validationContext.DisplayName;
                        var specificErrorMessage = ErrorMessage;
                        if (string.IsNullOrEmpty(specificErrorMessage))
                            specificErrorMessage = $"{name} is required.";

                        return new ValidationResult(specificErrorMessage, new[] { validationContext.MemberName });
                    }
                }
                return ValidationResult.Success;
            }
            return new ValidationResult(FormatErrorMessage(_dependentProperty));
        }
    }
}

