using GLOW.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GLOW.Web.Controllers;

[Authorize]
public class InsightController : Controller
{
    private readonly IInsightEngine _insightEngine;

    public InsightController(IInsightEngine insightEngine)
    {
        _insightEngine = insightEngine;
    }

    public async Task<IActionResult> Index()
    {
        var report = await _insightEngine.GenerateInsightReportAsync();
        return View(report);
    }
}
