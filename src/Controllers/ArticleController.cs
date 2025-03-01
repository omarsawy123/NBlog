using Application.Dtos;
using Application.Services.ArticleService;
using Microsoft.AspNetCore.Mvc;

namespace NBlog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        private readonly ArticleService _articleService;

        public ArticleController(ArticleService articleService)
        {
            _articleService = articleService;
        }


        [HttpGet("getAll")]
        public async Task<IActionResult> GetAllArticles([FromQuery] ArticleFilterQuery filterQuery)
        {
            var result = await _articleService.GetAllArticles(filterQuery);

            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return StatusCode(StatusCodes.Status500InternalServerError, result);

        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateArticle([FromBody] CreateArticleDto createArticleDto)
        {
            var result = await _articleService.CreateArticle(createArticleDto);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return StatusCode(StatusCodes.Status500InternalServerError, result);
        }
    }
}
