using System.ComponentModel.DataAnnotations;

namespace App.Mvc.Models
{
    public class RegisterViewModel
    {
        [Required]
        public string UserName { get; set; } = string.Empty;
        
        [Required, EmailAddress]    
        public string Email { get; set; }

        [Required, DataType(DataType.Password), StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string PasswordConfirm { get; set; } 

    }
}
