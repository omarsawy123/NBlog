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
        private readonly BaseRepo<Domain.Entites.Article, int> _articleRepository;
        private readonly ILogger<ArticleService> _logger;
        private readonly IValidator<ArticleFilterQuery> _queryValidator;
        private readonly IValidator<CreateArticleDto> _createValidator;
        private readonly IValidator<UpdateArticleDto> _updateValidator;

        public ArticleService(
            BaseRepo<Domain.Entites.Article, int> articleRepository,
            ILogger<ArticleService> logger,
            IValidator<ArticleFilterQuery> queryValidator,
            IValidator<CreateArticleDto> createValidator,
            IValidator<UpdateArticleDto> updateValidator
        )
        {
            _articleRepository = articleRepository;
            _logger = logger;
            _queryValidator = queryValidator;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
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

        public async Task<Result<Domain.Entites.Article>> CreateArticle(CreateArticleDto createArticleDto)
        {
            try
            {
                var validationResult = await _createValidator.ValidateAsync(createArticleDto);

                if (!validationResult.IsValid)
                {
                    return Result<Domain.Entites.Article>.Failure(
                        StatusCodes.Status400BadRequest,
                        validationResult.Errors.Select(e => e.ErrorMessage)
                    );
                }

                var article = new Domain.Entites.Article
                {
                    Title = createArticleDto.Title,
                    SubHeading = createArticleDto.SubHeading,
                    Content = createArticleDto.Content,
                    UserId = createArticleDto.UserId,
                };

                await _articleRepository.AddAsync(article);
                return Result<Domain.Entites.Article>.Success(article, StatusCodes.Status201Created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating article");
                return Result<Domain.Entites.Article>.Failure(StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<Result<ArticleDetailDto>> GetArticleById(int id)
        {
            try
            {
                

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

        public async Task<Result<Domain.Entites.Article>> UpdateArticle(UpdateArticleDto updateArticleDto)
        {
            try
            {
                var validationResult = await _updateValidator.ValidateAsync(updateArticleDto);

                if (!validationResult.IsValid)
                {
                    return Result<Domain.Entites.Article>.Failure(
                        StatusCodes.Status400BadRequest,
                        validationResult.Errors.Select(e => e.ErrorMessage)
                    );
                }

                var existingArticle = await _articleRepository.GetByIdAsync(updateArticleDto.ArticleId);

                if (existingArticle == null)
                {
                    return Result<Domain.Entites.Article>.Failure(
                        StatusCodes.Status404NotFound,
                        "Article not found"
                    );
                }

                if (existingArticle.UserId != updateArticleDto.UserId)
                {
                    return Result<Domain.Entites.Article>.Failure(
                        StatusCodes.Status403Forbidden,
                        "You are not authorized to update this article"
                    );
                }

                existingArticle.Title = updateArticleDto.Title;
                existingArticle.SubHeading = updateArticleDto.SubHeading;
                existingArticle.Content = updateArticleDto.Content;
                existingArticle.UpdatedAt = DateTime.UtcNow;

                await _articleRepository.UpdateAsync(existingArticle);
                return Result<Domain.Entites.Article>.Success(existingArticle, StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating article with ID: {Id}", updateArticleDto.ArticleId);
                return Result<Domain.Entites.Article>.Failure(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
