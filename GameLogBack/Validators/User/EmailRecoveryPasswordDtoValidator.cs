using FluentValidation;
using GameLogBack.Dtos.User.RequestDto;

namespace GameLogBack.Validators.User;

public class EmailRecoveryPasswordDtoValidator : AbstractValidator<EmailRecoveryPasswordDto>
{
    public EmailRecoveryPasswordDtoValidator()
    {
        RuleFor(x => x.UserEmail).NotEmpty().WithMessage("Email is required");
        RuleFor(x => x.UserEmail).EmailAddress().WithMessage("Invalid email");
    }
}
