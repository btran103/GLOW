using GLOW.Data;
using GLOW.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GLOW.Web.Controllers;

[Authorize]
public class MoodController : Controller
{
    private readonly GLOWDbContext _context;

    public MoodController(GLOWDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var entries = await _context.MoodEntries
            .OrderByDescending(m => m.Date)
            .Take(30)
            .ToListAsync();

        var today = DateTime.Today;
        var todayEntry = entries.FirstOrDefault(m => m.Date.Date == today);

        ViewBag.TodayEntry = todayEntry;
        ViewBag.AverageMood = entries.Any() ? Math.Round(entries.Average(m => m.MoodScore), 1) : 8.0;

        return View(entries);
    }

    [HttpPost]
    public async Task<IActionResult> LogMood(int moodScore, string moodLabel, string moodEmoji, int energyScore, string joyTriggers, string stressTriggers, string note)
    {
        string aiAnalysis = moodScore >= 8
            ? "Tâm trạng bạn đang ở mức rất cao và rạng rỡ! Hãy giữ vững cảm xúc tích cực này nhé! ✨"
            : moodScore >= 6
                ? "Tâm trạng cân bằng và ổn định. Đừng quên dành thời gian thư giãn cá nhân."
                : "Có vẻ bạn đang hơi căng thẳng hoặc mệt mỏi. Hãy cho phép bản thân nghỉ ngơi, bạn xứng đáng được yêu thương! ❤️";

        var entry = new MoodEntry
        {
            Date = DateTime.Now,
            MoodScore = moodScore,
            MoodLabel = moodLabel ?? "Bình thường",
            MoodEmoji = moodEmoji ?? "😊",
            EnergyScore = energyScore,
            JoyTriggers = joyTriggers ?? string.Empty,
            StressTriggers = stressTriggers ?? string.Empty,
            Note = note ?? string.Empty,
            AiAnalysis = aiAnalysis
        };

        _context.MoodEntries.Add(entry);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
