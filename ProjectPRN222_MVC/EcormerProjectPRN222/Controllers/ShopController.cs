using EcormerProjectPRN222.Models;
using Microsoft.AspNetCore.Mvc;

namespace EcormerProjectPRN222.Controllers
{
    public class ShopController : Controller
    {
        private readonly MyProjectClothingContext _context;

        public ShopController(MyProjectClothingContext context)
        {
            _context = context;
        }
        public IActionResult Index(int? idCate)
        {
            var categories = _context.Categories.ToList();
            if (idCate == null)
            {

                var indexCate = 1;
                var products = _context.Products.Where(p => p.CategoryId == indexCate).ToList();
                ViewBag.Products = products;
                ViewBag.IdCate = indexCate;
            }
            else
            {
                var indexCate = idCate;
                var products = _context.Products.Where(p => p.CategoryId == indexCate).ToList();
                ViewBag.Products = products;
                ViewBag.IdCate = indexCate;
            }

            ViewBag.Categories = categories;
            return View();
        }
    }
}
