using GLOW.Data;
using GLOW.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GLOW.Web.Controllers;

[Authorize]
public class AiChatController : Controller
{
    private readonly GLOWDbContext _context;
    private readonly IGlowAiService _aiService;

    public AiChatController(GLOWDbContext context, IGlowAiService aiService)
    {
        _context = context;
        _aiService = aiService;
    }

    public async Task<IActionResult> Index()
    {
        var messages = await _context.AiChatMessages
            .OrderBy(m => m.Timestamp)
            .ToListAsync();

        return View(messages);
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage(string userMessage)
    {
        if (!string.IsNullOrWhiteSpace(userMessage))
        {
            await _aiService.ChatWithAiAsync(userMessage.Trim());
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> ClearChat()
    {
        var messages = await _context.AiChatMessages.ToListAsync();
        _context.AiChatMessages.RemoveRange(messages);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
