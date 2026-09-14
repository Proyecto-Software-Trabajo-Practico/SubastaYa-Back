using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

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

    public async Task<(IReadOnlyList<Auditoria> Items, int TotalItems)> GetPaginadasAsync(
        int pagina,
        int tamanoPagina,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Auditorias
            .AsNoTracking()
            .OrderByDescending(a => a.Fecha);

        var totalItems = await query.CountAsync(cancellationToken);

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

        var items = await query
            .Skip((paginaSegura - 1) * tamanoSeguro)
            .Take(tamanoSeguro)
            .ToListAsync(cancellationToken);

        return (items, totalItems);
    }

    public async Task<(IReadOnlyList<Auditoria> Items, int TotalItems)> GetByEntidadPaginadasAsync(
        string entidad,
        int entidadId,
        int pagina,
        int tamanoPagina,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Auditorias
            .AsNoTracking()
            .Where(a => a.Entidad == entidad && a.EntidadId == entidadId)
            .OrderByDescending(a => a.Fecha);

        var totalItems = await query.CountAsync(cancellationToken);

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

        var items = await query
            .Skip((paginaSegura - 1) * tamanoSeguro)
            .Take(tamanoSeguro)
            .ToListAsync(cancellationToken);

        return (items, totalItems);
    }

    public async Task AddAsync(Auditoria auditoria)
    {
        await _context.Auditorias.AddAsync(auditoria);
    }
}