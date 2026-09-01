using Domain.Common;
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

    public Puja(int subastaId, int compradorId, decimal monto)
    {
        SubastaId = subastaId;
        CompradorId = compradorId;
        Monto = monto;
    }

    private Puja() 
    { 
    } // Para EF Core
}
