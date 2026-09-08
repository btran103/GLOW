namespace GLOW.Data.Entities;

public class WorkTask
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ProjectName { get; set; } = "Dự án cá nhân";
    public string Description { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
    public string Priority { get; set; } = "High"; // Low, Medium, High, Urgent
    public string Status { get; set; } = "In Progress"; // Todo, In Progress, Review, Done
    public int WorkMinutesLogged { get; set; } = 0;
    public string Tags { get; set; } = "#dev, #frontend";
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? CompletedAt { get; set; }
}

public class ExpenseItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Type { get; set; } = "Chi tiêu"; // Chi tiêu, Thu nhập
    public string Category { get; set; } = "Ăn uống"; // Ăn uống, Học tập, Đi lại, Mua sắm, Giải trí, Sức khỏe, Khác
    public string PaymentMethod { get; set; } = "Chuyển khoản / Ví điện tử";
    public DateTime Date { get; set; } = DateTime.Now;
    public string Notes { get; set; } = string.Empty;
    public string LocationName { get; set; } = string.Empty;
}
