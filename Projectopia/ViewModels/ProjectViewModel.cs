using System.ComponentModel.DataAnnotations;

namespace Projectopia.ViewModels;

public class ProjectViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Project Name is required")]
    [Display(Name = "Project Name")]
    public string Name { get; set; }

    [Display(Name = "Project Details")]
    public string Details { get; set; }

    [Display(Name = "Supervisors")]
    public List<string> SupervisorIds { get; set; } = new List<string>();

    [Display(Name = "Students")]
    public List<string> StudentIds { get; set; } = new List<string>();
}

public class ProjectDetailsViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Details { get; set; }
    public List<SupervisorInfo> Supervisors { get; set; } = new List<SupervisorInfo>();
    public List<StudentInfo> Students { get; set; } = new List<StudentInfo>();
}

public class SupervisorInfo
{
    public string Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
}

public class StudentInfo
{
    public string Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
}
