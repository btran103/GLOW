using GLOW.Data.Entities;

namespace GLOW.Services;

public interface IGlowAiService
{
    Task<DiaryRecord> GenerateDailyDiaryAsync(DateTime date);
    Task<AiChatMessage> ChatWithAiAsync(string userMessage);
    Task<StyleAdvice> AnalyzeOutfitAndStyleAsync(string itemName, string category, string color, string style, string? imageUrl);
    Task<TarotReading> DrawTarotAsync(string questionTopic, string spreadType);
    Task<TomorrowPlan> GenerateTomorrowPlanAsync();
    Task<string> AnalyzeFamilyStoryAsync(string title, string content, string feelings, string unsaidWords);
}

public class InsightReport
{
    public string MainHighlight { get; set; } = string.Empty;
    public List<InsightItem> Insights { get; set; } = new();
    public int HabitStreakDays { get; set; }
    public double AverageMood { get; set; }
    public double TotalStudyHoursWeek { get; set; }
    public double AverageSleepHours { get; set; }
    public decimal TotalExpenseWeek { get; set; }
    public List<GrowthMetric> GrowthTrend { get; set; } = new();
}

public class InsightItem
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IconEmoji { get; set; } = "💡";
    public string Category { get; set; } = "Cảm xúc & Năng lượng";
    public string TagColor { get; set; } = "#A7F3D0";
}

public class GrowthMetric
{
    public string Period { get; set; } = "Tuần này";
    public double Score { get; set; }
    public string Comment { get; set; } = string.Empty;
}

public interface IInsightEngine
{
    Task<InsightReport> GenerateInsightReportAsync();
}
