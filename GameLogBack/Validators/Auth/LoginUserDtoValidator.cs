using FluentValidation;
using GameLogBack.Dtos.Auth.RequestDto;

namespace GameLogBack.Validators.Auth;

public class LoginUserDtoValidator : AbstractValidator<LoginUserDto>
{
    public LoginUserDtoValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().WithMessage("UserName is required");

        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");
    }
}
