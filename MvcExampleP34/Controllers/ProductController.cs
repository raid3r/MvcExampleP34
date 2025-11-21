using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcExampleP34.Models;
using MvcExampleP34.Models.Forms;

namespace MvcExampleP34.Controllers;

public class ProductController(StoreContext context) : Controller
{
    /// <summary>
    /// Список продуктів
    /// </summary>
    /// <returns></returns>
    public async Task<IActionResult> Index()
    {
        var products = await context
            .Products
            .Include(x => x.Category)
            .ToListAsync();
        return View(products);
    }

    /// <summary>
    /// Створення продукту (форма)
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var catefories = await context.Categories.ToListAsync();
        ViewData["Categories"] = catefories;

        return View(new ProductForm());
    }

    /// <summary>
    /// Створення продукту (обробка форми)
    /// </summary>
    /// <param name="product"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromForm] ProductForm form)
    {
        if (!ModelState.IsValid)
        {
            var catefories = await context.Categories.ToListAsync();
            ViewData["Categories"] = catefories;

            return View(form);
        }

        var category = await context.Categories.FindAsync(form.CategoryId);
        var model = new Product
        {
            Name = form.Name,
            Price = form.Price,
            Quantity = form.Quantity,
            Description = form.Description,
            Category = category
        };

        context.Add(model);
        await context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    /// <summary>
    /// Редагування продукту (форма)
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var catefories = await context.Categories.ToListAsync();
        ViewData["Categories"] = catefories;

        var product = await context.Products.Include(x => x.Category).FirstAsync(x => x.Id == id);
        if (product == null)
        {
            return NotFound();
        }
        return View(new ProductForm
        {
            Name = product.Name,
            Price = product.Price,
            Quantity = product.Quantity,
            Description = product.Description,
            CategoryId = product.Category?.Id ?? 0
        });
    }

    /// <summary>
    /// Редагування продукту (обробка форми)
    /// </summary>
    /// <param name="id"></param>
    /// <param name="product"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Edit(int id, [FromForm] ProductForm form)
    {
        if (!ModelState.IsValid)
        {
            var catefories = await context.Categories.ToListAsync();
            ViewData["Categories"] = catefories;
            return View(form);
        }

        var model = await context.Products.FirstAsync(x => x.Id == id);
        var category = await context.Categories.FindAsync(form.CategoryId); 

        model.Name = form.Name;
        model.Price = form.Price;
        model.Quantity = form.Quantity;
        model.Description = form.Description;
        model.Category = category;

        await context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    /// <summary>
    /// Видалення продукту
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await context.Products.FirstAsync(x => x.Id == id);
        if (product == null)
        {
            return NotFound();
        }
        context.Products.Remove(product);
        await context.SaveChangesAsync();
        return new JsonResult(new { ok = true });
    }
}
