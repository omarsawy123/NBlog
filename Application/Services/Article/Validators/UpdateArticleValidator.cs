using Application.Dtos;
using FluentValidation;

namespace Application.Services.Article.Validators
{
    public class UpdateArticleValidator : AbstractValidator<UpdateArticleDto>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateArticleValidator"/> class with validation rules for updating articles.
        /// </summary>
        /// <remarks>
        /// This validator enforces the following rules for an <see cref="UpdateArticleDto"/>:
        /// <list type="bullet">
        ///   <item>
        ///     <description><c>ArticleId</c> must be greater than 0.</description>
        ///   </item>
        ///   <item>
        ///     <description><c>Title</c> is required and cannot exceed 100 characters.</description>
        ///   </item>
        ///   <item>
        ///     <description><c>SubHeading</c> is required and cannot exceed 200 characters.</description>
        ///   </item>
        ///   <item>
        ///     <description><c>Content</c> is required.</description>
        ///   </item>
        ///   <item>
        ///     <description><c>UserId</c> must be greater than 0.</description>
        ///   </item>
        /// </list>
        /// </remarks>
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
