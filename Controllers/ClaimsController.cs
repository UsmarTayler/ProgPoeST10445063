using CMCS.Mvc.Data;
using CMCS.Mvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

public class ClaimsController : Controller
{
    private readonly CmcsContext _db;
    private readonly IWebHostEnvironment _env;

    public ClaimsController(CmcsContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    // -------------------------------------------------------------------------
    // INDEX + SEARCH (shows all claims)
    // -------------------------------------------------------------------------
    public async Task<IActionResult> Index(string? q)
    {
        var claimsQuery = _db.Claims
            .Include(c => c.Lecturer)
            .OrderByDescending(c => c.SubmissionDate)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            q = q.ToLower().Trim();
            claimsQuery = claimsQuery.Where(c =>
                c.Month.ToLower().Contains(q) ||
                c.Lecturer!.FullName.ToLower().Contains(q) ||
                (c.Description ?? "").ToLower().Contains(q) ||
                c.Status.ToString().ToLower().Contains(q)
            );
        }

        ViewBag.Query = q;
        return View(await claimsQuery.ToListAsync());
    }

    // -------------------------------------------------------------------------
    // AJAX: GetRate (used for auto-fill in view)
    // -------------------------------------------------------------------------
    public IActionResult GetRate(int lecturerId)
    {
        var lecturer = _db.Lecturers.Find(lecturerId);
        if (lecturer == null)
            return Json(new { rate = 0 });

        return Json(new { rate = lecturer.HourlyRate });
    }

    // -------------------------------------------------------------------------
    // CREATE GET
    // -------------------------------------------------------------------------
    public IActionResult Create()
    {
        ReloadCreateLists();
        return View(new Claim());
    }

    // -------------------------------------------------------------------------
    // CREATE POST  (FIXED)
    // NOTE: view uses upload1 + upload2, so we accept both.
    // -------------------------------------------------------------------------
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Claim claim, IFormFile? upload1 = null, IFormFile? upload2 = null)

    {
        // If dropdowns / fields invalid, reload lists and show form again
        if (!ModelState.IsValid)
        {
            ReloadCreateLists();
            return View(claim);
        }

        // SERVER-SIDE hourly rate pull (rubric requirement)
        var lecturer = await _db.Lecturers.FirstOrDefaultAsync(l => l.LecturerId == claim.LecturerId);
        if (lecturer == null)
        {
            ModelState.AddModelError("LecturerId", "Selected lecturer not found.");
            ReloadCreateLists();
            return View(claim);
        }

        claim.HourlyRate = lecturer.HourlyRate;   // force correct rate from DB
        claim.Status = ClaimStatus.Pending;       // Pending = 0
        claim.SubmissionDate = DateTime.Now;

        _db.Claims.Add(claim);
        await _db.SaveChangesAsync();

        // Save uploads (if any)
        if (upload1 != null && upload1.Length > 0)
            await SaveDocument(claim.ClaimId, upload1);

        if (upload2 != null && upload2.Length > 0)
            await SaveDocument(claim.ClaimId, upload2);

        TempData["success"] = "Claim submitted successfully!";
        return RedirectToAction(nameof(Index));
    }

    // -------------------------------------------------------------------------
    // Helper: reload dropdowns
    // -------------------------------------------------------------------------
    private void ReloadCreateLists()
    {
        ViewBag.Lecturers = new SelectList(_db.Lecturers, "LecturerId", "FullName");
        ViewBag.Months = new SelectList(new[]
        {
            "January","February","March","April","May","June",
            "July","August","September","October","November","December"
        });
    }

    // -------------------------------------------------------------------------
    // Helper: save a single supporting document
    // -------------------------------------------------------------------------
    private async Task SaveDocument(int claimId, IFormFile upload)
    {
        var allowed = new[] { ".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png" };
        var ext = Path.GetExtension(upload.FileName).ToLower();

        if (!allowed.Contains(ext))
        {
            TempData["error"] = $"Invalid file type: {ext}. Allowed: PDF, DOC, DOCX, JPG, PNG.";
            return;
        }

        var folder = Path.Combine(_env.WebRootPath, "uploads");
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        var savedName = $"{Guid.NewGuid()}{ext}";
        var filePath = Path.Combine(folder, savedName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await upload.CopyToAsync(stream);
        }

        var doc = new SupportingDocument
        {
            ClaimId = claimId,
            FileName = upload.FileName,
            FilePath = "/uploads/" + savedName,
            UploadedAt = DateTime.Now
        };

        _db.SupportingDocuments.Add(doc);
        await _db.SaveChangesAsync();
    }

    // -------------------------------------------------------------------------
    // REVIEW SCREEN (legacy - optional)
    // -------------------------------------------------------------------------
    public async Task<IActionResult> Review()
    {
        var pending = await _db.Claims
            .Include(c => c.Lecturer)
            .Where(c => c.Status == ClaimStatus.Pending)
            .OrderByDescending(c => c.SubmissionDate)
            .ToListAsync();

        return View(pending);
    }

    // -------------------------------------------------------------------------
    // APPROVE (legacy - optional)
    // -------------------------------------------------------------------------
    [HttpPost]
    public async Task<IActionResult> Approve(int id)
    {
        var claim = await _db.Claims.FindAsync(id);
        if (claim == null) return NotFound();

        claim.Status = ClaimStatus.Approved;
        claim.ApprovedOn = DateTime.Now;

        await _db.SaveChangesAsync();
        TempData["msg"] = $"Claim #{id} approved.";

        return RedirectToAction(nameof(Review));
    }

    // -------------------------------------------------------------------------
    // REJECT (legacy - optional)
    // -------------------------------------------------------------------------
    [HttpPost]
    public async Task<IActionResult> Reject(int id)
    {
        var claim = await _db.Claims.FindAsync(id);
        if (claim == null) return NotFound();

        claim.Status = ClaimStatus.Rejected;
        await _db.SaveChangesAsync();

        TempData["msg"] = $"Claim #{id} rejected.";
        return RedirectToAction(nameof(Review));
    }
}
