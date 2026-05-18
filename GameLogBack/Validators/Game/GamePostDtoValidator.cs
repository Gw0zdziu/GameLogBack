using System.Data;
using FluentValidation;
using GameLogBack.Dtos.Game.RequestDto;

namespace GameLogBack.Validators.Game;

public class GamePostDtoValidator : AbstractValidator<GamePostDto>
{
    public GamePostDtoValidator()
    {
        RuleFor(x => x.GameName).NotEmpty().WithMessage("Game name cannot be empty");
        RuleFor(x => x.YearPlayed).NotEmpty().WithMessage("Year played cannot be empty");
    }

}
