using System.ComponentModel.DataAnnotations;

namespace tl2_parcial2_2025_Gonz0x.ViewModels
{
    public class TotalComplejidadAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is int complejidad)
            {
                if (SumaTotalComplejidad() > 50)
                {
                    return new ValidationResult("La suma total de complejidad no puede superar 50.");
                }
            }
            return ValidationResult.Success;
        }
    }
}