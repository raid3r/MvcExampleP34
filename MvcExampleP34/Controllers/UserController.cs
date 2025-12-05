using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcExampleP34.Models;
using MvcExampleP34.Models.Forms;
using MvcExampleP34.Models.Services;

namespace MvcExampleP34.Controllers;

[Authorize(Roles = RoleConstants.Admin)]
public class UserController(
    UserManager<User> userManager,
    StoreContext context,
    IFileStorage fileStorage
    ) : Controller
{
    public async Task<IActionResult> Index()
    {
        ViewData["CurrentUserId"] = userManager.GetUserId(User);

        var users = await userManager.Users
            .Include(x => x.Avatar)
            .ToListAsync();

        
        var list = users.Select(user =>
        {
            return new UserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Avatar = user.Avatar,
            };
        }).ToList();

        var allRoles = await context.Roles.ToListAsync();
        foreach (var userModel in list)
        {
            var user = users.First(x => x.Id == userModel.Id);
            var userRoles = await userManager.GetRolesAsync(user);
            userModel.Roles = userRoles.Select(role => new UserRoleFormItem
            {
                Name = role,
                IsEnabled = true
            }).ToList();
        }

        return View(list);
    }


    [HttpGet]
    public async Task<IActionResult> ResetPassword(int id)
    {
        if (userManager.GetUserId(User) == id.ToString())
        {
            return Forbid();
        }

        var user = await userManager.Users
            .Include(x => x.Avatar)
            .FirstOrDefaultAsync(x => x.Id == id);
        ViewData["User"] = user;
        return View(new ResetPasswordForm());
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(int id, [FromForm] ResetPasswordForm form)
    {
        if (userManager.GetUserId(User) == id.ToString())
        {
            return Forbid();
        }
        var user = await userManager.Users.FirstOrDefaultAsync(x => x.Id == id);
        ViewData["User"] = user;
        if (!ModelState.IsValid)
        {
            return View(form);
        }
        if (form.NewPassword != form.ConfirmPassword)
        {
            ModelState.AddModelError(nameof(form.ConfirmPassword), "New password and confirmation do not match.");
            return View(form);
        }

        var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
        var result = await userManager.ResetPasswordAsync(user, resetToken, form.NewPassword);

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


    [HttpDelete]
    public async Task<IActionResult> Delete(int id)
    {
        if (userManager.GetUserId(User) == id.ToString())
        {
            return Forbid();
        }


        var user = await userManager.Users.Include(x => x.Avatar).FirstOrDefaultAsync(x => x.Id == id);
        if (user != null)
        {
            if (user.Avatar != null)
            {
                await fileStorage.DeleteFileAsync(user.Avatar.FileName);
                context.Remove(user.Avatar);
                await context.SaveChangesAsync();
            }

            var result = await userManager.DeleteAsync(user);
        }
        return new JsonResult(new { success = true });
    }

    [HttpGet]
    public async Task<IActionResult> EditRoles(int id)
    {
        var user = await userManager.Users
            .Include(x => x.Avatar)
            .FirstAsync(x => x.Id == id);
        ViewData["User"] = user;

        var allRoles = await context.Roles.ToListAsync();
        var list = allRoles.Select(role => new UserRoleFormItem
        {
            Name = role.Name,
            IsEnabled = userManager.IsInRoleAsync(user, role.Name).Result
        }).ToList();

        return View(new UserRolesForm {
            Items = list
        });
    }

    [HttpPost]
    public async Task<IActionResult> EditRoles(int id, [FromForm] UserRolesForm form)
    {
        var user = await userManager.Users.FirstAsync(x => x.Id == id);

        foreach (var roleItem in form.Items)
        {
            var isInRole = await userManager.IsInRoleAsync(user, roleItem.Name);
            if (roleItem.IsEnabled && !isInRole)
            {
                await userManager.AddToRoleAsync(user, roleItem.Name);
            }
            else if (!roleItem.IsEnabled && isInRole)
            {
                await userManager.RemoveFromRoleAsync(user, roleItem.Name);
            }
        }

        return RedirectToAction("Index");
    }
}
