using System.ComponentModel.DataAnnotations;

namespace Projectopia.ViewModels;

public class UserManagementViewModel
{
    public string Id { get; set; }

    [Required(ErrorMessage = "Full Name is required")]
    [Display(Name = "Full Name")]
    public string FullName { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public string Email { get; set; }

    [Display(Name = "User Type")]
    public string UserType { get; set; }

    [Display(Name = "Phone Number")]
    public string PhoneNumber { get; set; }

    [Display(Name = "Is Active")]
    public bool IsActive { get; set; }
}

public class UserListViewModel
{
    public string Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string UserType { get; set; }
    public string PhoneNumber { get; set; }
    public bool IsActive { get; set; }
}
