using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class TransaccionLedgerRepository : ITransaccionLedgerRepository
{
    private readonly AppDbContext _context;

    public TransaccionLedgerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<TransaccionLedger?> GetByIdAsync(int id)
    {
        return await _context.TransaccionesLedger.FindAsync(id);
    }

    // Historial de movimientos de una billetera (para mostrar en el panel del usuario)
    public async Task<(IReadOnlyList<TransaccionLedger> Items, int TotalItems)> GetByBilleteraIdAsync(
        int billeteraId, 
        int pagina, 
        int tamanoPagina, 
        CancellationToken cancellationToken = default)
    {
        var query = _context.TransaccionesLedger
            .AsNoTracking() // Solo lectura para el historial
            .Where(t => t.BilleteraId == billeteraId)
            .OrderByDescending(t => t.Fecha); // Los más recientes primero

        // 1. Conteo total de transacciones de la billetera en SQL Server
        var totalItems = await query.CountAsync(cancellationToken);

        // 2. Normalización defensiva de paginación tradicional
        int paginaSegura = pagina;
        if (paginaSegura < 1)
        {
            paginaSegura = 1;
        }

        int tamanoSeguro = tamanoPagina;
        if (tamanoSeguro <= 0)
        {
            tamanoSeguro = 10;
        }
        else if (tamanoSeguro > 25)
        {
            tamanoSeguro = 25;
        }

        // 3. Paginación delegada a nivel motor SQL Server (OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY)
        var items = await query
            .Skip((paginaSegura - 1) * tamanoSeguro)
            .Take(tamanoSeguro)
            .ToListAsync(cancellationToken);

        return (items, totalItems);
    }

    // Movimientos contables vinculados a una subasta específica (para auditoría)
    public async Task<IReadOnlyList<TransaccionLedger>> GetBySubastaIdAsync(int subastaId)
    {
        return await _context.TransaccionesLedger
            .AsNoTracking() // Solo lectura para auditoría
            .Where(t => t.SubastaId == subastaId)
            .OrderByDescending(t => t.Fecha)
            .ToListAsync();
    }

    // Registrar un nuevo asiento contable
    public async Task AddAsync(TransaccionLedger transaccion)
    {
        await _context.TransaccionesLedger.AddAsync(transaccion);
    }
}