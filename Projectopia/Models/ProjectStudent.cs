namespace Projectopia.Models;

public class ProjectStudent
{
    public int Id { get; set; }
    
    public int StudentId { get; set; }
    public Student Student { get; set; }
    
    public int ProjectId { get; set; }
    public Project Project { get; set; }
}