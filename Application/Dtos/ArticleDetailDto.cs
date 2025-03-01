namespace Application.Dtos
{
    public class ArticleDetailDto
    {
        public int ArticleId { get; set; }
        public string Title { get; set; } = null!;
        public string SubHeading { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string AuthorName { get; set; } = null!;
        public string AuthorProfilePic { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
