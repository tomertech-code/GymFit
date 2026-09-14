using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymFit.Application.Interfaces;
using GymFit.Domain.Entities;
using GymFit.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GymFit.Infrastructure.Repositories
{

    public class TrainerRepository : GenericRepository<Trainer>, ITrainerRepository
    {
        public TrainerRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Trainer?> GetTrainerByUserIdAsync(string userId)
        {
            return await _context.Trainers
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.UserId == userId);
        }

        public async Task<IEnumerable<Trainer>> GetActiveTrainersAsync()
        {
            return await _context.Trainers
                .Include(t => t.User)
                .Where(t => t.IsActive)
                .ToListAsync();
        }
    }
}
