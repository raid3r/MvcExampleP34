using System.ComponentModel.DataAnnotations;

namespace MvcExampleP34.Models.Forms;

public class LoginForm
{
    [Required]
    [Display(Name = "Login (email)")]
    public string Email { get; set; } = string.Empty;
    [Required]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;
}
