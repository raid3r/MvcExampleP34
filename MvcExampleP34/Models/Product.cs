namespace MvcExampleP34.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string Description { get; set; }
    public virtual Category? Category { get; set; } = null;
    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();

    public virtual ICollection<ImageUploaded> Images { get; set; } = new List<ImageUploaded>();

}
