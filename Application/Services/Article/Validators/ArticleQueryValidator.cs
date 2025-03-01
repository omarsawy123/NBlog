using Application.Dtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.ArticleService.Validators
{
    internal class ArticleQueryValidator : AbstractValidator<ArticleFilterQuery>
    {
        public ArticleQueryValidator() {

            RuleFor(x => x.SearchKey)
                .NotEmpty()
                .NotNull()
                .WithMessage("Search key is required");

        }

    }
}
