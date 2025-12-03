using System.ComponentModel.DataAnnotations;

namespace MvcExampleP34.Models.Forms;

public class ChangePasswordForm
{
    [Required]
    [Display(Name = "Old Password")]
    public string OldPassword { get; set; } = string.Empty;
    [Display(Name = "New Password")]
    [Required]
    public string NewPassword { get; set; } = string.Empty;
    [Display(Name = "Confirm new password")]
    [Required]
    public string ConfirmPassword { get; set; } = string.Empty;
}
