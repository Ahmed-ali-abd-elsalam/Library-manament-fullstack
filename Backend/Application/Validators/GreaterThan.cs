using System.ComponentModel.DataAnnotations;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class GreaterThanAttribute : ValidationAttribute
{
    private readonly double _minValue;

    public GreaterThanAttribute(double minValue)
    {
        _minValue = minValue;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
            return ValidationResult.Success; // nulls handled by [Required] if needed

        try
        {
            double doubleValue = Convert.ToDouble(value);
            if (doubleValue <= _minValue)
            {
                return new ValidationResult(
                    $"{validationContext.DisplayName} must be greater than {_minValue}."
                );
            }
        }
        catch (FormatException)
        {
            return new ValidationResult($"{validationContext.DisplayName} must be a numeric value.");
        }

        return ValidationResult.Success;
    }
}
