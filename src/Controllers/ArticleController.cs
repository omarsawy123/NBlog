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

        [HttpGet("all")]
        public async Task<IActionResult> GetAllArticles([FromQuery] ArticleFilterQuery filterQuery)
        {
            var result = await _articleService.GetAllArticles(filterQuery);

            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return StatusCode(StatusCodes.Status500InternalServerError, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetArticleById(int id)
        {
            var result = await _articleService.GetArticleById(id);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            if (result.StatusCode == StatusCodes.Status404NotFound)
            {
                return NotFound(result);
            }

            return StatusCode(result.StatusCode, result);
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

        [HttpPut("update")]
        public async Task<IActionResult> UpdateArticle([FromBody] UpdateArticleDto updateArticleDto)
        {
            var result = await _articleService.UpdateArticle(updateArticleDto);
            
            if (result.IsSuccess)
            {
                return Ok(result);
            }

            if (result.StatusCode == StatusCodes.Status404NotFound)
            {
                return NotFound(result);
            }

            if (result.StatusCode == StatusCodes.Status403Forbidden)
            {
                return StatusCode(StatusCodes.Status403Forbidden, result);
            }

            return StatusCode(result.StatusCode, result);
        }
    }
}
