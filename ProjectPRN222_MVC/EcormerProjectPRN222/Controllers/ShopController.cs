using Microsoft.AspNetCore.Mvc;

namespace EcormerProjectPRN222.Controllers
{
    public class ShopController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
