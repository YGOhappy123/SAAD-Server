using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace milktea_server.Utilities
{
    public class CustomPriceValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is int number)
            {
                if (number > 0 && number % 1000 == 0)
                {
                    return ValidationResult.Success;
                }
                return new ValidationResult("The number must be greater than 0 and divisible by 1000.");
            }

            return new ValidationResult("Invalid input, the value must be a number.");
        }
    }
}
