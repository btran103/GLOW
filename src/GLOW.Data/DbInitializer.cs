using GLOW.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace GLOW.Data;

public static class DbInitializer
{
    public static void Initialize(GLOWDbContext context)
    {
        context.Database.EnsureCreated();

        // Check if Users table is created / seeded
        if (!context.Users.Any())
        {
            // Seed default application user
            // Password: Glow@2026!
            string salt = "GLOW_SALT_2026_XYZ";
            // Hash with PasswordSecurity standard
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            byte[] combined = System.Text.Encoding.UTF8.GetBytes("Glow@2026!" + salt + "GLOW_SECRET_PEPPER_2026");
            string hash = Convert.ToBase64String(sha256.ComputeHash(combined));

            var defaultUser = new ApplicationUser
            {
                FullName = "Bảo Trân",
                Email = "baotran@gmail.com",
                Username = "baotran_glow",
                PasswordHash = hash,
                Salt = salt,
                IsEmailConfirmed = true,
                AvatarUrl = "https://images.unsplash.com/photo-1534528741775-53994a69daeb?auto=format&fit=crop&w=400&q=80",
                CreatedAt = DateTime.Now
            };
            context.Users.Add(defaultUser);
            context.SaveChanges();
        }

        if (context.UserProfiles.Any())
        {
            return; // DB already seeded with profile
        }

        // 1. User Profile & Settings
        var user = new UserProfile
        {
            Username = "baotran_glow",
            Nickname = "Bảo Trân",
            AvatarUrl = "https://images.unsplash.com/photo-1534528741775-53994a69daeb?auto=format&fit=crop&w=400&q=80",
            Bio = "Ghi lại từng khoảnh khắc thanh xuân, học hỏi & yêu thương chính mình mỗi ngày ✨🌱",
            PreferredStyle = "Gen Z Pastel, Streetwear & Cozy Minimalist",
            DailyWaterGoalMl = 2200,
            DailyStudyGoalMinutes = 240,
            DailySleepGoalHours = 8
        };
        context.UserProfiles.Add(user);

        context.PrivacySettings.Add(new UserPrivacySetting
        {
            LocationEnabled = true,
            AllowAiDataAnalysis = true,
            FamilyStoriesPrivate = true,
            CameraHistoryLocalOnly = true
        });

        // 2. Study Courses & Tasks
        var course1 = new StudyCourse
        {
            Code = "PRG301",
            Name = "Lập trình C# & ASP.NET Core",
            Instructor = "ThS. Nguyễn Văn A",
            Room = "Phòng Lab 402",
            ColorHex = "#A7F3D0",
            ScheduleDayOfWeek = "Thứ 2, Thứ 4",
            TimeSlot = "08:00 - 11:15",
            Credits = 4
        };
        var course2 = new StudyCourse
        {
            Code = "DES202",
            Name = "UI/UX Design & Figma",
            Instructor = "Cô Lê Thị B",
            Room = "Design Studio 2",
            ColorHex = "#FDE68A",
            ScheduleDayOfWeek = "Thứ 3, Thứ 6",
            TimeSlot = "13:30 - 16:45",
            Credits = 3
        };
        var course3 = new StudyCourse
        {
            Code = "ENG103",
            Name = "IELTS Speaking & Writing Prep",
            Instructor = "Mr. David Smith",
            Room = "E-Learning Online",
            ColorHex = "#E9D5FF",
            ScheduleDayOfWeek = "Thứ 7",
            TimeSlot = "18:00 - 20:00",
            Credits = 2
        };

        context.StudyCourses.AddRange(course1, course2, course3);
        context.SaveChanges();

        context.StudyTasks.AddRange(
            new StudyTask
            {
                CourseId = course1.Id,
                Title = "Hoàn thiện Entity Framework và Controller cho GLOW",
                Description = "Tạo DbContext, Controller và Views hoàn chỉnh cho hệ thống",
                DueDate = DateTime.Today.AddDays(2),
                EstimatedMinutes = 120,
                CompletedMinutes = 90,
                IsCompleted = false,
                Priority = "High"
            },
            new StudyTask
            {
                CourseId = course2.Id,
                Title = "Thiết kế Bento Grid Component trên Figma",
                Description = "Tông màu Pastel: Mint, Lime, Pink, Sky Blue",
                DueDate = DateTime.Today.AddDays(3),
                EstimatedMinutes = 90,
                CompletedMinutes = 90,
                IsCompleted = true,
                Priority = "Medium"
            },
            new StudyTask
            {
                CourseId = course3.Id,
                Title = "Luyện nói Speaking Part 2 về Daily Routine",
                Description = "Thu âm 2 phút về chủ đề Quản lý thời gian cá nhân",
                DueDate = DateTime.Today.AddDays(1),
                EstimatedMinutes = 45,
                CompletedMinutes = 20,
                IsCompleted = false,
                Priority = "Medium"
            }
        );

        // Study Sessions
        context.StudySessions.AddRange(
            new StudySession { Subject = "Lập trình C# ASP.NET", DurationMinutes = 90, FocusQuality = "Rất tập trung 🚀", SessionDate = DateTime.Today.AddDays(-2), Notes = "Làm xong phần DbContext và Services" },
            new StudySession { Subject = "UI/UX Design Bento", DurationMinutes = 60, FocusQuality = "Tuyệt vời 🌟", SessionDate = DateTime.Today.AddDays(-1), Notes = "Phối màu pastel Gen Z rất ưng ý" },
            new StudySession { Subject = "Lập trình C# ASP.NET", DurationMinutes = 75, FocusQuality = "Tập trung cao độ 🔥", SessionDate = DateTime.Today, Notes = "Xây dựng các Controller chính" }
        );

        // 3. Work Tasks
        context.WorkTasks.AddRange(
            new WorkTask
            {
                Title = "Review code và tối ưu layout Responsive cho Mobile",
                ProjectName = "GLOW Web App",
                Description = "Đảm bảo trên iPhone và Android hiển thị Bento Cards cân đối, mượt mà",
                DueDate = DateTime.Today.AddDays(1),
                Priority = "Urgent",
                Status = "In Progress",
                WorkMinutesLogged = 60,
                Tags = "#frontend, #mobile, #css"
            },
            new WorkTask
            {
                Title = "Tích hợp biểu đồ Chart.js cho Module Cảm Xúc & Chi Tiêu",
                ProjectName = "GLOW Web App",
                Description = "Vẽ biểu đồ đường cho Mood & Biểu đồ tròn cho Danh mục chi tiêu",
                DueDate = DateTime.Today.AddDays(2),
                Priority = "High",
                Status = "Done",
                WorkMinutesLogged = 90,
                Tags = "#charts, #javascript"
            },
            new WorkTask
            {
                Title = "Viết tài liệu hướng dẫn sử dụng GLOW cho Gen Z",
                ProjectName = "Tài liệu dự án",
                Description = "Cách dùng Bento Dashboard, AI Diary và Hộp thời gian",
                DueDate = DateTime.Today.AddDays(5),
                Priority = "Medium",
                Status = "Todo",
                WorkMinutesLogged = 0,
                Tags = "#docs, #guide"
            }
        );

        // 4. Expenses
        context.Expenses.AddRange(
            new ExpenseItem { Title = "Ly Matcha Latte dừa nướng", Amount = 45000, Category = "Ăn uống", Type = "Chi tiêu", LocationName = "The Coffee House", Notes = "Nạp năng lượng trước giờ học", Date = DateTime.Today.AddDays(-3) },
            new ExpenseItem { Title = "Mua giáo trình lập trình", Amount = 120000, Category = "Học tập", Type = "Chi tiêu", LocationName = "Nhà sách Fahasa", Notes = "Sách tham khảo C# chuyên sâu", Date = DateTime.Today.AddDays(-2) },
            new ExpenseItem { Title = "Bữa trưa cơm tấm sườn trứng", Amount = 50000, Category = "Ăn uống", Type = "Chi tiêu", LocationName = "Quán cơm gần trường", Notes = "Ăn no đủ năng lượng", Date = DateTime.Today.AddDays(-1) },
            new ExpenseItem { Title = "Nạp tiền vé xe buýt tháng", Amount = 100000, Category = "Đi lại", Type = "Chi tiêu", LocationName = "Bến xe buýt", Notes = "Tiết kiệm chi phí đi học", Date = DateTime.Today.AddDays(-1) },
            new ExpenseItem { Title = "Học bổng khuyến khích học tập", Amount = 1500000, Category = "Học tập", Type = "Thu nhập", LocationName = "Trường Đại học", Notes = "Phần thưởng nỗ lực kỳ trước!", Date = DateTime.Today.AddDays(-5) },
            new ExpenseItem { Title = "Bữa tối salad ức gà và sinh tố bơ", Amount = 65000, Category = "Ăn uống", Type = "Chi tiêu", LocationName = "Healthy Corner", Notes = "Bữa ăn nhẹ nhàng lành mạnh", Date = DateTime.Today }
        );

        // 5. Health Logs & Reminders
        context.HealthLogs.AddRange(
            new HealthLog { Date = DateTime.Today.AddDays(-2), SleepHours = 8.0, SleepQuality = "Rất sâu giấc, sảng khoái", StepsCount = 7400, ActiveMinutes = 50, EnergyLevel = 9, WaterMl = 2100, HealthNotes = "Cơ thể tràn đầy năng lượng" },
            new HealthLog { Date = DateTime.Today.AddDays(-1), SleepHours = 6.5, SleepQuality = "Hơi thiếu ngủ một xíu", StepsCount = 5200, ActiveMinutes = 35, EnergyLevel = 7, WaterMl = 1800, HealthNotes = "Hơi mỏi mắt vào buổi chiều" },
            new HealthLog { Date = DateTime.Today, SleepHours = 7.8, SleepQuality = "Tốt, thức dậy tỉnh táo", StepsCount = 6800, ActiveMinutes = 45, EnergyLevel = 8, WaterMl = 1950, HealthNotes = "Cảm thấy rất thoải mái" }
        );

        context.HealthReminders.AddRange(
            new HealthReminder { Type = "Water", Title = "Uống một ngụm nước mát nhé! 💧", Message = "Cấp ẩm giúp não bộ phản xạ nhanh hơn và da dẻ tươi tắn.", IntervalMinutes = 45, IsActive = true },
            new HealthReminder { Type = "EyeBreak", Title = "Quy tắc 20-20-20 cho mắt 👀", Message = "Nhìn xa 6 mét trong vòng 20 giây để thư giãn cơ mắt nhé.", IntervalMinutes = 30, IsActive = true },
            new HealthReminder { Type = "Stretch", Title = "Đứng dậy vươn vai vận động xíu nào 🪑", Message = "Đứng dậy đi lại vài bước để máu huyết lưu thông nha.", IntervalMinutes = 60, IsActive = true },
            new HealthReminder { Type = "Rest", Title = "Nghỉ ngơi và hít thở sâu 😴", Message = "Nhắm mắt hít một hơi thật sâu để tái tạo năng lượng.", IntervalMinutes = 90, IsActive = true }
        );

        // 6. Nutrition Entries
        context.NutritionEntries.AddRange(
            new NutritionEntry
            {
                MealType = "Sáng",
                DishName = "Bánh mì ngũ cốc trứng ốp la + Bơ tươi",
                Time = DateTime.Today.AddHours(7).AddMinutes(30),
                ImageUrl = "https://images.unsplash.com/photo-1525351484163-7529414344d8?auto=format&fit=crop&w=400&q=80",
                EstimatedCalories = 380,
                NutrientsSummary = "Protein: 18g, Carb: 35g, Fat: 14g, Fiber: 5g",
                FeelingAfterMeal = "Tươi tỉnh và tràn đầy sinh lực",
                AiNutritionAdvice = "Bữa sáng tuyệt vời kết hợp chất béo tốt và carb phức hợp!"
            },
            new NutritionEntry
            {
                MealType = "Trưa",
                DishName = "Cơm gạo lứt ức gà nướng mật ong + Rau củ luộc",
                Time = DateTime.Today.AddHours(12).AddMinutes(15),
                ImageUrl = "https://images.unsplash.com/photo-1546069901-ba9599a7e63c?auto=format&fit=crop&w=400&q=80",
                EstimatedCalories = 520,
                NutrientsSummary = "Protein: 38g, Carb: 55g, Fat: 10g, Fiber: 8g",
                FeelingAfterMeal = "No lâu, không bị đầy bụng",
                AiNutritionAdvice = "Hàm lượng protein dồi dào, giúp duy trì năng lượng suốt buổi chiều."
            },
            new NutritionEntry
            {
                MealType = "Tối",
                DishName = "Salad cá hồi bơ chanh dây + Súp bí đỏ",
                Time = DateTime.Today.AddHours(18).AddMinutes(45),
                ImageUrl = "https://images.unsplash.com/photo-1540420773420-3366772f4999?auto=format&fit=crop&w=400&q=80",
                EstimatedCalories = 410,
                NutrientsSummary = "Protein: 26g, Carb: 28g, Fat: 16g, Fiber: 6g",
                FeelingAfterMeal = "Nhẹ bụng, dễ tiêu hóa cho giấc ngủ ngon",
                AiNutritionAdvice = "Omega-3 từ cá hồi rất tốt cho não bộ và hỗ trợ giấc ngủ sâu."
            }
        );

        // 7. Mood Entries
        context.MoodEntries.AddRange(
            new MoodEntry
            {
                Date = DateTime.Today.AddDays(-3),
                MoodScore = 7,
                MoodLabel = "Bình yên & Nhẹ nhàng",
                MoodEmoji = "🌱",
                EnergyScore = 7,
                JoyTriggers = "Đọc được một cuốn sách hay, nghe nhạc Lofi",
                StressTriggers = "Thời tiết mưa nhẹ",
                Note = "Một ngày êm đềm trôi qua.",
                AiAnalysis = "Tâm trạng cân bằng, năng lượng duy trì ở mức ổn định."
            },
            new MoodEntry
            {
                Date = DateTime.Today.AddDays(-2),
                MoodScore = 9,
                MoodLabel = "Siêu hào hứng & Đắc ý",
                MoodEmoji = "🥳",
                EnergyScore = 9,
                JoyTriggers = "Nhận điểm kiểm tra cao và đi cafe với bạn thân",
                StressTriggers = "Không có gì đáng kể",
                Note = "Hôm nay vui nổ trời luôn á!",
                AiAnalysis = "Các hoạt động giao tiếp và kết quả học tập tốt đã đẩy mood lên cực đại."
            },
            new MoodEntry
            {
                Date = DateTime.Today.AddDays(-1),
                MoodScore = 6,
                MoodLabel = "Hơi mệt & Cần nghỉ ngơi",
                MoodEmoji = "🥱",
                EnergyScore = 5,
                JoyTriggers = "Xem xong một tập phim hài",
                StressTriggers = "Ngủ hơi muộn đêm qua",
                Note = "Cần đi ngủ sớm để lấy lại phong độ.",
                AiAnalysis = "Giấc ngủ ngắn đêm trước ảnh hưởng trực tiếp đến mức năng lượng hôm nay."
            },
            new MoodEntry
            {
                Date = DateTime.Today,
                MoodScore = 9,
                MoodLabel = "Tự tin & Tràn đầy cảm hứng",
                MoodEmoji = "✨",
                EnergyScore = 8,
                JoyTriggers = "Xây dựng xong dự án GLOW cực xịn và xinh xắn!",
                StressTriggers = "Không có",
                Note = "Cảm giác hoàn thành một sản phẩm tâm huyết thật tuyệt vời!",
                AiAnalysis = "Cảm giác thành tựu giúp nâng cao mood và sự tự tin mạnh mẽ."
            }
        );

        // 8. Moments
        context.Moments.AddRange(
            new MomentItem
            {
                Title = "Buổi chiều nắng đẹp ở quán cafe quen",
                Caption = "Góc bàn yêu thích, ly matcha latte và chiếc laptop để hiện thực hóa ước mơ 💻☕",
                ImageUrl = "https://images.unsplash.com/photo-1501339847302-ac426a4a7cbb?auto=format&fit=crop&w=600&q=80",
                LocationName = "Quán Cafe Góc Phố Xanh",
                Latitude = 10.7769,
                Longitude = 106.7009,
                MoodTag = "✨ Cảm hứng dồi dào",
                Tags = "#study, #cafe, #cozy, #matcha",
                Timestamp = DateTime.Today.AddHours(15).AddMinutes(30)
            },
            new MomentItem
            {
                Title = "Khoảnh khắc hoàng hôn rực rỡ bên bờ sông",
                Caption = "Mỗi khi nhìn hoàng hôn, mọi muộn phiền đều tan biến hết trơn 🌅",
                ImageUrl = "https://images.unsplash.com/photo-1507525428034-b723cf961d3e?auto=format&fit=crop&w=600&q=80",
                LocationName = "Công viên Bến Bạch Đằng",
                Latitude = 10.7725,
                Longitude = 106.7058,
                MoodTag = "🥰 Bình yên & Biết ơn",
                Tags = "#sunset, #peaceful, #walk",
                Timestamp = DateTime.Today.AddDays(-1).AddHours(17).AddMinutes(45)
            },
            new MomentItem
            {
                Title = "Bữa ăn healthy nhiều màu sắc",
                Caption = "Tự thưởng cho bản thân một bữa ăn thật xinh xắn và đủ chất 🥗🥑",
                ImageUrl = "https://images.unsplash.com/photo-1512621776951-a57141f2eefd?auto=format&fit=crop&w=600&q=80",
                LocationName = "Bếp Nhà Ấm Áp",
                Latitude = 10.7800,
                Longitude = 106.6900,
                MoodTag = "😋 Hạnh phúc & No nê",
                Tags = "#healthyfood, #selfcare, #delicious",
                Timestamp = DateTime.Today.AddDays(-2).AddHours(12).AddMinutes(0)
            }
        );

        // 9. Map Memories
        context.MapMemories.AddRange(
            new MapMemory
            {
                PlaceName = "Trường Đại học Bách Khoa / KHTN",
                Category = "Trường học",
                Latitude = 10.7720,
                Longitude = 106.6578,
                ArrivedAt = DateTime.Today.AddHours(7).AddMinutes(45),
                LeftAt = DateTime.Today.AddHours(11).AddMinutes(30),
                DurationMinutes = 225,
                Activity = "Học môn Lập trình Web & Thực hành Lab",
                Feeling = "Tập trung và tiếp thu được nhiều kiến thức bổ ích",
                PhotoUrl = "https://images.unsplash.com/photo-1523050854058-8df90110c9f1?auto=format&fit=crop&w=400&q=80"
            },
            new MapMemory
            {
                PlaceName = "The Coffee House & Tea Lounge",
                Category = "Cafe",
                Latitude = 10.7769,
                Longitude = 106.7009,
                ArrivedAt = DateTime.Today.AddHours(14).AddMinutes(0),
                LeftAt = DateTime.Today.AddHours(17).AddMinutes(0),
                DurationMinutes = 180,
                Activity = "Code tính năng cho GLOW và đọc tài liệu",
                Feeling = "Rất nhiều cảm hứng sáng tạo",
                PhotoUrl = "https://images.unsplash.com/photo-1554118811-1e0d58224f24?auto=format&fit=crop&w=400&q=80"
            },
            new MapMemory
            {
                PlaceName = "Công viên Cây Xanh Trung Tâm",
                Category = "Công viên",
                Latitude = 10.7745,
                Longitude = 106.6950,
                ArrivedAt = DateTime.Today.AddHours(17).AddMinutes(30),
                LeftAt = DateTime.Today.AddHours(18).AddMinutes(30),
                DurationMinutes = 60,
                Activity = "Đi bộ dạo mát 6000 bước và nghe podcast",
                Feeling = "Thư giãn đầu óc, sảng khoái",
                PhotoUrl = "https://images.unsplash.com/photo-1476514525535-07fb3b4ae5f1?auto=format&fit=crop&w=400&q=80"
            }
        );

        // 10. Diary Records
        context.DiaryRecords.Add(new DiaryRecord
        {
            Date = DateTime.Today,
            Title = "Bản giao hưởng của năng lượng và sự sáng tạo 🌈",
            Summary = "Hôm nay là một ngày tuyệt vời khi bạn cân bằng được giữa học tập, vận động và thư giãn cá nhân.",
            GoodThings = "• Hoàn thành xuất sắc bài tập C# và thiết kế giao diện Bento Grid.\n• Uống đủ gần 2 lít nước và đạt 6.800 bước chân.\n• Thưởng thức ly matcha thơm ngon trong lúc học tập.",
            NotGoodThings = "• Buổi chiều mắt hơi mỏi do nhìn màn hình liên tục 2 tiếng không nghỉ ngơi kịp thời.",
            LessonsLearned = "• Bật chế độ nghỉ mắt 20-20-20 giúp duy trì độ tập trung bền bỉ hơn rất nhiều.\n• Uống đủ nước làm tinh thần luôn tỉnh táo.",
            MemorableMoments = "• Khoảnh khắc chạy thử nghiệm thành công giao diện GLOW rực rỡ sắc màu.\n• Hoàng hôn mát mẻ ở công viên lúc tan giờ học.",
            Improvements = "• Tối nay nên tắt điện thoại trước 23:00 để duy trì chất lượng giấc ngủ 8 tiếng.",
            TomorrowSuggestions = "• Ưu tiên kiểm tra lại các API kết nối.\n• Mặc trang phục màu Mint Green năng động để thu hút may mắn và sự tươi mới!",
            IsGeneratedByAi = true,
            MoodEmoji = "✨"
        });

        // 11. AI Chat Messages
        context.AiChatMessages.AddRange(
            new AiChatMessage { Sender = "Assistant", Message = "Chào Bảo Trân nha! 🌟 Hôm nay của bạn diễn ra thế nào rồi nè? Mình đã sẵn sàng lắng nghe và đồng hành cùng bạn đây!", Timestamp = DateTime.Today.AddHours(8) },
            new AiChatMessage { Sender = "User", Message = "Hôm nay tui thấy tràn đầy năng lượng luôn ó! Tui vừa hoàn thành xong bài tập lập trình nè.", Timestamp = DateTime.Today.AddHours(16) },
            new AiChatMessage { Sender = "Assistant", Message = "Chúc mừng bạn nha! 🎉 Mình thấy bạn đã dành 75 phút học tập chất lượng cao và tâm trạng đang ở mức 9/10 (Cực kỳ hứng khởi). Bạn nhớ nghỉ mắt và uống thêm một ngụm nước để giữ vững năng lượng này tới tối nha!", Timestamp = DateTime.Today.AddHours(16).AddMinutes(1), ContextReferenced = "Study, Health, Mood" }
        );

        // 12. Family Stories
        context.FamilyStories.AddRange(
            new FamilyStory
            {
                Title = "Bát canh chua mẹ nấu ngày trở về",
                Content = "Mỗi lần về nhà, dù bận đến mấy mẹ cũng luôn nấu món canh chua cá lóc và thịt kho trứng mà mình thích nhất. Cảm giác ngồi quanh mâm cơm gia đình ấm áp không gì sánh bằng.",
                Feelings = "Ấm lòng, biết ơn và cảm thấy luôn có một nơi bình yên để trở về.",
                UnsaidWords = "Con muốn nói lời cảm ơn ba mẹ thật nhiều vì đã luôn âm thầm hy sinh và ủng hộ mọi quyết định của con.",
                Date = DateTime.Today.AddDays(-6),
                PhotoUrl = "https://images.unsplash.com/photo-1547592180-85f173990554?auto=format&fit=crop&w=600&q=80",
                AiReflection = "Tình cảm gia đình luôn là nguồn năng lượng chữa lành mạnh mẽ nhất. Hãy gửi một tin nhắn hỏi thăm mẹ nhé, một lời chúc nhỏ cũng làm mẹ vui cả ngày đó!"
            },
            new FamilyStory
            {
                Title = "Lời dặn dò bình dị của Ba",
                Content = "Ba ít khi nói những lời hoa mỹ, nhưng sáng nào ba cũng kiểm tra lốp xe và nhắc mình chạy xe cẩn thận, nhớ ăn uống đầy đủ đừng bỏ bữa.",
                Feelings = "Cảm nhận được tình thương sâu sắc qua từng hành động nhỏ.",
                UnsaidWords = "Ba ơi, con lớn rồi và con sẽ luôn cố gắng sống thật tốt để ba tự hào về con.",
                Date = DateTime.Today.AddDays(-12),
                PhotoUrl = "https://images.unsplash.com/photo-1511895426328-dc8714191300?auto=format&fit=crop&w=600&q=80",
                AiReflection = "Tình thương của người cha thường trầm lắng như ngọn núi. Bạn đang làm rất tốt và chắc chắn ba rất tự hào về bạn!"
            }
        );

        // 13. Wardrobe Items
        context.WardrobeItems.AddRange(
            new WardrobeItem { Name = "Áo Thun Oversize Mint Green", Category = "Áo", Color = "Mint Green", Style = "Casual Gen Z, Năng động", Pattern = "Trơn Minimalist", ImageUrl = "https://images.unsplash.com/photo-1521572267360-ee0c2909d518?auto=format&fit=crop&w=400&q=80", UsageCount = 12, Notes = "Chất cotton mát mịn, mặc đi học siêu thoải mái" },
            new WardrobeItem { Name = "Quần Jean Ống Rộng Màu Kem", Category = "Quần", Color = "Trắng kem / Beige", Style = "Streetwear Hàn Quốc", Pattern = "Trơn", ImageUrl = "https://images.unsplash.com/photo-1541099649105-f69ad21f3246?auto=format&fit=crop&w=400&q=80", UsageCount = 9, Notes = "Hack dáng cực đỉnh, phối với áo nào cũng đẹp" },
            new WardrobeItem { Name = "Áo Blazer Pastel Lavender", Category = "Áo khoác", Color = "Lavender", Style = "Chic, Thanh lịch", Pattern = "Trơn", ImageUrl = "https://images.unsplash.com/photo-1591047139829-d91aecb6caea?auto=format&fit=crop&w=400&q=80", UsageCount = 5, Notes = "Mặc khi thuyết trình hoặc tham gia sự kiện trang trọng" },
            new WardrobeItem { Name = "Giày Sneaker Trắng Đế Chunky", Category = "Giày", Color = "Trắng / Lime Accent", Style = "Sporty Chic", Pattern = "Phối màu nhẹ", ImageUrl = "https://images.unsplash.com/photo-1595950653106-6c9ebd614d3a?auto=format&fit=crop&w=400&q=80", UsageCount = 20, Notes = "Đôi giày quốc dân đi êm chân cả ngày" }
        );

        // 14. Tarot Cards & Seed
        var tarotCards = new List<TarotCard>
        {
            new TarotCard {
                Id = 1,
                NameVi = "0 - Kẻ Khờ (The Fool)",
                NameEn = "The Fool",
                ImageUrl = "https://images.unsplash.com/photo-1518709268805-4e9042af9f23?auto=format&fit=crop&w=400&q=80",
                Element = "Khí",
                Keywords = "Khởi đầu mới, tự do, phiêu lưu, niềm tin ngây thơ",
                UprightMeaning = "Một chương mới tràn ngập cơ hội đang mở ra. Hãy can đảm bước tới với trái tim cởi mở và tinh thần không ngại thử thách.",
                LoveMeaning = "Một tình cảm mới mẻ, lãng mạn và đầy bất ngờ đang đón chờ bạn.",
                StudyWorkMeaning = "Đừng ngại bắt đầu một dự án mới hoặc học một kỹ năng chưa từng thử.",
                Affirmation = "Tôi luôn sẵn sàng đón nhận những khởi đầu kỳ diệu của cuộc sống ✨"
            },
            new TarotCard {
                Id = 2,
                NameVi = "I - Nhà Ảo Thuật (The Magician)",
                NameEn = "The Magician",
                ImageUrl = "https://images.unsplash.com/photo-1509198397868-475647b2a1e5?auto=format&fit=crop&w=400&q=80",
                Element = "Khí",
                Keywords = "Tài năng, sự tập trung, biến ước mơ thành hiện thực",
                UprightMeaning = "Bạn có đầy đủ mọi nguồn lực, kỹ năng và trí tuệ để kiến tạo điều mình mong muốn. Đây là lúc hành động dứt khoát!",
                LoveMeaning = "Sự tự tin và cuốn hút tự nhiên của bạn đang tỏa sáng rực rỡ.",
                StudyWorkMeaning = "Tập trung cao độ sẽ giúp bạn hoàn thành xuất sắc mọi mục tiêu học tập và công việc.",
                Affirmation = "Tôi có sức mạnh và trí tuệ để biến mọi ý tưởng thành hiện thực 🔥"
            },
            new TarotCard {
                Id = 3,
                NameVi = "XVII - Ngôi Sao (The Star)",
                NameEn = "The Star",
                ImageUrl = "https://images.unsplash.com/photo-1519681393784-d120267933ba?auto=format&fit=crop&w=400&q=80",
                Element = "Khí / Nước",
                Keywords = "Hy vọng, niềm tin, cảm hứng, sự chữa lành",
                UprightMeaning = "Sau những cơn mưa, bầu trời lại lấp lánh những vì sao hy vọng. Bạn đang bước vào giai đoạn bình yên, thảnh thơi và tràn đầy cảm hứng.",
                LoveMeaning = "Tình yêu chan hòa, thấu hiểu và cùng nhau hướng tới tương lai tốt đẹp.",
                StudyWorkMeaning = "Ý tưởng sáng tạo tuôn trào, con đường học vấn rộng mở và sáng tỏ.",
                Affirmation = "Tôi tin tưởng vào hành trình của chính mình và luôn tỏa sáng rực rỡ 🌟"
            },
            new TarotCard {
                Id = 4,
                NameVi = "XIX - Mặt Trời (The Sun)",
                NameEn = "The Sun",
                ImageUrl = "https://images.unsplash.com/photo-1470240731273-7821a6eeb6bd?auto=format&fit=crop&w=400&q=80",
                Element = "Lửa",
                Keywords = "Thành công, niềm vui, sự ấm áp, tích cực rạng rỡ",
                UprightMeaning = "Lá bài mang năng lượng tích cực bậc nhất! Thành công, sự công nhận và niềm hạnh phúc thuần khiết đang bao quanh bạn.",
                LoveMeaning = "Tình cảm chân thành, ấm áp và ngập tràn tiếng cười hạnh phúc.",
                StudyWorkMeaning = "Đạt kết quả xuất sắc trong học tập, dự án hoàn thành mỹ mãn.",
                Affirmation = "Ánh sáng và niềm vui luôn tràn ngập trong mỗi bước đi của tôi ☀️"
            },
            new TarotCard {
                Id = 5,
                NameVi = "III - Nữ Hoàng (The Empress)",
                NameEn = "The Empress",
                ImageUrl = "https://images.unsplash.com/photo-1490481651871-ab68de25d43d?auto=format&fit=crop&w=400&q=80",
                Element = "Đất",
                Keywords = "Nuôi dưỡng, phong phú, yêu thương bản thân, sắc đẹp",
                UprightMeaning = "Hãy chăm sóc bản thân thật chu đáo, đón nhận sự trù phú và để vẻ đẹp tâm hồn cùng sự sáng tạo được nở rộ.",
                LoveMeaning = "Tình cảm ngọt ngào, dịu dàng và được yêu thương, chiều chuộng.",
                StudyWorkMeaning = "Công việc và học tập gặt hái nhiều trái ngọt sau thời gian chăm chỉ gieo trồng.",
                Affirmation = "Tôi yêu thương và trân trọng cơ thể cũng như tâm hồn của mình 🌸"
            }
        };
        context.TarotCards.AddRange(tarotCards);

        // 15. Tomorrow Plan
        context.TomorrowPlans.Add(new TomorrowPlan
        {
            PlanForDate = DateTime.Today.AddDays(1),
            PriorityStudy = "Luyện tập 1 đề Speaking Part 2 và đọc trước chương mới PRG301",
            PriorityWork = "Kiểm tra responsive trên thiết bị thực tế",
            SuggestedOutfit = "Áo thun Mint Green + Quần Jean ống rộng màu kem + Sneaker trắng năng động",
            OutfitLuckyColor = "Mint Green & Cream White (Thu hút năng lượng tươi mới)",
            NutritionSuggestion = "Bữa sáng: Yến mạch sữa hạt chuối; Bữa trưa: Salad cá ngừ; Bữa tối: Canh sườn hầm rau củ",
            SelfCareReminders = "💧 Uống đủ 2L nước • 👀 Nghỉ mắt mỗi 30 phút • 😴 Đi ngủ trước 23:00",
            PositiveMessage = "Ngày mai là một trang giấy mới tinh khôi, hãy tô điểm nó bằng nụ cười và sự tự tin rạng rỡ của bạn nhé! ✨🌈"
        });

        // 16. Time Capsule
        context.TimeCapsules.AddRange(
            new TimeCapsuleItem
            {
                Title = "Thư gửi Bảo Trân của 1 năm sau 💌",
                MessageToFutureSelf = "Chào tôi của tương lai! Bạn còn nhớ những ngày tháng miệt mài gõ code, vừa uống matcha vừa mơ ước xây dựng những ứng dụng tuyệt vời cho mọi người chứ? Lúc này bạn đã tốt nghiệp xuất sắc và có công việc mơ ước chưa? Hãy luôn giữ ngọn lửa đam mê và nụ cười rạng rỡ này nhé!",
                CurrentGoals = "1. Tốt nghiệp loại Giỏi\n2. Đạt IELTS 7.5\n3. Ra mắt ứng dụng GLOW được hàng ngàn bạn trẻ đón nhận\n4. Đi du lịch Đà Lạt cùng bạn thân",
                CurrentFeelings = "Đang tràn đầy hoài bão, một chút hồi hộp nhưng tự tin 100% vào năng lực của bản thân!",
                PhotoUrl = "https://images.unsplash.com/photo-1529156069898-49953e39b3ac?auto=format&fit=crop&w=600&q=80",
                SealedDate = DateTime.Today.AddDays(-30),
                UnlockDate = DateTime.Today.AddYears(1),
                IsOpened = false
            },
            new TimeCapsuleItem
            {
                Title = "Khoảnh khắc khai trương dự án GLOW 🌟",
                MessageToFutureSelf = "Dự án GLOW chính thức được thành hình! Đây là minh chứng cho sự kiên trì và sáng tạo không ngừng nghỉ.",
                CurrentGoals = "Tiếp tục cải tiến và phát triển tính năng AI hiểu bản thân sâu sắc hơn nữa.",
                CurrentFeelings = "Tự hào và hạnh phúc vô cùng!",
                PhotoUrl = "https://images.unsplash.com/photo-1492684223066-81342ee5ff30?auto=format&fit=crop&w=600&q=80",
                SealedDate = DateTime.Today,
                UnlockDate = DateTime.Today.AddMonths(6),
                IsOpened = false
            }
        );

        context.SaveChanges();
    }
}
