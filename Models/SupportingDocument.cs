using System.ComponentModel.DataAnnotations;

namespace CMCS.Mvc.Models
{
    public class SupportingDocument
    {
        [Key]
        public int DocumentId { get; set; }

        [Required]
        public int ClaimId { get; set; }
        public Claim? Claim { get; set; }

        [Required, MaxLength(260)]
        public string FileName { get; set; } = "";

        [Required]
        public string FilePath { get; set; } = "";

        public DateTime UploadedAt { get; set; }
    }
}
