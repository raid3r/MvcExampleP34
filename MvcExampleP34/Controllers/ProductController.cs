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
            .Include(x => x.Tags)
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
        await PopulateViewData();

        var tags = await context.Tags.ToListAsync();
        return View(new ProductForm()
        {
            Tags = new ProductTagsForm
            {
                Items = [.. tags.Select(tag => new ProductTagsFormItem
                {
                    Id = tag.Id,
                    Name = tag.Name,
                    IsSelected = false
                })]
            }
        });
    }

    private async Task PopulateViewData()
    {
        var catefories = await context.Categories.ToListAsync();
        ViewData["Categories"] = catefories;
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
            await PopulateViewData();

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

        // Список вибраних тегів (їх Id)
        var selectedTagIds = form.Tags.Items
            .Where(x => x.IsSelected)
            .Select(x => x.Id)
            .ToList();

        // Дістати з бази відповідні теги
        var selectedTags = await context.Tags.Where(x => selectedTagIds.Contains(x.Id)).ToListAsync();
        // Додати теги до продукту
        foreach (var tag in selectedTags)
        {
            model.Tags.Add(tag);
        }

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
        await PopulateViewData();

        var product = await context.Products
            .Include(x => x.Category)
            .Include(x => x.Tags)
            .FirstAsync(x => x.Id == id);
        if (product == null)
        {
            return NotFound();
        }

        var tags = await context.Tags.ToListAsync();

        return View(new ProductForm
        {
            Name = product.Name,
            Price = product.Price,
            Quantity = product.Quantity,
            Description = product.Description,
            CategoryId = product.Category?.Id ?? 0,
            Tags = new ProductTagsForm
            {
                Items = [.. tags.Select(tag => new ProductTagsFormItem
                {
                    Id = tag.Id,
                    Name = tag.Name,
                    IsSelected = product.Tags.Any(t => t.Id == tag.Id)
                })]
            }
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
            await PopulateViewData();

            return View(form);
        }

        var model = await context.Products
            .Include(x => x.Tags)
            .FirstAsync(x => x.Id == id);
        var category = await context.Categories.FindAsync(form.CategoryId);

        model.Name = form.Name;
        model.Price = form.Price;
        model.Quantity = form.Quantity;
        model.Description = form.Description;
        model.Category = category;

        // Оновлення тегів
        var selectedTagIds = form.Tags.Items
            .Where(x => x.IsSelected)
            .Select(x => x.Id)
            .ToList();
        // Очистка поточних тегів
        model.Tags.Clear();
        // Додавання вибраних тегів
        var selectedTags = await context.Tags
            .Where(x => selectedTagIds.Contains(x.Id))
            .ToListAsync();
        // Додавання вибраних тегів до продукту
        foreach (var tag in selectedTags)
        {
            model.Tags.Add(tag);
        }

        // Збереження змін
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
