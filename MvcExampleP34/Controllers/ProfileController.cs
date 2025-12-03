using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcExampleP34.Models;
using MvcExampleP34.Models.Forms;

namespace MvcExampleP34.Controllers;

[Authorize]
public class ProfileController(
    StoreContext context,
    UserManager<User> userManager
    ) : Controller
{
    private async Task<User> GetCurrentUserAsync()
    {
        //return  await userManager.GetUserAsync(User);

        var userId = int.Parse(userManager.GetUserId(User));
        return await userManager.Users
            .FirstOrDefaultAsync(x => x.Id == userId);
    }


    public async Task<IActionResult> Index()
    {
        return View(await GetCurrentUserAsync());
    }

    [HttpGet]
    public async Task<IActionResult> ChangePassword()
    {
        ViewData["User"] = await GetCurrentUserAsync();
        return View(new ChangePasswordForm());
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword([FromForm] ChangePasswordForm form)
    {
        var user = await GetCurrentUserAsync();
        ViewData["User"] = user;
        if (!ModelState.IsValid)
        {
            return View(form);
        }

        if (form.NewPassword != form.ConfirmPassword)
        {
            ModelState.AddModelError(string.Empty, "New password and confirmation do not match.");
            return View(form);
        }

        if (!await userManager.CheckPasswordAsync(user, form.OldPassword))
        {
            ModelState.AddModelError(nameof(form.OldPassword), "Old password is incorrect.");
            return View(form);
        }

        // Зміна паролю
        var result = await userManager.ChangePasswordAsync(user, form.OldPassword, form.NewPassword);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(nameof(form.NewPassword), error.Description);
            }
            return View(form);
        }

        return RedirectToAction("Index");
    }
}
