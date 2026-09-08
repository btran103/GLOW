using GLOW.Data;
using GLOW.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GLOW.Web.Controllers;

[Authorize]
public class StudyController : Controller
{
    private readonly GLOWDbContext _context;

    public StudyController(GLOWDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var courses = await _context.StudyCourses
            .Include(c => c.Tasks)
            .ToListAsync();

        var tasks = await _context.StudyTasks
            .Include(t => t.Course)
            .OrderBy(t => t.IsCompleted)
            .ThenBy(t => t.DueDate)
            .ToListAsync();

        var sessions = await _context.StudySessions
            .OrderByDescending(s => s.SessionDate)
            .Take(10)
            .ToListAsync();

        var weekAgo = DateTime.Today.AddDays(-7);
        var allSessions = await _context.StudySessions.ToListAsync();
        var totalStudyMinutesWeek = allSessions
            .Where(s => s.SessionDate >= weekAgo)
            .Sum(s => s.DurationMinutes);

        ViewBag.Courses = courses;
        ViewBag.Tasks = tasks;
        ViewBag.Sessions = sessions;
        ViewBag.TotalStudyMinutesWeek = totalStudyMinutesWeek;

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AddTask(string title, int? courseId, DateTime? dueDate, int estimatedMinutes, string priority)
    {
        if (!string.IsNullOrWhiteSpace(title))
        {
            var task = new StudyTask
            {
                Title = title.Trim(),
                CourseId = courseId,
                DueDate = dueDate,
                EstimatedMinutes = estimatedMinutes > 0 ? estimatedMinutes : 60,
                Priority = priority ?? "Medium",
                IsCompleted = false,
                CreatedAt = DateTime.Now
            };
            _context.StudyTasks.Add(task);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> ToggleTask(int id)
    {
        var task = await _context.StudyTasks.FindAsync(id);
        if (task != null)
        {
            task.IsCompleted = !task.IsCompleted;
            if (task.IsCompleted && task.CompletedMinutes == 0)
            {
                task.CompletedMinutes = task.EstimatedMinutes;
            }
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> LogSession(string subject, int durationMinutes, string focusQuality, string notes)
    {
        if (!string.IsNullOrWhiteSpace(subject) && durationMinutes > 0)
        {
            var session = new StudySession
            {
                Subject = subject.Trim(),
                DurationMinutes = durationMinutes,
                FocusQuality = focusQuality ?? "Tốt 🌟",
                Notes = notes ?? string.Empty,
                SessionDate = DateTime.Now
            };
            _context.StudySessions.Add(session);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> AddCourse(string code, string name, string instructor, string room, string scheduleDayOfWeek, string timeSlot, string colorHex)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            var course = new StudyCourse
            {
                Code = code ?? "MÔN MỚI",
                Name = name.Trim(),
                Instructor = instructor ?? string.Empty,
                Room = room ?? string.Empty,
                ScheduleDayOfWeek = scheduleDayOfWeek ?? "Thứ 2, 4",
                TimeSlot = timeSlot ?? "08:00 - 10:30",
                ColorHex = colorHex ?? "#A7F3D0"
            };
            _context.StudyCourses.Add(course);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
