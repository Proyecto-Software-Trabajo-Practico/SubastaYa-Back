using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Domain.Entities;

namespace Application.Interfaces;

public interface ICategoriaRepository
{
    Task<Categoria?> GetByIdAsync(int id);
    Task<IReadOnlyList<Categoria>> GetAllAsync();
    Task<Categoria?> GetByNombreAsync(string nombre);

    Task AddAsync(Categoria categoria);
    void Update(Categoria categoria);
    void Delete(Categoria categoria);
}
