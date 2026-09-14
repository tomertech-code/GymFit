using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymFit.Domain.Entities;

namespace GymFit.Application.Interfaces
{

    public interface IMemberRepository : IGenericRepository<Member>
    {
        Task<Member?> GetMemberWithDetailsAsync(int id);
        Task<IEnumerable<Member>> GetActiveMembersAsync();
        Task<Member?> GetMemberByUserIdAsync(string userId);
    }

}
