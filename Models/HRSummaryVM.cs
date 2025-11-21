namespace CMCS.Mvc.Models
{
    public class HrSummaryVM
    {
        public int LecturerId { get; set; }
        public string LecturerName { get; set; } = string.Empty;

        public int ClaimsCount { get; set; }
        public double TotalHours { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
