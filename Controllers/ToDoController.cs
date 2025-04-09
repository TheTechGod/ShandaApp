using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShandaApp.Data;
using ShandaApp.Models;
using ShandaApp.Services;
using TaskStatusEnum = ShandaApp.Models.Enums.ToDoTaskStatus;
using System;
using System.Linq;
using System.Threading.Tasks;
using ShandaApp.Models.Enums;

namespace ShandaApp.Controllers
{
    [Authorize]
    public class ToDoController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public ToDoController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            AIResponseService aiResponseService
        ) : base(userManager, aiResponseService)
        {
            _context = context;
        }

        // 📋 Show all tasks with sorting
        public async Task<IActionResult> Index(string sortOrder = "asc")
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            ViewBag.SortOrder = sortOrder == "desc" ? "desc" : "asc";

            var tasksQuery = _context.ToDoItems
                .Include(t => t.SubTasks)
                .Where(t => t.UserId == user.Id);

            tasksQuery = sortOrder == "desc"
                ? tasksQuery.OrderByDescending(t => t.DueDate)
                : tasksQuery.OrderBy(t => t.DueDate);

            return View(await tasksQuery.ToListAsync());
        }

        // 📝 Render create form
        [HttpGet]
        public IActionResult Create() => View();

        // 📝 Handle create form POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ToDoItem todo)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            // Set system fields
            todo.UserId = user.Id;
            todo.CreatedAt = DateTime.UtcNow;
            todo.Status = ToDoTaskStatus.Pending; // Set default status

            // Handle subtasks
            if (todo.SubTasks != null)
            {
                foreach (var subtask in todo.SubTasks.Where(s => !string.IsNullOrWhiteSpace(s.Title)))
                {
                    subtask.ToDoItem = null;
                    subtask.Status = SubTaskStatus.Pending;
                }
                todo.SubTasks = todo.SubTasks
                    .Where(s => !string.IsNullOrWhiteSpace(s.Title))
                    .ToList();
            }

            ModelState.Remove("SubTasks[0].ToDoItem");

            if (!ModelState.IsValid)
            {
                LogModelErrors();
                return View(todo);
            }

            _context.ToDoItems.Add(todo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // 🔍 Show task details
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var task = await _context.ToDoItems
                .Include(t => t.SubTasks)
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == user.Id);

            return task == null ? NotFound() : View(task);
        }

        // ✏️ Render edit form
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var task = await _context.ToDoItems
                .Include(t => t.SubTasks)
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == user.Id);

            return task == null ? NotFound() : View(task);
        }

        // ✏️ Handle edit form POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ToDoItem updated)
        {
            var user = await _userManager.GetUserAsync(User);
            var task = await _context.ToDoItems
                .Include(t => t.SubTasks)
                .FirstOrDefaultAsync(t => t.Id == updated.Id && t.UserId == user.Id);

            if (task == null) return NotFound();

            if (!ModelState.IsValid)
            {
                LogModelErrors();
                return View(updated);
            }

            task.Title = updated.Title;
            task.Subject = updated.Subject;
            task.DueDate = updated.DueDate;
            task.Status = updated.Status;
            task.IsComplete = (updated.Status == ToDoTaskStatus.Completed); // Sync completion status

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // 🔄 Update task status (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, TaskStatusEnum status)
        {
            var user = await _userManager.GetUserAsync(User);
            var task = await _context.ToDoItems
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == user.Id);

            if (task == null) return NotFound();

            task.Status = status;
            task.IsComplete = (status == ToDoTaskStatus.Completed); // Sync completion status

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // ❌ Delete task
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var task = await _context.ToDoItems
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == user.Id);

            if (task == null) return NotFound();

            _context.ToDoItems.Remove(task);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // 🔍 Log validation errors
        private void LogModelErrors()
        {
            foreach (var entry in ModelState.Where(e => e.Value.Errors.Count > 0))
            {
                foreach (var error in entry.Value.Errors)
                {
                    Console.WriteLine($"Validation Error - {entry.Key}: {error.ErrorMessage}");
                }
            }
        }
    }
}