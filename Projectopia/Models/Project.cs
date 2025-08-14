using System.ComponentModel.DataAnnotations;

namespace Projectopia.Models
{
    public class Project
    {
        public int Id { get; set; }

        [Required] public string Name { get; set; }

        public List<SupervisorProject> SupervisorProjects { get; set; }
        
        public List<ProjectStudent> ProjectStudents { get; set; }

        public string Details { get; set; }
    }
}