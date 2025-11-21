using System.ComponentModel.DataAnnotations;

namespace CMCS.Mvc.Models
{
    public class AdminUser
    {
        [Key]
        public int AdminId { get; set; }

        [Required, MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required, MaxLength(500)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string Role { get; set; } = "HR";  // default role
    }
}
