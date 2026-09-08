using GLOW.Data;
using GLOW.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace GLOW.Services;

public class GlowAiService : IGlowAiService
{
    private readonly GLOWDbContext _context;
    private readonly IConfiguration _configuration;

    public GlowAiService(GLOWDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<DiaryRecord> GenerateDailyDiaryAsync(DateTime date)
    {
        var targetDate = date.Date;

        // Fetch day's data from all modules with safe in-memory filtering for SQLite
        var allStudySessions = await _context.StudySessions.ToListAsync();
        var studySessions = allStudySessions.Where(s => s.SessionDate.Date == targetDate).ToList();
        var studyMinutes = studySessions.Sum(s => s.DurationMinutes);

        var allWorkTasks = await _context.WorkTasks.ToListAsync();
        var workTasks = allWorkTasks
            .Where(w => w.CreatedAt.Date == targetDate || (w.CompletedAt.HasValue && w.CompletedAt.Value.Date == targetDate))
            .ToList();
        var completedWork = workTasks.Count(w => w.Status == "Done");

        var allExpenses = await _context.Expenses.ToListAsync();
        var expenses = allExpenses
            .Where(e => e.Date.Date == targetDate && e.Type == "Chi tiêu")
            .ToList();
        var totalSpent = expenses.Sum(e => e.Amount);

        var allHealth = await _context.HealthLogs.ToListAsync();
        var health = allHealth.FirstOrDefault(h => h.Date.Date == targetDate);

        var allMeals = await _context.NutritionEntries.ToListAsync();
        var meals = allMeals.Where(n => n.Time.Date == targetDate).ToList();

        var allMoods = await _context.MoodEntries.OrderByDescending(m => m.Date).ToListAsync();
        var mood = allMoods.FirstOrDefault(m => m.Date.Date == targetDate);

        var allMoments = await _context.Moments.ToListAsync();
        var moments = allMoments.Where(m => m.Timestamp.Date == targetDate).ToList();

        var allPlaces = await _context.MapMemories.ToListAsync();
        var places = allPlaces.Where(p => p.ArrivedAt.Date == targetDate).ToList();

        // Synthesize into Diary Sections
        var goodList = new List<string>();
        var notGoodList = new List<string>();
        var lessonsList = new List<string>();
        var memorableList = new List<string>();
        var improveList = new List<string>();
        var tomorrowList = new List<string>();

        if (studyMinutes > 0)
            goodList.Add($"Đã dành trọn vẹn {studyMinutes} phút học tập tập trung ({string.Join(", ", studySessions.Select(s => s.Subject).Distinct())}).");
        if (completedWork > 0)
            goodList.Add($"Hoàn thành xuất sắc {completedWork} đầu việc quan trọng.");
        if (health != null && health.SleepHours >= 7)
            goodList.Add($"Duy trì giấc ngủ chất lượng cao ({health.SleepHours:F1} giờ) giúp tinh thần tỉnh táo.");
        if (health != null && health.WaterMl >= 1800)
            goodList.Add($"Cấp ẩm cơ thể rất tốt với {health.WaterMl}ml nước.");
        if (moments.Any())
            goodList.Add($"Lưu giữ được {moments.Count} khoảnh khắc đáng yêu trong ngày.");
        if (!goodList.Any())
            goodList.Add("Đã nỗ lực và trải qua một ngày bình an với nhiều trải nghiệm quý giá.");

        if (health != null && health.SleepHours < 6.5)
            notGoodList.Add("Giấc ngủ chưa đủ 7 tiếng khiến năng lượng có lúc bị chùng xuống vào buổi chiều.");
        if (health != null && health.WaterMl < 1500)
            notGoodList.Add("Lượng nước uống còn hơi ít, cần bổ sung đều đặn hơn.");
        if (totalSpent > 300000)
            notGoodList.Add($"Chi tiêu hôm nay hơi cao ({totalSpent:N0} đ), chủ yếu ở các khoản mua sắm.");
        if (!notGoodList.Any())
            notGoodList.Add("Không có điều gì quá tiêu cực, chỉ cần chú ý nghỉ ngơi đúng giờ để bảo vệ sức khỏe.");

        lessonsList.Add("Khi chia nhỏ mục tiêu học tập thành từng chặng 30-45 phút, hiệu suất tăng lên rõ rệt.");
        lessonsList.Add("Khoảng thời gian thả lỏng đi dạo giúp tái tạo năng lượng sáng tạo nhanh chóng.");

        if (moments.Any())
            memorableList.Add($"Khoảnh khắc đáng nhớ nhất: {moments.First().Title} ({moments.First().MoodTag}).");
        if (places.Any())
            memorableList.Add($"Điểm dừng chân thú vị tại {places.First().PlaceName} với cảm xúc {places.First().Feeling}.");
        if (!memorableList.Any())
            memorableList.Add("Một ngày trôi qua êm ả với những cảm xúc bình yên và ấm áp.");

        improveList.Add("Tối nay nên tắt các thiết bị điện tử trước khi ngủ 30 phút để ngủ sâu giấc hơn.");
        improveList.Add("Duy trì uống nước mỗi khi đồng hồ nhắc nhở kêu.");

        tomorrowList.Add("Ưu tiên hoàn thành các task có deadline gần nhất trong buổi sáng.");
        tomorrowList.Add("Chọn trang phục tone pastel tươi sáng để khởi đầu ngày mới tràn đầy may mắn!");

        var diary = new DiaryRecord
        {
            Date = targetDate,
            Title = $"Hành trình ngày {targetDate:dd/MM/yyyy}: {(mood != null ? mood.MoodLabel : "Tươi sáng & Tràn đầy hy vọng")}",
            Summary = $"Một ngày tràn ngập sắc màu với {studyMinutes} phút học tập, mức năng lượng {(mood != null ? mood.EnergyScore : 8)}/10 và tâm trạng {(mood != null ? mood.MoodEmoji : "🌈")}.",
            GoodThings = string.Join("\n• ", goodList.Prepend("")),
            NotGoodThings = string.Join("\n• ", notGoodList.Prepend("")),
            LessonsLearned = string.Join("\n• ", lessonsList.Prepend("")),
            MemorableMoments = string.Join("\n• ", memorableList.Prepend("")),
            Improvements = string.Join("\n• ", improveList.Prepend("")),
            TomorrowSuggestions = string.Join("\n• ", tomorrowList.Prepend("")),
            MoodEmoji = mood != null ? mood.MoodEmoji : "✨",
            IsGeneratedByAi = true,
            CreatedAt = DateTime.Now
        };

        // Save or update existing
        var allDiaries = await _context.DiaryRecords.ToListAsync();
        var existing = allDiaries.FirstOrDefault(d => d.Date.Date == targetDate);
        if (existing != null)
        {
            existing.Title = diary.Title;
            existing.Summary = diary.Summary;
            existing.GoodThings = diary.GoodThings;
            existing.NotGoodThings = diary.NotGoodThings;
            existing.LessonsLearned = diary.LessonsLearned;
            existing.MemorableMoments = diary.MemorableMoments;
            existing.Improvements = diary.Improvements;
            existing.TomorrowSuggestions = diary.TomorrowSuggestions;
            existing.MoodEmoji = diary.MoodEmoji;
            await _context.SaveChangesAsync();
            return existing;
        }

        _context.DiaryRecords.Add(diary);
        await _context.SaveChangesAsync();
        return diary;
    }

    public async Task<AiChatMessage> ChatWithAiAsync(string userMessage)
    {
        // Gather recent context safely
        var today = DateTime.Today;
        var allSessions = await _context.StudySessions.ToListAsync();
        var todayStudy = allSessions.Where(s => s.SessionDate.Date == today).Sum(s => s.DurationMinutes);

        var allHealth = await _context.HealthLogs.ToListAsync();
        var todayHealth = allHealth.FirstOrDefault(h => h.Date.Date == today);

        var allMoods = await _context.MoodEntries.OrderByDescending(m => m.Date).ToListAsync();
        var todayMood = allMoods.FirstOrDefault(m => m.Date.Date == today);

        var allExpenses = await _context.Expenses.ToListAsync();
        var todaySpent = allExpenses.Where(e => e.Date.Date == today && e.Type == "Chi tiêu").Sum(e => e.Amount);

        string reply;
        string contextRef = "General";
        string msgLower = userMessage.ToLower();

        if (msgLower.Contains("mệt") || msgLower.Contains("đuối") || msgLower.Contains("năng lượng"))
        {
            contextRef = "Health & Study & Mood";
            reply = $"Mình hiểu cảm giác của bạn nè! ❤️ Hôm nay bạn đã dành {todayStudy} phút học tập/làm việc";
            if (todayHealth != null && todayHealth.SleepHours < 7)
            {
                reply += $" và đêm qua ngủ khoảng {todayHealth.SleepHours:F1}h (hơi ít một xíu so với mức lý tưởng 8h). Đây chính là lý do khiến năng lượng bạn bị giảm sút. Bạn hãy uống một cốc nước ấm, nhắm mắt nghỉ ngơi 15 phút hoặc nghe một bài nhạc Lofi nhẹ nhàng nhé!";
            }
            else
            {
                reply += ". Việc não bộ vận hành liên tục dễ khiến bạn cảm thấy mệt. Hãy đứng dậy vươn vai và thưởng cho mình một món ăn vặt yêu thích nhé!";
            }
        }
        else if (msgLower.Contains("học") || msgLower.Contains("deadline") || msgLower.Contains("bài tập"))
        {
            contextRef = "Study & Work";
            var pendingTasks = await _context.StudyTasks.Where(t => !t.IsCompleted).Take(3).ToListAsync();
            var taskNames = pendingTasks.Any() ? string.Join(", ", pendingTasks.Select(t => $"\"{t.Title}\"")) : "tất cả task đều đã xong";
            reply = $"Về việc học tập: Hôm nay bạn đã tích lũy được {todayStudy} phút học tập. Các nhiệm vụ ưu tiên tiếp theo là: {taskNames}. Bạn hãy áp dụng phương pháp Pomodoro (25 phút tập trung + 5 phút nghỉ) để học mà không bị quá tải nha! 📚✨";
        }
        else if (msgLower.Contains("tiền") || msgLower.Contains("chi tiêu") || msgLower.Contains("mua"))
        {
            contextRef = "Money";
            reply = $"Hôm nay bạn đã chi tiêu tổng cộng {todaySpent:N0} VNĐ. Mình thấy bạn đang duy trì thói quen ghi chép tài chính rất tốt! Hãy tiếp tục ưu tiên các khoản chi thiết yếu cho sức khỏe và học tập nhé. 💰🌱";
        }
        else if (msgLower.Contains("vui") || msgLower.Contains("hạnh phúc") || msgLower.Contains("tuyệt"))
        {
            contextRef = "Mood";
            reply = $"Nghe bạn nói vậy mình cũng vui lây luôn á! 🎉 Năng lượng tích cực này rất quý giá, bạn hãy chụp một tấm ảnh lưu vào GLOW Moments hoặc viết vài dòng nhật ký để sau này nhìn lại hành trình rực rỡ này nha! ✨🥰";
        }
        else if (msgLower.Contains("ngày mai") || msgLower.Contains("outfit") || msgLower.Contains("mặc gì"))
        {
            contextRef = "Tomorrow & Style";
            reply = "Ngày mai dự báo sẽ là một ngày đầy hứng khởi! Mình gợi ý bạn chọn trang phục tone Mint Green phối cùng Trắng kem để mang lại cảm giác tươi mới, dễ chịu và may mắn nhé! 👗💚";
        }
        else
        {
            contextRef = "Comprehensive";
            reply = $"Mình luôn ở đây để đồng hành cùng bạn trên hành trình khám phá và phát triển bản thân! Hiện tại hôm nay bạn đã học {todayStudy} phút, chi tiêu {todaySpent:N0} đ và mức năng lượng rất ổn định. Bạn muốn tâm sự thêm về chuyện học tập, thói quen hay bất kỳ cảm xúc nào cứ thoải mái chia sẻ với mình nhé! 🌈✨";
        }

        var responseMsg = new AiChatMessage
        {
            Sender = "Assistant",
            Message = reply,
            Timestamp = DateTime.Now,
            ContextReferenced = contextRef
        };

        _context.AiChatMessages.Add(new AiChatMessage { Sender = "User", Message = userMessage, Timestamp = DateTime.Now });
        _context.AiChatMessages.Add(responseMsg);
        await _context.SaveChangesAsync();

        return responseMsg;
    }

    public async Task<StyleAdvice> AnalyzeOutfitAndStyleAsync(string itemName, string category, string color, string style, string? imageUrl)
    {
        var existingWardrobe = await _context.WardrobeItems.ToListAsync();

        var matchingItems = existingWardrobe
            .Where(w => w.Category != category)
            .Take(3)
            .Select(w => $"{w.Name} ({w.Color})")
            .ToList();

        string combos = matchingItems.Any()
            ? string.Join(" + ", matchingItems)
            : "Áo thun trắng basic + Quần jean ống suông + Giày sneaker chunky";

        string analysis = $"Món đồ '{itemName}' với tông màu '{color}' và phong cách '{style}' mang đậm chất Gen Z năng động và rất dễ ứng dụng hằng ngày.";

        string recommendation = "Đáng mua! Món đồ này rất hợp với phong cách hiện tại của bạn và có thể tạo ra ít nhất 3 set đồ khác nhau với tủ quần áo đang có.";

        var advice = new StyleAdvice
        {
            TargetItemName = itemName,
            TargetItemImageUrl = imageUrl ?? "https://images.unsplash.com/photo-1515886657613-9f3515b0c78f?auto=format&fit=crop&w=400&q=80",
            DetectedColor = color,
            DetectedStyle = style,
            MatchAnalysis = analysis,
            SuggestedCombinations = $"Phối gợi ý: {itemName} kết hợp cùng {combos}",
            BuyingRecommendation = recommendation,
            CreatedAt = DateTime.Now
        };

        _context.StyleAdvices.Add(advice);
        await _context.SaveChangesAsync();
        return advice;
    }

    public async Task<TarotReading> DrawTarotAsync(string questionTopic, string spreadType)
    {
        var allCards = await _context.TarotCards.ToListAsync();
        if (!allCards.Any())
        {
            allCards = new List<TarotCard>
            {
                new TarotCard { Id = 1, NameVi = "The Fool", Keywords = "Khởi đầu mới", UprightMeaning = "Hãy tự tin bước đi.", Affirmation = "Tôi đón nhận khởi đầu mới." }
            };
        }

        var random = new Random();
        int countToDraw = spreadType switch
        {
            "1 Card" => 1,
            "Yes/No" => 1,
            _ => 3
        };

        var drawn = allCards.OrderBy(x => random.Next()).Take(countToDraw).ToList();
        var cardIds = string.Join(", ", drawn.Select(c => c.Id));

        string interpretation;
        if (spreadType == "1 Card")
        {
            var card = drawn[0];
            interpretation = $"✨ **Thông điệp chính từ lá {card.NameVi}**:\n{card.UprightMeaning}\n\n💡 **Lời khuyên cho bạn**: {card.StudyWorkMeaning}\n\n🌟 **Affirmation**: \"{card.Affirmation}\"";
        }
        else if (spreadType == "Yes/No")
        {
            var card = drawn[0];
            bool isYes = card.Id % 2 == 0 || card.Element == "Lửa" || card.Element == "Khí";
            interpretation = $"🔮 **Kết quả trả lời**: **{(isYes ? "CÓ (YES! ✨)" : "HÃY KIÊN NHẪN (CHƯA PHẢI LÚC ⏳)")}**\n\nLá bài rút được: **{card.NameVi}**.\n{card.UprightMeaning}\nThông điệp nhắc nhở bạn hãy lắng nghe trực giác và tập trung vào hành động thực tế.";
        }
        else
        {
            var c1 = drawn[0];
            var c2 = drawn.Count > 1 ? drawn[1] : c1;
            var c3 = drawn.Count > 2 ? drawn[2] : c1;
            interpretation = $"🃏 **Trải bài 3 lá (Quá khứ - Hiện tại - Tương lai / Lời khuyên)**:\n\n" +
                             $"1️⃣ **Quá khứ / Nền tảng**: **{c1.NameVi}** — {c1.Keywords}. Những trải nghiệm đã qua đã tôi luyện sự bản lĩnh của bạn.\n\n" +
                             $"2️⃣ **Hiện tại / Tình thế**: **{c2.NameVi}** — {c2.Keywords}. {c2.UprightMeaning}\n\n" +
                             $"3️⃣ **Tương lai / Lời khuyên**: **{c3.NameVi}** — {c3.Keywords}. {c3.StudyWorkMeaning}\n\n" +
                             $"🌟 **Thông điệp chung**: Tarot là chiếc gương phản chiếu tâm thức để bạn thấu hiểu chính mình và tự tin kiến tạo con đường phía trước!";
        }

        var reading = new TarotReading
        {
            QuestionTopic = questionTopic,
            SpreadType = spreadType,
            CardIdsDrawn = cardIds,
            AiInterpretation = interpretation,
            ReadingDate = DateTime.Now
        };

        _context.TarotReadings.Add(reading);
        await _context.SaveChangesAsync();
        return reading;
    }

    public async Task<TomorrowPlan> GenerateTomorrowPlanAsync()
    {
        var tomorrow = DateTime.Today.AddDays(1);
        var pendingStudy = await _context.StudyTasks.Where(t => !t.IsCompleted).FirstOrDefaultAsync();
        var pendingWork = await _context.WorkTasks.Where(w => w.Status != "Done").FirstOrDefaultAsync();

        var colors = new[] { "Mint Green & Trắng kem", "Pastel Yellow & Lavender", "Sky Blue & Lime", "Baby Pink & Beige" };
        var random = new Random();
        var luckyColor = colors[random.Next(colors.Length)];

        var plan = new TomorrowPlan
        {
            PlanForDate = tomorrow,
            PriorityStudy = pendingStudy != null ? $"Ôn tập & hoàn thành: {pendingStudy.Title} ({pendingStudy.EstimatedMinutes} phút)" : "Đọc 20 trang sách phát triển kỹ năng hoặc học từ vựng tiếng Anh",
            PriorityWork = pendingWork != null ? $"Giải quyết đầu việc: {pendingWork.Title}" : "Lên danh sách việc cần làm cho tuần mới và dọn dẹp bàn làm việc",
            SuggestedOutfit = $"Set đồ tone {luckyColor} thoải mái, kết hợp giày sneaker",
            OutfitLuckyColor = luckyColor,
            NutritionSuggestion = "Bữa sáng giàu đạm (trứng ngũ cốc) + Bữa trưa nhiều rau củ luộc + 1 ly sinh tố hoa quả tươi",
            SelfCareReminders = "💧 Uống đủ 2L nước • 👀 Nghỉ mắt 20-20-20 • 🪑 Vận động sau mỗi 60 phút ngồi học",
            PositiveMessage = "Ngày mai là cơ hội tuyệt vời để bạn tỏa sáng rực rỡ hơn hôm nay. Hãy mỉm cười và tự tin nhé! ✨🌈",
            GeneratedAt = DateTime.Now
        };

        _context.TomorrowPlans.Add(plan);
        await _context.SaveChangesAsync();
        return plan;
    }

    public async Task<string> AnalyzeFamilyStoryAsync(string title, string content, string feelings, string unsaidWords)
    {
        await Task.Yield();
        return "Gia đình là bến đỗ bình yên nhất sau mọi giông bão. Cảm xúc bạn vừa chia sẻ thật ấm áp và chân thành. Nếu có dịp, hãy thử gửi một cái ôm hoặc một lời nhắn ngọt ngào tới người thân nhé, điều đó sẽ lan tỏa niềm vui vô bờ bến!";
    }
}
