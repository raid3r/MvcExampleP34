
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MvcExampleP34.Models.Services;

public class CartService(IHttpContextAccessor httpContextAccessor, StoreContext context)
{
    private async Task<Cart> CreateNewCartAsync()
    {
        var response = httpContextAccessor.HttpContext!.Response;
        var httpContextUser = httpContextAccessor.HttpContext!.User;

        var cartUid = Guid.NewGuid().ToString();
        response.Cookies.Append("cart_uuid", cartUid, new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddDays(30)
        });
        var userId = httpContextUser.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        var user = !string.IsNullOrEmpty(userId)
            ? context.Users.Find(int.Parse(userId))
            : null;
        var newCart = new Cart
        {
            UniqueId = Guid.Parse(cartUid),
            UpdatedAt = DateTime.UtcNow,
            User = user
        };
        context.Carts.Add(newCart);
        await context.SaveChangesAsync();
        return newCart;
    }

    public async Task<Cart> GetCartAsync()
    {
        var request = httpContextAccessor.HttpContext!.Request;
        var cartUid = request.Cookies["cart_uuid"];
        Cart? cart = null;
        if (string.IsNullOrEmpty(cartUid))
        {
            cart = await CreateNewCartAsync();
        }
        else
        {
            cart = await context.Carts
                .Include(x => x.Items)
                .ThenInclude(x => x.Product)
                .ThenInclude(x => x.Images)
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.UniqueId.ToString() == cartUid);

            if (cart == null)
            {
                cart = await CreateNewCartAsync();
            }
        }

        var httpContextUser = httpContextAccessor.HttpContext!.User;

        if (cart.User == null && httpContextUser != null)
        {
            var userId = httpContextUser.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                var user = await context.Users.FindAsync(int.Parse(userId));
                cart.User = user;
                context.Carts.Update(cart);
                await context.SaveChangesAsync();
            }
        }

        return cart;
    }
}
