using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using MvcExampleP34.Models;
using MvcExampleP34.Models.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IFileStorage, FileStorage>();

builder.Services.AddDbContext<StoreContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
{
    options.SignIn.RequireConfirmedPhoneNumber = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedAccount = false;

    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 3;
})
    .AddRoles<IdentityRole<int>>()
    .AddEntityFrameworkStores<StoreContext>()
    .AddDefaultTokenProviders();


builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = new PathString("/Account/Login");
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
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

// 
// запит - автентифікація користувача   user id 1
//   - перевірка авторизації - ресурс - відповідь
//  гість  -  неавтентифікований користувач
//  автентифікований користувач
//  ролі - адміністратор, менеджер, користувач
//



app.UseAuthentication();
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

/*
 * Додати категорії до продуктів
 * При створенні/редагуванні продукту вибирати категорію з випадаючого списку
 * Показувати категорію у списку продуктів
 * 
 */

/*
 * Додати теги продуктам (багато-до-багатьох)
 * Зробити управління тегами (створення, редагування, видалення)
 * 
 * 
 * Додати вибір тегів при створенні/редагуванні продукту
 * черех чекбокси
 */


/*
 * 1. Додати можливість завантаження зображень для категорій
 * 
 * 2. Додати можливість завантаження кількох зображень для продуктів
 *
 *
 * 3. Додати можливість видалення завантажених зображень (для продукту та категорій)
 *    Для продукту - по одному зображенню
 *
 * 4. Додати видалення зображень по одному
 * 
 * 5. Для продукту зробити модливість вибору головного зображення з наявних в ньому зображень
 *
 */

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