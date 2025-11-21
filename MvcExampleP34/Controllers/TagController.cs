using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcExampleP34.Models;

namespace MvcExampleP34.Controllers;

public class TagController(StoreContext context) : Controller
{
    public async Task<IActionResult> Index()
    {
        var models = await context.Tags.ToListAsync();
        return View(models);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        return View(new Tag());
    }

    
    [HttpPost]
    public async Task<IActionResult> Create([FromForm] Tag form)
    {
        if (ModelState.IsValid)
        {
            context.Add(form);
            await context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        return View(form);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var model = await context.Tags.FindAsync(id);
        if (model == null)
        {
            return NotFound();
        }
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, [FromForm] Tag form)
    {
        if (!ModelState.IsValid)
        {
            return View(form);
        }

        var model = await context.Tags.FirstAsync(x => x.Id == id);
        model.Name = form.Name;
        await context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(int id)
    {
        var model = await context.Tags.FindAsync(id);
        if (model == null)
        {
            return NotFound();
        }
        context.Remove(model);
        await context.SaveChangesAsync();
        return new JsonResult(new { ok = true });
    }
}
