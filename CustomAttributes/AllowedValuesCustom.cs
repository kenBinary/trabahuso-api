using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace trabahuso_api.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class AllowedValuesCustom : AllowedValuesAttribute
    {
        public AllowedValuesCustom(params object?[] values) : base(values)
        {

        }

        public string GetErrorMessage()
        {
            return $"Invalid SortBy value, value must be one of the following: {string.Join(", ", Values)}";
        }

        protected override ValidationResult? IsValid(
            object? value, ValidationContext validationContext)
        {

            if (value == null)
            {
                return ValidationResult.Success;
            }

            if (!Values.Contains(value))
            {
                Console.WriteLine("no contain");
                return new ValidationResult(GetErrorMessage());
            }

            return ValidationResult.Success;
        }
    }
}