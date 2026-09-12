namespace Application.DTOs;

public record SubastasProcesadasDTO(
    int TotalProcesadas,
    int FinalizadasConGanador,
    int DeclaradasDesiertas,
    string Mensaje
);
