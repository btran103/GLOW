using GLOW.Data.Entities;

namespace GLOW.Models;

public class HomeDashboardViewModel
{
    public UserProfile Profile { get; set; } = new();
    public MoodEntry? TodayMood { get; set; }
    public HealthLog? TodayHealth { get; set; }
    public List<StudyTask> PendingStudyTasks { get; set; } = new();
    public List<WorkTask> PendingWorkTasks { get; set; } = new();
    public decimal TodaySpent { get; set; }
    public int TodayStudyMinutes { get; set; }
    public int TodayWaterMl { get; set; }
    public int WaterGoalMl { get; set; } = 2000;
    public List<NutritionEntry> TodayMeals { get; set; } = new();
    public List<MomentItem> RecentMoments { get; set; } = new();
    public List<MapMemory> RecentPlaces { get; set; } = new();
    public DiaryRecord? TodayDiary { get; set; }
    public TomorrowPlan? TomorrowPlan { get; set; }
    public string DailyAffirmation { get; set; } = "Hôm nay là một ngày tuyệt vời để bạn tỏa sáng rực rỡ! ✨";
}
