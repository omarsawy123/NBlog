using Domain.Primitives;

namespace Domain.Entites;

public partial class Article : IAuditableEntity
{
    public int ArticleId { get; set; }

    public string Title { get; set; } = null!;
    public string SubHeading { get; set; } = null!;

    public string Content { get; set; } = null!;

    public int UserId { get; set; }

    public virtual ApplicationUser User { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
