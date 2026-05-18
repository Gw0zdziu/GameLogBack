using System.Data;
using FluentValidation;
using GameLogBack.Dtos.User;

namespace GameLogBack.Validators.User;

public class ConfirmCodeDtoValidator : AbstractValidator<ConfirmCodeDto>
{
    public ConfirmCodeDtoValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required");

        RuleFor(x => x.ConfirmCode).NotNull().WithMessage("Confirm Code cannot be null");
        RuleFor(x => x.ConfirmCode).Length(4).WithMessage("Confirm Code cannot exceed 4 characters");
        RuleFor(x => x.ConfirmCode).Matches("^[0-9]+$").WithMessage("ConfirmCode must be digits only");
    }
}
