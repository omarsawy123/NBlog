using Application.Dtos;
using Application.Shared;
using Domain.Entites;
using FluentValidation;
using Infrastructure.Repos;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services.ArticleService
{
    public class ArticleService
    {
        private readonly BaseRepo<Article, int> _articleRepository;
        private readonly ILogger<ArticleService> _logger;
        private readonly IValidator<ArticleFilterQuery> _queryValidator;

        public ArticleService(BaseRepo<Article, int> articleRepository,
            ILogger<ArticleService> logger,
            IValidator<ArticleFilterQuery> queryValidator)
        {
            _articleRepository = articleRepository;
            _logger = logger;
            _queryValidator = queryValidator;
        }


        public async Task<Result<IEnumerable<ArticleDto>>> GetAllArticles(ArticleFilterQuery filterQuery)
        {
            try
            {

                var validQuery = await _queryValidator.ValidateAsync(filterQuery);

                if (!validQuery.IsValid)
                {
                    return Result<IEnumerable<ArticleDto>>.Failure(StatusCodes.Status400BadRequest, validQuery.Errors.Select(e => e.ErrorMessage));
                }

                var query = _articleRepository.GetAll()
                    .Where(a => a.Title.Contains(filterQuery.SearchKey) || a.SubHeading.Contains(filterQuery.SearchKey))
                    .Select(a => new ArticleDto
                    {
                        ArticleId = a.ArticleId,
                        Title = a.Title,
                        SubHeading = a.SubHeading,
                        AuthorName = a.User.UserName!,
                    });

                var result = await query.ToListAsync();

                return Result<IEnumerable<ArticleDto>>.Success(result, StatusCodes.Status200OK);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all articles.");
                return Result<IEnumerable<ArticleDto>>.Failure(StatusCodes.Status500InternalServerError);

            }
        }

        public async Task<Result> CreateArticle(CreateArticleDto createArticleDto)
        {

            var article = new Article
            {
                Title = createArticleDto.Title,
                SubHeading = createArticleDto.SubHeading,
                Content = createArticleDto.Content,
                UserId = createArticleDto.UserId
            };
            await _articleRepository.AddAsync(article);

            return Result.Success();
        }
    }
}
