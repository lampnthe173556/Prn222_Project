using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcormerProjectPRN222.Models;

namespace EcormerProjectPRN222.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoriesController : AdminBaseController
    {
        private readonly MyProjectClothingContext _context;

        public CategoriesController(MyProjectClothingContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Categories";
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.Categories.ToListAsync();
            return Json(new { data = categories });
        }

        [HttpGet]
        public async Task<IActionResult> GetCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return Json(new { success = false, message = "Category not found" });
            }
            return Json(new { success = true, data = category });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryName,Status,Description")] Category category)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(category);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Category created successfully" });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Error creating category" });
                }
            }
            return Json(new { success = false, message = "Invalid data" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("CategoryId,CategoryName,Status,Description")] Category category)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Category updated successfully" });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.CategoryId))
                    {
                        return Json(new { success = false, message = "Category not found" });
                    }
                    throw;
                }
            }
            return Json(new { success = false, message = "Invalid data" });
        }

      

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.CategoryId == id);
        }
    }
}
