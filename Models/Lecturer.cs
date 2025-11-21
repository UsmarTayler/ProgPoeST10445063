namespace CMCS.Mvc.Models
{
    public class Lecturer
    {
        public int LecturerId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public decimal HourlyRate { get; set; }
    }
}
