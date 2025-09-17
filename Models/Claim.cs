using System.ComponentModel.DataAnnotations;
//For Commit purposes
namespace CMCS.Mvc.Models
{
    public class Claim
    {
        public int ClaimId { get; set; }

        [Required]
        public string LecturerName { get; set; } = string.Empty;

        [Required]
        public string Month { get; set; } = string.Empty;

        [Range(0.1, 1000)]
        public double HoursWorked { get; set; }

        [Range(0.1, 100000)]
        public decimal HourlyRate { get; set; }

        public string? Description { get; set; }
        public ClaimStatus Status { get; set; } = ClaimStatus.Pending;
        public DateTime SubmissionDate { get; set; } = DateTime.Now;

        public List<SupportingDocument> Documents { get; set; } = new();

        public decimal TotalAmount => (decimal)HoursWorked * HourlyRate;
    }
}
