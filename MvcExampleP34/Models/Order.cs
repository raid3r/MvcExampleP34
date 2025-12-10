namespace MvcExampleP34.Models;

public class Order
{
    public int Id { get; set; }
    public int Number { get; set; }
    public Guid UniqueId { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string Comment { get; set; } = string.Empty;
    public string Status { get; set; } = OrderStatusConstants.Draft;
    public virtual User User { get; set; } = null!;
    public virtual ICollection<OrderItem> Items { get; set; } = [];
}
