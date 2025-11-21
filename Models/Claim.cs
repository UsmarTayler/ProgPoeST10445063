using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMCS.Mvc.Models
{
    public class Claim
    {
        [Key]
        public int ClaimId { get; set; }

        [Required]
        public int LecturerId { get; set; }
        public Lecturer? Lecturer { get; set; }

        [Required]
        public string Month { get; set; } = "";

        [Range(1, 300)]
        public int HoursWorked { get; set; }

        [Range(50, 3000)]
        public decimal HourlyRate { get; set; }

        [NotMapped]
        public decimal TotalAmount => (HoursWorked * HourlyRate);

        public string? Description { get; set; }

        public ClaimStatus Status { get; set; } = ClaimStatus.Pending;
        public DateTime SubmissionDate { get; set; }

        public DateTime? ApprovedOn { get; set; }

        // ⭐ FIXED — navigation property required by EF
        public List<SupportingDocument> Documents { get; set; } = new();
    }

    public enum ClaimStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2,
        Processed = 3
    }

}
