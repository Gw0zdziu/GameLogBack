using System.Data;
using FluentValidation;
using GameLogBack.Dtos.User;

namespace GameLogBack.Validators.User;

public class RecoveryUpdatePasswordDtoValidator : AbstractValidator<RecoveryUpdatePasswordDto>
{
    public RecoveryUpdatePasswordDtoValidator()
    {
        RuleFor(x => x.NewPassword).NotEmpty().WithMessage("New password is required");

        RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage("Confirm password is required");
        RuleFor(x => x.NewPassword).Equal(x => x.NewPassword).WithMessage("New password and confirm password do not match");

        RuleFor(x => x.Token).NotEmpty().WithMessage("Token is required");
        RuleFor(x => x.Token).Length(4).WithMessage("Token is too long");
    }
}
