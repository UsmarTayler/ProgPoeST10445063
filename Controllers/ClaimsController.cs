using CMCS.Mvc.Data;
using CMCS.Mvc.Models;
using Microsoft.AspNetCore.Mvc;
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

        _db.Claims.Add(claim);
        await _db.SaveChangesAsync();

        async Task SaveDoc(IFormFile? file)
        {
            if (file == null || file.Length == 0) return;

            var allowed = new[] { ".pdf", ".docx", ".xlsx", ".png", ".jpg", ".jpeg" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowed.Contains(ext) || file.Length > 5 * 1024 * 1024)
            {
                TempData["error"] = "Invalid file type or size (max 5 MB).";
                return;
            }

            var folder = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(folder);
            var newName = $"{Guid.NewGuid()}{ext}";
            var path = Path.Combine(folder, newName);
            using var fs = System.IO.File.Create(path);
            await file.CopyToAsync(fs);

            _db.SupportingDocuments.Add(new SupportingDocument
            {
                ClaimId = claim.ClaimId,
                FileName = file.FileName,
                FilePath = $"/uploads/{newName}"
            });
            await _db.SaveChangesAsync();
        }

        await SaveDoc(upload1);
        await SaveDoc(upload2);

        TempData["msg"] = "Claim submitted successfully.";
        return RedirectToAction(nameof(Index));
    }

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
        return RedirectToAction(nameof(Review));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(int id)
    {
        var claim = await _db.Claims.FindAsync(id);
        if (claim == null) return NotFound();
        claim.Status = ClaimStatus.Rejected;
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Review));
    }

    private static List<string> Months() => new() {
        "January","February","March","April","May","June",
        "July","August","September","October","November","December"
    };
}
