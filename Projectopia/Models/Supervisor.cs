using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Projectopia.Models;

public class Supervisor : User
{
    public List<SupervisorProject> SupervisorProjects { get; set; }
}