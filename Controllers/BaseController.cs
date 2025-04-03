using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ShandaApp.Models;
using ShandaApp.Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ShandaApp.Controllers
{
    public class BaseController : Controller
    {
        protected readonly UserManager<ApplicationUser> _userManager;
        protected readonly AIResponseService _aiResponseService;

        public BaseController(UserManager<ApplicationUser> userManager, AIResponseService aiResponseService)
        {
            _userManager = userManager;
            _aiResponseService = aiResponseService;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _userManager.FindByIdAsync(userId);

                if (user != null)
                {
                    ViewBag.AvatarUrl = string.IsNullOrWhiteSpace(user.AvatarUrl)
                        ? "/img/avatars/users/default-avatar.png"
                        : user.AvatarUrl.ToLowerInvariant();

                    ViewBag.IsPersonalMode = user.IsPersonalMode;
                    ViewBag.PreferredVoice = user.PreferredVoice ?? "Google US English Female";

                    bool isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
                    bool isKid = await _userManager.IsInRoleAsync(user, "Kid");
                    string role = isAdmin ? "Admin" : isKid ? "Kid" : "User";
                    ViewBag.UserRole = role;

                    string fallback = string.IsNullOrWhiteSpace(user.UserName) ? "Guest" : user.UserName.Split('@')[0];
                    string firstName = string.IsNullOrWhiteSpace(user.FirstName) ? fallback : user.FirstName;

                    string area = context.RouteData.Values["area"]?.ToString() ?? "";
                    string controller = context.RouteData.Values["controller"]?.ToString() ?? "";
                    string action = context.RouteData.Values["action"]?.ToString() ?? "";

                    bool personalMode = user.IsPersonalMode && !user.UseAdminTone;

                    ViewBag.AIResponse = _aiResponseService.GenerateContextAwareGreeting(
                        role, personalMode, firstName, user.AIMood,
                        area, controller, action
                    );
                }
            }
            else
            {
                ViewBag.AvatarUrl = "/img/avatars/users/default-avatar.png";
                ViewBag.UserRole = "Guest";
                ViewBag.IsPersonalMode = false;
                ViewBag.PreferredVoice = "Google US English Female";
                ViewBag.AIResponse = "Hello, Guest. Please login to unlock SHANDA.";
            }

            await next();
        }
    }
}
