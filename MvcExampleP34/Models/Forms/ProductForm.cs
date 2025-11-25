using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MvcExampleP34.Models.Forms;

public class ProductForm
{
    [DisplayName("Назва")]
    public string Name { get; set; } = string.Empty;
    [DisplayName("Ціна")]
    public decimal Price { get; set; }
    [DisplayName("Кількість")]
    public int Quantity { get; set; }
    [DisplayName("Опис")]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;
    [DisplayName("Категорія")]
    [Required]
    public int CategoryId { get; set; }

    public ProductTagsForm Tags { get; set; } = new ProductTagsForm();

    public ICollection<IFormFile>? Images { get; set; } = new List<IFormFile>();
}
