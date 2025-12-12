using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MvcExampleP34.Models;
using MvcExampleP34.Models.Forms;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MvcExampleP34.Controllers;

public class AccountController(UserManager<User> userManager) : Controller
{
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View(new RegisterForm());
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromForm] RegisterForm form)
    {
        if (!ModelState.IsValid)
        {
            return View(form);
        }

        // Перевірка унікальності Email (немає бути користувача з таким Email)
        var existingUser = await userManager.FindByEmailAsync(form.Email);

        // Співпадіння паролю та підтвердження паролю
        if (form.Password != form.ConfirmPassword)
        {
            ModelState.AddModelError(nameof(form.ConfirmPassword), "Passwords do not match.");
            return View(form);
        }

        // Якщо користувач з таким Email вже існує, додаємо помилку в ModelState
        if (existingUser != null)
        {
            ModelState.AddModelError(nameof(form.Email), "Email is already in use.");
            return View(form);
        }

        var user = new User
        {
            UserName = form.Email, // Використовуємо Email як UserName
            Email = form.Email
        };

        var result = await userManager.CreateAsync(user, form.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(nameof(form.Password), error.Description);
            }
            return View(form);
        }

        /*
         * {{id:1, email:"test@test.com"}}
         * 
         */
        await SignInUserAsync(user);

        return RedirectToAction("Index", "Home");
    }

    private async Task SignInUserAsync(User user)
    {
        var identity = new ClaimsIdentity(IdentityConstants.ApplicationScheme);
        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
        identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));

        var userRoles = await userManager.GetRolesAsync(user);
        foreach (var role in userRoles)
        {
            identity.AddClaim(new Claim(ClaimTypes.Role, role));
        }


        var principal = new ClaimsPrincipal(identity);
        await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal);
    }


    [HttpGet]
    public IActionResult Login()
    {
        return View(new LoginForm());
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromForm] LoginForm form)
    {
        if (!ModelState.IsValid)
        {
            return View(form);
        }

        var user = await userManager.FindByEmailAsync(form.Email);

        if (user == null)
        {
            ModelState.AddModelError(nameof(form.Email), "User not found");
            return View(form);
        }

        if (!await userManager.CheckPasswordAsync(user, form.Password))
        {
            ModelState.AddModelError(nameof(form.Password), "Wrong password");
            return View(form);
        }

        await SignInUserAsync(user);

        //if (await userManager.IsInRoleAsync(user, RoleConstants.Admin))
        //{
        //    return RedirectToAction("User", "Index");
        //}

        return RedirectToAction("Index", "Home");
    }


    public async Task<IActionResult> AccessDenied()
    {
        return View();
    }
}
