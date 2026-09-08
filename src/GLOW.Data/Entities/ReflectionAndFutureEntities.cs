namespace GLOW.Data.Entities;

public class DiaryRecord
{
    public int Id { get; set; }
    public DateTime Date { get; set; } = DateTime.Today;
    public string Title { get; set; } = "Nhật ký hôm nay";
    public string Summary { get; set; } = string.Empty;
    public string GoodThings { get; set; } = string.Empty; // ✨ Điều tốt hôm nay
    public string NotGoodThings { get; set; } = string.Empty; // 🌧 Điều chưa tốt
    public string LessonsLearned { get; set; } = string.Empty; // 🧠 Bài học hôm nay
    public string MemorableMoments { get; set; } = string.Empty; // 💭 Điều đáng nhớ
    public string Improvements { get; set; } = string.Empty; // 🌱 Điều có thể cải thiện
    public string TomorrowSuggestions { get; set; } = string.Empty; // 🌅 Gợi ý cho ngày mai
    public bool IsGeneratedByAi { get; set; } = true;
    public string MoodEmoji { get; set; } = "🌈";
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

public class AiChatMessage
{
    public int Id { get; set; }
    public string Sender { get; set; } = "User"; // User, Assistant
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public string? ContextReferenced { get; set; } // ví dụ: "Study, Sleep, Mood"
}

public class FamilyStory
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Feelings { get; set; } = string.Empty;
    public string UnsaidWords { get; set; } = string.Empty; // Điều muốn nói nhưng chưa nói
    public DateTime Date { get; set; } = DateTime.Now;
    public string? PhotoUrl { get; set; }
    public string AiReflection { get; set; } = string.Empty; // AI phản hồi nhẹ nhàng
    public bool IsLockedWithPin { get; set; } = false;
}

public class WardrobeItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = "Áo"; // Áo, Quần, Váy/Đầm, Giày, Phụ kiện, Áo khoác
    public string Color { get; set; } = "Mint Green / Trắng kem";
    public string Style { get; set; } = "Casual Gen Z, Năng động";
    public string Pattern { get; set; } = "Trơn / Minimalist";
    public string? ImageUrl { get; set; }
    public int UsageCount { get; set; } = 0;
    public string Notes { get; set; } = string.Empty;
    public DateTime AddedDate { get; set; } = DateTime.Now;
}

public class StyleAdvice
{
    public int Id { get; set; }
    public string TargetItemName { get; set; } = string.Empty;
    public string? TargetItemImageUrl { get; set; }
    public string DetectedColor { get; set; } = string.Empty;
    public string DetectedStyle { get; set; } = string.Empty;
    public string MatchAnalysis { get; set; } = string.Empty;
    public string SuggestedCombinations { get; set; } = string.Empty;
    public string BuyingRecommendation { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

public class TarotCard
{
    public int Id { get; set; }
    public string NameVi { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string Element { get; set; } = "Lửa / Nước / Khí / Đất";
    public string Keywords { get; set; } = string.Empty;
    public string UprightMeaning { get; set; } = string.Empty;
    public string LoveMeaning { get; set; } = string.Empty;
    public string StudyWorkMeaning { get; set; } = string.Empty;
    public string Affirmation { get; set; } = string.Empty;
}

public class TarotReading
{
    public int Id { get; set; }
    public string QuestionTopic { get; set; } = "Tổng quan"; // Tổng quan, Tình cảm, Học tập, Công việc, Yes/No
    public string SpreadType { get; set; } = "3 Cards"; // 1 Card, 3 Cards, Yes/No
    public string CardIdsDrawn { get; set; } = "1, 2, 3"; // CSV IDs
    public string AiInterpretation { get; set; } = string.Empty;
    public DateTime ReadingDate { get; set; } = DateTime.Now;
}

public class TomorrowPlan
{
    public int Id { get; set; }
    public DateTime PlanForDate { get; set; } = DateTime.Today.AddDays(1);
    public string PriorityStudy { get; set; } = string.Empty;
    public string PriorityWork { get; set; } = string.Empty;
    public string SuggestedOutfit { get; set; } = string.Empty;
    public string OutfitLuckyColor { get; set; } = "Mint Green & Cream White";
    public string NutritionSuggestion { get; set; } = string.Empty;
    public string SelfCareReminders { get; set; } = string.Empty;
    public string PositiveMessage { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; } = DateTime.Now;
}

public class TimeCapsuleItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string MessageToFutureSelf { get; set; } = string.Empty;
    public string CurrentGoals { get; set; } = string.Empty;
    public string CurrentFeelings { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
    public DateTime SealedDate { get; set; } = DateTime.Now;
    public DateTime UnlockDate { get; set; } = DateTime.Now.AddYears(1);
    public bool IsOpened { get; set; } = false;
}
