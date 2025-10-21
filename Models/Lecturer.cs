using System.ComponentModel.DataAnnotations;

namespace CMCS.Mvc.Models
{
    public class Lecturer
    {
        [Key] public int LecturerId { get; set; }
        [Required, StringLength(100)] public string FullName { get; set; } = string.Empty;
        [EmailAddress] public string? Email { get; set; }
    }
}
