using GLOW.Data;
using GLOW.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GLOW.Web.Controllers;

[Authorize]
public class WorkController : Controller
{
    private readonly GLOWDbContext _context;

    public WorkController(GLOWDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var tasks = await _context.WorkTasks
            .OrderBy(t => t.Status == "Done")
            .ThenByDescending(t => t.Priority)
            .ToListAsync();

        ViewBag.TodoTasks = tasks.Where(t => t.Status == "Todo").ToList();
        ViewBag.InProgressTasks = tasks.Where(t => t.Status == "In Progress").ToList();
        ViewBag.DoneTasks = tasks.Where(t => t.Status == "Done").ToList();
        ViewBag.TotalHoursLogged = Math.Round(tasks.Sum(t => t.WorkMinutesLogged) / 60.0, 1);

        return View(tasks);
    }

    [HttpPost]
    public async Task<IActionResult> AddTask(string title, string projectName, string description, DateTime? dueDate, string priority, string tags)
    {
        if (!string.IsNullOrWhiteSpace(title))
        {
            var task = new WorkTask
            {
                Title = title.Trim(),
                ProjectName = string.IsNullOrWhiteSpace(projectName) ? "Công việc chung" : projectName.Trim(),
                Description = description ?? string.Empty,
                DueDate = dueDate,
                Priority = priority ?? "Medium",
                Status = "Todo",
                Tags = tags ?? "#work",
                CreatedAt = DateTime.Now
            };
            _context.WorkTasks.Add(task);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> UpdateStatus(int id, string status)
    {
        var task = await _context.WorkTasks.FindAsync(id);
        if (task != null)
        {
            task.Status = status;
            if (status == "Done")
                task.CompletedAt = DateTime.Now;
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
