using FluentValidation;
using GameLogBack.Dtos.Category;
using GameLogBack.Dtos.Category.RequestDto;

namespace GameLogBack.Validators;

public class CategoryPutDtoValidator : AbstractValidator<CategoryPutDto>
{
    public CategoryPutDtoValidator()
    {
        RuleFor(x => x.CategoryName).NotEmpty().WithMessage("Category name cannot be empty");
    }

}
