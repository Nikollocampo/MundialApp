using MundialApp.Models.Entities;
using MundialApp.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace MundialApp.Services;

public sealed class ResultService(ResultRepository repository)
{
    public Task<List<Resultado>> GetAllAsync(CancellationToken cancellationToken = default) => repository.GetAllAsync(cancellationToken);
    public Task<Resultado?> GetByPartidoAsync(int idPartido, CancellationToken cancellationToken = default) => repository.GetByPartidoAsync(idPartido, cancellationToken);
    public Task SaveAsync(Resultado resultado, CancellationToken cancellationToken = default) => repository.SaveAsync(resultado, cancellationToken);
    public Task DeleteAsync(int idResultado, CancellationToken cancellationToken = default) => repository.DeleteAsync(idResultado, cancellationToken);
}