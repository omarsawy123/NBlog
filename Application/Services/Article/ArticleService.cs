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
        private readonly IValidator<CreateArticleDto> _createValidator;
        private readonly IValidator<int> _idValidator;

        public ArticleService(
            BaseRepo<Article, int> articleRepository,
            ILogger<ArticleService> logger,
            IValidator<ArticleFilterQuery> queryValidator,
            IValidator<CreateArticleDto> createValidator,
            IValidator<int> idValidator
        )
        {
            _articleRepository = articleRepository;
            _logger = logger;
            _queryValidator = queryValidator;
            _createValidator = createValidator;
            _idValidator = idValidator;
        }

        public async Task<Result<IEnumerable<ArticleDto>>> GetAllArticles(
            ArticleFilterQuery filterQuery
        )
        {
            try
            {
                var validQuery = await _queryValidator.ValidateAsync(filterQuery);

                if (!validQuery.IsValid)
                {
                    return Result<IEnumerable<ArticleDto>>.Failure(
                        StatusCodes.Status400BadRequest,
                        validQuery.Errors.Select(e => e.ErrorMessage)
                    );
                }

                var query = _articleRepository
                    .GetAll()
                    .Where(a =>
                        a.Title.Contains(filterQuery.SearchKey)
                        || a.SubHeading.Contains(filterQuery.SearchKey)
                    )
                    .Select(a => new ArticleDto
                    {
                        ArticleId = a.ArticleId,
                        Title = a.Title,
                        SubHeading = a.SubHeading,
                        AuthorName = a.User.UserName!,
                        AuthorProfilePic = "",
                        CreatedAt = a.CreatedAt,
                        UpdatedAt = a.UpdatedAt,
                    });

                var result = await query.ToListAsync();

                return Result<IEnumerable<ArticleDto>>.Success(result, StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all articles.");
                return Result<IEnumerable<ArticleDto>>.Failure(
                    StatusCodes.Status500InternalServerError
                );
            }
        }

        public async Task<Result<Article>> CreateArticle(CreateArticleDto createArticleDto)
        {
            try
            {
                var validationResult = await _createValidator.ValidateAsync(createArticleDto);

                if (!validationResult.IsValid)
                {
                    return Result<Article>.Failure(
                        StatusCodes.Status400BadRequest,
                        validationResult.Errors.Select(e => e.ErrorMessage)
                    );
                }

                var article = new Article
                {
                    Title = createArticleDto.Title,
                    SubHeading = createArticleDto.SubHeading,
                    Content = createArticleDto.Content,
                    UserId = createArticleDto.UserId,
                };

                await _articleRepository.AddAsync(article);
                return Result<Article>.Success(article, StatusCodes.Status201Created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating article");
                return Result<Article>.Failure(StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<Result<ArticleDetailDto>> GetArticleById(int id)
        {
            try
            {
                var validationResult = await _idValidator.ValidateAsync(id);

                if (!validationResult.IsValid)
                {
                    return Result<ArticleDetailDto>.Failure(
                        StatusCodes.Status400BadRequest,
                        validationResult.Errors.Select(e => e.ErrorMessage)
                    );
                }

                var article = await _articleRepository.GetByIdAsync(id);

                if (article == null)
                {
                    return Result<ArticleDetailDto>.Failure(
                        StatusCodes.Status404NotFound,
                        "Article not found"
                    );
                }

                var articleDetailDto = new ArticleDetailDto
                {
                    ArticleId = article.ArticleId,
                    Title = article.Title,
                    SubHeading = article.SubHeading,
                    Content = article.Content,
                    AuthorName = article.User.UserName!,
                    AuthorProfilePic = "",
                    CreatedAt = article.CreatedAt,
                    UpdatedAt = article.UpdatedAt,
                };

                return Result<ArticleDetailDto>.Success(articleDetailDto, StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving article with ID: {Id}", id);
                return Result<ArticleDetailDto>.Failure(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
