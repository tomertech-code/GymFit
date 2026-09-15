using System.ComponentModel.DataAnnotations;

namespace GymFit.Application.ViewModels;

public class WorkoutPlanViewModel
{
    public int Id { get; set; }
    [Required, StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public int MemberId { get; set; }

    [Required]
    public DateTime StartDate { get; set; } = DateTime.UtcNow.Date;

    public DateTime? EndDate { get; set; }
}
