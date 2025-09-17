using CMCS.Mvc.Models;
using CMCS.Mvc.Services;
using Microsoft.AspNetCore.Mvc;
//Updateed SOme Files
namespace CMCS.Mvc.Controllers
{
    public class ClaimsController : Controller
    {
        private readonly IDummyDataProvider _data;
        public ClaimsController(IDummyDataProvider data) { _data = data; }

        public IActionResult Index(string? q)
        {
            var data = _data.GetAll();
            if (!string.IsNullOrWhiteSpace(q))
            {
                var f = q.Trim().ToLower();
                data = data.Where(c =>
                  (c.Month ?? string.Empty).ToLower().Contains(f) ||
                  c.Status.ToString().ToLower().Contains(f) ||
                  (c.LecturerName ?? string.Empty).ToLower().Contains(f)
                ).ToList();
            }
            ViewBag.Query = q;
            return View(data.OrderByDescending(c => c.SubmissionDate).ToList());
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Months = Months();
            return View(new Claim());
        }

        public IActionResult Review()
        {
            var pending = _data.GetPending();
            return View(pending.OrderByDescending(c => c.SubmissionDate).ToList());
        }

        private static List<string> Months() => new(){
    "January","February","March","April","May","June",
    "July","August","September","October","November","December"
  };
    }
}