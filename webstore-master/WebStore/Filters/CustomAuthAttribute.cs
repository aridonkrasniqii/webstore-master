using Microsoft.AspNetCore.Mvc.Filters;

namespace WebStore.Filters
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CustomAuthAttribute : ActionFilterAttribute
    {
        private List<string> _roles;
        public CustomAuthAttribute(string roles)
        {
            _roles = roles.Split(',').ToList();
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context != null)
            {
                var currentRole = context.HttpContext.Request.Cookies["role"];
                if (currentRole == null || !_roles.Contains(currentRole))
                {
                    context.Cancel = true;
                }
            }

            base.OnResultExecuting(context);
        }
    }
}
