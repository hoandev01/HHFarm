using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Farm.Models;
using mvcbasic.Data;

namespace ConsoleDI.Example.Controllers.Admin
{
    public class SurveysController : Controller
    {
        private readonly MvcBasicDbContext _context;

        public SurveysController(MvcBasicDbContext context)
        {
            _context = context;
        }

        private int GetUserRole()
        {
            return HttpContext.Session.GetInt32("Role") ?? 0; // 0: Customer, 1: Admin, 2: Employee
        }

        // GET: Surveys/Index
        public async Task<IActionResult> Index()
        {
            int role = GetUserRole();
            if (role != 1 && role != 0 && role != 2) // Chỉ Admin (1), nhân viên (2) và Khách (0) được truy cập
            {
                return View("~/Views/Shared/Unauthorized.cshtml");
            }

            try
            {
                var surveys = await _context.Surveys.ToListAsync();
                return View(surveys);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy danh sách: {ex.Message}");
                return View(new List<Survey>());
            }
        }

        // GET: Surveys/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            int role = GetUserRole();
            if (role == 0 || role == 1 || role == 2) // Customer (0), Admin (1), Employee (2) đều được
            {
                if (id == null)
                {
                    return NotFound();
                }

                var survey = await _context.Surveys.FirstOrDefaultAsync(m => m.Id == id);
                if (survey == null)
                {
                    return NotFound();
                }

                return View(survey);
            }
            return View("~/Views/Shared/Unauthorized.cshtml");
        }

        // GET: Surveys/Create
        public IActionResult Create()
        {
            int role = GetUserRole();
            if (role == 1 || role == 2) // Admin (1) và Employee (2) được tạo
            {
                return View();
            }
            return View("~/Views/Shared/Unauthorized.cshtml");
        }

        // POST: Surveys/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CageName,CageCode,Location,CageArea,ChickenCount,ChickenBreed,BreedOther,Purpose,PurposeOther,MainFeed,WaterSource,WaterSourceOther,Ventilation,Lighting,Hygiene,Notes")] Survey survey)
        {
            int role = GetUserRole();
            if (role == 1 || role == 2) // Admin (1) và Employee (2) được tạo
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors);
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"Validation Error: {error.ErrorMessage}");
                    }
                    return View(survey);
                }

                try
                {
                    _context.Surveys.Add(survey);
                    await _context.SaveChangesAsync();
                    Console.WriteLine($"Thêm thành công Survey với Id: {survey.Id}");
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi khi lưu: {ex.InnerException?.Message ?? ex.Message}");
                    ModelState.AddModelError("", "Lỗi khi lưu dữ liệu vào cơ sở dữ liệu.");
                    return View(survey);
                }
            }
            return View("~/Views/Shared/Unauthorized.cshtml");
        }

        // GET: Surveys/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            int role = GetUserRole();
            if (role == 1) // Chỉ Admin (1) được chỉnh sửa
            {
                if (id == null)
                {
                    return NotFound();
                }

                var survey = await _context.Surveys.FindAsync(id);
                if (survey == null)
                {
                    return NotFound();
                }
                return View(survey);
            }
            return View("~/Views/Shared/Unauthorized.cshtml");
        }

        // POST: Surveys/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CageName,CageCode,Location,CageArea,ChickenCount,ChickenBreed,BreedOther,Purpose,PurposeOther,MainFeed,WaterSource,WaterSourceOther,Ventilation,Lighting,Hygiene,Notes")] Survey survey)
        {
            int role = GetUserRole();
            if (role == 1) // Chỉ Admin (1) được chỉnh sửa
            {
                if (id != survey.Id)
                {
                    return NotFound();
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors);
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"Validation Error: {error.ErrorMessage}");
                    }
                    return View(survey);
                }

                try
                {
                    _context.Update(survey);
                    await _context.SaveChangesAsync();
                    Console.WriteLine($"Cập nhật thành công Survey với Id: {survey.Id}");
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SurveyExists(survey.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi khi cập nhật: {ex.InnerException?.Message ?? ex.Message}");
                    ModelState.AddModelError("", "Lỗi khi cập nhật dữ liệu.");
                    return View(survey);
                }
            }
            return View("~/Views/Shared/Unauthorized.cshtml");
        }

        // GET: Surveys/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            int role = GetUserRole();
            if (role == 1) // Chỉ Admin (1) được xóa
            {
                if (id == null)
                {
                    return NotFound();
                }

                var survey = await _context.Surveys.FirstOrDefaultAsync(m => m.Id == id);
                if (survey == null)
                {
                    return NotFound();
                }

                return View(survey);
            }
            return View("~/Views/Shared/Unauthorized.cshtml");
        }

        // POST: Surveys/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            int role = GetUserRole();
            if (role == 1) // Chỉ Admin (1) được xóa
            {
                var survey = await _context.Surveys.FindAsync(id);
                if (survey != null)
                {
                    _context.Surveys.Remove(survey);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/Shared/Unauthorized.cshtml");
        }

        private bool SurveyExists(int id)
        {
            return _context.Surveys.Any(e => e.Id == id);
        }
    }
}