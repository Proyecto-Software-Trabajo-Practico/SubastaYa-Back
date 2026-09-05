using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces;

public interface IPujaRepository
{
    Task<Puja?> GetByIdAsync(int id);
    Task<IEnumerable<Puja>> GetBySubastaIdAsync(int subastaId);
    Task<Puja?> GetPujaMasAltaBySubastaIdAsync(int subastaId);
    Task AddAsync(Puja puja);
}
