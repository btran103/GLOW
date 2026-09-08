using GLOW.Data;
using GLOW.Data.Entities;
using GLOW.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GLOW.Web.Controllers;

[Authorize]
public class FamilyController : Controller
{
    private readonly GLOWDbContext _context;
    private readonly IGlowAiService _aiService;

    public FamilyController(GLOWDbContext context, IGlowAiService aiService)
    {
        _context = context;
        _aiService = aiService;
    }

    public async Task<IActionResult> Index()
    {
        var stories = await _context.FamilyStories
            .OrderByDescending(f => f.Date)
            .ToListAsync();

        return View(stories);
    }

    [HttpPost]
    public async Task<IActionResult> AddStory(string title, string content, string feelings, string unsaidWords, string? photoUrl)
    {
        if (!string.IsNullOrWhiteSpace(title) || !string.IsNullOrWhiteSpace(content))
        {
            var reflection = await _aiService.AnalyzeFamilyStoryAsync(title, content, feelings, unsaidWords);

            var story = new FamilyStory
            {
                Title = title ?? "Khoảnh khắc gia đình ấm áp",
                Content = content ?? string.Empty,
                Feelings = feelings ?? string.Empty,
                UnsaidWords = unsaidWords ?? string.Empty,
                PhotoUrl = photoUrl ?? "https://images.unsplash.com/photo-1511895426328-dc8714191300?auto=format&fit=crop&w=600&q=80",
                AiReflection = reflection,
                Date = DateTime.Now
            };
            _context.FamilyStories.Add(story);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
