namespace GymFit.Domain.Entities;

public class DietPlan
{
    public int Id { get; set; }
    public int MemberId { get; set; }
    public int? TrainerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Goal { get; set; } = string.Empty;
    public int DailyCalories { get; set; }
    public int ProteinGrams { get; set; }
    public int CarbsGrams { get; set; }
    public int FatGrams { get; set; }
    public DateTime StartDate { get; set; } = DateTime.UtcNow.Date;
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public virtual Member Member { get; set; } = null!;
    public virtual Trainer? Trainer { get; set; }
    public virtual ICollection<DietMeal> Meals { get; set; } = new List<DietMeal>();
}

public class DietMeal
{
    public int Id { get; set; }
    public int DietPlanId { get; set; }
    public int MealOrder { get; set; }
    public string MealType { get; set; } = string.Empty;
    public string FoodItems { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public int? Calories { get; set; }
    public virtual DietPlan DietPlan { get; set; } = null!;
}
