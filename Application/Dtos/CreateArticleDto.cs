namespace Application.Dtos
{
    public class CreateArticleDto
    {
        public string Title { get; set; } = null!;
        public string SubHeading { get; set; } = null!;
        public string Content { get; set; } = null!;
        public int UserId { get; set; }
    }
}
