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

        /// <summary>
        /// Initializes a new instance of the ArticleService with the specified repository and validators.
        /// </summary>
        /// <remarks>
        /// This constructor establishes the service's dependencies for article operations, including data access
        /// via a repository and validation for filter queries, article creation, updates, and identifier integrity.
        /// </remarks>
        /// <param name="articleRepository">The repository for accessing domain article entities.</param>
        /// <param name="queryValidator">Validates filter queries for retrieving articles.</param>
        /// <param name="createValidator">Validates data for creating new articles.</param>
        /// <param name="updateValidator">Validates data for updating existing articles.</param>
        /// <param name="idValidator">Validates article identifier values.</param>
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

        /// <summary>
        /// Retrieves articles that match the provided filter query.
        /// </summary>
        /// <param name="filterQuery">The filter criteria used to search articles by title or subheading.</param>
        /// <returns>
        /// An asynchronous task that returns a Result containing an enumerable of ArticleDto. On success, the result includes the matched articles with a 200 status code; if the query is invalid, it returns a failure result with a 400 status code, or a 500 status code if an internal error occurs.
        /// </returns>
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

        /// <summary>
        /// Validates and creates a new article.
        /// </summary>
        /// <remarks>
        /// Validates the provided article creation data. On validation failure, returns a result with a 400 status code and corresponding error messages.
        /// If valid, creates a new article from the provided data and adds it to the repository, returning the created article with a 201 status code.
        /// Any exceptions encountered are logged and yield a failure result with a 500 status code.
        /// </remarks>
        /// <param name="createArticleDto">Data transfer object containing the article details.</param>
        /// <returns>A result containing the created article and a status code indicating the operation's outcome.</returns>
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

        /// <summary>
        /// Retrieves the details of an article using its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the article to retrieve.</param>
        /// <returns>
        /// A Result containing an ArticleDetailDto with HTTP status codes as follows:
        /// <list type="bullet">
        /// <item><description>200 if the article is found.</description></item>
        /// <item><description>400 if the provided ID fails validation.</description></item>
        /// <item><description>404 if no article exists with the given ID.</description></item>
        /// <item><description>500 if an unexpected error occurs.</description></item>
        /// </list>
        /// </returns>
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

        /// <summary>
        /// Validates the update details and applies changes to an existing article.
        /// </summary>
        /// <param name="updateArticleDto">The DTO containing the article's new title, subheading, content, and identification details necessary for validation and authorization.</param>
        /// <returns>
        /// A result containing the updated article with a 200 status code on success, or a failure result with appropriate status codes:
        /// 400 for validation errors, 404 if the article is not found, 403 if the user is not authorized, and 500 in case of an exception.
        /// </returns>
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
