namespace GLOW.Data.Entities;

public class StudyCourse
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Instructor { get; set; } = string.Empty;
    public string Room { get; set; } = string.Empty;
    public string ColorHex { get; set; } = "#A7F3D0";
    public string ScheduleDayOfWeek { get; set; } = "Thứ 2, Thứ 4";
    public string TimeSlot { get; set; } = "08:00 - 10:30";
    public int Credits { get; set; } = 3;
    public List<StudyTask> Tasks { get; set; } = new();
}

public class StudyTask
{
    public int Id { get; set; }
    public int? CourseId { get; set; }
    public StudyCourse? Course { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
    public int EstimatedMinutes { get; set; } = 60;
    public int CompletedMinutes { get; set; } = 0;
    public bool IsCompleted { get; set; } = false;
    public string Priority { get; set; } = "Medium"; // Low, Medium, High
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

public class StudySession
{
    public int Id { get; set; }
    public int? TaskId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public DateTime SessionDate { get; set; } = DateTime.Now;
    public string FocusQuality { get; set; } = "Tuyệt vời 🌟"; // Thấp, Bình thường, Tốt, Tuyệt vời
    public string Notes { get; set; } = string.Empty;
}
