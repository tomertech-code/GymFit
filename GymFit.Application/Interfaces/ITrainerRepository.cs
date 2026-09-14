using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymFit.Domain.Entities;

namespace GymFit.Application.Interfaces
{

    public interface ITrainerRepository : IGenericRepository<Trainer>
    {
        Task<Trainer?> GetTrainerByUserIdAsync(string userId);
        Task<IEnumerable<Trainer>> GetActiveTrainersAsync();
    }

}
