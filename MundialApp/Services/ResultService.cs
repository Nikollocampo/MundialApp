using MundialApp.Models.Entities;
using MundialApp.Repositories;

namespace MundialApp.Services;

public sealed class ResultService(ResultRepository repository, AuditService auditService, SessionState sessionState)
    : AuditedServiceBase(auditService, sessionState)
{
    private readonly ResultRepository _repository = repository;

    public Task<List<Resultado>> GetAllAsync(CancellationToken cancellationToken = default) 
        => _repository.GetAllAsync(cancellationToken);

    public Task<Resultado?> GetByPartidoAsync(int idPartido, CancellationToken cancellationToken = default) 
        => _repository.GetByPartidoAsync(idPartido, cancellationToken);

    public async Task SaveAsync(Resultado resultado, CancellationToken cancellationToken = default)
    {
        var esNuevo = resultado.IdResultado == 0;
        var idResultado = resultado.IdResultado.ToString();

        await _repository.SaveAsync(resultado, cancellationToken);

        if (esNuevo)
        {
            await RegistrarInsercionAsync(
                "RESULTADO",
                idResultado,
                $"Partido: {resultado.Partido?.EquipoLocal} vs {resultado.Partido?.EquipoVisitante}, Goles: {resultado.GolesLocal}-{resultado.GolesVisitante}",
                cancellationToken);
        }
        else
        {
            await RegistrarActualizacionAsync(
                "RESULTADO",
                idResultado,
                "Goles Local, Goles Visitante, Ganador",
                cancellationToken);
        }
    }

    public async Task DeleteAsync(int idResultado, CancellationToken cancellationToken = default)
    {
        var resultados = await _repository.GetAllAsync(cancellationToken);
        var resultado = resultados.FirstOrDefault(r => r.IdResultado == idResultado);

        await _repository.DeleteAsync(idResultado, cancellationToken);

        if (resultado is not null)
        {
            await RegistrarEliminacionAsync(
                "RESULTADO",
                idResultado.ToString(),
                $"Partido: {resultado.Partido?.EquipoLocal} vs {resultado.Partido?.EquipoVisitante}",
                cancellationToken);
        }
    }
}
