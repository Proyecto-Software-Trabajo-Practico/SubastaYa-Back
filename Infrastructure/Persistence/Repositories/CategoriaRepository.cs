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

public class CategoriaRepository : ICategoriaRepository
{
    private readonly AppDbContext _context;

    public CategoriaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Categoria?> GetByIdAsync(int id)
    {
        return await _context.Categorias.FindAsync(id);
    }

    // Usamos AsNoTracking porque solo queremos leer las categorías para mostrarlas
    public async Task<IReadOnlyList<Categoria>> GetAllAsync()
    {
        return await _context.Categorias
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Categoria?> GetByNombreAsync(string nombre)
    {
        return await _context.Categorias
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Nombre == nombre);
    }

    public async Task AddAsync(Categoria categoria)
    {
        await _context.Categorias.AddAsync(categoria);
    }

    public void Update(Categoria categoria)
    {
        _context.Categorias.Update(categoria);
    }

    public void Delete(Categoria categoria)
    {
        _context.Categorias.Remove(categoria);
    }
}