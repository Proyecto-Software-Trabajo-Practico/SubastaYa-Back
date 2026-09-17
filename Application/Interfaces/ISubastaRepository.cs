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
        Task<IReadOnlyList<Subasta>> GetSubastasProgramadasParaInicioAsync();
        // Búsqueda dinámica con filtros, ordenamiento y paginación para el catálogo
        Task<(IReadOnlyList<Subasta> Items, int TotalItems)> GetFiltradasAsync(string? estado, int? categoriaId, int? vendedorId, string? orden, int pagina, int tamanoPagina, CancellationToken cancellationToken = default);
        /*
         * Consulta paginada de subastas por vendedor para el panel de usuario (Módulo 5).
         * Retorna la lista de subastas y el total general para la navegación paginada.
         */
        Task<(IReadOnlyList<Subasta> Items, int TotalItems)> GetByVendedorPaginadoAsync(int vendedorId, int pagina, int tamanoPagina, CancellationToken cancellationToken = default);
        Task AddAsync(Subasta subasta);
        void Update(Subasta subasta);
        void Delete(Subasta subasta);
    }
}
