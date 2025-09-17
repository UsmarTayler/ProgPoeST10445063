using CMCS.Mvc.Models;
namespace CMCS.Mvc.Services
{
    public class DummyDataProvider : IDummyDataProvider
    {
        private static readonly List<Claim> _claims = new();
        private static int _nextId = 1001;
        private static bool _seeded = false;
        public DummyDataProvider()
        {
            if (_seeded) return; _seeded = true;
            _claims.Add(new Claim
            {
                ClaimId = _nextId++,
                LecturerName = "Dr. M. Dlamini",
                Month = "May",
                HoursWorked = 18,
                HourlyRate = 350,
                Status = ClaimStatus.Pending,
                SubmissionDate = DateTime.Today.AddDays(-2),
                Documents = new() { new SupportingDocument { FileName = "Timesheet_May.pdf" } }
            });
            _claims.Add(new Claim
            {
                ClaimId = _nextId++,
                LecturerName = "Prof. A. Naidoo",
                Month = "April",
                HoursWorked = 22,
                HourlyRate = 360,
                Status = ClaimStatus.Approved,
                SubmissionDate = DateTime.Today.AddDays(-12),
                Documents = new() { new SupportingDocument { FileName = "Evidence_April.docx" } }
            });
        }
        public List<Claim> GetAll() => _claims.ToList();
        public List<Claim> GetPending() => _claims.Where(c => c.Status == ClaimStatus.Pending).ToList();
    }
}