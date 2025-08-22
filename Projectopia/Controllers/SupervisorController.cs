using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projectopia.Data;
using Projectopia.Models;

namespace Projectopia.Controllers;

[Authorize(Roles = "Supervisor")]
public class SupervisorController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;

    public SupervisorController(ApplicationDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        var supervisor = await _context.Supervisors
            .Include(s => s.SupervisorProjects)
            .ThenInclude(sp => sp.Project)
            .ThenInclude(p => p.ProjectStudents)
            .ThenInclude(ps => ps.Student)
            .FirstOrDefaultAsync(s => s.Id == currentUser.Id);

        var dashboardData = new
        {
            TotalProjects = supervisor?.SupervisorProjects?.Count ?? 0,
            TotalStudents = supervisor?.SupervisorProjects?.SelectMany(sp => sp.Project.ProjectStudents).Count() ?? 0,
            Projects = supervisor?.SupervisorProjects?.Select(sp => sp.Project).ToList() ?? new List<Project>()
        };

        return View(dashboardData);
    }

    public async Task<IActionResult> MyProjects()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        var projects = await _context.SupervisorProjects
            .Include(sp => sp.Project)
            .ThenInclude(p => p.ProjectStudents)
            .ThenInclude(ps => ps.Student)
            .Where(sp => sp.SupervisorId == currentUser.Id)
            .Select(sp => sp.Project)
            .ToListAsync();

        return View(projects);
    }

    public async Task<IActionResult> ProjectDetails(int id)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        var project = await _context.SupervisorProjects
            .Include(sp => sp.Project)
            .ThenInclude(p => p.ProjectStudents)
            .ThenInclude(ps => ps.Student)
            .Where(sp => sp.SupervisorId == currentUser.Id && sp.ProjectId == id)
            .Select(sp => sp.Project)
            .FirstOrDefaultAsync();

        if (project == null)
            return NotFound();

        return View(project);
    }

    public async Task<IActionResult> Students()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        var students = await _context.SupervisorProjects
            .Include(sp => sp.Project)
            .ThenInclude(p => p.ProjectStudents)
            .ThenInclude(ps => ps.Student)
            .Where(sp => sp.SupervisorId == currentUser.Id)
            .SelectMany(sp => sp.Project.ProjectStudents)
            .Select(ps => ps.Student)
            .Distinct()
            .ToListAsync();

        return View(students);
    }
}
