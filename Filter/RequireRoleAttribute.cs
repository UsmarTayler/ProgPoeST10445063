using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CMCS.Mvc.Filters
{
    public class RequireRoleAttribute : ActionFilterAttribute
    {
        private readonly string _role;

        public RequireRoleAttribute(string role)
        {
            _role = role;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var sessionRole = context.HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(sessionRole) || sessionRole != _role)
            {
                context.Result = new RedirectToActionResult("Login", "Account", new { role = _role });
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
