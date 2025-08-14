using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Projectopia.Models;
using Projectopia.ViewModels;

namespace Projectopia.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<User> _signInManager;

    public AccountController(SignInManager<User> signInManager)
    {
        _signInManager = signInManager;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
        
        //TODO make your own page
        if (result.Succeeded)
            return RedirectToAction("Index", "Home");

        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return View(model);
    }
}