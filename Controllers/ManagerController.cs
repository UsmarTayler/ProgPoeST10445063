using CMCS.Mvc.Data;
using CMCS.Mvc.Filters;
using CMCS.Mvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMCS.Mvc.Controllers
{
    [RequireRole("Manager")]
    public class ManagerController : Controller
    {
        private readonly CmcsContext _db;
        public ManagerController(CmcsContext db) => _db = db;

        // Manager dashboard (same pending list, separate role)
        public async Task<IActionResult> Index()
        {
            var pending = await _db.Claims
                .Include(c => c.Lecturer)
                .Where(c => c.Status == ClaimStatus.Pending)
                .OrderByDescending(c => c.SubmissionDate)
                .ToListAsync();

            return View("Pending", pending);
        }

        public async Task<IActionResult> Review(int id)
        {
            var claim = await _db.Claims
                .Include(c => c.Lecturer)
                .Include(c => c.Documents)
                .FirstOrDefaultAsync(c => c.ClaimId == id);

            if (claim == null) return NotFound();
            return View(claim);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var claim = await _db.Claims.FindAsync(id);
            if (claim == null) return NotFound();

            claim.Status = ClaimStatus.Approved;
            claim.ApprovedOn = DateTime.Now;
            await _db.SaveChangesAsync();

            TempData["msg"] = $"Claim #{id} approved by Manager.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var claim = await _db.Claims.FindAsync(id);
            if (claim == null) return NotFound();

            claim.Status = ClaimStatus.Rejected;
            await _db.SaveChangesAsync();

            TempData["msg"] = $"Claim #{id} rejected by Manager.";
            return RedirectToAction(nameof(Index));
        }
    }
}
