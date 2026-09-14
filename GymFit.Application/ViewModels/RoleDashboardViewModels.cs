using GymFit.Application.DTOs;
using GymFit.Domain.Entities;

namespace GymFit.Application.ViewModels;

public class ReceptionDashboardViewModel
{
    public int TodayCheckIns { get; init; }
    public int ActiveMembers { get; init; }
    public int ExpiredMemberships { get; init; }
    public int NewEnquiries { get; init; }
    public int TodayPayments { get; init; }
    public int ExpiringSoon { get; init; }
}

public class TrainerDashboardViewModel
{
    public int AssignedMemberCount { get; init; }
    public int TodayAttendance { get; init; }
    public IReadOnlyList<MemberDto> AssignedMembers { get; init; } = Array.Empty<MemberDto>();
}
