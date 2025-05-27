using Farm.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mvcbasic.Data;

namespace ConsoleDI.Example.Controllers.Admin
{
    public class SurveyFormsController : Controller
    {
        private readonly MvcBasicDbContext _context;

        public SurveyFormsController(MvcBasicDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var role = HttpContext.Session.GetInt32("Role");
            if (role != 2) // Chỉ Employee (Role = 2)
            {
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            var surveyForms = await _context.SurveyForms.Include(s => s.Cage).ToListAsync();
            return View(surveyForms);
        }

        public async Task<IActionResult> Create()
        {
            var role = HttpContext.Session.GetInt32("Role");
            if (role != 2)
            {
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            ViewBag.Cages = new SelectList(await _context.Cage.ToListAsync(), "Id", "CageNumber");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CageId,ChickenCount")] SurveyForm surveyForm)
        {
            var role = HttpContext.Session.GetInt32("Role");
            if (role != 2)
            {
                return View("~/Views/Shared/Unauthorized.cshtml");
            }
            if (ModelState.IsValid)
            {
                surveyForm.SurveyDate = DateTime.Now;
                _context.Add(surveyForm);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Cages = new SelectList(await _context.Cage.ToListAsync(), "Id", "CageNumber");
            return View(surveyForm);
        }
    }
}