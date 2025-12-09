namespace MvcExampleP34.Models;

public class Cart
{
    public int Id { get; set; }
    public Guid UniqueId { get; set; } = Guid.NewGuid();
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public virtual User? User { get; set; }
    public virtual ICollection<CartItem> Items { get; set; } = [];    
}
