using GLOW.Data;
using GLOW.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GLOW.Web.Controllers;

[Authorize]
public class CapsuleController : Controller
{
    private readonly GLOWDbContext _context;

    public CapsuleController(GLOWDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var capsules = await _context.TimeCapsules
            .OrderByDescending(c => c.SealedDate)
            .ToListAsync();

        return View(capsules);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCapsule(string title, string messageToFutureSelf, string currentGoals, string currentFeelings, DateTime unlockDate, string? photoUrl)
    {
        if (!string.IsNullOrWhiteSpace(title) && !string.IsNullOrWhiteSpace(messageToFutureSelf))
        {
            var capsule = new TimeCapsuleItem
            {
                Title = title.Trim(),
                MessageToFutureSelf = messageToFutureSelf.Trim(),
                CurrentGoals = currentGoals ?? string.Empty,
                CurrentFeelings = currentFeelings ?? string.Empty,
                PhotoUrl = photoUrl ?? "https://images.unsplash.com/photo-1529156069898-49953e39b3ac?auto=format&fit=crop&w=600&q=80",
                SealedDate = DateTime.Now,
                UnlockDate = unlockDate > DateTime.Now ? unlockDate : DateTime.Now.AddYears(1),
                IsOpened = false
            };
            _context.TimeCapsules.Add(capsule);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> OpenCapsule(int id)
    {
        var capsule = await _context.TimeCapsules.FindAsync(id);
        if (capsule != null)
        {
            capsule.IsOpened = true;
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
