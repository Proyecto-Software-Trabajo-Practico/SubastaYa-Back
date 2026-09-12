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

    // Solo lectura para la sala de subasta en vivo (carga ansiosa de categoría, vendedor y pujas con postor)
    public async Task<Subasta?> GetDetalleByIdAsync(int id)
    {
        return await _context.Subastas
            .AsNoTracking() 
            .Include(s => s.Categoria)
            .Include(s => s.Vendedor)
            .Include(s => s.Pujas)
                .ThenInclude(p => p.Comprador)
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

    /*
     Búsqueda dinámica para el catálogo de subastas.
     Aplica filtros opcionales por estado y categoría, carga ansiosa de categoría y pujas,
     y ordena según el criterio solicitado sin rastreo de EF Core (.AsNoTracking).
    */
    public async Task<(IReadOnlyList<Subasta> Items, int TotalItems)> GetFiltradasAsync(
        string? estado, 
        int? categoriaId, 
        string? orden, 
        int pagina,
        int tamanoPagina,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Subastas
            .AsNoTracking()
            .Include(s => s.Categoria)
            .Include(s => s.Pujas)
            .AsQueryable();

        // 1. Filtro opcional por Estado (ej: "ACTIVA", "PROGRAMADA", "FINALIZADA")
        if (!string.IsNullOrWhiteSpace(estado))
        {
            var estadoNormalizado = estado.Trim().ToUpperInvariant();
            query = query.Where(s => s.Estado == estadoNormalizado);
        }

        // 2. Filtro opcional por Categoría
        if (categoriaId.HasValue && categoriaId.Value > 0)
        {
            query = query.Where(s => s.CategoriaId == categoriaId.Value);
        }

        // 3. Criterios de ordenamiento
        var ordenNormalizado = orden?.Trim().ToLowerInvariant();
        query = ordenNormalizado switch
        {
            "tiempo" => query.OrderBy(s => s.FechaFin),
            "precio_asc" => query.OrderBy(s => s.Pujas.Max(p => (decimal?)p.Monto) ?? s.PrecioBase),
            "precio_desc" => query.OrderByDescending(s => s.Pujas.Max(p => (decimal?)p.Monto) ?? s.PrecioBase),
            _ => string.IsNullOrWhiteSpace(estado)
                // Orden por defecto cuando no se filtra por estado: 1. ACTIVAS, 2. PROGRAMADAS, 3. FINALIZADAS
                ? query.OrderBy(s => s.Estado == "ACTIVA" ? 1 : (s.Estado == "PROGRAMADA" ? 2 : 3))
                       .ThenBy(s => s.FechaFin)
                // Orden por defecto cuando ya hay un estado seleccionado: menor tiempo restante
                : query.OrderBy(s => s.FechaFin)
        };

        // 4. Conteo total de elementos que cumplen los filtros (SELECT COUNT(*) en SQL)
        var totalItems = await query.CountAsync(cancellationToken);

        // 5. Normalización defensiva de paginación
        int paginaSegura = pagina;
        if (paginaSegura < 1)
        {
            paginaSegura = 1;
        }

        int tamanoSeguro = tamanoPagina;
        if (tamanoSeguro <= 0)
        {
            tamanoSeguro = 10;
        }
        else if (tamanoSeguro > 25)
        {
            tamanoSeguro = 25;
        }

        // 6. Paginación delegada a nivel motor SQL (OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY)
        var items = await query
            .Skip((paginaSegura - 1) * tamanoSeguro)
            .Take(tamanoSeguro)
            .ToListAsync(cancellationToken);

        return (items, totalItems);
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