using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class PujaRepository : IPujaRepository
    {
        private readonly AppDbContext _context;

        public PujaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Puja?> GetByIdAsync(int id)
        {
            return await _context.Pujas.FindAsync(id);
        }

        public async Task<IEnumerable<Puja>> GetBySubastaIdAsync(int subastaId)
        {
            return await _context.Pujas
                .AsNoTracking()
                .Where(p => p.SubastaId == subastaId)
                .OrderByDescending(p => p.FechaPuja)
                .ToListAsync();
        }

        public async Task<Puja?> GetPujaMasAltaBySubastaIdAsync(int subastaId)
        {
            return await _context.Pujas
                .AsNoTracking()
                .Where(p => p.SubastaId == subastaId)
                .OrderByDescending(p => p.Monto)
                .FirstOrDefaultAsync();
        }

        public async Task AddAsync(Puja puja)
        {
            await _context.Pujas.AddAsync(puja);
        }
    }
}
