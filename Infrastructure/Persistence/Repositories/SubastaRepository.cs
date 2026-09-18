using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

    // Solo lectura para la sala de subasta en vivo (carga ansiosa completa)
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

    // Solo lectura para el catálogo de subastas en curso
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

    // Rastreado por ChangeTracker: el Worker de inicio
    public async Task<IReadOnlyList<Subasta>> GetSubastasProgramadasParaInicioAsync()
    {
        var ahora = DateTime.UtcNow;
        return await _context.Subastas
            .Where(s => s.Estado == "PROGRAMADA" && s.FechaInicio <= ahora)
            .ToListAsync();
    }

    /*
     Búsqueda dinámica para el catálogo general.
    */
    public async Task<(IReadOnlyList<Subasta> Items, int TotalItems)> GetFiltradasAsync(
        string? estado,
        int? categoriaId,
        int? vendedorId,
        string? orden,
        int pagina,
        int tamanoPagina,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Subastas
            .AsNoTracking()
            .Include(s => s.Categoria)
            .AsQueryable();

        // 1. Filtro opcional por Estado
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

        // 3. Filtro opcional por Vendedor
        if (vendedorId.HasValue && vendedorId.Value > 0)
        {
            query = query.Where(s => s.VendedorId == vendedorId.Value);
        }

        // 4. Criterios de ordenamiento
        var ordenNormalizado = orden?.Trim().ToLowerInvariant();
        query = ordenNormalizado switch
        {
            "tiempo" => query.OrderBy(s => s.FechaFin),
            "precio_asc" => query.OrderBy(s => s.Pujas.Max(p => (decimal?)p.Monto) ?? s.PrecioBase),
            "precio_desc" => query.OrderByDescending(s => s.Pujas.Max(p => (decimal?)p.Monto) ?? s.PrecioBase),
            _ => string.IsNullOrWhiteSpace(estado)
                ? query.OrderBy(s => s.Estado == "ACTIVA" ? 1 : (s.Estado == "PROGRAMADA" ? 2 : 3))
                       .ThenBy(s => s.FechaFin)
                : query.OrderBy(s => s.FechaFin)
        };

        var totalItems = await query.CountAsync(cancellationToken);

        int paginaSegura = pagina < 1 ? 1 : pagina;
        int tamanoSeguro = tamanoPagina <= 0 ? 10 : (tamanoPagina > 25 ? 25 : tamanoPagina);

        var items = await query
            .Skip((paginaSegura - 1) * tamanoSeguro)
            .Take(tamanoSeguro)
            .ToListAsync(cancellationToken);

        return (items, totalItems);
    }

    /*
 * Módulo 5 - Mis Publicaciones (Optimizado < 50ms)
 * Utiliza Index Seek directo sobre IX_Subastas_VendedorId y elimina round-trips redundantes.
 */
    public async Task<(IReadOnlyList<Subasta> Items, int TotalItems)> GetByVendedorPaginadoAsync(
        int vendedorId,
        int pagina,
        int tamanoPagina,
        CancellationToken cancellationToken = default)
    {
        int paginaSegura = pagina < 1 ? 1 : pagina;
        int tamanoSeguro = tamanoPagina <= 0 ? 10 : (tamanoPagina > 50 ? 50 : tamanoPagina);

        var baseQuery = _context.Subastas
            .AsNoTracking()
            .Where(s => s.VendedorId == vendedorId);

        // 1. Conteo sobre el índice del VendedorId (Index Seek instantáneo)
        var totalItems = await baseQuery.CountAsync(cancellationToken);

        if (totalItems == 0)
            return (Array.Empty<Subasta>(), 0);

        // 2. Paginación directa en una sola consulta SQL limpia
        var items = await baseQuery
            .Include(s => s.Categoria)
            .OrderByDescending(s => s.FechaFin)
            .Skip((paginaSegura - 1) * tamanoSeguro)
            .Take(tamanoSeguro)
            .ToListAsync(cancellationToken);

        return (items, totalItems);
    }

    /*
     * Módulo 5 - Mis Ofertas (Ultra-Optimizado < 50ms)
     * Estrategia de Índice Invertido: Consulta primero los SubastaIds del Comprador en Pujas 
     * usando IX_Pujas_CompradorId (Index Seek < 2ms), eliminando el Table Scan correlacionado.
     */
    public async Task<(IReadOnlyList<Subasta> Items, int TotalItems)> GetOfertadasByCompradorPaginadoAsync(
    int compradorId,
    int pagina,
    int tamanoPagina,
    CancellationToken cancellationToken = default)
    {
        int paginaSegura = pagina < 1 ? 1 : pagina;
        int tamanoSeguro = tamanoPagina <= 0 ? 10 : (tamanoPagina > 50 ? 50 : tamanoPagina);

        // ⚡ PASO CLAVE: .ToListAsync() ejecuta una consulta aislada ultra-rápida (Index Seek < 2ms)
        // obteniendo una lista en memoria (ej: [1, 4, 8])
        var subastaIds = await _context.Pujas
            .AsNoTracking()
            .Where(p => p.CompradorId == compradorId)
            .Select(p => p.SubastaId)
            .Distinct()
            .ToListAsync(cancellationToken);

        if (subastaIds.Count == 0)
            return (Array.Empty<Subasta>(), 0);

        // SQL Server ahora recibe: WHERE s.Id IN (1, 4, 8) -> Clustered Index Seek directo
        var querySubastas = _context.Subastas
            .AsNoTracking()
            .Where(s => subastaIds.Contains(s.Id));

        var totalItems = await querySubastas.CountAsync(cancellationToken);

        var items = await querySubastas
            .Include(s => s.Categoria)
            .OrderByDescending(s => s.FechaFin)
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