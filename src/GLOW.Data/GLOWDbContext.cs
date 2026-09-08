using Microsoft.EntityFrameworkCore;
using GLOW.Data.Entities;

namespace GLOW.Data;

public class GLOWDbContext : DbContext
{
    public GLOWDbContext(DbContextOptions<GLOWDbContext> options) : base(options)
    {
    }

    public DbSet<ApplicationUser> Users => Set<ApplicationUser>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<UserPrivacySetting> PrivacySettings => Set<UserPrivacySetting>();

    public DbSet<StudyCourse> StudyCourses => Set<StudyCourse>();
    public DbSet<StudyTask> StudyTasks => Set<StudyTask>();
    public DbSet<StudySession> StudySessions => Set<StudySession>();

    public DbSet<WorkTask> WorkTasks => Set<WorkTask>();
    public DbSet<ExpenseItem> Expenses => Set<ExpenseItem>();

    public DbSet<HealthLog> HealthLogs => Set<HealthLog>();
    public DbSet<WaterLog> WaterLogs => Set<WaterLog>();
    public DbSet<HealthReminder> HealthReminders => Set<HealthReminder>();

    public DbSet<NutritionEntry> NutritionEntries => Set<NutritionEntry>();
    public DbSet<MoodEntry> MoodEntries => Set<MoodEntry>();
    public DbSet<MomentItem> Moments => Set<MomentItem>();
    public DbSet<MapMemory> MapMemories => Set<MapMemory>();

    public DbSet<DiaryRecord> DiaryRecords => Set<DiaryRecord>();
    public DbSet<AiChatMessage> AiChatMessages => Set<AiChatMessage>();
    public DbSet<FamilyStory> FamilyStories => Set<FamilyStory>();

    public DbSet<WardrobeItem> WardrobeItems => Set<WardrobeItem>();
    public DbSet<StyleAdvice> StyleAdvices => Set<StyleAdvice>();

    public DbSet<TarotCard> TarotCards => Set<TarotCard>();
    public DbSet<TarotReading> TarotReadings => Set<TarotReading>();

    public DbSet<TomorrowPlan> TomorrowPlans => Set<TomorrowPlan>();
    public DbSet<TimeCapsuleItem> TimeCapsules => Set<TimeCapsuleItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure relations and defaults
        modelBuilder.Entity<StudyCourse>()
            .HasMany(c => c.Tasks)
            .WithOne(t => t.Course)
            .HasForeignKey(t => t.CourseId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<ExpenseItem>()
            .Property(e => e.Amount)
            .HasColumnType("decimal(18,2)");
    }
}
