using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MvcExampleP34.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<StoreContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

//  Razor Pages
//  /Index  or  /
//  /Index.cshtml + /Index.cshtml.cs  

//  /Controller/Action/Id
//  /Home/Index/ == /


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
    );

app.Run();


/*
 * Створіть дію контроллера Список продуктів
 * Виведіть продукти у вигляді таблиці на сторінці
 * 
 * Продукти створити та заповнити у дії контроллера
 * 
 */


/*
 * Зробити управління продуктами: створення, редагування, видалення
 * Додати модель категорій 
 * Зробити управління категоріями
 * 
 * https://github.com/raid3r/MvcExampleP34
 */ 