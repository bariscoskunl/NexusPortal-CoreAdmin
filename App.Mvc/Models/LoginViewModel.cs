using System.ComponentModel.DataAnnotations;

namespace App.Mvc.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email zorunludur.")]       
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz.")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;


        [Required(ErrorMessage = "Şifre zorunludur.")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6 karakter olmalı.")]
        public string Password { get; set; } = string.Empty;
    }
}
