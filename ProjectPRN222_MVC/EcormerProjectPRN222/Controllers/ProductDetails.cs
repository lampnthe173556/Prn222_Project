using EcormerProjectPRN222.Models;
using Microsoft.AspNetCore.Mvc;

namespace EcormerProjectPRN222.Controllers
{
    public class ProductDetails : Controller
    {
        public IActionResult Index(int productId)
        {
            var context = new MyProjectClothingContext();
            Product product = context.Products.FirstOrDefault(x => x.ProductId == productId);
            ViewBag.product = product;
            return View();
        }
    }
}
