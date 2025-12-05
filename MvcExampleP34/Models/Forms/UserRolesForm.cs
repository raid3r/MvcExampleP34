using System.ComponentModel.DataAnnotations;

namespace MvcExampleP34.Models.Forms;

public class UserRolesForm
{
    public List<UserRoleFormItem> Items { get; set; } = [];
}
