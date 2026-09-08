using GLOW.Data;
using GLOW.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GLOW.Web.Controllers;

[Authorize]
public class MomentsController : Controller
{
    private readonly GLOWDbContext _context;

    public MomentsController(GLOWDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var moments = await _context.Moments
            .OrderByDescending(m => m.Timestamp)
            .ToListAsync();

        return View(moments);
    }

    [HttpPost]
    public async Task<IActionResult> AddMoment(string title, string caption, string locationName, string moodTag, string tags, string? imageUrl)
    {
        if (!string.IsNullOrWhiteSpace(title) || !string.IsNullOrWhiteSpace(caption))
        {
            var moment = new MomentItem
            {
                Title = string.IsNullOrWhiteSpace(title) ? "Khoảnh khắc đáng nhớ" : title.Trim(),
                Caption = caption ?? string.Empty,
                LocationName = locationName ?? "Góc nhỏ thân quen",
                MoodTag = moodTag ?? "✨ Vui vẻ & Bình yên",
                Tags = tags ?? "#moment, #glow",
                ImageUrl = imageUrl ?? "https://images.unsplash.com/photo-1501339847302-ac426a4a7cbb?auto=format&fit=crop&w=600&q=80",
                Timestamp = DateTime.Now,
                IsFavorite = true
            };
            _context.Moments.Add(moment);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
