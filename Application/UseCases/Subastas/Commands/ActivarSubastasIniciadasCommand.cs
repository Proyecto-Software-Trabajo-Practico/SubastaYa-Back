namespace Application.UseCases.Subastas.Commands;

/*
 * Comando CQRS para orquestar la activación de subastas programadas
 * cuya fecha y hora de inicio ya se ha cumplido.
 */
public record ActivarSubastasIniciadasCommand();
