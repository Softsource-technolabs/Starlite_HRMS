using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace StarLine.Core.Common
{
    public class RequiredIfAttribute : ValidationAttribute, IClientModelValidator
    {
        private readonly string _dependentProperty;
        private readonly string _targetValue;

        public RequiredIfAttribute(string dependentProperty, string targetValue)
        {
            _dependentProperty = dependentProperty;
            _targetValue = targetValue;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var property = validationContext.ObjectType.GetProperty(_dependentProperty);
            if (property == null)
                return new ValidationResult($"Unknown property: {_dependentProperty}");

            var dependentValue = property.GetValue(validationContext.ObjectInstance, null)?.ToString();

            if (dependentValue == _targetValue && string.IsNullOrWhiteSpace(Convert.ToString(value)))
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }

        // 👇 Add client-side rules
        public void AddValidation(ClientModelValidationContext context)
        {
            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-requiredif", ErrorMessage);
            MergeAttribute(context.Attributes, "data-val-requiredif-dependentproperty", _dependentProperty);
            MergeAttribute(context.Attributes, "data-val-requiredif-targetvalue", _targetValue);
        }

        private bool MergeAttribute(IDictionary<string, string> attributes, string key, string value)
        {
            if (attributes.ContainsKey(key)) return false;
            attributes.Add(key, value);
            return true;
        }
    }
}
