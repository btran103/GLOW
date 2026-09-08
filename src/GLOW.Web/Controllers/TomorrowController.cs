using GLOW.Data;
using GLOW.Data.Entities;
using GLOW.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GLOW.Web.Controllers;

[Authorize]
public class TomorrowController : Controller
{
    private readonly GLOWDbContext _context;
    private readonly IGlowAiService _aiService;

    public TomorrowController(GLOWDbContext context, IGlowAiService aiService)
    {
        _context = context;
        _aiService = aiService;
    }

    public async Task<IActionResult> Index()
    {
        var plan = await _context.TomorrowPlans
            .OrderByDescending(p => p.PlanForDate)
            .FirstOrDefaultAsync();

        if (plan == null)
        {
            plan = await _aiService.GenerateTomorrowPlanAsync();
        }

        return View(plan);
    }

    [HttpPost]
    public async Task<IActionResult> Regenerate()
    {
        await _aiService.GenerateTomorrowPlanAsync();
        return RedirectToAction(nameof(Index));
    }
}
