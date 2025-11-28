using Microsoft.AspNetCore.Mvc.Rendering;
using MvcExampleP34.Models.Forms;

namespace MvcExampleP34.Models.PageModels;

public class HomeIndexPageModel
{
    public List<Category> Categories { get; set; } = [];
    public List<Product> Products { get; set; } = [];

    public HomePageSearchForm SearchForm { get; set; } = new ();

    public SelectList SelectListCategories => new (Categories, "Id", "Name");

}
