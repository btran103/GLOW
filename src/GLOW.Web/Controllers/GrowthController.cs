using GLOW.Data;
using GLOW.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GLOW.Web.Controllers;

[Authorize]
public class GrowthController : Controller
{
    private readonly GLOWDbContext _context;
    private readonly IInsightEngine _insightEngine;

    public GrowthController(GLOWDbContext context, IInsightEngine insightEngine)
    {
        _context = context;
        _insightEngine = insightEngine;
    }

    public async Task<IActionResult> Index()
    {
        var report = await _insightEngine.GenerateInsightReportAsync();

        var momentsCount = await _context.Moments.CountAsync();
        var allSessions = await _context.StudySessions.ToListAsync();
        var studyHoursTotal = Math.Round(allSessions.Sum(s => s.DurationMinutes) / 60.0, 1);
        var diaryCount = await _context.DiaryRecords.CountAsync();

        ViewBag.MomentsCount = momentsCount;
        ViewBag.StudyHoursTotal = studyHoursTotal;
        ViewBag.DiaryCount = diaryCount;

        return View(report);
    }
}
