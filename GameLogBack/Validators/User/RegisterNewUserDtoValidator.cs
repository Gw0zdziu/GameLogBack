using System.Data;
using FluentValidation;
using GameLogBack.Dtos.User;

namespace GameLogBack.Validators.User;

public class RegisterNewUserDtoValidator : AbstractValidator<RegisterNewUserDto>
{
    public RegisterNewUserDtoValidator()
    {
        RuleFor(x => x.Username).NotEmpty().WithMessage("Username is required");
        RuleFor(x => x.Username).MinimumLength(3).WithMessage("Username must be at least 3 characters long");

        RuleFor(x => x.FirstName).NotEmpty().WithMessage("First name is required");
        RuleFor(x => x.FirstName).MinimumLength(3).WithMessage("Firstname must be at least 3 characters long");

        RuleFor(x => x.LastName).NotEmpty().WithMessage("Last name is required");
        RuleFor(x => x.LastName).MinimumLength(3).WithMessage("Lastname must be at least 3 characters long");

        RuleFor(x => x.UserEmail).NotEmpty().WithMessage("Email is required");
        RuleFor(x => x.UserEmail).EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");
        RuleFor(x => x.Password).MinimumLength(3).WithMessage("Password must be at least 3 characters long");

        RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage("Confirm password is required");
        RuleFor(x => x.Password).MinimumLength(3).WithMessage("Password must be at least 3 characters long");
        RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage("Passwords don't match");

        RuleFor(x => x.InvitationCode).NotEmpty().WithMessage("Invitation code is required");
        RuleFor(x => x.InvitationCode).Length(4).WithMessage("Invitation code must be at least 4 characters long");
    }

}
