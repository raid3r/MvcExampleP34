using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MvcExampleP34.Models;

namespace MvcExampleP34.Controllers;

public class HomeController(ILogger<HomeController> logger) : Controller
{
    // GET: /Home/Index
    // POST: /Home/Index
    // DELETE: /Home/Index
    public IActionResult Index()
    {
        var product = new Product
        {
            Id = 1,
            Name = "Sample Product",
            Description = "This is a sample product description.",
            Price = 9.99M,
            Quantity = 100
        };
        ViewData["Product"] = product;
        ViewData["Product1"] = product;
        ViewBag.BestProduct = product;

        return View();
    }


    // GET: /Home/Privacy
    // POST: /Home/Privacy
    // DELETE: /Home/Privacy
    public IActionResult Privacy()
    {
        return View();
    }

    // GET: /Home/ProductView
    // POST: /Home/ProductView
    // DELETE: /Home/ProductView
    public IActionResult ProductView(int id)
    {
        var product = new Product
        {
            Id = 1,
            Name = "Sample Product",
            Description = "This is a sample product description.",
            Price = 9.99M,
            Quantity = 100
        };

        return View(product);
    }

    /*
 * 
 * 1.Створіть дію контроллера Список продуктів
 * Виведіть продукти у вигляді таблиці на сторінці
 * 
 * Продукти створити та заповнити у дії контроллера
 * 
 * 2. Створіть дію контроллера Деталі продукту з параметром id
 * Додайте в таблицю посилання на сторінку Деталі продукту (кнопку)
 * На сторінці Деталі продукту виведіть інформацію про вибраний продукт
 * та кнопку Повернутися до списку продуктів
 * 
 * 
 * 
 * 
 */








    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
