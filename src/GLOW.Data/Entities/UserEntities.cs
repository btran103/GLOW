namespace GLOW.Data.Entities;

public class ApplicationUser
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Salt { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = "https://images.unsplash.com/photo-1534528741775-53994a69daeb?auto=format&fit=crop&w=400&q=80";
    public bool IsEmailConfirmed { get; set; } = false;
    public string? EmailVerificationCode { get; set; }
    public DateTime? VerificationCodeExpiresAt { get; set; }
    public string? ResetPasswordCode { get; set; }
    public DateTime? ResetPasswordExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

public class UserProfile
{
    public int Id { get; set; }
    public string Username { get; set; } = "Bao Tran";
    public string Nickname { get; set; } = "Bao";
    public string AvatarUrl { get; set; } = "/images/default-avatar.png";
    public string Bio { get; set; } = "Học tập & Tận hưởng từng khoảnh khắc cuộc sống ✨";
    public string PreferredStyle { get; set; } = "Gen Z, Streetwear, Minimalist Pastel";
    public int DailyWaterGoalMl { get; set; } = 2000;
    public int DailyStudyGoalMinutes { get; set; } = 180;
    public int DailySleepGoalHours { get; set; } = 8;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

public class UserPrivacySetting
{
    public int Id { get; set; }
    public int UserId { get; set; } = 1;
    public bool LocationEnabled { get; set; } = true;
    public bool AllowAiDataAnalysis { get; set; } = true;
    public bool FamilyStoriesPrivate { get; set; } = true;
    public bool CameraHistoryLocalOnly { get; set; } = true;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
