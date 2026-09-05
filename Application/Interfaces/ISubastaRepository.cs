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
        Task<IReadOnlyList<Subasta>> GetSubastasActivasAsync();
        Task<IReadOnlyList<Subasta>> GetSubastasVencidasParaCierreAsync();
        Task AddAsync(Subasta subasta);
        void Update(Subasta subasta);
        void Delete(Subasta subasta);
    }
}
