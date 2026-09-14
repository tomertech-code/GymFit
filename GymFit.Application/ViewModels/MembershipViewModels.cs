using System.ComponentModel.DataAnnotations;

namespace GymFit.Application.ViewModels;

public class MembershipAssignmentViewModel
{
    [Required]
    public int MemberId { get; set; }

    [Required]
    public int MembershipPlanId { get; set; }

    [Required]
    public DateTime StartDate { get; set; } = DateTime.UtcNow.Date;
}
