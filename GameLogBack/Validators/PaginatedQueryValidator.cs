using FluentValidation;
using GameLogBack.Dtos.PaginatedQuery;

namespace GameLogBack.Validators;

public class PaginatedQueryValidator : AbstractValidator<PaginatedQuery>
{
    public PaginatedQueryValidator()
    {
        RuleFor(x => x.PageSize)
            .NotNull().WithMessage("{PropertyName} is required")
            .InclusiveBetween(5,20).WithMessage("Page size must be between {From} and {To}");
        
        
        RuleFor(x => x.PageNumber)
            .NotNull().WithMessage("{PropertyName} is required")
            .GreaterThanOrEqualTo(1).WithMessage("The minimum page number is 1");

    }
}