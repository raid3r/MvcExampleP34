using System.ComponentModel.DataAnnotations;

namespace MvcExampleP34.Models.Forms;

public class ResetPasswordForm
{
    [Display(Name = "New Password")]
    [Required]
    public string NewPassword { get; set; } = string.Empty;
    [Display(Name = "Confirm new password")]
    [Required]
    public string ConfirmPassword { get; set; } = string.Empty;
}
