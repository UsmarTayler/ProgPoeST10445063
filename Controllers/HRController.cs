using CMCS.Mvc.Data;
using CMCS.Mvc.Filters;
using CMCS.Mvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMCS.Mvc.Controllers
{
    [RequireRole("HR")]
    public class HRController : Controller
    {
        private readonly CmcsContext _db;
        public HRController(CmcsContext db) => _db = db;

        // ----------------------- DASHBOARD -----------------------
        public IActionResult Dashboard() => View();


        // --------------------- MANAGE LECTURERS ------------------
        public async Task<IActionResult> ManageLecturers()
        {
            var lecturers = await _db.Lecturers.AsNoTracking().ToListAsync();
            return View(lecturers);
        }

        [HttpGet]
        public IActionResult CreateLecturer() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateLecturer(Lecturer lecturer)
        {
            if (!ModelState.IsValid) return View(lecturer);

            _db.Lecturers.Add(lecturer);
            await _db.SaveChangesAsync();
            TempData["msg"] = "Lecturer added.";
            return RedirectToAction(nameof(ManageLecturers));
        }

        [HttpGet]
        public async Task<IActionResult> EditLecturer(int id)
        {
            var lec = await _db.Lecturers.FindAsync(id);
            if (lec == null) return NotFound();
            return View(lec);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLecturer(Lecturer lecturer)
        {
            if (!ModelState.IsValid) return View(lecturer);

            _db.Lecturers.Update(lecturer);
            await _db.SaveChangesAsync();
            TempData["msg"] = "Lecturer updated.";
            return RedirectToAction(nameof(ManageLecturers));
        }

        [HttpGet]
        public async Task<IActionResult> DeleteLecturer(int id)
        {
            var lec = await _db.Lecturers.FindAsync(id);
            if (lec == null) return NotFound();

            _db.Lecturers.Remove(lec);
            await _db.SaveChangesAsync();
            TempData["msg"] = "Lecturer deleted.";
            return RedirectToAction(nameof(ManageLecturers));
        }


        // ----------------------- MONTHLY SUMMARY -----------------------
        [HttpGet]
        public async Task<IActionResult> Summary(int? month, int? year)
        {
            int selectedMonth = month ?? DateTime.Now.Month;
            int selectedYear = year ?? DateTime.Now.Year;

            var approved = await _db.Claims
                .Include(c => c.Lecturer)
                .Where(c => c.Status == ClaimStatus.Approved &&
                            c.SubmissionDate.Month == selectedMonth &&
                            c.SubmissionDate.Year == selectedYear)
                .ToListAsync();

            var data = approved
                .GroupBy(c => c.Lecturer!)
                .Select(g => new HrSummaryVM
                {
                    LecturerId = g.Key.LecturerId,
                    LecturerName = g.Key.FullName,
                    ClaimsCount = g.Count(),
                    TotalHours = g.Sum(x => x.HoursWorked),
                    TotalAmount = g.Sum(x => x.TotalAmount)
                })
                .ToList();

            ViewBag.Month = selectedMonth;
            ViewBag.Year = selectedYear;

            return View(data);     // ✅ MUST RETURN HrSummaryVM LIST
        }


        // ----------------- PROCESS APPROVED CLAIMS --------------------
        [HttpGet]
        public IActionResult ProcessMonth() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessMonth(string month, int year)
        {
            if (string.IsNullOrWhiteSpace(month) || year == 0)
            {
                TempData["error"] = "Please select both month and year.";
                return RedirectToAction(nameof(ProcessMonth));
            }

            var claims = await _db.Claims
                .Where(c => c.Month == month &&
                            c.SubmissionDate.Year == year &&
                            c.Status == ClaimStatus.Approved)
                .ToListAsync();

            if (!claims.Any())
            {
                TempData["info"] = "No approved claims found for this selection.";
                return RedirectToAction(nameof(ProcessMonth));
            }

            foreach (var claim in claims)
                claim.Status = ClaimStatus.Processed;

            await _db.SaveChangesAsync();

            TempData["success"] = $"{claims.Count} claims processed.";
            return RedirectToAction(nameof(ProcessMonth));
        }
    }
}
