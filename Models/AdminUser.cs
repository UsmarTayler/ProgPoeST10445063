using System.ComponentModel.DataAnnotations;

namespace CMCS.Mvc.Models
{
    public class AdminUser
    {
        [Key] public int AdminUserId { get; set; }
        [Required, StringLength(100)] public string FullName { get; set; } = string.Empty;
        [EmailAddress] public string? Email { get; set; }
        [Required, StringLength(50)] public string Role { get; set; } = "Coordinator";
    }
}
