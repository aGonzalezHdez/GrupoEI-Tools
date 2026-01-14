namespace Application.Common.Validator;

public static class InputValidators
{
    public static bool IsValidUuid(string? value)
        => Guid.TryParse(value, out _);

    public static bool IsDigitsOnly(string? value)
        => !string.IsNullOrWhiteSpace(value) && value.All(char.IsDigit);
}