using System.ComponentModel.DataAnnotations;

namespace MvcExampleP34.Models.Forms;

public class CategoryForm
{
    [MaxLength(100)]
    [Display(Name = "Назва")]
    [Required(ErrorMessage = "Поле Назва є обов'язковим.")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Зображення")]
    public IFormFile? Image { get; set; }
}


/*
 * 
 */ 