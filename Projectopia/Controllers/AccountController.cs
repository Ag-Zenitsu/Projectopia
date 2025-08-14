using Microsoft.AspNetCore.Mvc;
using Projectopia.ViewModels;

namespace Projectopia.Controllers;

public class AccountController : Controller
{
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        //logic login
        ModelState.AddModelError("", "Invalid email or password");
        return View(model);
    }
}