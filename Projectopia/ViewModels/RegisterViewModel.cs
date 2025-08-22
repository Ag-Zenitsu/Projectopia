using System.ComponentModel.DataAnnotations;

namespace Projectopia.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Full Name is required")]
    [Display(Name = "Full Name")]
    public string FullName { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }

    [Required(ErrorMessage = "User Type is required")]
    [Display(Name = "User Type")]
    public string UserType { get; set; }
}
