using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcExampleP34.Models;
using MvcExampleP34.Models.Dto;
using MvcExampleP34.Models.Forms;
using MvcExampleP34.Models.PageModels;
using MvcExampleP34.Models.Services;

namespace MvcExampleP34.Controllers;

public class CartController(StoreContext context, CartService cartService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var cart =  await cartService.GetCartAsync();
        if (cart.Items.Count == 0)
        {
            return RedirectToAction("Index", "Home");
        }
        return View(cart);
    }

    // Index
    [HttpGet]
    public async Task<IActionResult> Count()
    {
        var cart = await cartService.GetCartAsync();
        return new JsonResult(new { Count = cart.Items.Count });
    }


    // Add
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddProductDto addProduct)
    {
        var cart = await cartService.GetCartAsync();

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

    [HttpPost]
    public async Task<IActionResult> Increase([FromBody] AddProductDto addProduct)
    {
        var cart = await cartService.GetCartAsync();

        var existingItem = cart.Items.FirstOrDefault(x => x.Product.Id == addProduct.ProductId);
        if (existingItem != null)
        {
            existingItem.Quantity += addProduct.Quantity;
            context.CartItems.Update(existingItem);
            await context.SaveChangesAsync();
        }

        return new JsonResult(new { Success = true });
    }

    [HttpPost]
    public async Task<IActionResult> Decrease([FromBody] AddProductDto addProduct)
    {
        var cart = await cartService.GetCartAsync();

        var existingItem = cart.Items.FirstOrDefault(x => x.Product.Id == addProduct.ProductId);
        if (existingItem != null)
        {
            existingItem.Quantity -= addProduct.Quantity;
            if (existingItem.Quantity <= 0)
            {
                context.CartItems.Remove(existingItem);
            }
            else
            {
                context.CartItems.Update(existingItem);
            }
            await context.SaveChangesAsync();
        }
        return new JsonResult(new { Success = true });
    }

    [HttpPost]
    public async Task<IActionResult> Remove([FromBody] AddProductDto addProduct)
    {
        var cart = await cartService.GetCartAsync();

        var existingItem = cart.Items.FirstOrDefault(x => x.Product.Id == addProduct.ProductId);
        if (existingItem != null)
        {
            context.CartItems.Remove(existingItem);
            await context.SaveChangesAsync();
        }
        return new JsonResult(new { Success = true });
    }

}
