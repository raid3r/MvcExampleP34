using Microsoft.AspNetCore.Identity;

namespace MvcExampleP34.Models;

public class User: IdentityUser<int>
{
    public string? FullName { get; set; }

    public virtual ImageUploaded? Avatar { get; set; }
}
