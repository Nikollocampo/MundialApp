using MundialApp.Models.Dto;
using MundialApp.Repositories;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace MundialApp.Services;

public sealed class ReportService(ReportRepository repository)
{
    private readonly ReportRepository _repository = repository;

    static ReportService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public Task<List<BitacoraReporteItem>> GetBitacoraAsync(DateTime desde, DateTime hasta, CancellationToken cancellationToken = default)
        => _repository.GetBitacoraByDateRangeAsync(desde, hasta, cancellationToken);

    public Task<List<JugadorReporteItem>> GetPlayersAsync(decimal? pesoMin, decimal? pesoMax, decimal? alturaMin, decimal? alturaMax, int? idEquipo, CancellationToken cancellationToken = default)
        => _repository.GetPlayersByFiltersAsync(pesoMin, pesoMax, alturaMin, alturaMax, idEquipo, cancellationToken);

    public Task<List<ValorEquipoConfederacionReporteItem>> GetTeamValuesAsync(int idConfederacion, CancellationToken cancellationToken = default)
        => _repository.GetTeamValueByConfederationAsync(idConfederacion, cancellationToken);

    public Task<List<PaisPorSedeReporteItem>> GetCountriesByHostAsync(CancellationToken cancellationToken = default)
        => _repository.GetCountriesPlayingByHostCountryAsync(cancellationToken);

    public byte[] BuildBitacoraPdf(
    IEnumerable<BitacoraReporteItem> items,
    DateTime desde,
    DateTime hasta)
    => BuildTablePdf(
        "Reporte de bitácora",
        $"Rango: {desde:dd/MM/yyyy HH:mm} - {hasta:dd/MM/yyyy HH:mm}",
        new[]
        {
            "Usuario",
            "Tabla",
            "Tipo Acción",
            "Descripción",
            "Fecha"
        },
        items.Select(item => new[]
        {
            item.Usuario,
            item.TablaAfectada,
            item.TipoAccion,
            item.Descripcion ?? "-",
            item.FechaAccion.ToString("dd/MM/yyyy HH:mm")
        }));

    public byte[] BuildJugadoresPdf(IEnumerable<JugadorReporteItem> items)
        => BuildTablePdf(
            "Reporte de jugadores",
            "Jugadores filtrados por peso, estatura y equipo",
            new[] { "Jugador", "Peso", "Altura", "Equipo" },
            items.Select(item => new[]
            {
                item.Jugador,
                item.Peso?.ToString("N2") ?? "-",
                item.Altura?.ToString("N2") ?? "-",
                item.Equipo
            }));

    public byte[] BuildValorEquiposPdf(IEnumerable<ValorEquipoConfederacionReporteItem> items)
        => BuildTablePdf(
            "Valor total por equipo",
            "Equipos por confederación",
            new[] { "Confederación", "Equipo", "Valor total" },
            items.Select(item => new[]
            {
                item.Confederacion,
                item.Equipo,
                item.ValorTotal.ToString("C")
            }));

    public byte[] BuildPaisesPorSedePdf(IEnumerable<PaisPorSedeReporteItem> items)
        => BuildTablePdf(
            "Países por sede",
            "Países que jugarán en cada país anfitrión",
            new[] { "País anfitrión", "País participante" },
            items.Select(item => new[]
            {
                item.PaisAnfitrion,
                item.PaisParticipante
            }));

    private static byte[] BuildTablePdf(string title, string subtitle, IReadOnlyList<string> headers, IEnumerable<string[]> rows)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.Header().Column(column =>
                {
                    column.Item().Text(title).FontSize(20).Bold().FontColor(QuestPDF.Helpers.Colors.Blue.Medium);
                    column.Item().Text(subtitle).FontSize(11).FontColor(QuestPDF.Helpers.Colors.Grey.Darken1);
                });

                page.Content().PaddingVertical(20).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        for (var i = 0; i < headers.Count; i++)
                        {
                            columns.RelativeColumn();
                        }
                    });

                    table.Header(header =>
                    {
                        foreach (var head in headers)
                        {
                            CellStyle(header.Cell()).Text(head).Bold();
                        }
                    });

                    foreach (var row in rows)
                    {
                        foreach (var cell in row)
                        {
                            CellStyle(table.Cell()).Text(cell);
                        }
                    }
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("Generado por MundialApp - ");
                    text.CurrentPageNumber();
                });
            });
        }).GeneratePdf();

        static QuestPDF.Infrastructure.IContainer CellStyle(QuestPDF.Infrastructure.IContainer container)
            => container.Border(1).BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten2).Padding(6);
    }
}
