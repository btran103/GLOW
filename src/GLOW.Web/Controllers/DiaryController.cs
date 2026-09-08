using GLOW.Data;
using GLOW.Data.Entities;
using GLOW.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GLOW.Web.Controllers;

[Authorize]
public class DiaryController : Controller
{
    private readonly GLOWDbContext _context;
    private readonly IGlowAiService _aiService;

    public DiaryController(GLOWDbContext context, IGlowAiService aiService)
    {
        _context = context;
        _aiService = aiService;
    }

    public async Task<IActionResult> Index(DateTime? date)
    {
        var targetDate = date ?? DateTime.Today;

        var allDiaries = await _context.DiaryRecords.ToListAsync();
        var diary = allDiaries.FirstOrDefault(d => d.Date.Date == targetDate.Date);

        var history = allDiaries
            .OrderByDescending(d => d.Date)
            .Take(15)
            .ToList();

        ViewBag.SelectedDate = targetDate;
        ViewBag.History = history;

        return View(diary);
    }

    [HttpPost]
    public async Task<IActionResult> GenerateAiDiary(DateTime date)
    {
        var diary = await _aiService.GenerateDailyDiaryAsync(date);
        return RedirectToAction(nameof(Index), new { date = date.ToString("yyyy-MM-dd") });
    }

    [HttpPost]
    public async Task<IActionResult> SaveCustomDiary(DateTime date, string title, string summary, string goodThings, string notGoodThings, string lessonsLearned, string memorableMoments, string improvements, string tomorrowSuggestions, string moodEmoji)
    {
        var targetDate = date.Date;
        var allDiaries = await _context.DiaryRecords.ToListAsync();
        var existing = allDiaries.FirstOrDefault(d => d.Date.Date == targetDate);

        if (existing == null)
        {
            existing = new DiaryRecord
            {
                Date = targetDate,
                CreatedAt = DateTime.Now
            };
            _context.DiaryRecords.Add(existing);
        }

        existing.Title = title ?? "Nhật ký của tôi";
        existing.Summary = summary ?? string.Empty;
        existing.GoodThings = goodThings ?? string.Empty;
        existing.NotGoodThings = notGoodThings ?? string.Empty;
        existing.LessonsLearned = lessonsLearned ?? string.Empty;
        existing.MemorableMoments = memorableMoments ?? string.Empty;
        existing.Improvements = improvements ?? string.Empty;
        existing.TomorrowSuggestions = tomorrowSuggestions ?? string.Empty;
        existing.MoodEmoji = moodEmoji ?? "✨";
        existing.IsGeneratedByAi = false;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index), new { date = date.ToString("yyyy-MM-dd") });
    }
}
