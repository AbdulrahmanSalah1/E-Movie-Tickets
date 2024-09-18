using eTickets.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
namespace eTickets
{
    public class RoleBasedRedirectMiddleware
    {
        private readonly RequestDelegate _next;

        public RoleBasedRedirectMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, UserManager<ApplicationUser> userManager)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                var user = await userManager.GetUserAsync(context.User);

                if (user != null) // Replace `IsAdmin` with your actual admin property
                {
                    var isAdmin = await userManager.IsInRoleAsync(user, "Admin");

                    if (isAdmin)
                    {
                        if (context.Request.Path == "/")
                        {
                            context.Response.Redirect("/Account/Dashboard");
                            return;
                        }
                    }
                }
            }

            await _next(context);
        }
    }
}
