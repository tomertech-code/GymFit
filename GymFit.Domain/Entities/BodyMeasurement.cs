namespace GymFit.Domain.Entities;

public class BodyMeasurement
{
    public int Id { get; set; }
    public int MemberId { get; set; }
    public DateTime MeasurementDate { get; set; } = DateTime.UtcNow.Date;
    public decimal? ChestCm { get; set; }
    public decimal? WaistCm { get; set; }
    public decimal? HipsCm { get; set; }
    public decimal? LeftArmCm { get; set; }
    public decimal? RightArmCm { get; set; }
    public decimal? LeftThighCm { get; set; }
    public decimal? RightThighCm { get; set; }
    public string? Notes { get; set; }
    public virtual Member Member { get; set; } = null!;
}
