using Microsoft.AspNetCore.Identity;

namespace Domain.Entites;

public partial class ApplicationUser : IdentityUser<int>
{
    public List<Article> Articles { get; set; } = new();
}
