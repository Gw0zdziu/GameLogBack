using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using GameLogBack.Exceptions;

namespace GameLogBack.Validators;

public class PasswordValidationAttribute : ValidationAttribute
{
    private readonly string _displayName;

    public PasswordValidationAttribute(string displayName)
    {
        _displayName = displayName;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var password = value as string;
        if (password is null) throw new BadRequestException("Password is required");
        var containsNumberRegex = @"[0-9]{1,}@g";
        var containsUppercaseRegex = @"[A-Z]{1,}@g";
        var containsLowercaseRegex = @"[a-z]{1,}@g";
        var containsSpecialCharacterRegex = @"[^a-zA-Z0-9]{1,}";
        if (!Regex.IsMatch(password, containsNumberRegex))
            throw new BadRequestException($"{_displayName} must contain at least one number");

        if (!Regex.IsMatch(password, containsUppercaseRegex))
            throw new BadRequestException($"{_displayName} must contain at least one uppercase letter");

        if (!Regex.IsMatch(password, containsLowercaseRegex))
            throw new BadRequestException($"{_displayName} must contain at least one lowercase letter");

        if (!Regex.IsMatch(password, containsSpecialCharacterRegex))
            throw new BadRequestException(
                $"{_displayName} must contain at least one special character");

        return ValidationResult.Success;
    }
}