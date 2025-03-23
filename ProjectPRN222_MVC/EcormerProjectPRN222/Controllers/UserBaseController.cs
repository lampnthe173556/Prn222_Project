using EcormerProjectPRN222.Filters;
using Microsoft.AspNetCore.Mvc;

namespace EcormerProjectPRN222.Controllers
{
    [AuthorizeRole(0)] // User role = 0
    public abstract class UserBaseController : Controller
    {
        // Common functionality for user controllers can be added here
    }
}