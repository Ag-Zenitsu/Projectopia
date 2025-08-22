using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projectopia.Data;
using Projectopia.Models;

namespace Projectopia.Controllers;

[Authorize(Roles = "Student")]
public class StudentController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;

    public StudentController(ApplicationDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        var student = await _context.Students
            .Include(s => s.ProjectStudents)
            .ThenInclude(ps => ps.Project)
            .ThenInclude(p => p.SupervisorProjects)
            .ThenInclude(sp => sp.Supervisor)
            .FirstOrDefaultAsync(s => s.Id == currentUser.Id);

        var dashboardData = new
        {
            TotalProjects = student?.ProjectStudents?.Count ?? 0,
            TotalSupervisors = student?.ProjectStudents?.SelectMany(ps => ps.Project.SupervisorProjects).Count() ?? 0,
            Projects = student?.ProjectStudents?.Select(ps => ps.Project).ToList() ?? new List<Project>()
        };

        return View(dashboardData);
    }

    public async Task<IActionResult> MyProjects()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        var projects = await _context.ProjectStudents
            .Include(ps => ps.Project)
            .ThenInclude(p => p.SupervisorProjects)
            .ThenInclude(sp => sp.Supervisor)
            .Where(ps => ps.StudentId == currentUser.Id)
            .Select(ps => ps.Project)
            .ToListAsync();

        return View(projects);
    }

    public async Task<IActionResult> ProjectDetails(int id)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        var project = await _context.ProjectStudents
            .Include(ps => ps.Project)
            .ThenInclude(p => p.SupervisorProjects)
            .ThenInclude(sp => sp.Supervisor)
            .Where(ps => ps.StudentId == currentUser.Id && ps.ProjectId == id)
            .Select(ps => ps.Project)
            .FirstOrDefaultAsync();

        if (project == null)
            return NotFound();

        return View(project);
    }

    public async Task<IActionResult> Supervisors()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        var supervisors = await _context.ProjectStudents
            .Include(ps => ps.Project)
            .ThenInclude(p => p.SupervisorProjects)
            .ThenInclude(sp => sp.Supervisor)
            .Where(ps => ps.StudentId == currentUser.Id)
            .SelectMany(ps => ps.Project.SupervisorProjects)
            .Select(sp => sp.Supervisor)
            .Distinct()
            .ToListAsync();

        return View(supervisors);
    }
}
