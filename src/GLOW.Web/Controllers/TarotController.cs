using GLOW.Data;
using GLOW.Data.Entities;
using GLOW.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GLOW.Web.Controllers;

[Authorize]
public class TarotController : Controller
{
    private readonly GLOWDbContext _context;
    private readonly IGlowAiService _aiService;

    public TarotController(GLOWDbContext context, IGlowAiService aiService)
    {
        _context = context;
        _aiService = aiService;
    }

    public async Task<IActionResult> Index()
    {
        var cards = await _context.TarotCards.ToListAsync();
        var recentReadings = await _context.TarotReadings
            .OrderByDescending(r => r.ReadingDate)
            .Take(10)
            .ToListAsync();

        ViewBag.RecentReadings = recentReadings;

        return View(cards);
    }

    [HttpPost]
    public async Task<IActionResult> Draw(string questionTopic, string spreadType)
    {
        var reading = await _aiService.DrawTarotAsync(questionTopic ?? "Tổng quan", spreadType ?? "3 Cards");
        return RedirectToAction(nameof(Index));
    }
}
