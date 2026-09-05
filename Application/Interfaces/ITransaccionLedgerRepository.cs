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
    Task<IReadOnlyList<TransaccionLedger>> GetByBilleteraIdAsync(int billeteraId);
    Task<IReadOnlyList<TransaccionLedger>> GetBySubastaIdAsync(int subastaId);

    Task AddAsync(TransaccionLedger transaccion);
}
