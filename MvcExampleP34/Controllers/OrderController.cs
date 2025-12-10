using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcExampleP34.Models;
using MvcExampleP34.Models.Dto;
using MvcExampleP34.Models.Forms;
using MvcExampleP34.Models.PageModels;
using MvcExampleP34.Models.Services;
using System.Diagnostics;
using System.Security.Claims;

namespace MvcExampleP34.Controllers;

[Authorize]
public class OrderController(
    StoreContext context, 
    CartService cartService,
    UserManager<User> userManager
    ) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var cart = await cartService.GetCartAsync();
        var user = await userManager.GetUserAsync(User);
        var orderNumber = 1 + await context.Orders.MaxAsync(x => x.Number);

        var order = new Order
        {
            UniqueId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Status = OrderStatusConstants.Draft,
            User = user!,
            Items = cart.Items.Select(cartItem => new OrderItem
            {
                Product = cartItem.Product,
                Price = cartItem.Product.Price,
                Quantity = cartItem.Quantity
            }).ToList(),
        };

        // Очистити кошик
        cart.Items.Clear();
        // Записати замовлення в базу даних - статус "Draft"
        context.Orders.Add(order);
        await context.SaveChangesAsync();
        
        // Відправити на сторінку оформлення замовлення (де буде вводитися додаткова інформація)
        return RedirectToAction("Create", new { guid = order.UniqueId });
    }

    [HttpGet]
    public async Task<IActionResult> Create(Guid guid)
    {
        var order = await GetOrderByGuidAsync(guid);

        ViewData["Order"] = order;

        // Показати сторінку оформлення замовлення
        return View(new CreateNewOrderForm());
    }

    private async Task<Order> GetOrderByGuidAsync(Guid guid)
    {
        var order = await context.Orders
            .Include(x => x.Items)
            .ThenInclude(x => x.Product)
            .FirstOrDefaultAsync(x => x.UniqueId == guid);
        return order!;
    }

    [HttpPost]
    public async Task<IActionResult> Create(Guid guid, [FromForm]CreateNewOrderForm form)
    {
        var order = await GetOrderByGuidAsync(guid);

        if (!ModelState.IsValid)
        {
            ViewData["Order"] = order;
            return View(form);
        }
        // Обробити форму оформлення замовлення
        order.Comment = form.Comment;
        // Змінити статус замовлення на "New"
        order.Status = OrderStatusConstants.New;
        order.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();

        // Перенаправити на сторінку успішного оформлення замовлення
        return RedirectToAction("CreateSuccess", new { guid = guid });
    }

    public async Task<IActionResult> CreateSuccess(Guid guid)
    {
        // Показати сторінку успішного оформлення замовлення
        return View(await GetOrderByGuidAsync(guid);
    }

    public async Task<IActionResult> List()
    {
        var userId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value);

        var orders = await context.Orders
            .Where(x => x.User.Id == userId)
            .Include(x => x.Items)
            .ThenInclude(x => x.Product)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        return View(orders);
    }
}
