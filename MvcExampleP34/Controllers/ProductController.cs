using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcExampleP34.Models;
using MvcExampleP34.Models.Forms;
using MvcExampleP34.Models.Services;

namespace MvcExampleP34.Controllers;

public class ProductController(StoreContext context, IFileStorage fileStorage) : Controller
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
            .Include(x => x.Images)
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

        if (form.Images != null) {
            foreach (var imageFile in form.Images)
            {
                var image = new ImageUploaded
                {
                    FileName = await fileStorage.SaveFileAsync(imageFile),
                };
                model.Images.Add(image);
            }
        }

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
        ViewData["ProductId"] = id;
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
            ViewData["ProductId"] = id;
            await PopulateViewData();

            return View(form);
        }

        var model = await context.Products
            .Include(x => x.Tags)
            .Include(x => x.Images)
            .FirstAsync(x => x.Id == id);
        var category = await context.Categories.FindAsync(form.CategoryId);

        model.Name = form.Name;
        model.Price = form.Price;
        model.Quantity = form.Quantity;
        model.Description = form.Description;
        model.Category = category;

        // delete old images if new uploaded
        if (form.Images != null && form.Images.Count > 0)
        {
            // видалення старих збережених файлів
            foreach (var image in model.Images)
            {
                await fileStorage.DeleteFileAsync(image.FileName);
            }
            // видалення записів з бази
            model.Images.Clear();
            // додавання нових збережених файлів
            foreach (var imageFile in form.Images)
            {
                var image = new ImageUploaded
                {
                    FileName = await fileStorage.SaveFileAsync(imageFile),
                };
                model.Images.Add(image);
            }
        }


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
        var product = await context.Products.Include(x => x.Images).FirstAsync(x => x.Id == id);
        if (product == null)
        {
            return NotFound();
        }
        //delete images from storage
        foreach (var image in product.Images)
        {
            await fileStorage.DeleteFileAsync(image.FileName);
            context.Remove(image);
        }
        context.Remove(product);
        await context.SaveChangesAsync();
        return new JsonResult(new { ok = true });
    }

    public async Task<IActionResult> Images(int id)
    {
        var product = await context.Products
            .Include(x => x.Category)
            .Include(x => x.Tags)
            .Include(x => x.Images)
            .FirstAsync(x => x.Id == id);

        if (product == null)
        {
            return NotFound();
        }

        return PartialView("_EditImages", product.Images);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteImage(int id)
    {
        var image = await context.Images.FirstAsync(x => x.Id == id);
        await fileStorage.DeleteFileAsync(image.FileName);
        context.Remove(image);
        await context.SaveChangesAsync();
        return new JsonResult(new { success = true });
    }
}
