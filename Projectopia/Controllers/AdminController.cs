using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projectopia.Data;
using Projectopia.Models;
using Projectopia.ViewModels;

namespace Projectopia.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _context;

    public AdminController(
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var dashboardData = new
        {
            TotalUsers = await _userManager.Users.CountAsync(),
            TotalProjects = await _context.Projects.CountAsync(),
            TotalStudents = await _context.Students.CountAsync(),
            TotalSupervisors = await _context.Supervisors.CountAsync()
        };

        return View(dashboardData);
    }

    public async Task<IActionResult> Users()
    {
        var users = await _userManager.Users
            .Select(u => new UserListViewModel
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                IsActive = u.LockoutEnd == null || u.LockoutEnd < DateTimeOffset.UtcNow
            })
            .ToListAsync();

        // Get roles for each user
        foreach (var user in users)
        {
            var userEntity = await _userManager.FindByIdAsync(user.Id);
            var roles = await _userManager.GetRolesAsync(userEntity);
            user.UserType = string.Join(", ", roles);
        }

        return View(users);
    }

    public async Task<IActionResult> CreateUser()
    {
        ViewBag.Roles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateUser(UserManagementViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Roles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            return View(model);
        }

        User user;
        switch (model.UserType)
        {
            case "Student":
                user = new Student { FullName = model.FullName };
                break;
            case "Supervisor":
                user = new Supervisor { FullName = model.FullName };
                break;
            case "Admin":
                user = new Admin { FullName = model.FullName };
                break;
            default:
                user = new User { FullName = model.FullName };
                break;
        }

        user.UserName = model.Email;
        user.Email = model.Email;
        user.PhoneNumber = model.PhoneNumber;

        var result = await _userManager.CreateAsync(user, "DefaultPassword123!");

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, model.UserType);
            return RedirectToAction(nameof(Users));
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        ViewBag.Roles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
        return View(model);
    }

    public async Task<IActionResult> EditUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound();

        var roles = await _userManager.GetRolesAsync(user);
        var viewModel = new UserManagementViewModel
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            UserType = roles.FirstOrDefault() ?? "User",
            IsActive = user.LockoutEnd == null || user.LockoutEnd < DateTimeOffset.UtcNow
        };

        ViewBag.Roles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditUser(string id, UserManagementViewModel model)
    {
        if (id != model.Id)
            return NotFound();

        if (!ModelState.IsValid)
        {
            ViewBag.Roles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            return View(model);
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound();

        user.FullName = model.FullName;
        user.Email = model.Email;
        user.UserName = model.Email;
        user.PhoneNumber = model.PhoneNumber;

        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
            // Update role if changed
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (!currentRoles.Contains(model.UserType))
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRoleAsync(user, model.UserType);
            }

            return RedirectToAction(nameof(Users));
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        ViewBag.Roles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleUserStatus(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound();

        if (user.LockoutEnd == null || user.LockoutEnd < DateTimeOffset.UtcNow)
        {
            // Lock user
            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
        }
        else
        {
            // Unlock user
            await _userManager.SetLockoutEndDateAsync(user, null);
        }

        return RedirectToAction(nameof(Users));
    }

    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound();

        return View(user);
    }

    [HttpPost, ActionName("DeleteUser")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUserConfirmed(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user != null)
        {
            await _userManager.DeleteAsync(user);
        }

        return RedirectToAction(nameof(Users));
    }
}
