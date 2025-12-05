using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcExampleP34.Models;
using MvcExampleP34.Models.Forms;
using MvcExampleP34.Models.Services;

namespace MvcExampleP34.Controllers;

[Authorize]
public class ProfileController(
    StoreContext context,
    UserManager<User> userManager,
    IFileStorage fileStorage
    ) : Controller
{
    private async Task<User> GetCurrentUserAsync()
    {
        //return  await userManager.GetUserAsync(User);

        var userId = int.Parse(userManager.GetUserId(User));
        return await userManager.Users
            .Include(x => x.Avatar)
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

    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        var user = await GetCurrentUserAsync();
        ViewData["User"] = user;
        return View(new UserProfileForm()
        {
            FullName = user.FullName
        });
    }

    [HttpPost]
    public async Task<IActionResult> Edit([FromForm] UserProfileForm form)
    {
        var user = await GetCurrentUserAsync();
        ViewData["User"] = user;

        if (!ModelState.IsValid)
        {
            return View(form);
        }

        user.FullName = form.FullName;

        if (form.Image != null)
        {
            if (user.Avatar != null)
            {
                await fileStorage.DeleteFileAsync(user.Avatar.FileName);
                context.Remove(user.Avatar);
                await context.SaveChangesAsync();
            }

            var image = new ImageUploaded
            {
                FileName = await fileStorage.SaveFileAsync(form.Image),
            };
            user.Avatar = image;
        }

        await userManager.UpdateAsync(user);
        return RedirectToAction("Index");
    }



}
