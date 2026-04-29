using MundialApp.Models.Entities;
using MundialApp.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace MundialApp.Services;

public sealed class ParticipationService(ParticipationRepository repository)
{
    public Task<List<Participacion>> GetAllAsync(CancellationToken cancellationToken = default) => repository.GetAllAsync(cancellationToken);
    public Task<List<Participacion>> GetByPartidoAsync(int idPartido, CancellationToken cancellationToken = default) => repository.GetByPartidoAsync(idPartido, cancellationToken);
    public Task SaveAsync(Participacion participacion, CancellationToken cancellationToken = default) => repository.SaveAsync(participacion, cancellationToken);
    public Task DeleteAsync(int codigo, CancellationToken cancellationToken = default) => repository.DeleteAsync(codigo, cancellationToken);
}