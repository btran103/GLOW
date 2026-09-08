namespace GLOW.Data.Entities;

public class HealthLog
{
    public int Id { get; set; }
    public DateTime Date { get; set; } = DateTime.Today;
    public double SleepHours { get; set; } = 7.5;
    public string SleepQuality { get; set; } = "Sâu giấc, tỉnh táo"; // Rất tốt, Tốt, Chập chờn, Mệt mỏi
    public int StepsCount { get; set; } = 6500;
    public int ActiveMinutes { get; set; } = 45;
    public int EnergyLevel { get; set; } = 8; // 1 - 10
    public int WaterMl { get; set; } = 1800;
    public string HealthNotes { get; set; } = string.Empty;
}

public class WaterLog
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public int AmountMl { get; set; } = 250;
}

public class HealthReminder
{
    public int Id { get; set; }
    public string Type { get; set; } = "Water"; // Water, EyeBreak, Stretch, Rest
    public string Title { get; set; } = "Uống một ngụm nước mát nhé! 💧";
    public string Message { get; set; } = "Hãy cấp ẩm cho cơ thể để não bộ hoạt động năng suất nhất nha.";
    public int IntervalMinutes { get; set; } = 45;
    public bool IsActive { get; set; } = true;
}

public class NutritionEntry
{
    public int Id { get; set; }
    public string MealType { get; set; } = "Trưa"; // Sáng, Trưa, Tối, Ăn vặt
    public string DishName { get; set; } = string.Empty;
    public DateTime Time { get; set; } = DateTime.Now;
    public string? ImageUrl { get; set; }
    public int EstimatedCalories { get; set; } = 450;
    public string NutrientsSummary { get; set; } = "Protein: 25g, Carb: 50g, Fat: 12g, Chất xơ: 6g";
    public string FeelingAfterMeal { get; set; } = "No vừa phải, nhẹ bụng";
    public string AiNutritionAdvice { get; set; } = "Bữa ăn giàu đạm và rau xanh rất cân bằng!";
}

public class MoodEntry
{
    public int Id { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
    public int MoodScore { get; set; } = 8; // 1-10
    public string MoodLabel { get; set; } = "Rất vui & Hứng khởi"; // Tuyệt vời, Vui vẻ, Bình yên, Hơi mệt, Căng thẳng, Buồn
    public string MoodEmoji { get; set; } = "🥰";
    public int EnergyScore { get; set; } = 8; // 1-10
    public string JoyTriggers { get; set; } = "Hoàn thành bài tập sớm, uống ly matcha latte thơm";
    public string StressTriggers { get; set; } = "Hơi buồn ngủ xíu buổi chiều";
    public string Note { get; set; } = "Một ngày nhiều năng lượng tích cực!";
    public string AiAnalysis { get; set; } = "Tâm trạng bạn đang ở mức cao nhờ giấc ngủ tốt và hoàn thành mục tiêu học tập.";
}

public class MomentItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Caption { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public string LocationName { get; set; } = "Quán cà phê góc phố";
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string MoodTag { get; set; } = "✨ Bình yên & chill";
    public string Tags { get; set; } = "#chill, #cafe, #studying";
    public bool IsFavorite { get; set; } = true;
}

public class MapMemory
{
    public int Id { get; set; }
    public string PlaceName { get; set; } = string.Empty;
    public string Category { get; set; } = "Cafe & Học tập"; // Cafe, Trường học, Công ty, Công viên, Nhà, Ăn uống, Du lịch
    public double Latitude { get; set; } = 10.7769; // TP.HCM default coordinates
    public double Longitude { get; set; } = 106.7009;
    public DateTime ArrivedAt { get; set; } = DateTime.Now.AddHours(-3);
    public DateTime? LeftAt { get; set; } = DateTime.Now.AddHours(-1);
    public int DurationMinutes { get; set; } = 120;
    public string Activity { get; set; } = "Ngồi học bài và làm bài tập C#";
    public string Feeling { get; set; } = "Tập trung và thoải mái";
    public string? PhotoUrl { get; set; }
}
