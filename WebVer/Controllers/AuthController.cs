using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebVer.Domain.Identity;
using WebVer.Models;

namespace WebVer.Controllers;

public class AuthController : Controller
{
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;

    public AuthController(SignInManager<User> signInManager,UserManager<User> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }
    
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromForm]LoginViewModel loginViewModel)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(loginViewModel.Login);
            if (user == null)
            {
                ModelState.AddModelError("","Неправильный логин и (или) пароль");
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(user, loginViewModel.Password, false, false);

            if (result.Succeeded)
                return RedirectToAction("Index", "Home");
            
            ModelState.AddModelError("","Неправильный логин и (или) пароль");
        }

        return View(loginViewModel);
    }
    
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}