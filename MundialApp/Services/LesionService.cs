using MundialApp.Models.Entities;
using MundialApp.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace MundialApp.Services;

public sealed class LesionService(LesionRepository repository)
{
    public Task<List<Lesion>> GetAllAsync(CancellationToken cancellationToken = default) => repository.GetAllAsync(cancellationToken);
    public Task<List<Lesion>> GetByJugadorAsync(int idJugador, CancellationToken cancellationToken = default) => repository.GetByJugadorAsync(idJugador, cancellationToken);
    public Task SaveAsync(Lesion lesion, CancellationToken cancellationToken = default) => repository.SaveAsync(lesion, cancellationToken);
    public Task DeleteAsync(int idLesion, CancellationToken cancellationToken = default) => repository.DeleteAsync(idLesion, cancellationToken);
}