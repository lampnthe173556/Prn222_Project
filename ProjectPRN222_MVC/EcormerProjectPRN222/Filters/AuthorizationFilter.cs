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
                // For admin area, redirect with returnUrl
                var isAdminArea = context.RouteData.Values["area"]?.ToString()?.Equals("Admin", StringComparison.OrdinalIgnoreCase) ?? false;
                if (isAdminArea)
                {
                    var returnUrl = context.HttpContext.Request.Path;
                    var routeValues = new RouteValueDictionary();
                    routeValues["area"] = "";
                    routeValues["returnUrl"] = returnUrl;
                    context.Result = new RedirectToActionResult("Index", "Login", routeValues);
                }
                else
                {
                    var routeValues = new RouteValueDictionary();
                    routeValues["area"] = "";
                    context.Result = new RedirectToActionResult("Index", "Login", routeValues);
                }
                return;
            }

            var user = JsonSerializer.Deserialize<Account>(userJson);
            
            if (user == null || user.RoleId != _requiredRole)
            {
                // Clear session and cookies on unauthorized access
                context.HttpContext.Session.Clear();
                foreach (var cookie in context.HttpContext.Request.Cookies.Keys)
                {
                    context.HttpContext.Response.Cookies.Delete(cookie);
                }

                // Redirect to access denied
                var routeValues = new RouteValueDictionary();
                routeValues["area"] = "";
                context.Result = new RedirectToActionResult("AccessDenied", "Login", routeValues);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}