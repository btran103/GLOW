using GLOW.Data;
using GLOW.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GLOW.Web.Controllers;

[Authorize]
public class MoneyController : Controller
{
    private readonly GLOWDbContext _context;

    public MoneyController(GLOWDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _context.Expenses
            .OrderByDescending(e => e.Date)
            .ToListAsync();

        var totalExpense = items.Where(e => e.Type == "Chi tiêu").Sum(e => e.Amount);
        var totalIncome = items.Where(e => e.Type == "Thu nhập").Sum(e => e.Amount);
        var balance = totalIncome - totalExpense;

        var categoryExpenses = items
            .Where(e => e.Type == "Chi tiêu")
            .GroupBy(e => e.Category)
            .Select(g => new { Category = g.Key, Total = g.Sum(x => x.Amount) })
            .ToList();

        ViewBag.TotalExpense = totalExpense;
        ViewBag.TotalIncome = totalIncome;
        ViewBag.Balance = balance;
        ViewBag.CategoryExpenses = categoryExpenses;

        return View(items);
    }

    [HttpPost]
    public async Task<IActionResult> AddExpense(string title, decimal amount, string type, string category, string paymentMethod, string locationName, string notes, DateTime? date)
    {
        if (!string.IsNullOrWhiteSpace(title) && amount > 0)
        {
            var item = new ExpenseItem
            {
                Title = title.Trim(),
                Amount = amount,
                Type = type ?? "Chi tiêu",
                Category = category ?? "Ăn uống",
                PaymentMethod = paymentMethod ?? "Ví điện tử / Chuyển khoản",
                LocationName = locationName ?? string.Empty,
                Notes = notes ?? string.Empty,
                Date = date ?? DateTime.Now
            };
            _context.Expenses.Add(item);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
