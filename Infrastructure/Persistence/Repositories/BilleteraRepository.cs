using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class BilleteraRepository : IBilleteraRepository
{
    private readonly AppDbContext _context;

    public BilleteraRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Billetera?> GetByIdAsync(int id)
    {
        return await _context.Billeteras.FindAsync(id);
    }

    // Buscamos la billetera asociada a un usuario específico
    public async Task<Billetera?> GetByUsuarioIdAsync(int usuarioId)
    {
        return await _context.Billeteras
            .FirstOrDefaultAsync(b => b.UsuarioId == usuarioId);
    }

    public async Task AddAsync(Billetera billetera)
    {
        await _context.Billeteras.AddAsync(billetera);
    }

    public void Update(Billetera billetera)
    {
        _context.Billeteras.Update(billetera);
    }
}