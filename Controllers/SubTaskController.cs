using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShandaApp.Data;
using ShandaApp.Models;
using ShandaApp.Models.Enums;
using ShandaApp.Services;
using System.Threading.Tasks;

namespace ShandaApp.Controllers
{
    public class SubTaskController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public SubTaskController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            AIResponseService aiResponse
        ) : base(userManager, aiResponse)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSubTask(SubTask subTask)
        {
            if (ModelState.IsValid)
            {
                _context.SubTasks.Add(subTask);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", "ToDo", new { id = subTask.ToDoItemId });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, SubTaskStatus status)
        {
            var subTask = await _context.SubTasks
                .Include(s => s.ToDoItem)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (subTask == null)
                return NotFound();

            subTask.Status = status;
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "ToDo", new { id = subTask.ToDoItemId });
        }

        [HttpGet]
        public async Task<IActionResult> EditSubTask(int id)
        {
            var subTask = await _context.SubTasks.FindAsync(id);
            if (subTask == null)
                return NotFound();

            return View(subTask);
        }

        [HttpPost]
        public async Task<IActionResult> EditSubTask(SubTask updated)
        {
            if (!ModelState.IsValid)
                return View(updated);

            _context.SubTasks.Update(updated);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "ToDo", new { id = updated.ToDoItemId });
        }

        [HttpGet]
        public async Task<IActionResult> DeleteSubTask(int id)
        {
            var subTask = await _context.SubTasks.FindAsync(id);
            if (subTask == null)
                return NotFound();

            var parentId = subTask.ToDoItemId;

            _context.SubTasks.Remove(subTask);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "ToDo", new { id = parentId });
        }
    }
}
