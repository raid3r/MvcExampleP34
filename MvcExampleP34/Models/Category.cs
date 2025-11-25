using System.ComponentModel.DataAnnotations;

namespace MvcExampleP34.Models;

public class Category
{
    public int Id { get; set; }
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual ImageUploaded? Image { get; set; }
}
