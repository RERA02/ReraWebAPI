using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DemoMicroService1.Code.Attribute
{
    public class CustomeAuthorizeAttribute : System.Attribute, IAuthorizationFilter
    {
        public CustomeAuthorizeAttribute()
        {

        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.HttpContext.User == null || context.HttpContext.User.Identity == null || !context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new JsonResult(new 
                {
                    State = 0,
                    Message = "MSG_UNAUTHORIZED_ACCESS",                    
                });

            }
        }
    }
}
