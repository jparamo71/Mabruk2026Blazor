using MabrukBlazor2026.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MabrukBlazor2026.Shared.Helpers
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class ConditionalValidation : ValidationAttribute
    {
        
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var productReading = (ProductPhysicalDto)validationContext.ObjectInstance;
            if (!productReading.Justified || productReading.Difference == 0)
                return ValidationResult.Success;

            var notes = value as string;

            if (string.IsNullOrWhiteSpace(notes))
            {
                return new ValidationResult("Error: Las observaciones son un dato requerido", new[] { validationContext.MemberName });
            }


            return ValidationResult.Success;
        }
    }
}
