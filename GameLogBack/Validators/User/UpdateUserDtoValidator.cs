using FluentValidation;
using GameLogBack.Dtos.User;
using GameLogBack.Dtos.User.RequestDto;

namespace GameLogBack.Validators.User;

public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserDtoValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().WithMessage("First name is required");
        RuleFor(x => x.FirstName).MinimumLength(3).WithMessage("Firstname must be at least 3 characters long");

        RuleFor(x => x.LastName).NotEmpty().WithMessage("Last name is required");
        RuleFor(x => x.LastName).MinimumLength(3).WithMessage("Lastname must be at least 3 characters long");

        RuleFor(x => x.UserEmail).NotEmpty().WithMessage("Email is required");
        RuleFor(x => x.UserEmail).EmailAddress().WithMessage("Invalid email format");
    }
}
