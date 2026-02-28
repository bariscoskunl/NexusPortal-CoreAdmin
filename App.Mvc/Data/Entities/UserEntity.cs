using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Mvc.Data.Entities
{
    public class UserEntity
    {
        [Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string UserName { get; set; } = default!;

        [Required, MaxLength(100), EmailAddress]
        public string Email { get; set; } = default!;

        [Required, MaxLength(100)]
        public string Password { get; set; } = default!;

        [Required]
        public int RoleId { get; set; }

        [ForeignKey(nameof(RoleId))]
        public RoleEntity Role { get; set; } = default!;

        public bool IsApproved { get; set; } = false;

        public bool IsRejected { get; set; } = false;


    }

}
