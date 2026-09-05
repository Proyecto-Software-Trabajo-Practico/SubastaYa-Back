using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Domain.Entities;

namespace Application.Interfaces;

public interface IBilleteraRepository
{
    Task<Billetera?> GetByIdAsync(int id);
    Task<Billetera?> GetByUsuarioIdAsync(int usuarioId);

    Task AddAsync(Billetera billetera);
    void Update(Billetera billetera);
}