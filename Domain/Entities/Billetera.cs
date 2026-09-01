using Domain.Common;
using System;
using System.Collections.Generic;

namespace Domain.Entities;

public class Billetera : BaseEntity
{
    public int UsuarioId { get; private set; }
    public virtual Usuario Usuario { get; private set; } = null!;
    
    public decimal SaldoTotal { get; private set; }
    public decimal SaldoRetenido { get; private set; }
    public decimal SaldoDisponible { get; private set; }

    public virtual ICollection<TransaccionLedger> Transacciones { get; private set; } = new List<TransaccionLedger>();

    public int Version { get; private set; }

    public Billetera(int usuarioId)
    {
        UsuarioId = usuarioId;
        SaldoTotal = 0m;
        SaldoRetenido = 0m;
        SaldoDisponible = 0m;
    }

    private Billetera() { 
    }

    public void Depositar(decimal monto)
    {
        SaldoTotal += monto;
        SaldoDisponible += monto;
    }

    public void RetenerSaldo(decimal monto)
    {
        if (SaldoDisponible < monto)
            throw new InvalidOperationException("Saldo disponible insuficiente para realizar la retención.");

        SaldoDisponible -= monto;
        SaldoRetenido += monto;
    }
}
