using Application.Dtos;
using FluentValidation;

namespace Application.Services.ArticleService.Validators
{
    public class CreateArticleValidator : AbstractValidator<CreateArticleDto>
    {
        public CreateArticleValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .NotNull()
                .WithMessage("Title is required")
                .MaximumLength(200)
                .WithMessage("Title cannot exceed 200 characters");

            RuleFor(x => x.SubHeading)
                .NotEmpty()
                .NotNull()
                .WithMessage("SubHeading is required")
                .MaximumLength(500)
                .WithMessage("SubHeading cannot exceed 500 characters");

            RuleFor(x => x.Content).NotEmpty().NotNull().WithMessage("Content is required");

            RuleFor(x => x.UserId).GreaterThan(0).WithMessage("UserId must be greater than 0");
        }
    }
}
