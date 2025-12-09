using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcExampleP34.Models;
using MvcExampleP34.Models.Dto;
using MvcExampleP34.Models.Forms;
using MvcExampleP34.Models.PageModels;

namespace MvcExampleP34.Controllers;

public class CartController(StoreContext context) : Controller
{
    private async Task<Cart> CreateNewCart()
    {
        var cartUid = Guid.NewGuid().ToString();
        Response.Cookies.Append("cart_uuid", cartUid, new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddDays(30)
        });
        var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
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

    private async Task<Cart> GetCart()
    {
        var cartUid = Request.Cookies["cart_uuid"];
        Cart? cart = null;
        if (string.IsNullOrEmpty(cartUid))
        {
            cart = await CreateNewCart();
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
                cart = await CreateNewCart();
            }
        }

        if (cart.User == null && User.Identity.IsAuthenticated)
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
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


    public async Task<IActionResult> Index()
    {
        return View(await GetCart());
    }

    // Index
    public async Task<IActionResult> Count()
    {
        var cart = await GetCart();
        return new JsonResult(new { Count = cart.Items.Count });
    }


    // Add
    public async Task<IActionResult> Add([FromBody] AddProductDto addProduct)
    {
        var cart = await GetCart();
        var product = await context.Products.FindAsync(addProduct.ProductId);
        if (product == null)
        {
            return BadRequest(new { Success = false, Message = "Product not found" });
        }
        var existingItem = cart.Items.FirstOrDefault(x => x.Product.Id == addProduct.ProductId);
        if (existingItem != null)
        {
            existingItem.Quantity += addProduct.Quantity;
            context.CartItems.Update(existingItem);
        }
        else
        {
            var newItem = new CartItem
            {
                Cart = cart,
                Product = product,
                Quantity = addProduct.Quantity
            };
            context.CartItems.Add(newItem);
        }
        await context.SaveChangesAsync();

        return new JsonResult(new { Success = true });
    }

}
