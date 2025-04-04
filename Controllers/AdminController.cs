using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShandaApp.Models.Enums;
using ShandaApp.Models.ViewModels;

namespace ShandaApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            // Preload default values (optional)
            var model = new AdminPreferencesViewModel
            {
                UseAdminTone = true,
                AIMood = AIMood.Motivational
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateToneAndMood(AdminPreferencesViewModel model)
        {
            // TODO: Save to DB or session
            TempData["Success"] = "✅ Preferences saved successfully!";
            return RedirectToAction("Index");
        }
    }
}
