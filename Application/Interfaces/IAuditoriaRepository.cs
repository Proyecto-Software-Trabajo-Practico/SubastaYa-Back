using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces;
public interface IAuditoriaRepository
{
    Task<Auditoria?> GetByIdAsync(int id);
    Task<IEnumerable<Auditoria>> GetByEntidadAsync(string entidad, int entidadId);
    Task AddAsync(Auditoria auditoria);
}
