using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebVer.Domain.Identity;
using WebVer.Models;

namespace WebVer.Controllers;

[Authorize(Roles = "Админ")]
public class AdminController : Controller
{
    private readonly UserManager<User> _userManager;

    public AdminController(UserManager<User> userManager)
    {
        _userManager = userManager;
    }
    
    public IActionResult Dashbord()
    {
        return View();
    }

    public IActionResult AddUser()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AddUser([FromForm]AddUserViewModel addUserViewModel)
    {
        if (ModelState.IsValid)
        {
            var user = new User
            {
                Email = addUserViewModel.Email,
                UserName = addUserViewModel.Email,
                Name = addUserViewModel.Name,
                EmailConfirmed = true,
                LockoutEnabled = false
            };

            var result = await _userManager.CreateAsync(user, addUserViewModel.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Пользователь");
                return RedirectToAction("Dashbord");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(String.Empty, error.Description);
            }
        }

        return View(addUserViewModel);
    }
}