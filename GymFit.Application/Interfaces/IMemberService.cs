using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymFit.Application.DTOs;
using GymFit.Application.ViewModels;

namespace GymFit.Application.Interfaces
{

    public interface IMemberService
    {
        Task<IEnumerable<MemberDto>> GetAllMembersAsync();
        Task<MemberDto?> GetMemberByIdAsync(int id);
        Task<MemberDto?> GetMemberByUserIdAsync(string userId);
        Task<bool> CreateMemberAsync(MemberViewModel model);
        Task<bool> UpdateMemberAsync(MemberViewModel model);
        Task<bool> DeleteMemberAsync(int id);
        Task<int> GetActiveMembersCountAsync();
    }
}
