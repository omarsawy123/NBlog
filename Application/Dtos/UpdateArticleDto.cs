using System.ComponentModel.DataAnnotations;

namespace Application.Dtos
{
    public class UpdateArticleDto
    {
        public int ArticleId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string SubHeading { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public int UserId { get; set; }
    }
}
