using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Projectopia.Models;

namespace Projectopia.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public DbSet<SupervisorProject> SupervisorProjects { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectStudent> ProjectStudents { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Supervisor> Supervisors { get; set; }
  
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().UseTphMappingStrategy();
        base.OnModelCreating(modelBuilder);
    }
}