using MvcExampleP34.Models.Forms;

namespace MvcExampleP34.Models;

public class UserViewModel
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public ImageUploaded? Avatar { get; set; }

    public List<UserRoleFormItem> Roles { get; set; } = [];
}
