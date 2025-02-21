using Application.Dtos;
using Application.Shared;
using Domain.Entites;
using Infrastructure.Repos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services.ArticleService
{
    public class ArticleService
    {
        private readonly BaseRepo<Article, int> _articleRepository;
        private readonly ILogger<ArticleService> _logger;

        public ArticleService(BaseRepo<Article, int> articleRepository, ILogger<ArticleService> logger)
        {
            _articleRepository = articleRepository;
            _logger = logger;
        }


        public async Task<IEnumerable<ArticleDto>> GetAllArticles()
        {
            var articles = await _articleRepository.GetAll().ToListAsync();
            return articles.Select(article => new ArticleDto
            {
                ArticleId = article.ArticleId,
                Title = article.Title,
                SubHeading = article.SubHeading, 
                AuthorName = article.User.UserName!,
                AuthorProfilePic = "",
                CreatedAt = article.CreatedAt
            });
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
