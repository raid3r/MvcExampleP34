using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcExampleP34.Models;
using MvcExampleP34.Models.Forms;
using MvcExampleP34.Models.Services;

namespace MvcExampleP34.Controllers;

[Authorize(Roles = RoleConstants.Manager)]
public class CategoryController(StoreContext context, IFileStorage fileStorage) : Controller
{
    
    public async Task<IActionResult> Index()
    {
        var models = await context.Categories
            .Include(x => x.Image)
            .ToListAsync();
        return View(models);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        return View(new CategoryForm());
    }


    [HttpPost]
    public async Task<IActionResult> Create([FromForm] CategoryForm form)
    {
        if (!ModelState.IsValid)
        {
            return View(form);
        }

        var model = new Category
        {
            Name = form.Name
        };
        if (form.Image != null)
        {
            var image = new ImageUploaded
            {
                FileName = await fileStorage.SaveFileAsync(form.Image),
            };
            model.Image = image;
        }

        context.Add(model);
        await context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        ViewData["CategoryId"] = id;
        var model = await context.Categories.FindAsync(id);
        if (model == null)
        {
            return NotFound();
        }
        return View(new CategoryForm
        {
            Name = model.Name
        });
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, [FromForm] CategoryForm form)
    {
        if (!ModelState.IsValid)
        {
            ViewData["CategoryId"] = id;
            return View(form);
        }

        var model = await context.Categories.Include(x => x.Image).FirstAsync(x => x.Id == id);
        model.Name = form.Name;

        if (form.Image != null)
        {
            if (model.Image != null)
            {
                await fileStorage.DeleteFileAsync(model.Image.FileName);
            }

            var image = new ImageUploaded
            {
                FileName = await fileStorage.SaveFileAsync(form.Image),
            };
            model.Image = image;
        }

        await context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(int id)
    {
        var model = await context.Categories.Include(x => x.Image).FirstAsync(x => x.Id == id);
        if (model == null)
        {
            return NotFound();
        }
        
        if (model.Image != null)
        {
            await fileStorage.DeleteFileAsync(model.Image.FileName);
            context.Remove(model.Image);
        }

        context.Remove(model);
        await context.SaveChangesAsync();
        return new JsonResult(new { ok = true });
    }

    public async Task<IActionResult> Images(int id)
    {
        var category = await context.Categories
            .Include(x => x.Image)
            .FirstAsync(x => x.Id == id);

        if (category == null)
        {
            return NotFound();
        }

        var images = new List<ImageUploaded>();
        if (category.Image != null)
        {
            images.Add(category.Image);
        }

        return PartialView("_EditImages", images);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteImage(int id)
    {
        var image = await context.Images.FirstAsync(x => x.Id == id);

        var category = await context.Categories
            .Include(x => x.Image)
            .FirstAsync(x => x.Image != null && x.Image.Id == id);

        category.Image = null;

        await fileStorage.DeleteFileAsync(image.FileName);
        context.Remove(image);

        await context.SaveChangesAsync();
        return new JsonResult(new { success = true });
    }

}
