using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MvcExampleP34.Models;

public class StoreContext : IdentityDbContext<User, IdentityRole<int>, int>
{
    // Конструктор за замовченням
    public StoreContext() : base() { }

    // Конструктор з параметрами для налаштування контексту
    public StoreContext(DbContextOptions<StoreContext> options) : base(options) { }

    // Визначення DbSet для сутностей
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<ImageUploaded> Images { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }


    // Метод для налаштування моделі та конфігурації бази даних
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Data Source=SILVERSTONE\\SQLEXPRESS;Initial Catalog=AspStoreP34;Integrated Security=True;Persist Security Info=False;Pooling=False;Multiple Active Result Sets=False;Connect Timeout=60;Encrypt=True;Trust Server Certificate=True;");
            
        }
        // Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False

    }
}
