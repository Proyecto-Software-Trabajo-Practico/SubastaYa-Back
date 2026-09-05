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
    public class AuditoriaRepository : IAuditoriaRepository
    {
        private readonly AppDbContext _context;

        public AuditoriaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Auditoria?> GetByIdAsync(int id)
        {
            return await _context.Auditorias.FindAsync(id);
        }

        public async Task<IEnumerable<Auditoria>> GetByEntidadAsync(string entidad, int entidadId)
        {
            return await _context.Auditorias
                .AsNoTracking()
                .Where(a => a.Entidad == entidad && a.EntidadId == entidadId)
                .OrderByDescending(a => a.Fecha)
                .ToListAsync();
        }

        public async Task AddAsync(Auditoria auditoria)
        {
            await _context.Auditorias.AddAsync(auditoria);
        }
    }
}
