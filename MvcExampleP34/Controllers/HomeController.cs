using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcExampleP34.Models;
using MvcExampleP34.Models.Forms;
using MvcExampleP34.Models.PageModels;

namespace MvcExampleP34.Controllers;

public class HomeController(StoreContext context) : Controller
{
    /// <summary>
    /// Головна сторінка сайту
    /// </summary>
    /// <returns></returns>
    public async Task<IActionResult> Index([FromQuery] HomePageSearchForm form)
    {
        var model = new HomeIndexPageModel
        {
            SearchForm = form,
            Categories = await context.Categories
            .Include(x => x.Image)
            .ToListAsync(),
            Products = await context.Products
            .Include(x => x.Tags)
            .Include(x => x.Images)
            .Where(x => x.Category.Id == (form.CategoryId ?? x.Category.Id))
            .Where(x => string.IsNullOrEmpty(form.Query)
                || x.Name.Contains(form.Query)
                || x.Description.Contains(form.Query))
            .ToListAsync()
        };

        return View(model);
    }

    /// <summary>
    /// Сторінка категорії
    /// </summary>
    /// <returns></returns>
    public async Task<IActionResult> Category()
    {
        // 
        return View();
    }

    /// <summary>
    /// Сторінка продукту
    /// </summary>
    /// <returns></returns>
    public async Task<IActionResult> Product()
    {
        // 
        return View();
    }


    public IActionResult Privacy()
    {
        return View();
    }


    /*
 * Головна сторінка сайту
 * - список категорій з картинками
 * - форма пошуку продуктів (по назві та опису)
 * - список продуктів
 * 
 * Сторінка категорії
 * 
 * Сторінка продукту
 * 
 */








    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
