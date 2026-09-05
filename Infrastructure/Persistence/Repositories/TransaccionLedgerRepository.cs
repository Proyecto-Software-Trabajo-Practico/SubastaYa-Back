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
    public async Task<IReadOnlyList<TransaccionLedger>> GetByBilleteraIdAsync(int billeteraId)
    {
        return await _context.TransaccionesLedger
            .AsNoTracking() // Solo lectura para el historial
            .Where(t => t.BilleteraId == billeteraId)
            .OrderByDescending(t => t.Fecha) // Los más recientes primero
            .ToListAsync();
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