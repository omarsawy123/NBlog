using FluentValidation;

namespace Application.Services.ArticleService.Validators
{
    public class ArticleIdValidator : AbstractValidator<int>
    {
        public ArticleIdValidator()
        {
            RuleFor(id => id).GreaterThan(0).WithMessage("Article ID must be greater than 0");
        }
    }
}
