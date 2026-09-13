namespace Application.Interfaces;

public interface ISubastaNotificationService
{
    Task NotificarNuevaPujaAsync(int subastaId, decimal nuevoMonto, string usuario);
    Task NotificarExtensionAntiSnipingAsync(int subastaId, DateTime nuevaFechaFin);
}
