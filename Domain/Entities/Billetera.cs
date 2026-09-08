using Domain.Common;
using System;
using System.Collections.Generic;
using Domain.Exceptions;

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
        if (monto <= 0)
            throw new DomainException("El monto a depositar debe ser mayor a cero.");
        SaldoTotal += monto;
        SaldoDisponible += monto;
    }

    public void RetenerSaldo(decimal monto)
    {
        if (SaldoDisponible < monto)
            throw new DomainException("Saldo disponible insuficiente...");

        SaldoDisponible -= monto;
        SaldoRetenido += monto;
    }
}
