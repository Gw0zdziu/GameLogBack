using FluentValidation;
using GameLogBack.Dtos.Category.RequestDto;

namespace GameLogBack.Validators.Category;

public class CategoryPostDtoValidator : AbstractValidator<CategoryPostDto>
{
    public CategoryPostDtoValidator()
    {
        RuleFor(x => x.CategoryName).NotEmpty().WithMessage("Category name cannot be empty");
    }
}
