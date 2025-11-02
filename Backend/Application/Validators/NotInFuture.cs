using System.ComponentModel.DataAnnotations;

public class NotInFutureAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value == null) return true; // [Required] should handle nulls

        if (value is DateOnly dateValue)
        {
            return dateValue <= DateOnly.FromDateTime(DateTime.Now);
        }

        return false;
    }

    public override string FormatErrorMessage(string name)
        => $"{name} cannot be in the future.";
}
