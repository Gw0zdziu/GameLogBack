using FluentValidation;
using GameLogBack.Dtos.Category;
using GameLogBack.Dtos.Category.RequestDto;

namespace GameLogBack.Validators;

public class CategoryPostDtoValidator : AbstractValidator<CategoryPostDto>
{
    public CategoryPostDtoValidator()
    {
        RuleFor(x => x.CategoryName).NotEmpty().WithMessage("Category name cannot be empty");
    }
}
