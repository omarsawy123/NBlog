using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos;
using FluentValidation;

namespace Application.Services.ArticleService.Validators
{
    public class ArticleQueryValidator : AbstractValidator<ArticleFilterQuery>
    {
        public ArticleQueryValidator()
        {
            RuleFor(x => x.SearchKey).NotEmpty().NotNull().WithMessage("Search key is required");
        }
    }
}
