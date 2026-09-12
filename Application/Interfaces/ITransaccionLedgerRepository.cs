using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Domain.Entities;

namespace Application.Interfaces;

public interface ITransaccionLedgerRepository
{
    Task<TransaccionLedger?> GetByIdAsync(int id);
    Task<(IReadOnlyList<TransaccionLedger> Items, int TotalItems)> GetByBilleteraIdAsync(int billeteraId, int pagina, int tamanoPagina, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TransaccionLedger>> GetBySubastaIdAsync(int subastaId);

    Task AddAsync(TransaccionLedger transaccion);
}
