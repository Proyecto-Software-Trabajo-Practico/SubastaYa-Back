namespace Application.UseCases.Billeteras.Commands;

public record DepositarFondosCommand(int UsuarioId, decimal Monto);
