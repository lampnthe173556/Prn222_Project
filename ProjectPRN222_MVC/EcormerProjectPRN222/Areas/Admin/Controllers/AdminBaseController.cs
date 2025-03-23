using EcormerProjectPRN222.Filters;
using Microsoft.AspNetCore.Mvc;

namespace EcormerProjectPRN222.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthorizeRole(1)] // Admin role = 1
    public abstract class AdminBaseController : Controller
    {
        // Common functionality for admin controllers can be added here
    }
}