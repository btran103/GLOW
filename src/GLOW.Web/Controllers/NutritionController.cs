using GLOW.Data;
using GLOW.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GLOW.Web.Controllers;

[Authorize]
public class NutritionController : Controller
{
    private readonly GLOWDbContext _context;

    public NutritionController(GLOWDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var entries = await _context.NutritionEntries
            .OrderByDescending(n => n.Time)
            .Take(15)
            .ToListAsync();

        var todayCalories = entries
            .Where(n => n.Time.Date == DateTime.Today)
            .Sum(n => n.EstimatedCalories);

        ViewBag.TodayCalories = todayCalories;

        return View(entries);
    }

    [HttpPost]
    public async Task<IActionResult> AddEntry(string mealType, string dishName, int estimatedCalories, string nutrientsSummary, string feelingAfterMeal, string? imageUrl)
    {
        if (!string.IsNullOrWhiteSpace(dishName))
        {
            var entry = new NutritionEntry
            {
                MealType = mealType ?? "Trưa",
                DishName = dishName.Trim(),
                EstimatedCalories = estimatedCalories > 0 ? estimatedCalories : 450,
                NutrientsSummary = nutrientsSummary ?? "Đầy đủ Protein, Carb và Chất xơ",
                FeelingAfterMeal = feelingAfterMeal ?? "Thoải mái và ngon miệng",
                ImageUrl = imageUrl ?? "https://images.unsplash.com/photo-1546069901-ba9599a7e63c?auto=format&fit=crop&w=400&q=80",
                AiNutritionAdvice = "Bữa ăn cân bằng dinh dưỡng, cung cấp nguồn năng lượng dồi dào!",
                Time = DateTime.Now
            };
            _context.NutritionEntries.Add(entry);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
