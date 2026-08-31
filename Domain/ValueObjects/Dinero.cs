using System;

namespace Domain.ValueObjects;

public record Dinero
{
    public decimal Monto { get; init; }

    public Dinero(decimal monto)
    {
        if (monto < 0)
            throw new ArgumentException("El monto no puede ser negativo.", nameof(monto));

        Monto = monto;
    }

    // Constructor privado para EF Core
    private Dinero() { }

    public static Dinero Zero => new Dinero(0);
}
