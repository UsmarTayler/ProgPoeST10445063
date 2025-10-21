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

        [Required]
        public string Month { get; set; } = string.Empty;

        [Range(0.1, 1000)]
        public double HoursWorked { get; set; }

        [Range(0.1, 100000)]
        public decimal HourlyRate { get; set; }

        public string? Description { get; set; }

        [Required]
        public ClaimStatus Status { get; set; } = ClaimStatus.Pending;

        public DateTime SubmissionDate { get; set; } = DateTime.Now;

        // Navigation property (not required but useful if you join Lecturer later)
        public Lecturer? Lecturer { get; set; }

        public List<SupportingDocument> Documents { get; set; } = new();

        [NotMapped]
        public decimal TotalAmount => (decimal)HoursWorked * HourlyRate;
    }
}
