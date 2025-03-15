using System.ComponentModel.DataAnnotations;

namespace Application.Dtos
{
    public class UpdateArticleDto
    {
        public int ArticleId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        [StringLength(200)]
        public string SubHeading { get; set; } = string.Empty;
        
        [Required]
        public string Content { get; set; } = string.Empty;
        
        public int UserId { get; set; }
    }
}
