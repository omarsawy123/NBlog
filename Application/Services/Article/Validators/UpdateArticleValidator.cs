using Application.Dtos;
using FluentValidation;

namespace Application.Services.Article.Validators
{
    public class UpdateArticleValidator : AbstractValidator<UpdateArticleDto>
    {
        public UpdateArticleValidator()
        {
            RuleFor(x => x.ArticleId)
                .GreaterThan(0)
                .WithMessage("Article ID must be greater than 0");

            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Title is required")
                .MaximumLength(100)
                .WithMessage("Title cannot exceed 100 characters");

            RuleFor(x => x.SubHeading)
                .NotEmpty()
                .WithMessage("SubHeading is required")
                .MaximumLength(200)
                .WithMessage("SubHeading cannot exceed 200 characters");

            RuleFor(x => x.Content)
                .NotEmpty()
                .WithMessage("Content is required");

            RuleFor(x => x.UserId)
                .GreaterThan(0)
                .WithMessage("User ID must be greater than 0");
        }
    }
}
