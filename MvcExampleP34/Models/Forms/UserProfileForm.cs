using System.ComponentModel.DataAnnotations;

namespace MvcExampleP34.Models.Forms;

public class UserProfileForm
{
    [MaxLength(100)]
    [Display(Name = "Повне ім'я")]
    [Required(ErrorMessage = "Поле Повне ім'я є обов'язковим.")]
    public string? FullName { get; set; } = string.Empty;

    [Display(Name = "Зображення")]
    public IFormFile? Image { get; set; }
}
