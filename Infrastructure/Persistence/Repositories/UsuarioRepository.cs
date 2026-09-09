using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{   
    public class UsuarioRepository : IUsuarioRepository
    { // Creo que este repositorio quedó obsoleto, ya que IdentityUser maneja la persistencia de usuarios, pero lo dejo por si acaso
        private readonly AppDbContext _context;

        public UsuarioRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<Usuario?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<Usuario>> GetAllAsync()
        {
            return await _context.Users
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddAsync(Usuario usuario)
        {
            await _context.Users.AddAsync(usuario);
        }

        public Task UpdateAsync(Usuario usuario)
        {
            _context.Users.Update(usuario);
            return Task.CompletedTask;
        }
    }
}
