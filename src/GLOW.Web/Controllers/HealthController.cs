using GLOW.Data;
using GLOW.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GLOW.Web.Controllers;

[Authorize]
public class HealthController : Controller
{
    private readonly GLOWDbContext _context;

    public HealthController(GLOWDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var logs = await _context.HealthLogs
            .OrderByDescending(h => h.Date)
            .Take(14)
            .ToListAsync();

        var today = DateTime.Today;
        var todayLog = logs.FirstOrDefault(h => h.Date.Date == today) ?? new HealthLog { Date = today };
        var reminders = await _context.HealthReminders.ToListAsync();

        ViewBag.TodayLog = todayLog;
        ViewBag.Reminders = reminders;

        return View(logs);
    }

    [HttpPost]
    public async Task<IActionResult> AddWater(int amountMl)
    {
        var today = DateTime.Today;
        var allLogs = await _context.HealthLogs.ToListAsync();
        var log = allLogs.FirstOrDefault(h => h.Date.Date == today);
        if (log == null)
        {
            log = new HealthLog { Date = today, WaterMl = amountMl > 0 ? amountMl : 250 };
            _context.HealthLogs.Add(log);
        }
        else
        {
            log.WaterMl += amountMl > 0 ? amountMl : 250;
        }

        _context.WaterLogs.Add(new WaterLog { AmountMl = amountMl > 0 ? amountMl : 250, Timestamp = DateTime.Now });
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> UpdateLog(double sleepHours, string sleepQuality, int stepsCount, int activeMinutes, int energyLevel, string healthNotes)
    {
        var today = DateTime.Today;
        var allLogs = await _context.HealthLogs.ToListAsync();
        var log = allLogs.FirstOrDefault(h => h.Date.Date == today);
        if (log == null)
        {
            log = new HealthLog
            {
                Date = today,
                SleepHours = sleepHours,
                SleepQuality = sleepQuality ?? "Tốt",
                StepsCount = stepsCount,
                ActiveMinutes = activeMinutes,
                EnergyLevel = energyLevel,
                HealthNotes = healthNotes ?? string.Empty
            };
            _context.HealthLogs.Add(log);
        }
        else
        {
            log.SleepHours = sleepHours;
            log.SleepQuality = sleepQuality ?? log.SleepQuality;
            log.StepsCount = stepsCount;
            log.ActiveMinutes = activeMinutes;
            log.EnergyLevel = energyLevel;
            log.HealthNotes = healthNotes ?? string.Empty;
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
