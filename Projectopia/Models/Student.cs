using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Projectopia.Models;

public class Student : User
{
    public List<ProjectStudent> ProjectStudents { get; set; }
}