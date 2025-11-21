using CMCS.Mvc.Data;
using CMCS.Mvc.Filters;
using CMCS.Mvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[RequireRole("Coordinator")]
public class CoordinatorController : Controller
{
    private readonly CmcsContext _db;

    public CoordinatorController(CmcsContext db)
    {
        _db = db;
    }

    // -------------------------------------------------------------------------
    // COORDINATOR DASHBOARD – show submitted claims only
    // -------------------------------------------------------------------------
    public async Task<IActionResult> Index()
    {
        var submitted = await _db.Claims
            .Include(c => c.Lecturer)
            .Where(c => c.Status == ClaimStatus.Pending)
            .OrderByDescending(c => c.SubmissionDate)
            .ToListAsync();

        return View(submitted);
    }

    // -------------------------------------------------------------------------
    // REVIEW A SINGLE CLAIM
    // -------------------------------------------------------------------------
    public async Task<IActionResult> Review(int id)
    {
        var claim = await _db.Claims
            .Include(c => c.Lecturer)
            .Include(c => c.Documents)
            .FirstOrDefaultAsync(c => c.ClaimId == id);

        if (claim == null) return NotFound();

        return View(claim);
    }

    // -------------------------------------------------------------------------
    // APPROVE CLAIM (moves to Manager queue)
    // -------------------------------------------------------------------------
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(int id)
    {
        var claim = await _db.Claims.FindAsync(id);
        if (claim == null) return NotFound();

        claim.Status = ClaimStatus.Approved;
        claim.ApprovedOn = DateTime.Now;

        await _db.SaveChangesAsync();

        TempData["msg"] = $"Claim #{id} approved by Coordinator.";
        return RedirectToAction(nameof(Index));
    }

    // -------------------------------------------------------------------------
    // REJECT CLAIM
    // -------------------------------------------------------------------------
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(int id)
    {
        var claim = await _db.Claims.FindAsync(id);
        if (claim == null) return NotFound();

        claim.Status = ClaimStatus.Rejected;

        await _db.SaveChangesAsync();

        TempData["msg"] = $"Claim #{id} rejected.";
        return RedirectToAction(nameof(Index));
    }
}
