using System.ComponentModel.DataAnnotations;

namespace MvcExampleP34.Models.Forms;

public class RegisterForm
{
    [Required]
    [Display(Name = "Login (email)")]
    public string Email { get; set; } = string.Empty;
    [Required]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;
    [Display(Name = "Confirm password")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
