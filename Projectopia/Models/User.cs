using Microsoft.AspNetCore.Identity;

namespace Projectopia.Models;

public class User : IdentityUser<int>
{
    public string FullName { get; set; }
}