namespace Projectopia.Models;

public class SupervisorProject
{
    public int Id { get; set; }
    
    public int ProjectId { get; set; }
    public Project Project { get; set; }
    
    public string SupervisorId { get; set; }
    public Supervisor Supervisor { get; set; }
}