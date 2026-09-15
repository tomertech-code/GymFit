using System.ComponentModel.DataAnnotations;

namespace GymFit.Application.ViewModels;

public class DietPlanCreateViewModel
{
    [Required] public int MemberId { get; set; }
    public int? TrainerId { get; set; }
    [Required, StringLength(150)] public string Name { get; set; } = string.Empty;
    [Required, StringLength(100)] public string Goal { get; set; } = string.Empty;
    [Range(1, 10000)] public int DailyCalories { get; set; }
    [Range(0, 1000)] public int ProteinGrams { get; set; }
    [Range(0, 1000)] public int CarbsGrams { get; set; }
    [Range(0, 1000)] public int FatGrams { get; set; }
    [DataType(DataType.Date)] public DateTime StartDate { get; set; } = DateTime.UtcNow.Date;
    [DataType(DataType.Date)] public DateTime? EndDate { get; set; }
}
public class DietMealCreateViewModel
{
    [Required] public int DietPlanId { get; set; }
    [Range(1, 20)] public int MealOrder { get; set; }
    [Required, StringLength(60)] public string MealType { get; set; } = string.Empty;
    [Required, StringLength(1000)] public string FoodItems { get; set; } = string.Empty;
    [StringLength(500)] public string? Notes { get; set; }
    [Range(0, 10000)] public int? Calories { get; set; }
}
public class ProgressRecordViewModel
{
    [Required] public int MemberId { get; set; }
    [DataType(DataType.Date)] public DateTime RecordDate { get; set; } = DateTime.UtcNow.Date;
    [Range(1, 500)] public decimal? WeightKg { get; set; }
    [Range(0, 100)] public decimal? BodyFatPercentage { get; set; }
    [Range(1, 500)] public decimal? MuscleMassKg { get; set; }
    [Range(0, 100000)] public decimal? StrengthScore { get; set; }
    [StringLength(1000)] public string? Notes { get; set; }
}
public class BodyMeasurementViewModel
{
    [Required] public int MemberId { get; set; }
    [DataType(DataType.Date)] public DateTime MeasurementDate { get; set; } = DateTime.UtcNow.Date;
    [Range(1, 300)] public decimal? ChestCm { get; set; }
    [Range(1, 300)] public decimal? WaistCm { get; set; }
    [Range(1, 300)] public decimal? HipsCm { get; set; }
    [Range(1, 150)] public decimal? LeftArmCm { get; set; }
    [Range(1, 150)] public decimal? RightArmCm { get; set; }
    [Range(1, 200)] public decimal? LeftThighCm { get; set; }
    [Range(1, 200)] public decimal? RightThighCm { get; set; }
    [StringLength(1000)] public string? Notes { get; set; }
}
public class EquipmentViewModel
{
    [Required] public int BranchId { get; set; }
    [Required, StringLength(150)] public string EquipmentName { get; set; } = string.Empty;
    [Required, StringLength(80)] public string Category { get; set; } = string.Empty;
    [Range(1, 100000)] public int Quantity { get; set; } = 1;
    [StringLength(100)] public string? Brand { get; set; }
    [DataType(DataType.Date)] public DateTime? PurchaseDate { get; set; }
    [StringLength(200)] public string? MaintenanceSchedule { get; set; }
    public bool IsWorking { get; set; } = true;
}
