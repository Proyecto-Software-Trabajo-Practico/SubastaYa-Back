using Domain.Entities;

namespace Application.Interfaces;

public interface IAuditoriaRepository
{
    Task<Auditoria?> GetByIdAsync(int id);
    Task<(IReadOnlyList<Auditoria> Items, int TotalItems)> GetPaginadasAsync(int pagina, int tamanoPagina, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Auditoria> Items, int TotalItems)> GetByEntidadPaginadasAsync(string entidad, int entidadId, int pagina, int tamanoPagina, CancellationToken cancellationToken = default);
    Task AddAsync(Auditoria auditoria);
}
