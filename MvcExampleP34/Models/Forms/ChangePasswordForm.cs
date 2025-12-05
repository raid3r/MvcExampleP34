using System.ComponentModel.DataAnnotations;

namespace MvcExampleP34.Models.Forms;

public class ChangePasswordForm: ResetPasswordForm
{
    [Required]
    [Display(Name = "Old Password")]
    public string OldPassword { get; set; } = string.Empty;
}
