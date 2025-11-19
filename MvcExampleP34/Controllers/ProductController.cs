using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcExampleP34.Models;

namespace MvcExampleP34.Controllers;

public class ProductController(StoreContext context) : Controller
{
    /// <summary>
    /// Список продуктів
    /// </summary>
    /// <returns></returns>
    public async Task<IActionResult> Index()
    {
        var products = await context.Products.ToListAsync();
        return View(products);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        return View(new Product());
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] Product product)
    {
        if (ModelState.IsValid)
        {
            context.Products.Add(product);
            await context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        return View(product);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var product = await context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        return View(product);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, [FromForm] Product product)
    {
        if (!ModelState.IsValid)
        {
            return View(product);
        }

        var existingProduct = await context.Products.FirstAsync(x => x.Id == id);
        existingProduct.Name = product.Name;
        existingProduct.Price = product.Price;
        existingProduct.Quantity = product.Quantity;
        existingProduct.Description = product.Description;
        await context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        context.Products.Remove(product);
        await context.SaveChangesAsync();
        return RedirectToAction("Index");
    }
}
