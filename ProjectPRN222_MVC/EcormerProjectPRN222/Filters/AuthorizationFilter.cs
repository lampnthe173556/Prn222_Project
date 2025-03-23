using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;
using EcormerProjectPRN222.Models;

namespace EcormerProjectPRN222.Filters
{
    public class AuthorizeRoleAttribute : ActionFilterAttribute
    {
        private readonly int _requiredRole;

        public AuthorizeRoleAttribute(int role)
        {
            _requiredRole = role;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var userJson = context.HttpContext.Session.GetString("user");
            
            if (string.IsNullOrEmpty(userJson))
            {
                context.Result = new RedirectToActionResult("Index", "Login", null);
                return;
            }

            var user = JsonSerializer.Deserialize<Account>(userJson);
            
            if (user == null || user.RoleId != _requiredRole)
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Login", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}