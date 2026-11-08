using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RaceDayAPI.Helpers;

namespace RaceDayAPI.Helpers
{
    public class RequireRoleAttribute : ActionFilterAttribute
    {
        private readonly string _role;
        public RequireRoleAttribute(string role) { _role = role; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var userId = context.HttpContext.GetUserId();
            var role = context.HttpContext.GetRole();

            if (userId == null || role != _role)
            {
                context.Result = new ForbidResult(); // Returns 403 Forbidden
            }
            base.OnActionExecuting(context);
        }
    }

    public class RequireAuthAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var userId = context.HttpContext.GetUserId();
            if (userId == null)
            {
                context.Result = new UnauthorizedResult(); // Returns 401 Unauthorized
            }
            base.OnActionExecuting(context);
        }
    }
}