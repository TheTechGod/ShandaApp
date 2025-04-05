using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShandaApp.Models;
using ShandaApp.Models.Enums;
using ShandaApp.Models.ViewModels;

namespace ShandaApp.Controllers
{

    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                ViewData["AvatarUrl"] = user.AvatarUrl ?? "/img/avatars/admin/admin-avatar1.png";
                ViewData["PreferredVoice"] = user.PreferredVoice ?? "Google US English Female";
            }

            var model = new AdminPreferencesViewModel
            {
                UseAdminTone = user?.UseAdminTone ?? true,
                AIMood = user?.AIMood ?? AIMood.Motivational
            };

            return View(model);
        }


        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateToneAndMood(AdminPreferencesViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                user.UseAdminTone = model.UseAdminTone;
                user.AIMood = model.AIMood;
                await _userManager.UpdateAsync(user);
                TempData["Success"] = "✅ Preferences saved!";
            }

            return RedirectToAction("Index");
        }
    }
}
