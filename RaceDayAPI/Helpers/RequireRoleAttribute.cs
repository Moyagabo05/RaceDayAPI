using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RaceDayAPI.Helpers
{
    public class RequireRoleAttribute : ActionFilterAttribute
    {
        private readonly string _role;
        public RequireRoleAttribute(string role) { _role = role; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var sessionRole = context.HttpContext.Session.GetString("UserRole");
            var sessionUserId = context.HttpContext.Session.GetInt("UserId");

            if (sessionUserId == null || sessionRole != _role)
            {
                context.Result = new ForbidResult();
            }
            base.OnActionExecuting(context);
        }
    }

    public class RequireAuthAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var sessionUserId = context.HttpContext.Session.GetInt("UserId");
            if (sessionUserId == null)
            {
                context.Result = new UnauthorizedResult();
            }
            base.OnActionExecuting(context);
        }
    }
}
