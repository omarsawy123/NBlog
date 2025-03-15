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

        /// <summary>
        /// Creates a new article using the provided data.
        /// </summary>
        /// <param name="createArticleDto">A DTO containing the details required to create a new article.</param>
        /// <returns>
        /// A 200 OK response with the result if the article is created successfully, or a 500 Internal Server Error response if the creation fails.
        /// </returns>
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

        /// <summary>
        /// Updates an existing article using the provided update details.
        /// </summary>
        /// <param name="updateArticleDto">A data transfer object containing the updated article information.</param>
        /// <returns>
        /// An IActionResult representing the HTTP response:
        /// <list type="bullet">
        /// <item>
        /// <description>200 OK with the result if the update is successful.</description>
        /// </item>
        /// <item>
        /// <description>404 Not Found if the article does not exist.</description>
        /// </item>
        /// <item>
        /// <description>403 Forbidden if the update is not permitted.</description>
        /// </item>
        /// <item>
        /// <description>An appropriate status code for any other errors.</description>
        /// </item>
        /// </list>
        /// </returns>
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
