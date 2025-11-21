using CMCS.Mvc.Data;
using CMCS.Mvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace CMCS.Mvc.Controllers
{
    public class AccountController : Controller
    {
        private readonly CmcsContext _db;
        public AccountController(CmcsContext db) => _db = db;

        // GET: /Account/Login?role=HR (or Coordinator/Manager)
        [HttpGet]
        public IActionResult Login(string role)
        {
            ViewBag.Role = role;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password, string role)
        {
            var hash = Hash(password);

            var user = await _db.AdminUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email && u.PasswordHash == hash && u.Role == role);

            if (user == null)
            {
                ViewBag.Role = role;
                ViewBag.Error = "Invalid login details.";
                return View();
            }

            // ✅ store login in session
            HttpContext.Session.SetString("Role", user.Role);
            HttpContext.Session.SetString("UserName", user.FullName);

            // redirect per role
            return role switch
            {
                "Coordinator" => RedirectToAction("Index", "Coordinator"),
                "Manager" => RedirectToAction("Index", "Manager"),
                "HR" => RedirectToAction("Dashboard", "HR"),
                _ => RedirectToAction("Index", "Claims")
            };
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Claims");
        }

        private static string Hash(string input)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToHexString(bytes);
        }
    }
}
