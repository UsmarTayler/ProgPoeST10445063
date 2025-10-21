using System.ComponentModel.DataAnnotations;

namespace CMCS.Mvc.Models
{
    public class SupportingDocument
    {
        [Key] public int DocumentId { get; set; }
        [Required] public int ClaimId { get; set; }

        [Required, StringLength(255)] public string FileName { get; set; } = string.Empty;
        [StringLength(400)] public string? FilePath { get; set; }   // e.g. /uploads/xxx.pdf
        public DateTime UploadedAt { get; set; } = DateTime.Now;
    }
}
