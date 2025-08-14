using Microsoft.AspNetCore.Identity;

namespace Projectopia.Models;

public class User : IdentityUser
{
    public string FullName { get; set; }
}