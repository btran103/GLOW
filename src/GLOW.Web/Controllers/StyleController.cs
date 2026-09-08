using GLOW.Data;
using GLOW.Data.Entities;
using GLOW.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GLOW.Web.Controllers;

[Authorize]
public class StyleController : Controller
{
    private readonly GLOWDbContext _context;
    private readonly IGlowAiService _aiService;

    public StyleController(GLOWDbContext context, IGlowAiService aiService)
    {
        _context = context;
        _aiService = aiService;
    }

    public async Task<IActionResult> Index()
    {
        var wardrobe = await _context.WardrobeItems
            .OrderByDescending(w => w.UsageCount)
            .ToListAsync();

        var recentAdvices = await _context.StyleAdvices
            .OrderByDescending(s => s.CreatedAt)
            .Take(5)
            .ToListAsync();

        ViewBag.RecentAdvices = recentAdvices;

        return View(wardrobe);
    }

    [HttpPost]
    public async Task<IActionResult> AddWardrobeItem(string name, string category, string color, string style, string pattern, string? imageUrl, string notes)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            var item = new WardrobeItem
            {
                Name = name.Trim(),
                Category = category ?? "Áo",
                Color = color ?? "Pastel",
                Style = style ?? "Gen Z Casual",
                Pattern = pattern ?? "Trơn",
                Notes = notes ?? string.Empty,
                ImageUrl = imageUrl ?? "https://images.unsplash.com/photo-1521572267360-ee0c2909d518?auto=format&fit=crop&w=400&q=80",
                AddedDate = DateTime.Now
            };
            _context.WardrobeItems.Add(item);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> RequestStyleAdvice(string itemName, string category, string color, string style, string? imageUrl)
    {
        if (!string.IsNullOrWhiteSpace(itemName))
        {
            await _aiService.AnalyzeOutfitAndStyleAsync(itemName, category, color, style, imageUrl);
        }
        return RedirectToAction(nameof(Index));
    }
}
