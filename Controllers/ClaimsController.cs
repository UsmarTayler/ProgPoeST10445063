using CMCS.Mvc.Data;
using CMCS.Mvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;

public class ClaimsController : Controller
{
    private readonly CmcsContext _db;
    private readonly IWebHostEnvironment _env;
    public ClaimsController(CmcsContext db, IWebHostEnvironment env)
    {
        _db = db; _env = env;
    }

    // List + optional filter
    public async Task<IActionResult> Index(string? q)
    {
        var data = await _db.Claims.AsNoTracking()
            .OrderByDescending(c => c.SubmissionDate)
            .ToListAsync();

        if (!string.IsNullOrWhiteSpace(q))
        {
            var f = q.Trim().ToLowerInvariant();
            data = data.Where(c =>
                (c.Month ?? string.Empty).ToLowerInvariant().Contains(f) ||
                c.Status.ToString().ToLowerInvariant().Contains(f) ||
                (c.Description ?? string.Empty).ToLowerInvariant().Contains(f) ||
                c.LecturerId.ToString().Contains(f)
            ).ToList();
        }

        ViewBag.Query = q;
        return View(data);
    }

    // Submit Claim (GET)
    public IActionResult Create()
    {
        // Populate dropdown for lecturer selection
        ViewBag.Lecturers = new SelectList(
            _db.Lecturers.AsNoTracking().ToList(),
            "LecturerId",
            "FullName"
        );

        // Populate dropdown for months
        ViewBag.Months = Months();

        return View(new Claim());
    }

    // Submit Claim (POST) + file upload
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Claim claim, IFormFile? upload1, IFormFile? upload2)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Months = Months();
            return View(claim);
        }

        claim.Status = ClaimStatus.Pending;
        claim.SubmissionDate = DateTime.Now;

        try
        {
            // Save claim first so we have ClaimId for documents
            _db.Claims.Add(claim);
            await _db.SaveChangesAsync();

            // Local helper to store a single file
            async Task SaveDocAsync(IFormFile? file)
            {
                if (file == null || file.Length == 0) return;

                var allowed = new[] { ".pdf", ".docx", ".xlsx", ".png", ".jpg", ".jpeg" };
                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

                // 35 MB maximum
                const long MaxBytes = 35L * 1024 * 1024;

                if (!allowed.Contains(ext) || file.Length > MaxBytes)
                {
                    TempData["error"] = "Invalid file. Allowed: PDF/DOCX/XLSX/PNG/JPG (max 35 MB).";
                    return; // skip this file; still keep the claim
                }

                var folder = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(folder);

                var newName = $"{Guid.NewGuid()}{ext}";
                var fullPath = Path.Combine(folder, newName);

                using (var fs = System.IO.File.Create(fullPath))
                    await file.CopyToAsync(fs);

                _db.SupportingDocuments.Add(new SupportingDocument
                {
                    ClaimId = claim.ClaimId,
                    FileName = file.FileName,          // original name
                    FilePath = $"/uploads/{newName}",  // public path
                    UploadedAt = DateTime.Now
                });

                await _db.SaveChangesAsync();
            }

            // Save up to two docs (safe if null)
            await SaveDocAsync(upload1);
            await SaveDocAsync(upload2);

            TempData["msg"] = "Claim submitted successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception)
        {
            // Friendly message; keep the user on the form with their data
            TempData["error"] = "Something went wrong while saving your claim or file. Please try again.";
            ViewBag.Months = Months();
            return View(claim);
        }
    }


    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(int id)
    {
        var claim = await _db.Claims.FindAsync(id);
        if (claim == null) return NotFound();
        claim.Status = ClaimStatus.Approved;
        await _db.SaveChangesAsync();
        return RedirectToAction("Review");

    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(int id)
    {
        var claim = await _db.Claims.FindAsync(id);
        if (claim == null) return NotFound();
        claim.Status = ClaimStatus.Rejected;
        await _db.SaveChangesAsync();
        return RedirectToAction("Review");

    }

    private static List<string> Months() => new() {
        "January","February","March","April","May","June",
        "July","August","September","October","November","December"
    };
}
