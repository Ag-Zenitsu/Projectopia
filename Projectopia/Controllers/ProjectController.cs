using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projectopia.Data;
using Projectopia.Models;
using Projectopia.ViewModels;

namespace Projectopia.Controllers;

[Authorize]
public class ProjectController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;

    public ProjectController(ApplicationDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: Project
    public async Task<IActionResult> Index()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");
        var isSupervisor = await _userManager.IsInRoleAsync(currentUser, "Supervisor");

        List<Project> projects;
        if (isAdmin)
        {
            projects = await _context.Projects
                .Include(p => p.SupervisorProjects)
                .ThenInclude(sp => sp.Supervisor)
                .Include(p => p.ProjectStudents)
                .ThenInclude(ps => ps.Student)
                .ToListAsync();
        }
        else if (isSupervisor)
        {
            var supervisor = await _context.Supervisors
                .Include(s => s.SupervisorProjects)
                .ThenInclude(sp => sp.Project)
                .ThenInclude(p => p.ProjectStudents)
                .ThenInclude(ps => ps.Student)
                .FirstOrDefaultAsync(s => s.Id == currentUser.Id);

            projects = supervisor?.SupervisorProjects.Select(sp => sp.Project).ToList() ?? new List<Project>();
        }
        else
        {
            var student = await _context.Students
                .Include(s => s.ProjectStudents)
                .ThenInclude(ps => ps.Project)
                .ThenInclude(p => p.SupervisorProjects)
                .ThenInclude(sp => sp.Supervisor)
                .FirstOrDefaultAsync(s => s.Id == currentUser.Id);

            projects = student?.ProjectStudents.Select(ps => ps.Project).ToList() ?? new List<Project>();
        }

        return View(projects);
    }

    // GET: Project/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var project = await _context.Projects
            .Include(p => p.SupervisorProjects)
            .ThenInclude(sp => sp.Supervisor)
            .Include(p => p.ProjectStudents)
            .ThenInclude(ps => ps.Student)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (project == null)
            return NotFound();

        var viewModel = new ProjectDetailsViewModel
        {
            Id = project.Id,
            Name = project.Name,
            Details = project.Details,
            Supervisors = project.SupervisorProjects?.Select(sp => new SupervisorInfo
            {
                Id = sp.Supervisor.Id,
                FullName = sp.Supervisor.FullName,
                Email = sp.Supervisor.Email
            }).ToList() ?? new List<SupervisorInfo>(),
            Students = project.ProjectStudents?.Select(ps => new StudentInfo
            {
                Id = ps.Student.Id,
                FullName = ps.Student.FullName,
                Email = ps.Student.Email
            }).ToList() ?? new List<StudentInfo>()
        };

        return View(viewModel);
    }

    // GET: Project/Create
    [Authorize(Roles = "Admin,Supervisor")]
    public async Task<IActionResult> Create()
    {
        var supervisors = await _context.Supervisors.ToListAsync();
        var students = await _context.Students.ToListAsync();

        ViewBag.Supervisors = supervisors;
        ViewBag.Students = students;

        return View();
    }

    // POST: Project/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Supervisor")]
    public async Task<IActionResult> Create(ProjectViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            var supervisors = await _context.Supervisors.ToListAsync();
            var students = await _context.Students.ToListAsync();
            ViewBag.Supervisors = supervisors;
            ViewBag.Students = students;
            return View(viewModel);
        }

        var project = new Project
        {
            Name = viewModel.Name,
            Details = viewModel.Details
        };

        _context.Add(project);
        await _context.SaveChangesAsync();

        // Add supervisors
        if (viewModel.SupervisorIds != null)
        {
            foreach (var supervisorId in viewModel.SupervisorIds)
            {
                var supervisorProject = new SupervisorProject
                {
                    ProjectId = project.Id,
                    SupervisorId = supervisorId
                };
                _context.SupervisorProjects.Add(supervisorProject);
            }
        }

        // Add students
        if (viewModel.StudentIds != null)
        {
            foreach (var studentId in viewModel.StudentIds)
            {
                var projectStudent = new ProjectStudent
                {
                    ProjectId = project.Id,
                    StudentId = studentId
                };
                _context.ProjectStudents.Add(projectStudent);
            }
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: Project/Edit/5
    [Authorize(Roles = "Admin,Supervisor")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var project = await _context.Projects
            .Include(p => p.SupervisorProjects)
            .Include(p => p.ProjectStudents)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (project == null)
            return NotFound();

        var viewModel = new ProjectViewModel
        {
            Id = project.Id,
            Name = project.Name,
            Details = project.Details,
            SupervisorIds = project.SupervisorProjects?.Select(sp => sp.SupervisorId).ToList() ?? new List<string>(),
            StudentIds = project.ProjectStudents?.Select(ps => ps.StudentId).ToList() ?? new List<string>()
        };

        var supervisors = await _context.Supervisors.ToListAsync();
        var students = await _context.Students.ToListAsync();

        ViewBag.Supervisors = supervisors;
        ViewBag.Students = students;

        return View(viewModel);
    }

    // POST: Project/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Supervisor")]
    public async Task<IActionResult> Edit(int id, ProjectViewModel viewModel)
    {
        if (id != viewModel.Id)
            return NotFound();

        if (!ModelState.IsValid)
        {
            var supervisors = await _context.Supervisors.ToListAsync();
            var students = await _context.Students.ToListAsync();
            ViewBag.Supervisors = supervisors;
            ViewBag.Students = students;
            return View(viewModel);
        }

        try
        {
            var project = await _context.Projects
                .Include(p => p.SupervisorProjects)
                .Include(p => p.ProjectStudents)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
                return NotFound();

            project.Name = viewModel.Name;
            project.Details = viewModel.Details;

            // Update supervisors
            _context.SupervisorProjects.RemoveRange(project.SupervisorProjects);
            if (viewModel.SupervisorIds != null)
            {
                foreach (var supervisorId in viewModel.SupervisorIds)
                {
                    var supervisorProject = new SupervisorProject
                    {
                        ProjectId = project.Id,
                        SupervisorId = supervisorId
                    };
                    _context.SupervisorProjects.Add(supervisorProject);
                }
            }

            // Update students
            _context.ProjectStudents.RemoveRange(project.ProjectStudents);
            if (viewModel.StudentIds != null)
            {
                foreach (var studentId in viewModel.StudentIds)
                {
                    var projectStudent = new ProjectStudent
                    {
                        ProjectId = project.Id,
                        StudentId = studentId
                    };
                    _context.ProjectStudents.Add(projectStudent);
                }
            }

            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ProjectExists(viewModel.Id))
                return NotFound();
            else
                throw;
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: Project/Delete/5
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var project = await _context.Projects
            .Include(p => p.SupervisorProjects)
            .ThenInclude(sp => sp.Supervisor)
            .Include(p => p.ProjectStudents)
            .ThenInclude(ps => ps.Student)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (project == null)
            return NotFound();

        return View(project);
    }

    // POST: Project/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var project = await _context.Projects
            .Include(p => p.SupervisorProjects)
            .Include(p => p.ProjectStudents)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (project != null)
        {
            _context.SupervisorProjects.RemoveRange(project.SupervisorProjects);
            _context.ProjectStudents.RemoveRange(project.ProjectStudents);
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private bool ProjectExists(int id)
    {
        return _context.Projects.Any(e => e.Id == id);
    }
}