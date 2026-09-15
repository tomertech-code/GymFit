namespace GymFit.Domain.Entities;

public class ProgressRecord
{
    public int Id { get; set; }
    public int MemberId { get; set; }
    public DateTime RecordDate { get; set; } = DateTime.UtcNow.Date;
    public decimal? WeightKg { get; set; }
    public decimal? BodyFatPercentage { get; set; }
    public decimal? MuscleMassKg { get; set; }
    public decimal? StrengthScore { get; set; }
    public string? Notes { get; set; }
    public virtual Member Member { get; set; } = null!;
}
