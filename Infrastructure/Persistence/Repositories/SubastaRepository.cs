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

public class SubastaRepository : ISubastaRepository
{
    private readonly AppDbContext _context;

    public SubastaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Subasta?> GetByIdAsync(int id)
    {
        return await _context.Subastas.FindAsync(id);
    }

    // Solo lectura para catálogos o reportes
    public async Task<IReadOnlyList<Subasta>> GetAllAsync()
    {
        return await _context.Subastas
            .AsNoTracking()
            .ToListAsync();
    }

    // Rastreado: necesitamos agregarle pujas o extender tiempo
    public async Task<Subasta?> GetWithPujasByIdAsync(int id)
    {
        return await _context.Subastas
            .Include(s => s.Pujas)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    // Solo lectura para el catálogo de subastas en curso del Frontend
    public async Task<IReadOnlyList<Subasta>> GetSubastasActivasAsync()
    {
        return await _context.Subastas
            .AsNoTracking()
            .Where(s => s.Estado == "ACTIVA")
            .ToListAsync();
    }

    // Rastreado: el Worker va a cambiarles el estado a FINALIZADA o DESIERTA
    public async Task<IReadOnlyList<Subasta>> GetSubastasVencidasParaCierreAsync()
    {
        var ahora = DateTime.UtcNow;
        return await _context.Subastas
            .Include(s => s.Pujas)
            .Where(s => s.Estado == "ACTIVA" && s.FechaFin <= ahora)
            .ToListAsync();
    }

    public async Task AddAsync(Subasta subasta)
    {
        await _context.Subastas.AddAsync(subasta);
    }

    public void Update(Subasta subasta)
    {
        _context.Subastas.Update(subasta);
    }

    public void Delete(Subasta subasta)
    {
        _context.Subastas.Remove(subasta);
    }
}