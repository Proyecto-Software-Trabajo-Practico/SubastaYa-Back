using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities; 

namespace Application.Interfaces
{
    public interface ISubastaRepository
    {
        Task<Subasta?> GetByIdAsync(int id);
        Task<IReadOnlyList<Subasta>> GetAllAsync();
        Task<Subasta?> GetWithPujasByIdAsync(int id);
        Task<Subasta?> GetDetalleByIdAsync(int id);
        Task<IReadOnlyList<Subasta>> GetSubastasActivasAsync();
        Task<IReadOnlyList<Subasta>> GetSubastasVencidasParaCierreAsync();
        // Búsqueda dinámica con filtros, ordenamiento y paginación para el catálogo
        Task<(IReadOnlyList<Subasta> Items, int TotalItems)> GetFiltradasAsync(string? estado, int? categoriaId, string? orden, int pagina, int tamanoPagina, CancellationToken cancellationToken = default);
        Task AddAsync(Subasta subasta);
        void Update(Subasta subasta);
        void Delete(Subasta subasta);
    }
}
