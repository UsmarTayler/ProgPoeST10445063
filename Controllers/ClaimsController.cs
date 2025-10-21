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
        ViewBag.Lecturers = new SelectList(
            _db.Lecturers.AsNoTracking().ToList(),
            "LecturerId",
            "FullName"
        );

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
            _db.Claims.Add(claim);
            await _db.SaveChangesAsync();

            async Task SaveDocAsync(IFormFile? file)
            {
                if (file == null || file.Length == 0) return;

                var allowed = new[] { ".pdf", ".docx", ".xlsx", ".png", ".jpg", ".jpeg" };
                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                const long MaxBytes = 35L * 1024 * 1024;

                if (!allowed.Contains(ext) || file.Length > MaxBytes)
                {
                    TempData["error"] = "Invalid file. Allowed: PDF/DOCX/XLSX/PNG/JPG (max 35 MB).";
                    return;
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
                    FileName = file.FileName,
                    FilePath = $"/uploads/{newName}",
                    UploadedAt = DateTime.Now
                });

                await _db.SaveChangesAsync();
            }

            await SaveDocAsync(upload1);
            await SaveDocAsync(upload2);

            TempData["msg"] = "Claim submitted successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception)
        {
            TempData["error"] = "Something went wrong while saving your claim or file. Please try again.";
            ViewBag.Months = Months();
            return View(claim);
        }
    }

    // ? Review Page (GET)
    [HttpGet]
    public async Task<IActionResult> Review()
    {
        var pending = await _db.Claims
            .Where(c => c.Status == ClaimStatus.Pending)
            .OrderByDescending(c => c.SubmissionDate)
            .ToListAsync();

        return View(pending);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(int id)
    {
        var claim = await _db.Claims.FindAsync(id);
        if (claim == null) return NotFound();

        claim.Status = ClaimStatus.Approved;
        await _db.SaveChangesAsync();

        TempData["msg"] = $"Claim #{id} has been successfully approved.";
        return RedirectToAction(nameof(Review));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(int id)
    {
        var claim = await _db.Claims.FindAsync(id);
        if (claim == null) return NotFound();

        claim.Status = ClaimStatus.Rejected;
        await _db.SaveChangesAsync();

        TempData["msg"] = $"Claim #{id} has been successfully rejected.";
        return RedirectToAction(nameof(Review));
    }


    private static List<string> Months() => new() {
        "January","February","March","April","May","June",
        "July","August","September","October","November","December"
    };
}
