using FluentValidation;
using GameLogBack.Dtos.Game.RequestDto;

namespace GameLogBack.Validators.Game;

public class GamePutDtoValidator : AbstractValidator<GamePutDto>
{
    public GamePutDtoValidator()
    {
        RuleFor(x => x.GameName).NotEmpty().WithMessage("Game name cannot be empty");
        RuleFor(x => x.YearPlayed).NotEmpty().WithMessage("Year played cannot be empty");
    }

}
