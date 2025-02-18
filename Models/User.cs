using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

public class User : IdentityUser
{
    [Key]
    public string Email { get; set; } // Email as primary key
    
    [Required]
    public string Password { get; set; } // Hashed password

    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }

    [Required]
    public string Role { get; set; } // "admin" or "contributor"

    public bool Approved { get; set; } = false; // Default false (admin must approve)
}
