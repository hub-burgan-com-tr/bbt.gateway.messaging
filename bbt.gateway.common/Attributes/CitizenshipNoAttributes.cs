using System.ComponentModel.DataAnnotations;

namespace bbt.gateway.common.Attributes
{
    public class CitizenshipNo : ValidationAttribute
    {
        private readonly int _minLength;
        private readonly int _maxLength;

        public CitizenshipNo(int minLength, int maxLength) : base("{0} length has to be between "+minLength+" and "+maxLength)
        {
            _minLength = minLength;
            _maxLength = maxLength;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            string actualValue = value.ToString();
            if(string.IsNullOrEmpty(actualValue))
                return ValidationResult.Success;

            if (actualValue.Length < _minLength || actualValue.Length > _maxLength)
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
            else
            {
                return ValidationResult.Success;
            }

        }
    }
}
