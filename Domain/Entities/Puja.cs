using Domain.Common;
using Domain.Exceptions;
using System;

namespace Domain.Entities;

public class Puja : BaseEntity
{
    public int SubastaId { get; private set; }
    public virtual Subasta Subasta { get; private set; } = null!;

    public int CompradorId { get; private set; }
    public virtual Usuario Comprador { get; private set; } = null!;

    public decimal Monto { get; private set; }
    public DateTime FechaPuja { get; private set; }

    public byte[] RowVersion { get; private set; } = null!;

    public Puja(int subastaId, int compradorId, decimal monto)
    {
        if (monto <= 0)
            throw new DomainException("El monto de la puja debe ser mayor a cero.");

        SubastaId = subastaId;
        CompradorId = compradorId;
        Monto = monto;
        FechaPuja = DateTime.UtcNow; // Garantiza fecha en memoria antes de ir a BD
    }

    private Puja() 
    { 
    } // Para EF Core
}
