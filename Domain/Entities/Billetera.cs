using Domain.Common;
using Domain.ValueObjects;
using System;

namespace Domain.Entities;

public class Billetera : BaseEntity
{
    public int UsuarioId { get; private set; }
    
    public Dinero SaldoTotal { get; private set; }
    public Dinero SaldoRetenido { get; private set; }
    public Dinero SaldoDisponible { get; private set; }

    // Control de concurrencia optimista: previene condiciones de carrera cuando 
    // operaciones simultáneas intentan modificar el saldo retenido/disponible al mismo tiempo.
    public int Version { get; private set; }

    public Billetera(int usuarioId)
    {
        UsuarioId = usuarioId;
        SaldoTotal = Dinero.Zero;
        SaldoRetenido = Dinero.Zero;
        SaldoDisponible = Dinero.Zero;
    }

    private Billetera() { 
        SaldoTotal = null!;
        SaldoRetenido = null!;
        SaldoDisponible = null!;
    } // Para EF Core

    public void Depositar(Dinero monto)
    {
        SaldoTotal = new Dinero(SaldoTotal.Monto + monto.Monto);
        SaldoDisponible = new Dinero(SaldoDisponible.Monto + monto.Monto);
    }

    public void RetenerSaldo(Dinero monto)
    {
        if (SaldoDisponible.Monto < monto.Monto)
            throw new InvalidOperationException("Saldo disponible insuficiente para realizar la retención.");

        SaldoDisponible = new Dinero(SaldoDisponible.Monto - monto.Monto);
        SaldoRetenido = new Dinero(SaldoRetenido.Monto + monto.Monto);
    }
}
