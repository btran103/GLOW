using GLOW.Data;
using GLOW.Models;
using GLOW.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GLOW.Web.Controllers;

public class HomeController : Controller
{
    private readonly GLOWDbContext _context;
    private readonly IGlowAiService _aiService;

    public HomeController(GLOWDbContext context, IGlowAiService aiService)
    {
        _context = context;
        _aiService = aiService;
    }

    public async Task<IActionResult> Index()
    {
        var today = DateTime.Today;

        var profile = await _context.UserProfiles.FirstOrDefaultAsync() ?? new Data.Entities.UserProfile();
        
        var allMoods = await _context.MoodEntries.OrderByDescending(m => m.Date).ToListAsync();
        var todayMood = allMoods.FirstOrDefault(m => m.Date.Date == today);

        var allHealth = await _context.HealthLogs.ToListAsync();
        var todayHealth = allHealth.FirstOrDefault(h => h.Date.Date == today);

        var pendingStudy = await _context.StudyTasks
            .Where(t => !t.IsCompleted)
            .OrderBy(t => t.DueDate)
            .Take(4)
            .ToListAsync();

        var pendingWork = await _context.WorkTasks
            .Where(w => w.Status != "Done")
            .OrderByDescending(w => w.Priority)
            .Take(4)
            .ToListAsync();

        var allExpenses = await _context.Expenses.ToListAsync();
        var todaySpent = allExpenses
            .Where(e => e.Date.Date == today && e.Type == "Chi tiêu")
            .Sum(e => e.Amount);

        var allSessions = await _context.StudySessions.ToListAsync();
        var todayStudyMinutes = allSessions
            .Where(s => s.SessionDate.Date == today)
            .Sum(s => s.DurationMinutes);

        var allMeals = await _context.NutritionEntries.ToListAsync();
        var todayMeals = allMeals
            .Where(n => n.Time.Date == today)
            .OrderBy(n => n.Time)
            .ToList();

        var recentMoments = await _context.Moments
            .OrderByDescending(m => m.Timestamp)
            .Take(4)
            .ToListAsync();

        var recentPlaces = await _context.MapMemories
            .OrderByDescending(p => p.ArrivedAt)
            .Take(3)
            .ToListAsync();

        var allDiaries = await _context.DiaryRecords.ToListAsync();
        var todayDiary = allDiaries.FirstOrDefault(d => d.Date.Date == today);

        var tomorrowPlan = await _context.TomorrowPlans
            .OrderByDescending(t => t.PlanForDate)
            .FirstOrDefaultAsync();

        var vm = new HomeDashboardViewModel
        {
            Profile = profile,
            TodayMood = todayMood,
            TodayHealth = todayHealth,
            PendingStudyTasks = pendingStudy,
            PendingWorkTasks = pendingWork,
            TodaySpent = todaySpent,
            TodayStudyMinutes = todayStudyMinutes,
            TodayWaterMl = todayHealth?.WaterMl ?? 1800,
            WaterGoalMl = profile.DailyWaterGoalMl,
            TodayMeals = todayMeals,
            RecentMoments = recentMoments,
            RecentPlaces = recentPlaces,
            TodayDiary = todayDiary,
            TomorrowPlan = tomorrowPlan,
            DailyAffirmation = "Mỗi ngày trôi qua là một cơ hội mới để bạn tỏa sáng rực rỡ và yêu thương chính mình! 🌟✨"
        };

        return View(vm);
    }
}
