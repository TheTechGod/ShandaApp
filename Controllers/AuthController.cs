using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShandaApp.Data;
using ShandaApp.Models;
using ShandaApp.Models.Enums;
using ShandaApp.Models.ViewModels;
using ShandaApp.Services;
using System.Threading.Tasks;
using ShandaApp.Extensions;
using Azure;


namespace ShandaApp.Controllers
{
    public class AuthController : BaseController
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context,
            AIResponseService aiService
        ) : base(userManager, aiService)
        {
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }

        // ✅ GET: Register Page
        [HttpGet]
        public IActionResult Register() => View();

        // ✅ POST: Handle Registration
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (model.UserName.Contains(" "))
            {
                ModelState.AddModelError("UserName", "Username cannot contain spaces.");
                return View(model);
            }

            await EnsureRolesExist();

            string avatarPath = (model.SelectedAvatar ?? "/img/avatars/users/default-avatar.png").ToLowerInvariant();
            string[] allowedAvatars =
            {
                "/img/avatars/users/avatar1.png",
                "/img/avatars/users/avatar2.png",
                "/img/avatars/users/default-avatar.png"
            };

            if (!allowedAvatars.Contains(avatarPath))
                avatarPath = "/img/avatars/users/default-avatar.png";

            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                BirthDate = model.BirthDate,
                AvatarUrl = avatarPath,
                PreferredVoice = GetVoiceForAvatar(avatarPath),
                IsPersonalMode = false
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                var role = (await _context.Users.CountAsync() == 1)
                    ? RoleConstants.Admin
                    : RoleConstants.User;

                await _userManager.AddToRoleAsync(user, role);
                await _signInManager.SignInAsync(user, isPersistent: false);

                return RedirectToAction("Index", role == RoleConstants.Admin ? "Admin" : "Dashboard");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

        // ✅ GET: Login Page
        [HttpGet]
        public IActionResult Login() => View();

        // ✅ POST: Handle Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                user.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                TempData["AvatarUrl"] = user.AvatarUrl ?? "/img/avatars/users/default-avatar.png";
                TempData["IsPersonalMode"] = user.IsPersonalMode.ToString();
                TempData["PreferredVoice"] = user.PreferredVoice;


                if (await _userManager.IsInRoleAsync(user, RoleConstants.Admin))
                    return RedirectToAction("Index", "Admin");

                if (await _userManager.IsInRoleAsync(user, RoleConstants.Kid))
                    return RedirectToAction("Index", "KidDashboard");

                return RedirectToAction("Dashboard", "Home");

            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }

        // ✅ POST: Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            HttpContext.Session.Clear();
            Response.Cookies.Delete(".AspNetCore.Identity.Application");

            return RedirectToAction("Login", "Auth");
        }

        // ✅ Role Seeder
        private async Task EnsureRolesExist()
        {
            string[] roles = { RoleConstants.Admin, RoleConstants.User, RoleConstants.Kid };

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                    await _roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // 🧠 Avatar to Voice Mapper
        public static string GetVoiceForAvatar(string avatarPath)
        {
            avatarPath = avatarPath?.ToLowerInvariant() ?? "";

            return avatarPath switch
            {
                var path when path.Contains("avatar1") => "Google US English Female",
                var path when path.Contains("avatar2") => "Microsoft Zira Desktop - English (United States)",
                var path when path.Contains("avatar3") => "Google UK English Male",
                var path when path.Contains("kid") => "Google UK English Female",
                _ => "Google US English Female"
            };
        }
    }
}
