using GLOW.Data;
using GLOW.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GLOW.Web.Controllers;

[Authorize]
public class MapController : Controller
{
    private readonly GLOWDbContext _context;

    public MapController(GLOWDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var memories = await _context.MapMemories
            .OrderByDescending(m => m.ArrivedAt)
            .ToListAsync();

        var settings = await _context.PrivacySettings.FirstOrDefaultAsync() ?? new UserPrivacySetting();

        ViewBag.PrivacySettings = settings;

        return View(memories);
    }

    [HttpPost]
    public async Task<IActionResult> AddPlace(string placeName, string category, double latitude, double longitude, string activity, string feeling, int durationMinutes, string? photoUrl)
    {
        if (!string.IsNullOrWhiteSpace(placeName))
        {
            var memory = new MapMemory
            {
                PlaceName = placeName.Trim(),
                Category = category ?? "Cafe",
                Latitude = latitude != 0 ? latitude : 10.7769,
                Longitude = longitude != 0 ? longitude : 106.7009,
                Activity = activity ?? string.Empty,
                Feeling = feeling ?? "Thoải mái và vui vẻ",
                DurationMinutes = durationMinutes > 0 ? durationMinutes : 60,
                ArrivedAt = DateTime.Now.AddMinutes(-durationMinutes),
                LeftAt = DateTime.Now,
                PhotoUrl = photoUrl ?? "https://images.unsplash.com/photo-1554118811-1e0d58224f24?auto=format&fit=crop&w=400&q=80"
            };
            _context.MapMemories.Add(memory);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> ToggleTracking()
    {
        var settings = await _context.PrivacySettings.FirstOrDefaultAsync();
        if (settings != null)
        {
            settings.LocationEnabled = !settings.LocationEnabled;
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> ClearHistory()
    {
        var all = await _context.MapMemories.ToListAsync();
        _context.MapMemories.RemoveRange(all);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
