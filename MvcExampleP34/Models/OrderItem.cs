namespace MvcExampleP34.Models;

public class OrderItem
{
    public int Id { get; set; }
    public Order Order { get; set; }
    public Product Product { get; set; }
    
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}
